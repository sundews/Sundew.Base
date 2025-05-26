// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResetEventAsync.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading;

using System;
using System.Threading;
using System.Threading.Tasks;
using Sundew.Base.Threading.Internal;

/// <summary>
/// Abstract base class for implementing async reset events.
/// </summary>
public abstract class ResetEventAsync
{
#pragma warning disable SA1401
    // ReSharper disable InconsistentNaming
#if NET9_0_OR_GREATER
    private protected readonly Lock lockObject = new();
#else
    private protected readonly object lockObject = new();
#endif
#pragma warning disable SA1306
    private protected bool privateIsSet;
#pragma warning restore SA1306
#pragma warning restore SA1401
    // ReSharper restore InconsistentNaming

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
        this.privateIsSet = isSet;
    }

    /// <summary>
    /// Gets a value indicating whether this instance is set.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is set; otherwise, <c>false</c>.
    /// </value>
    public bool IsSet
    {
        get
        {
            lock (this.lockObject)
            {
                return this.privateIsSet;
            }
        }
    }

    /// <summary>
    /// Reset this instance.
    /// </summary>
    public void Reset()
    {
        lock (this.lockObject)
        {
            this.privateIsSet = false;
            this.OnResetWhileLocked();
        }
    }

    /// <summary>
    /// Sets this instance.
    /// </summary>
    public abstract void Set();

    /// <summary>
    /// Waits synchronously.
    /// </summary>
    /// <returns>A boolean result indicating whether the signal was received.</returns>
    public bool Wait()
    {
        return this.Wait(Timeout.InfiniteTimeSpan, CancellationToken.None);
    }

    /// <summary>
    /// Waits synchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A boolean result indicating whether the signal was received.</returns>
    public bool Wait(CancellationToken cancellationToken)
    {
        return this.Wait(Timeout.InfiniteTimeSpan, cancellationToken);
    }

    /// <summary>
    /// Waits synchronously.
    /// </summary>
    /// <param name="timeout">The timeout.</param>
    /// <returns>A boolean result indicating whether the signal was received.</returns>
    public bool Wait(TimeSpan timeout)
    {
        return this.Wait(timeout, CancellationToken.None);
    }

    /// <summary>
    /// Waits synchronously.
    /// </summary>
    /// <param name="timeout">The timeout.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A boolean result indicating whether the signal was received.</returns>
    public bool Wait(TimeSpan timeout, CancellationToken cancellationToken)
    {
        var waitTask = this.WaitAsync(new Cancellation(timeout, cancellationToken));
        waitTask.Wait(timeout);
        return waitTask.Result;
    }

    /// <summary>
    /// Waits asynchronously.
    /// </summary>
    /// <returns>A task with a boolean result indicating whether the signal was received.</returns>
    public Task<bool> WaitAsync()
    {
        return this.WaitAsync(new Cancellation(Timeout.InfiniteTimeSpan, CancellationToken.None));
    }

    /// <summary>
    /// Waits asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task with a boolean result indicating whether the signal was received.</returns>
    public Task<bool> WaitAsync(CancellationToken cancellationToken)
    {
        return this.WaitAsync(new Cancellation(Timeout.InfiniteTimeSpan, cancellationToken));
    }

    /// <summary>
    /// Waits asynchronously.
    /// </summary>
    /// <param name="timeout">The timeout.</param>
    /// <returns>A task with a boolean result indicating whether the signal was received.</returns>
    public Task<bool> WaitAsync(TimeSpan timeout)
    {
        return this.WaitAsync(new Cancellation(timeout, CancellationToken.None));
    }

    /// <summary>
    /// Waits asynchronously.
    /// </summary>
    /// <param name="cancellation">The cancellation.</param>
    /// <returns>A task with a boolean result indicating whether the signal was received.</returns>
    public Task<bool> WaitAsync(Cancellation cancellation)
    {
        lock (this.lockObject)
        {
            if (this.privateIsSet)
            {
                this.OnIsSetDuringWaitWhileLocked(ref this.privateIsSet);
                return TaskHelper.CompletedTrueTask;
            }

            var timeout = cancellation.Timeout;
            var externalToken = cancellation.Token;
            if (timeout == TimeSpan.Zero || externalToken.IsCancellationRequested)
            {
                return TaskHelper.CompletedFalseTask;
            }

            return this.ConfigureAwaiterWhileLocked(cancellation);
        }
    }

    /// <summary>
    /// Called during a Wait or WaitAsync call when the event is set and the lock is acquired.
    /// </summary>
    /// <param name="isSet">if set to <c>true</c> [is set].</param>
    private protected abstract void OnIsSetDuringWaitWhileLocked(ref bool isSet);

    private protected abstract Task<bool> ConfigureAwaiterWhileLocked(Cancellation linkedCancellation);

    private protected virtual void OnResetWhileLocked()
    {
    }
}