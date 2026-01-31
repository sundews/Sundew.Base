// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManualResetEventAsync.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Wrapper around <see cref="Microsoft.VisualStudio.Threading.AsyncManualResetEvent"/> with additional WaitAsync methods.
/// </summary>
public sealed class ManualResetEventAsync
{
    private readonly Microsoft.VisualStudio.Threading.AsyncManualResetEvent manualResetEventAsync;

    /// <summary>
    /// Initializes a new instance of the <see cref="ManualResetEventAsync"/> class.
    /// </summary>
    public ManualResetEventAsync()
    {
        this.manualResetEventAsync = new Microsoft.VisualStudio.Threading.AsyncManualResetEvent();
    }

    /// <summary>
    /// Gets a value indicating whether the asynchronous manual reset event is currently set.
    /// </summary>
    /// <remarks>Use this property to determine if the event is in the signaled state, allowing awaiting
    /// operations to proceed without blocking. The value reflects the current state and may change if the event is set
    /// or reset by other operations.</remarks>
    public bool IsSet => this.manualResetEventAsync.IsSet;

    /// <summary>
    /// Sets the state of the asynchronous auto-reset event, allowing one waiting operation to proceed.
    /// </summary>
    /// <remarks>Calling this property signals the underlying event, releasing a single waiting asynchronous
    /// operation. If no operations are currently waiting, the event remains set until an operation waits. Multiple
    /// calls to this property before a wait will not accumulate signals; only one waiting operation will be released
    /// per set.</remarks>
    public void Set() => this.manualResetEventAsync.Set();

    /// <summary>
    /// Resets the state of the asynchronous manual reset event to unsignaled, causing threads that wait on the event to
    /// block until it is set again.
    /// </summary>
    /// <remarks>Use this method to reinitialize the event after it has been signaled, allowing it to be
    /// reused for subsequent synchronization operations. After calling this method, any threads that call wait methods
    /// on the event will block until the event is set.</remarks>
    public void Reset() => this.manualResetEventAsync.Reset();

    /// <summary>
    /// Signals all threads that are currently waiting, allowing them to proceed.
    /// </summary>
    /// <remarks>Use this method when multiple threads are waiting for a condition to be met and should be
    /// released simultaneously. Ensure that shared resources are properly synchronized to prevent race conditions when
    /// multiple threads resume execution.</remarks>
    public void PulseAll() => this.manualResetEventAsync.PulseAll();

    /// <summary>
    /// Asynchronously waits until the event is set or the operation is canceled.
    /// </summary>
    /// <remarks>If the cancellation token is canceled before the event is set, the returned task will be
    /// canceled. This method is thread-safe and can be awaited by multiple callers concurrently.</remarks>
    /// <returns>A value task that represents the asynchronous wait operation. The task completes when the event is set or the
    /// cancellation token is triggered.</returns>
    public Task<bool> WaitAsync()
    {
        return this.WaitAsync(CancellationToken.None);
    }

    /// <summary>
    /// Asynchronously waits until the event is set or the operation is canceled.
    /// </summary>
    /// <remarks>If the cancellation token is canceled before the event is set, the returned task will be
    /// canceled. This method is thread-safe and can be awaited by multiple callers concurrently.</remarks>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the wait operation before the event is set.</param>
    /// <returns>A value task that represents the asynchronous wait operation. The task completes when the event is set or the
    /// cancellation token is triggered.</returns>
    public async Task<bool> WaitAsync(CancellationToken cancellationToken)
    {
        try
        {
            await this.manualResetEventAsync.WaitAsync(cancellationToken).ConfigureAwait(false);
            return !cancellationToken.IsCancellationRequested;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }

    /// <summary>
    /// Asynchronously waits for the specified asynchronous auto-reset event to be set, with support for cancellation.
    /// </summary>
    /// <remarks>This method enables cancellation support for waiting on an <see cref="AutoResetEventAsync"/>,
    /// allowing the operation to be aborted if the provided cancellation token is triggered.</remarks>
    /// <param name="cancellation">A cancellation token that can be used to cancel the wait operation.</param>
    /// <returns>A task that represents the asynchronous wait operation. The task result is <see langword="true"/> if the wait
    /// was canceled; otherwise, <see langword="false"/>.</returns>
    public async Task<bool> WaitAsync(Cancellation cancellation)
    {
        using var enabler = cancellation.EnableCancellation();
        try
        {
            await this.manualResetEventAsync.WaitAsync(enabler.Token);
            return !enabler.Token.IsCancellationRequested;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }

    /// <summary>
    /// Asynchronously waits for the specified time span for the event to be signaled.
    /// </summary>
    /// <remarks>This method uses a cancellation token to manage the wait operation. If the wait is canceled
    /// before the event is signaled, the method returns true.</remarks>
    /// <param name="timeSpan">The maximum duration to wait for the event to be signaled before timing out.</param>
    /// <returns>A task that represents the asynchronous wait operation. The task result is true if the wait was canceled;
    /// otherwise, false.</returns>
    public async Task<bool> WaitAsync(TimeSpan timeSpan)
    {
        using var cancellationTokenSource = new CancellationTokenSource(timeSpan);
        try
        {
            await this.manualResetEventAsync.WaitAsync(cancellationTokenSource.Token);
            return !cancellationTokenSource.IsCancellationRequested;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }
}