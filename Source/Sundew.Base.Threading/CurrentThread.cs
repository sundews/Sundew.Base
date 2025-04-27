// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CurrentThread.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading;

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
        Thread.Sleep(timeSpan);
    }

    /// <summary>
    /// Sleeps the specified milliseconds.
    /// </summary>
    /// <param name="milliseconds">The milliseconds.</param>
    public void Sleep(int milliseconds)
    {
        Thread.Sleep(milliseconds);
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
    /// <returns><c>true</c>, if sleep completed, otherwise <c>false</c>.</returns>
    public bool Sleep(TimeSpan timeSpan, CancellationToken cancellationToken)
    {
        var cancelSignal = new object();
        lock (cancelSignal)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            using var cancellationTokenRegistration = cancellationToken.Register(() =>
            {
                lock (cancelSignal)
                {
                    Monitor.Pulse(cancelSignal);
                }
            });

            var completedWait = Monitor.Wait(cancelSignal, timeSpan);
            return !completedWait;
        }
    }

    /// <summary>
    /// Sleeps the specified milliseconds.
    /// </summary>
    /// <param name="milliseconds">The milliseconds.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c>, if sleep was completed, otherwise <c>false</c>.</returns>
    public bool Sleep(int milliseconds, CancellationToken cancellationToken)
    {
        return this.Sleep(TimeSpan.FromMilliseconds(milliseconds), cancellationToken);
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