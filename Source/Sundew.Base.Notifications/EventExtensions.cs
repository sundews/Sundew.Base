// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Notifications;

using System.Threading;
using System.Threading.Tasks;
using Sundew.Base.Collections;

/// <summary>
/// Provides extension methods for the <see cref="IEvent{TEvent}"/> interface.
/// </summary>
public static class EventExtensions
{
    /// <summary>
    /// Extends events.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    extension<TEvent>(IEvent<TEvent> @event)
    {
        /// <summary>
        /// Invokes all registered event handlers for the specified event.
        /// </summary>
        /// <remarks>If cancellation is requested via the cancellation token, not all handlers may be invoked. The
        /// degree of parallelism is determined by the specified parallelism parameter.</remarks>
        /// <param name="actualEvent">The event data to pass to each handler.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        public void Raise(TEvent actualEvent, CancellationToken cancellationToken)
        {
#pragma warning disable SA1101
            @event.Raise(@actualEvent, Parallelism.None, cancellationToken);
#pragma warning restore SA1101
        }

        /// <summary>
        /// Invokes all registered event handlers for the specified event.
        /// </summary>
        /// <remarks>If cancellation is requested via the cancellation token, not all handlers may be invoked. The
        /// degree of parallelism is determined by the specified parallelism parameter.</remarks>
        /// <param name="actualEvent">The event data to pass to each handler.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A ValueTask that represents the asynchronous operation of invoking all event handlers.</returns>
        public async ValueTask RaiseAsync(TEvent actualEvent, CancellationToken cancellationToken)
        {
#pragma warning disable SA1101
            await @event.RaiseAsync(@actualEvent, Parallelism.None, cancellationToken);
#pragma warning restore SA1101
        }
    }
}