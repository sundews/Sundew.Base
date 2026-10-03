// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INotifier.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Notifications;

using System.Threading;
using System.Threading.Tasks;
using Sundew.Base.Collections;

/// <summary>
/// Interface for implementing an event.
/// </summary>
/// <typeparam name="TEvent">The type of the event data passed to event handlers.</typeparam>
public interface INotifier<in TEvent>
{
    /// <summary>
    /// Invokes all registered event handlers for the specified event, using the provided parallelism settings.
    /// </summary>
    /// <remarks>If cancellation is requested via the cancellation token, not all handlers may be invoked. The
    /// degree of parallelism is determined by the specified parallelism parameter.</remarks>
    /// <param name="event">The event data to pass to each handler.</param>
    /// <param name="parallelism">The parallelism options that control how event handlers are invoked concurrently.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    void Raise(TEvent @event, Parallelism parallelism = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invokes all registered event handlers for the specified event, using the provided parallelism settings.
    /// </summary>
    /// <remarks>If cancellation is requested via the cancellation token, not all handlers may be invoked. The
    /// degree of parallelism is determined by the specified parallelism parameter.</remarks>
    /// <param name="event">The event data to pass to each handler.</param>
    /// <param name="parallelism">The parallelism options that control how event handlers are invoked concurrently.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A ValueTask that represents the asynchronous operation of invoking all event handlers.</returns>
    ValueTask RaiseAsync(TEvent @event, Parallelism parallelism = default, CancellationToken cancellationToken = default);
}