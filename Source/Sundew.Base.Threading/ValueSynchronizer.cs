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
using static Sundew.Base.Cancellation;

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
#if NET9_0_OR_GREATER
    private readonly Lock @lock = new Lock();
#else
    private readonly object @lock = new object();
#endif
    private readonly DelegateEvent<TValue> updateEvent = new();
    private readonly Dictionary<object, SubmissionContext> pendingSubmissions = new();
    private Task<TValue> getCurrentValueTask;
    private Task? lastSubmitValueTask;
    private PostSubmitAction<TParameter, TValue>.RefreshOnIdle? refreshOnIdle;

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
        get
        {
            lock (this.@lock)
            {
                return this.getCurrentValueTask;
            }
        }
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
    public async Task RefreshAsync(TParameter parameter, Cancellation cancellation = default)
    {
        using var enabler = cancellation.EnableCancellation();
        try
        {
            lock (this.@lock)
            {
                this.getCurrentValueTask = this.getValueFunc(parameter, enabler.Token);
            }

            await this.getCurrentValueTask.ConfigureAwait(false);
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
    /// Attempts to submit an asynchronous state update for the specified submission id using the provided update function.
    /// </summary>
    /// <remarks>If multiple updates are requested concurrently, they are queued and applied in order. This
    /// method ensures that updates are applied in a thread-safe manner.</remarks>
    /// <param name="submissionId">The object that identifies the submission for which the update is being submitted. Cannot be null.</param>
    /// <param name="submissionFunc">A function that asynchronously produces a PostApplyAction to be applied. Cannot be null.</param>
    /// <param name="cancellation">The cancellation.</param>
    /// <returns>A task that represents the asynchronous operation. The task completes when the update has been applied.</returns>
    public async Task TrySubmitAsync(object submissionId, Func<CancellationToken, Task<PostSubmitAction<TParameter, TValue>>> submissionFunc, Cancellation cancellation = default)
    {
        using var enabler = cancellation.EnableCancellation();
        try
        {
            Task? currentSubmitValueTask = null;
            lock (this.@lock)
            {
                ref var entry = ref CollectionsMarshal.GetValueRefOrAddDefault(this.pendingSubmissions, submissionId, out bool exists);
                entry.SubmissionFunc = submissionFunc;
                if (!exists)
                {
                    var cancellationToken = enabler.Token;
                    currentSubmitValueTask = this.lastSubmitValueTask.HasValue
                        ? this.lastSubmitValueTask.ContinueWith(task => ApplyValueAndTryRefresh(submissionId, cancellationToken), cancellationToken, TaskContinuationOptions.None, TaskScheduler.Default).Unwrap()
                        : Task.Run(() => ApplyValueAndTryRefresh(submissionId, cancellationToken), cancellationToken);
                    this.lastSubmitValueTask = currentSubmitValueTask;
                    entry = new SubmissionContext(currentSubmitValueTask, submissionFunc);
                }
                else
                {
                    currentSubmitValueTask = entry.SubmissionTask;
                }
            }

            if (currentSubmitValueTask.HasValue)
            {
                await currentSubmitValueTask.ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            await this.HandleCancellation(submissionId, enabler).ConfigureAwait(false);
        }

        async Task ApplyValueAndTryRefresh(object submissionId, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                SubmissionContext submissionContext = default;
                lock (this.@lock)
                {
                    this.pendingSubmissions.Remove(submissionId, out submissionContext);
                }

                if (!submissionContext.Equals(default))
                {
                    await this.PrivateSubmit(submissionContext.SubmissionFunc, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                await this.HandleCancellation(submissionId, enabler).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Disposes the resources used by the ValueSynchronizer.
    /// </summary>
    public void Dispose()
    {
        this.updateEvent.Dispose();
        GC.SuppressFinalize(this);
    }

    private async Task PrivateSubmit(Func<CancellationToken, Task<PostSubmitAction<TParameter, TValue>>> submissionFunc, CancellationToken cancellationToken)
    {
        var postApply = await submissionFunc(cancellationToken).ConfigureAwait(false);

        switch (postApply)
        {
            case PostSubmitAction<TParameter, TValue>.Refresh refresh:
                {
                    this.getCurrentValueTask = this.getValueFunc(refresh.Parameter, cancellationToken);
                    var value = await this.getCurrentValueTask.ConfigureAwait(false);
                    await this.updateEvent.RaiseAsync(value, Parallelism.Default, CancellationToken.None).ConfigureAwait(false);
                    this.refreshOnIdle = null;
                    break;
                }

            case PostSubmitAction<TParameter, TValue>.RefreshOnIdle refreshOnIdle:
                if (this.pendingSubmissions.IsEmpty)
                {
                    this.getCurrentValueTask = this.getValueFunc(refreshOnIdle.Parameter, cancellationToken);
                    var value = await this.getCurrentValueTask.ConfigureAwait(false);
                    await this.updateEvent.RaiseAsync(value, Parallelism.Default, CancellationToken.None).ConfigureAwait(false);
                    this.refreshOnIdle = null;
                }
                else
                {
                    this.refreshOnIdle = refreshOnIdle;
                }

                break;
            case PostSubmitAction<TParameter, TValue>.None:
                if (this.refreshOnIdle.HasValue)
                {
                    this.getCurrentValueTask = this.getValueFunc(this.refreshOnIdle.Parameter, cancellationToken);
                    var value = await this.getCurrentValueTask.ConfigureAwait(false);
                    await this.updateEvent.RaiseAsync(value, Parallelism.Default, CancellationToken.None).ConfigureAwait(false);
                }

                break;
            case PostSubmitAction<TParameter, TValue>.SetValue setValue:
                this.getCurrentValueTask = Task.FromResult(setValue.Value);
                await this.updateEvent.RaiseAsync(setValue.Value, Parallelism.Default, CancellationToken.None).ConfigureAwait(false);
                break;
        }
    }

    private async Task HandleCancellation(object submissionId, Enabler enabler)
    {
#if NET7_0_OR_GREATER
        await enabler.CancelAsync().ConfigureAwait(false);
#else
        enabler.Cancel();
#endif
        lock (this.@lock)
        {
            this.pendingSubmissions.Remove(submissionId, out var _);
        }
    }

    private record struct SubmissionContext(
        Task SubmissionTask,
        Func<CancellationToken, Task<PostSubmitAction<TParameter, TValue>>> SubmissionFunc);
}