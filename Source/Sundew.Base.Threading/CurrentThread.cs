// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CurrentThread.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Implementation of <see cref="ICurrentThread"/> for access to the current thread with support for cancellation.
    /// </summary>
    /// <seealso cref="Sundew.Base.Threading.ICurrentThread" />
    public sealed class CurrentThread : ICurrentThread
    {
        /// <summary>
        /// Gets the managed thread identifier.
        /// </summary>
        /// <value>
        /// The managed thread identifier.
        /// </value>
        public int ManagedThreadId => Environment.CurrentManagedThreadId;

        /// <summary>
        /// Sleeps the specified time span.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        public void Sleep(TimeSpan timeSpan)
        {
            var neverExitedObject = new object();
            Monitor.Enter(neverExitedObject);
            Monitor.Wait(neverExitedObject, timeSpan);
            Monitor.Exit(neverExitedObject);
        }

        /// <summary>
        /// Sleeps the specified milliseconds.
        /// </summary>
        /// <param name="milliseconds">The milliseconds.</param>
        public void Sleep(int milliseconds)
        {
            var neverExitedObject = new object();
            Monitor.Enter(neverExitedObject);
            Monitor.Wait(neverExitedObject, milliseconds);
            Monitor.Exit(neverExitedObject);
        }

        /// <summary>
        /// Delays the specified time span.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <returns>An async task.</returns>
        public Task Delay(TimeSpan timeSpan)
        {
            return Task.Delay(timeSpan);
        }

        /// <summary>
        /// Delays the specified milliseconds.
        /// </summary>
        /// <param name="milliseconds">The milliseconds.</param>
        /// <returns>An async task.</returns>
        public Task Delay(int milliseconds)
        {
            return Task.Delay(milliseconds);
        }

        /// <summary>
        /// Sleeps the specified time span.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public void Sleep(TimeSpan timeSpan, CancellationToken cancellationToken)
        {
            var cancelSignal = new object();
            Monitor.Enter(cancelSignal);
            var cancellationTokenRegistration = cancellationToken.Register(() =>
            {
                Monitor.Enter(cancelSignal);
                Monitor.Pulse(cancelSignal);
                Monitor.Exit(cancelSignal);
            });
            Monitor.Wait(cancelSignal, timeSpan);
            Monitor.Exit(cancelSignal);
            cancellationTokenRegistration.Dispose();
        }

        /// <summary>
        /// Sleeps the specified milliseconds.
        /// </summary>
        /// <param name="milliseconds">The milliseconds.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public void Sleep(int milliseconds, CancellationToken cancellationToken)
        {
            this.Sleep(TimeSpan.FromMilliseconds(milliseconds), cancellationToken);
        }

        /// <summary>
        /// Delays the specified time span.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An async task.</returns>
        public Task Delay(TimeSpan timeSpan, CancellationToken cancellationToken)
        {
            return Task.Delay(timeSpan, cancellationToken);
        }

        /// <summary>
        /// Delays the specified milliseconds.
        /// </summary>
        /// <param name="milliseconds">The milliseconds.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An async task.</returns>
        public Task Delay(int milliseconds, CancellationToken cancellationToken)
        {
            return Task.Delay(milliseconds, cancellationToken);
        }
    }
}