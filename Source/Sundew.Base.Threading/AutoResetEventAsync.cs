// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoResetEventAsync.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Wrapper around <see cref="Microsoft.VisualStudio.Threading.AsyncAutoResetEvent"/> with additional WaitAsync methods.
/// </summary>
public sealed class AutoResetEventAsync
{
    private readonly Microsoft.VisualStudio.Threading.AsyncAutoResetEvent autoResetEventAsync;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoResetEventAsync"/> class.
    /// </summary>
    public AutoResetEventAsync()
    {
        this.autoResetEventAsync = new Microsoft.VisualStudio.Threading.AsyncAutoResetEvent();
    }

    /// <summary>
    /// Sets the state of the asynchronous auto-reset event, allowing one waiting operation to proceed.
    /// </summary>
    /// <remarks>Calling this property signals the underlying event, releasing a single waiting asynchronous
    /// operation. If no operations are currently waiting, the event remains set until an operation waits. Multiple
    /// calls to this property before a wait will not accumulate signals; only one waiting operation will be released
    /// per set.</remarks>
    public void Set() => this.autoResetEventAsync.Set();

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
            await this.autoResetEventAsync.WaitAsync(cancellationToken).ConfigureAwait(false);
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
            await this.autoResetEventAsync.WaitAsync(enabler.Token);
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
            await this.autoResetEventAsync.WaitAsync(cancellationTokenSource.Token);
            return !cancellationTokenSource.IsCancellationRequested;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }
}