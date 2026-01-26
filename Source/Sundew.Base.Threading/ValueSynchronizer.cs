// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueSynchronizer.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Sundew.Base.Collections;
using Sundew.Base.Notifications;

/// <summary>
/// Provides a thread-safe mechanism for synchronizing and updating a value based on a parameter, supporting
/// asynchronous refresh and notification of value changes to subscribers.
/// </summary>
/// <typeparam name="TParameter">The type of the parameter used to retrieve or update the value.</typeparam>
/// <typeparam name="TValue">The type of the value object to be synchronized. Must be a reference type.</typeparam>
public class ValueSynchronizer<TParameter, TValue> : IValueSynchronizer<TParameter, TValue>
    where TValue : class
{
    private readonly Func<TParameter, CancellationToken, Task<TValue>> getValueFunc;
    private readonly AsyncLock @lock = new();
    private readonly DelegateEvent<TValue> updateEvent = new();
    private readonly Dictionary<object, Func<CancellationToken, Task<PostSubmitAction<TParameter, TValue>>>> pendingSubmissions = new();
    private Task<TValue> getCurrentValueTask;
    private Task? submitValueTask;
    private PostSubmitAction<TParameter, TValue>.RefreshOnIdle? invalidateOnIdle;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueSynchronizer{TParameter,TValue}"/> class.
    /// </summary>
    /// <param name="initialParameter">The initial parameter.</param>
    /// <param name="getValueFunc">The get value func.</param>
    public ValueSynchronizer(TParameter initialParameter, Func<TParameter, CancellationToken, Task<TValue>> getValueFunc)
    {
        this.getValueFunc = getValueFunc;
        this.getCurrentValueTask = this.getValueFunc(initialParameter, CancellationToken.None);
    }

    /// <summary>
    /// Gets the current value as an asynchronous operation.
    /// </summary>
    /// <remarks>Accessing this property returns a task that represents the current value. The returned task
    /// may be cached and reused for subsequent accesses until the value changes. Thread safety is ensured when
    /// retrieving the value.</remarks>
    public Task<TValue> Value
    {
        get => this.getCurrentValueTask;
    }

    /// <summary>
    /// Subscribes a handler to receive notifications for events of the specified type.
    /// </summary>
    /// <remarks>Disposing the returned Subscription will remove the handler from the list of subscribers. The
    /// handler will be invoked for each event of type TSubscribedEvent that is published.</remarks>
    /// <typeparam name="TSubscribedEvent">The type of event to subscribe to. Must derive from TValue.</typeparam>
    /// <param name="notificationTarget">The notification target that will receive the event notifications.</param>
    /// <param name="handler">A delegate that handles the event. The delegate receives the event data and a cancellation token, and returns a
    /// ValueTask representing the asynchronous operation.</param>
    /// <returns>A Subscription object that can be disposed to unsubscribe the handler.</returns>
    public Subscription Subscribe<TSubscribedEvent>(INotificationTarget notificationTarget, Func<TSubscribedEvent, CancellationToken, ValueTask> handler)
        where TSubscribedEvent : TValue
    {
        return this.updateEvent.Subscribe(handler, notificationTarget);
    }

    /// <summary>
    /// Refreshes the current state and asynchronously using the specified parameter.
    /// </summary>
    /// <param name="parameter">The parameter used to obtain the new value. The value provided will be passed to the value retrieval function.</param>
    /// <param name="cancellation">The cancellation.</param>
    /// <returns>A task that represents the asynchronous invalidate operation.</returns>
    public async Task RefreshAsync(TParameter parameter, Cancellation cancellation)
    {
        using var enabler = cancellation.EnableCancellation();
        try
        {
            using (await this.@lock.LockAsync().ConfigureAwait(false))
            {
                this.getCurrentValueTask = this.getValueFunc(parameter, enabler.Token);
                await this.getCurrentValueTask.ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
#if NET7_0_OR_GREATER
            await enabler.CancelAsync().ConfigureAwait(false);
#else
            enabler.Cancel();
#endif
        }
    }

    /// <summary>
    /// Disposes the resources used by the ValueSynchronizer.
    /// </summary>
    public void Dispose()
    {
        this.@lock.Dispose();
        this.updateEvent.Dispose();
    }

    /// <summary>
    /// Attempts to submit an asynchronous state update for the specified submission id using the provided update function.
    /// </summary>
    /// <remarks>If multiple updates are requested concurrently, they are queued and applied in order. This
    /// method ensures that updates are applied in a thread-safe manner.</remarks>
    /// <param name="submissionId">The object that identifies the submission for which the update is being submitted. Cannot be null.</param>
    /// <param name="applyFunc">A function that asynchronously produces a PostApplyAction to be applied. Cannot be null.</param>
    /// <param name="cancellation">The cancellation.</param>
    /// <returns>A task that represents the asynchronous operation. The task completes when the update has been applied.</returns>
    public async Task TrySubmitAsync(object submissionId, Func<CancellationToken, Task<PostSubmitAction<TParameter, TValue>>> applyFunc, Cancellation cancellation)
    {
        using var enabler = cancellation.EnableCancellation();
        Task? currentSubmitValueTask = null;
        try
        {
            using (await this.@lock.LockAsync(enabler.Token).ConfigureAwait(false))
            {
                ref var entry =
                    ref CollectionsMarshal.GetValueRefOrAddDefault(this.pendingSubmissions, submissionId, out bool exists);
                entry = applyFunc;
                if (!exists)
                {
                    var token = enabler.Token;
                    this.submitValueTask = currentSubmitValueTask = this.submitValueTask.HasValue && !this.submitValueTask.IsCompleted
                        ? this.submitValueTask.ContinueWith(task => ApplyValue(token), token, TaskContinuationOptions.None, TaskScheduler.Default)
                        : Task.Run(() => ApplyValue(token), token);
                }
            }

            if (currentSubmitValueTask.HasValue)
            {
                await currentSubmitValueTask.ConfigureAwait(false);
            }

            async Task ApplyValue(CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    using (await this.@lock.LockAsync(cancellationToken).ConfigureAwait(false))
                    {
                        if (this.pendingSubmissions.Remove(submissionId, out var actualUpdateFunc))
                        {
                            await this.PrivateApply(actualUpdateFunc, cancellationToken).ConfigureAwait(false);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    await HandleCancellation().ConfigureAwait(false);
                }
            }
        }
        catch (OperationCanceledException)
        {
            await HandleCancellation().ConfigureAwait(false);
        }

        async Task HandleCancellation()
        {
#if NET7_0_OR_GREATER
            await enabler.CancelAsync().ConfigureAwait(false);
#else
            enabler.Cancel();
#endif
            using (await this.@lock.LockAsync(CancellationToken.None).ConfigureAwait(false))
            {
                this.pendingSubmissions.Remove(submissionId, out var _);
            }
        }
    }

    private async Task PrivateApply(Func<CancellationToken, Task<PostSubmitAction<TParameter, TValue>>> applyFunc, CancellationToken cancellationToken)
    {
        var postApply = await applyFunc(cancellationToken).ConfigureAwait(false);

        switch (postApply)
        {
            case PostSubmitAction<TParameter, TValue>.Refresh invalidate:
                this.getCurrentValueTask = this.getValueFunc(invalidate.Parameter, cancellationToken);
                await this.updateEvent.RaiseAsync(await this.getCurrentValueTask.ConfigureAwait(false), Parallelism.Default, CancellationToken.None);
                this.invalidateOnIdle = null;
                break;
            case PostSubmitAction<TParameter, TValue>.RefreshOnIdle invalidateOnIdle:
                if (this.pendingSubmissions.IsEmpty)
                {
                    this.getCurrentValueTask = this.getValueFunc(invalidateOnIdle.Parameter, cancellationToken);
                    await this.updateEvent.RaiseAsync(await this.getCurrentValueTask.ConfigureAwait(false), Parallelism.Default, CancellationToken.None);
                    this.invalidateOnIdle = null;
                }
                else
                {
                    this.invalidateOnIdle = invalidateOnIdle;
                }

                break;
            case PostSubmitAction<TParameter, TValue>.None:
                if (this.invalidateOnIdle.HasValue)
                {
                    this.getCurrentValueTask = this.getValueFunc(this.invalidateOnIdle.Parameter, cancellationToken);
                    await this.updateEvent.RaiseAsync(await this.getCurrentValueTask.ConfigureAwait(false), Parallelism.Default, CancellationToken.None);
                }

                break;
            case PostSubmitAction<TParameter, TValue>.SetValue setValue:
                this.getCurrentValueTask = Task.FromResult(setValue.Value);
                await this.updateEvent.RaiseAsync(await this.getCurrentValueTask.ConfigureAwait(false), Parallelism.Default, CancellationToken.None);
                break;
        }
    }
}