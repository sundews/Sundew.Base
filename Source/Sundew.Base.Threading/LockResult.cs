// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LockResult.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading
{
    using System;
    using System.Threading;

    /// <summary>
    /// The result of an <see cref="AsyncLock"/> which validates whether the lock was acquired.
    /// </summary>
    /// <seealso cref="IDisposable" />
    public sealed class LockResult : IDisposable
    {
        private readonly SemaphoreSlim? semaphoreSlim;
        private readonly bool hasLock;
        private bool isConfirmed;

        /// <summary>
        /// Initializes a new instance of the <see cref="LockResult"/> class.
        /// </summary>
        /// <param name="hasLock">if set to <c>true</c> [has lock].</param>
        public LockResult(bool hasLock)
        {
            this.hasLock = hasLock;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LockResult"/> class.
        /// </summary>
        /// <param name="semaphoreSlim">The semaphore slim.</param>
        /// <param name="hasLock">if set to <c>true</c> the lock was acquired.</param>
        internal LockResult(SemaphoreSlim? semaphoreSlim, bool hasLock)
        {
            this.hasLock = hasLock;
            this.semaphoreSlim = semaphoreSlim;
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
                this.semaphoreSlim?.Release();
            }

            if (!this.isConfirmed)
            {
                throw new LockNotConfirmedException();
            }
        }
    }
}