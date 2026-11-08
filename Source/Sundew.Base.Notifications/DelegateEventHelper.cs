// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelegateEventHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Notifications;

using System;
using System.Threading;
using System.Threading.Tasks;
using Sundew.Base.Collections;

/// <summary>
/// The pieces of <see cref="DelegateEvent{TEvent}"/> that do not depend on the event type.
/// </summary>
internal static class DelegateEventHelper
{
    /// <summary>
    /// Adapts a handler for a subscribed event type to the event type, so that it is only invoked for events of its type.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <typeparam name="TSubscribedEvent">The subscribed event type.</typeparam>
    /// <param name="handler">The handler.</param>
    /// <returns>A handler for the event type.</returns>
    public static Func<TEvent, Subscription, CancellationToken, ValueTask> Adapt<TEvent, TSubscribedEvent>(
        Func<TSubscribedEvent, Subscription, CancellationToken, ValueTask> handler)
        where TSubscribedEvent : TEvent
    {
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        return typeof(TSubscribedEvent) == typeof(TEvent)
            ? (Func<TEvent, Subscription, CancellationToken, ValueTask>)(object)handler
            : (@event, subscription, token) => @event is TSubscribedEvent subscribedEvent ? handler(subscribedEvent, subscription, token) : default;
    }

    /// <summary>
    /// Determines whether the parallelism asks for nothing the sequential path cannot provide: one handler at a time, no
    /// cancellation of the raise itself and no custom scheduler.
    /// </summary>
    /// <param name="parallelism">The parallelism.</param>
    /// <returns><see langword="true"/> if the handlers can be awaited one after the other on the calling context.</returns>
    public static bool IsSequential(Parallelism parallelism)
    {
        return parallelism.MaxDegreeOfParallelism <= 1
            && !parallelism.Cancellation.CanBeCanceled
            && (parallelism.TaskScheduler is null || parallelism.TaskScheduler == TaskScheduler.Default);
    }
}
