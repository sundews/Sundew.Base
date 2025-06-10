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
    private const long Attempts = 3;

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
        var currentOwnerCount = this.owner & 0xFFFF_FFFF;
        var ownerId = (long)Thread.CurrentThread.ManagedThreadId << 32;
        var expectedOwner = ownerId | currentOwnerCount;
        var newOwner = ownerId | currentOwnerCount + 1;
        if ((newOwner & 0xFFFF_FFFF) == 0)
        {
        }

        var oldOwner = Interlocked.CompareExchange(ref this.owner, newOwner, expectedOwner);
        if (oldOwner == expectedOwner)
        {
            return new LockResult(this, true, ownerId);
        }

        var eachAttemptTimeout = TimeSpan.FromTicks(timeoutTimeSpan.Ticks / Attempts);
        if (timeoutTimeSpan < eachAttemptTimeout)
        {
            var result = await this.semaphoreSlim.WaitAsync(timeoutTimeSpan, cancellationToken).ConfigureAwait(false);
            if (result)
            {
                this.InitialOwner(ownerId);
                return new LockResult(this, true, ownerId);
            }

            return new LockResult(this, false, ownerId);
        }

        var remainingTimeout = timeoutTimeSpan;
        while (remainingTimeout > eachAttemptTimeout)
        {
            var result = await this.semaphoreSlim.WaitAsync(eachAttemptTimeout, cancellationToken).ConfigureAwait(false);
            if (result)
            {
                this.InitialOwner(ownerId);
                return new LockResult(this, true, ownerId);
            }

            remainingTimeout -= eachAttemptTimeout;
        }

        if (remainingTimeout > TimeSpan.Zero)
        {
            var result = await this.semaphoreSlim.WaitAsync(remainingTimeout, cancellationToken).ConfigureAwait(false);
            if (result)
            {
                this.InitialOwner(ownerId);
                return new LockResult(this, true, ownerId);
            }
        }

        return new LockResult(this, false, ownerId);
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
        var (ownerId, hasLock) = this.UpdateOwner();
        if (hasLock)
        {
            return new LockDisposer(this, ownerId);
        }

        var eachAttemptTimeout = TimeSpan.FromTicks(timeoutTimeSpan.Ticks / Attempts);
        if (timeoutTimeSpan < eachAttemptTimeout)
        {
            var result = await this.semaphoreSlim.WaitAsync(timeoutTimeSpan, cancellationToken).ConfigureAwait(false);
            if (result)
            {
                this.InitialOwner(ownerId);
                return new LockDisposer(this, ownerId);
            }

            return new LockDisposer(this, ownerId);
        }

        var remainingTimeout = timeoutTimeSpan;
        while (remainingTimeout > eachAttemptTimeout)
        {
            var result = await this.semaphoreSlim.WaitAsync(eachAttemptTimeout, cancellationToken).ConfigureAwait(false);
            if (result)
            {
                this.InitialOwner(ownerId);
                return new LockDisposer(this, ownerId);
            }

            remainingTimeout -= eachAttemptTimeout;
        }

        if (remainingTimeout > TimeSpan.Zero)
        {
            var result = await this.semaphoreSlim.WaitAsync(remainingTimeout, cancellationToken).ConfigureAwait(false);
            if (result)
            {
                this.InitialOwner(ownerId);
                return new LockDisposer(this, ownerId);
            }
        }

        throw new LockNotAcquiredException();
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
        var (ownerId, hasLock) = this.UpdateOwner();
        if (hasLock)
        {
            return new LockResult(this, true, ownerId);
        }

        var eachAttemptTimeout = TimeSpan.FromTicks(timeoutTimeSpan.Ticks / Attempts);
        if (timeoutTimeSpan < eachAttemptTimeout)
        {
            var result = this.semaphoreSlim.Wait(timeoutTimeSpan, cancellationToken);
            if (result)
            {
                this.InitialOwner(ownerId);
                return new LockResult(this, true, ownerId);
            }

            return new LockResult(this, false, ownerId);
        }

        var remainingTimeout = timeoutTimeSpan;
        while (remainingTimeout > eachAttemptTimeout)
        {
            var result = this.semaphoreSlim.Wait(eachAttemptTimeout, cancellationToken);
            if (result)
            {
                this.InitialOwner(ownerId);
                return new LockResult(this, true, ownerId);
            }

            remainingTimeout -= eachAttemptTimeout;
        }

        if (remainingTimeout > TimeSpan.Zero)
        {
            var result = this.semaphoreSlim.Wait(remainingTimeout, cancellationToken);
            if (result)
            {
                this.InitialOwner(ownerId);
                return new LockResult(this, true, ownerId);
            }
        }

        return new LockResult(this, false, ownerId);
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
        var (ownerId, hasLock) = this.UpdateOwner();
        if (hasLock)
        {
            return new LockResult(this, true, ownerId);
        }

        var result = this.semaphoreSlim.Wait(timeoutTimeSpan, cancellationToken);
        if (!result)
        {
            throw new LockNotAcquiredException();
        }

        this.InitialOwner(ownerId);
        return new LockDisposer(this, ownerId);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.semaphoreSlim.Dispose();
    }

    internal void InternalRelease(long ownerId)
    {
        var currentOwnerCount = this.owner & 0xFFFF_FFFF;
        var expectedOwner = ownerId | currentOwnerCount;
        var newOwner = currentOwnerCount <= 1 ? 0 : ownerId | currentOwnerCount - 1;
        var oldOwner = Interlocked.CompareExchange(ref this.owner, newOwner, expectedOwner);
        if (oldOwner == (ownerId | 1) || oldOwner == ownerId)
        {
            this.semaphoreSlim.Release();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | (MethodImplOptions)512)]
    private void InitialOwner(long ownerId)
    {
        Interlocked.Exchange(ref this.owner, ownerId | 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | (MethodImplOptions)512)]
    private (long OwnerId, bool HasLock) UpdateOwner()
    {
        var currentOwnerCount = this.owner & 0xFFFF_FFFF;
        var ownerId = (long)Thread.CurrentThread.ManagedThreadId << 32;
        var previousOwner = ownerId | currentOwnerCount;
        var expectedOwner = currentOwnerCount == 0 ? 0 : previousOwner;
        var newOwner = ownerId | currentOwnerCount + 1;
        var oldOwner = Interlocked.CompareExchange(ref this.owner, newOwner, expectedOwner);
        return (ownerId, oldOwner == previousOwner);
    }

    private class LockDisposer : IDisposable
    {
        private readonly AsyncLock asyncLock;
        private readonly long ownerId;

        public LockDisposer(AsyncLock asyncLock, long ownerId)
        {
            this.asyncLock = asyncLock;
            this.ownerId = ownerId;
        }

        public void Dispose()
        {
            this.asyncLock.InternalRelease(this.ownerId);
        }
    }
}