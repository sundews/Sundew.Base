// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncLock.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading;

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Enables async locks.
/// </summary>
/// <seealso cref="IDisposable" />
public sealed class AsyncLock : IDisposable
{
    private readonly SemaphoreSlim semaphoreSlim = new(1, 1);

    private long owner = 0;

    /// <summary>
    /// Waits asynchronously to acquire the lock.
    /// </summary>
    /// <returns>
    /// An async task with a lock result.
    /// </returns>
    public Task<LockResult> TryLockAsync()
    {
        return this.TryLockAsync(CancellationToken.None);
    }

    /// <summary>
    /// Waits asynchronously to acquire the lock.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// An async task with a lock result.
    /// </returns>
    public Task<LockResult> TryLockAsync(CancellationToken cancellationToken)
    {
        return this.TryLockAsync(Timeout.InfiniteTimeSpan, cancellationToken);
    }

    /// <summary>
    /// Waits asynchronously to acquire the lock.
    /// </summary>
    /// <param name="timeoutTimeSpan">The timeout time span.</param>
    /// <returns>
    /// An async task with a lock result.
    /// </returns>
    public Task<LockResult> TryLockAsync(TimeSpan timeoutTimeSpan)
    {
        return this.TryLockAsync(timeoutTimeSpan, CancellationToken.None);
    }

    /// <summary>
    /// Waits asynchronously to acquire the lock.
    /// </summary>
    /// <param name="timeoutTimeSpan">The timeout time span.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// An async task with a lock result.
    /// </returns>
    public async Task<LockResult> TryLockAsync(TimeSpan timeoutTimeSpan, CancellationToken cancellationToken)
    {
        if (this.IsOwned())
        {
            Interlocked.Increment(ref this.owner);
            return new LockResult(this, true);
        }

        var result = await this.semaphoreSlim.WaitAsync(timeoutTimeSpan, cancellationToken).ConfigureAwait(false);
        if (result)
        {
            this.InitialOwner();
        }

        return new LockResult(this, result);
    }

    /// <summary>
    /// Waits asynchronously to acquire the lock.
    /// </summary>
    /// <returns>
    /// An async task with a lock result.
    /// </returns>
    public Task<IDisposable> LockAsync()
    {
        return this.LockAsync(CancellationToken.None);
    }

    /// <summary>
    /// Waits asynchronously to acquire the lock.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// An async task with a lock result.
    /// </returns>
    public Task<IDisposable> LockAsync(CancellationToken cancellationToken)
    {
        return this.LockAsync(Timeout.InfiniteTimeSpan, cancellationToken);
    }

    /// <summary>
    /// Waits asynchronously to acquire the lock.
    /// </summary>
    /// <param name="timeoutTimeSpan">The timeout time span.</param>
    /// <returns>
    /// An async task with a lock result.
    /// </returns>
    public Task<IDisposable> LockAsync(TimeSpan timeoutTimeSpan)
    {
        return this.LockAsync(timeoutTimeSpan, CancellationToken.None);
    }

    /// <summary>
    /// Waits asynchronously to acquire the lock.
    /// </summary>
    /// <param name="timeoutTimeSpan">The timeout time span.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// An async task with a lock result.
    /// </returns>
    /// <exception cref="LockNotAcquiredException">Thrown if the lock could not be acquired.</exception>
    public async Task<IDisposable> LockAsync(TimeSpan timeoutTimeSpan, CancellationToken cancellationToken)
    {
        if (this.IsOwned())
        {
            Interlocked.Increment(ref this.owner);
            return new LockDisposer(this);
        }

        var result = await this.semaphoreSlim.WaitAsync(timeoutTimeSpan, cancellationToken).ConfigureAwait(false);
        if (result)
        {
            this.InitialOwner();
        }
        else
        {
            throw new LockNotAcquiredException();
        }

        return new LockDisposer(this);
    }

