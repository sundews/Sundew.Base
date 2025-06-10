// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LockResult.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading;

using System;

/// <summary>
/// The result of an <see cref="AsyncLock"/> which validates whether the lock was acquired.
/// </summary>
/// <seealso cref="IDisposable" />
public sealed class LockResult : IDisposable
{
    private readonly AsyncLock asyncLock;
    private readonly bool hasLock;
    private readonly long ownerId;
    private bool isConfirmed;

    /// <summary>
    /// Initializes a new instance of the <see cref="LockResult"/> class.
    /// </summary>
    /// <param name="asyncLock">The async lock.</param>
    /// <param name="hasLock">if set to <c>true</c> [has lock].</param>
    /// <param name="ownerId">The owner id.</param>
    internal LockResult(AsyncLock asyncLock, bool hasLock, long ownerId)
    {
        this.asyncLock = asyncLock;
        this.hasLock = hasLock;
        this.ownerId = ownerId;
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="LockResult"/> to <see cref="bool"/>.
    /// </summary>
    /// <param name="lockResult">The lock result.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    public static implicit operator bool(LockResult lockResult)
    {
        return lockResult.Check();
    }

    /// <summary>
    /// Checks whether this instance has the lock.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if this instance has lock; otherwise, <c>false</c>.
    /// </returns>
    public bool Check()
    {
        this.isConfirmed = true;
        return this.hasLock;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <exception cref="LockNotConfirmedException">Thrown if the user forgot to call Check().</exception>
    public void Dispose()
    {
        if (this.hasLock)
        {
            this.asyncLock.InternalRelease(this.ownerId);
        }

        if (!this.isConfirmed)
        {
            throw new LockNotConfirmedException();
        }
    }
}