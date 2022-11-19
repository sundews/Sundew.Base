// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResetEventAsync.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sundew.Base.Threading.Internal;

/// <summary>
/// Abstract base class for implementing async reset events.
/// </summary>
public abstract class ResetEventAsync
{
    private readonly object lockObject = new();
    private readonly LinkedList<Awaiter> awaiters = new();
    private bool isSet;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResetEventAsync"/> class.
    /// </summary>
    protected ResetEventAsync()
        : this(false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResetEventAsync"/> class.
    /// </summary>
    /// <param name="isSet">if set to <c>true</c> [initial state].</param>
    protected ResetEventAsync(bool isSet)
    {
        this.isSet = isSet;
    }

    /// <summary>
    /// Gets a value indicating whether this instance is set.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is set; otherwise, <c>false</c>.
    /// </value>
    public bool IsSet => this.isSet;

    /// <summary>
    /// Reset this instance.
    /// </summary>
    public void Reset()
    {
        lock (this.lockObject)
        {
            this.isSet = false;
        }
    }

    /// <summary>
    /// Sets this instance.
    /// </summary>
    public void Set()
    {
        TaskCompletionSource<bool>? completedWaiter = null;
        lock (this.lockObject)
        {
            var first = this.awaiters.First;
            if (first != null)
            {
                this.awaiters.RemoveFirst();
                completedWaiter = first.Value.TaskCompletionSource;
                first.Value.CancellationTokenSource.Cancel();
            }

            this.isSet = completedWaiter == null;
        }

        completedWaiter?.SetResult(true);
    }

    /// <summary>
    /// Waits the synchronously.
    /// </summary>
    /// <returns>A boolean result indicating whether the signal was received.</returns>
    public bool Wait()
    {
        return this.Wait(Timeout.InfiniteTimeSpan, CancellationToken.None);
    }

    /// <summary>
    /// Waits the synchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A boolean result indicating whether the signal was received.</returns>
    public bool Wait(CancellationToken cancellationToken)
    {
        return this.Wait(Timeout.InfiniteTimeSpan, cancellationToken);
    }

    /// <summary>
    /// Waits the synchronously.
    /// </summary>
    /// <param name="timeout">The timeout.</param>
    /// <returns>A boolean result indicating whether the signal was received.</returns>
    public bool Wait(TimeSpan timeout)
    {
        return this.Wait(timeout, CancellationToken.None);
    }

    /// <summary>
    /// Waits the synchronously.
    /// </summary>
    /// <param name="timeout">The timeout.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A boolean result indicating whether the signal was received.</returns>
    public bool Wait(TimeSpan timeout, CancellationToken cancellationToken)
    {
        var waitTask = this.WaitAsync(timeout, cancellationToken);
        waitTask.Wait(timeout);
        return waitTask.Result;
    }

    /// <summary>
    /// Waits the asynchronously.
    /// </summary>
    /// <returns>A task with a boolean result indicating whether the signal was received.</returns>
    public Task<bool> WaitAsync()
    {
        return this.WaitAsync(Timeout.InfiniteTimeSpan, CancellationToken.None);
    }

    /// <summary>
    /// Waits the asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task with a boolean result indicating whether the signal was received.</returns>
    public Task<bool> WaitAsync(CancellationToken cancellationToken)
    {
        return this.WaitAsync(Timeout.InfiniteTimeSpan, cancellationToken);
    }

    /// <summary>
    /// Waits the asynchronously.
    /// </summary>
    /// <param name="timeout">The timeout.</param>
    /// <returns>A task with a boolean result indicating whether the signal was received.</returns>
    public Task<bool> WaitAsync(TimeSpan timeout)
    {
        return this.WaitAsync(timeout, CancellationToken.None);
    }

    /// <summary>
    /// Waits the asynchronously.
    /// </summary>
    /// <param name="timeout">The timeout.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task with a boolean result indicating whether the signal was received.</returns>
    public Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
        lock (this.lockObject)
        {
            if (this.isSet)
            {
                this.OnIsSetDuringWaitWhileLocked(ref this.isSet);
                return TaskHelper.CompletedTrueTask;
            }

            if (timeout == TimeSpan.Zero || cancellationToken.IsCancellationRequested)
            {
                return TaskHelper.CompletedFalseTask;
            }

            var awaiter = new Awaiter(new TaskCompletionSource<bool>(), new CancellationTokenSource());
            this.awaiters.AddFirst(awaiter);
            this.HandleWaitTimeout(timeout, awaiter, cancellationToken);
            return awaiter.TaskCompletionSource.Task;
        }
    }

    /// <summary>
    /// Called during a Wait or WaitAsync call when the event is set and the lock is acquired.
    /// </summary>
    /// <param name="isSet">if set to <c>true</c> [is set].</param>
    protected abstract void OnIsSetDuringWaitWhileLocked(ref bool isSet);

    private void HandleWaitTimeout(
        TimeSpan timeout,
        Awaiter awaiter,
        CancellationToken cancellationToken)
    {
        Task.Run(
            async () =>
            {
                try
                {
                    using var linkedCancellationTokenSource =
                        CancellationTokenSource.CreateLinkedTokenSource(
                            cancellationToken,
                            awaiter.CancellationTokenSource.Token);
                    await Task.Delay(timeout, linkedCancellationTokenSource.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
                finally
                {
                    if (!awaiter.CancellationTokenSource.IsCancellationRequested)
                    {
                        lock (this.lockObject)
                        {
                            this.awaiters.Remove(awaiter);
                        }

                        awaiter.TaskCompletionSource.SetResult(false);
                    }
                }
            },
            awaiter.CancellationTokenSource.Token);
    }

    private class Awaiter
    {
        public Awaiter(TaskCompletionSource<bool> taskCompletionSource, CancellationTokenSource cancellationTokenSource)
        {
            this.TaskCompletionSource = taskCompletionSource;
            this.CancellationTokenSource = cancellationTokenSource;
        }

        public TaskCompletionSource<bool> TaskCompletionSource { get; }

        public CancellationTokenSource CancellationTokenSource { get; }
    }
}