    /// <summary>
    /// Waits synchronously to acquire the lock.
    /// </summary>
    /// <returns>An async task with a lock result.</returns>
    public LockResult TryLock()
    {
        return this.TryLock(CancellationToken.None);
    }

    /// <summary>
    /// Waits synchronously to acquire the lock.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// An async task with a lock result.
    /// </returns>
    public LockResult TryLock(CancellationToken cancellationToken)
    {
        return this.TryLock(Timeout.InfiniteTimeSpan, cancellationToken);
    }

    /// <summary>
    /// Waits synchronously to acquire the lock.
    /// </summary>
    /// <param name="timeoutTimeSpan">The timeout time span.</param>
    /// <returns>
    /// An async task with a lock result.
    /// </returns>
    public LockResult TryLock(TimeSpan timeoutTimeSpan)
    {
        return this.TryLock(timeoutTimeSpan, CancellationToken.None);
    }

    /// <summary>
    /// Waits synchronously to acquire the lock.
    /// </summary>
    /// <param name="timeoutTimeSpan">The timeout time span.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A lock result.</returns>
    public LockResult TryLock(TimeSpan timeoutTimeSpan, CancellationToken cancellationToken)
    {
        if (this.IsOwned())
        {
            Interlocked.Increment(ref this.owner);
            return new LockResult(this, true);
        }

        var result = this.semaphoreSlim.Wait(timeoutTimeSpan, cancellationToken);
        if (result)
        {
            this.InitialOwner();
        }

        return new LockResult(this, result);
    }

    /// <summary>
    /// Waits synchronously to acquire the lock.
    /// </summary>
    /// <returns>An async task with a lock result.</returns>
    public IDisposable Lock()
    {
        return this.Lock(CancellationToken.None);
    }

    /// <summary>
    /// Waits synchronously to acquire the lock.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// An async task with a lock result.
    /// </returns>
    public IDisposable Lock(CancellationToken cancellationToken)
    {
        return this.Lock(Timeout.InfiniteTimeSpan, cancellationToken);
    }

    /// <summary>
    /// Waits synchronously to acquire the lock.
    /// </summary>
    /// <param name="timeoutTimeSpan">The timeout time span.</param>
    /// <returns>
    /// An async task with a lock result.
    /// </returns>
    public IDisposable Lock(TimeSpan timeoutTimeSpan)
    {
        return this.Lock(timeoutTimeSpan, CancellationToken.None);
    }

    /// <summary>
    /// Waits synchronously to acquire the lock.
    /// </summary>
    /// <param name="timeoutTimeSpan">The timeout time span.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A lock result.</returns>
    public IDisposable Lock(TimeSpan timeoutTimeSpan, CancellationToken cancellationToken)
    {
        var result = this.semaphoreSlim.Wait(timeoutTimeSpan, cancellationToken);
        if (!result)
        {
            throw new LockNotAcquiredException();
        }

        return new LockDisposer(this);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.semaphoreSlim.Dispose();
    }

    internal void InternalRelease()
    {
        var entries = Interlocked.Decrement(ref this.owner);
        if ((int)(entries & 0xFFFFFFFF) == 0)
        {
            Interlocked.Exchange(ref this.owner, 0);
            this.semaphoreSlim.Release();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | (MethodImplOptions)512)]
    private void InitialOwner()
    {
        Interlocked.Exchange(ref this.owner, ((long)Thread.CurrentThread.ManagedThreadId << 32) | 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | (MethodImplOptions)512)]
    private bool IsOwned()
    {
        int upperInt = (int)(this.owner >> 32);
        return upperInt == Thread.CurrentThread.ManagedThreadId;
    }

    private class LockDisposer : IDisposable
    {
        private readonly AsyncLock asyncLock;

        public LockDisposer(AsyncLock asyncLock)
        {
            this.asyncLock = asyncLock;
        }

        public void Dispose()
        {
            this.asyncLock.InternalRelease();
        }
    }
}