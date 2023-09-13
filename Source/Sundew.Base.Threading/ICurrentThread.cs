// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICurrentThread.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Interface for accessing the current thread.
/// </summary>
public interface ICurrentThread
{
    /// <summary>
    /// Gets the managed thread identifier.
    /// </summary>
    /// <value>
    /// The managed thread identifier.
    /// </value>
    int ManagedThreadId { get; }

    /// <summary>
    /// Sleeps for the specified time span.
    /// </summary>
    /// <param name="timeSpan">The time span.</param>
    void Sleep(TimeSpan timeSpan);

    /// <summary>
    /// Sleeps for the specified milliseconds.
    /// </summary>
    /// <param name="milliseconds">The milliseconds.</param>
    void Sleep(int milliseconds);

    /// <summary>
    /// Delays for the specified time span.
    /// </summary>
    /// <param name="timeSpan">The time span.</param>
    /// <returns>An async task.</returns>
    Task Delay(TimeSpan timeSpan);

    /// <summary>
    /// Delays the specified milliseconds.
    /// </summary>
    /// <param name="milliseconds">The milliseconds.</param>
    /// <returns>An async task.</returns>
    Task Delay(int milliseconds);

    /// <summary>
    /// Sleeps for the specified time span.
    /// </summary>
    /// <param name="timeSpan">The time span.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    void Sleep(TimeSpan timeSpan, CancellationToken cancellationToken);

    /// <summary>
    /// Sleeps the specified milliseconds.
    /// </summary>
    /// <param name="milliseconds">The milliseconds.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    void Sleep(int milliseconds, CancellationToken cancellationToken);

    /// <summary>
    /// Delays for the specified time span.
    /// </summary>
    /// <param name="timeSpan">The time span.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An async task.</returns>
    Task Delay(TimeSpan timeSpan, CancellationToken cancellationToken);

    /// <summary>
    /// Delays the specified milliseconds.
    /// </summary>
    /// <param name="milliseconds">The milliseconds.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An async task.</returns>
    Task Delay(int milliseconds, CancellationToken cancellationToken);
}