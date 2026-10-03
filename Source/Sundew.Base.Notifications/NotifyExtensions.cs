// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotifyExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Notifications;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Provides extension methods for the <see cref="INotifyAny{TEvent}"/> interface.
/// </summary>
public static class NotifyExtensions
{
    /// <summary>
    /// Extends TValue with option-like functionality.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    extension<TEvent>(INotifyAny<TEvent> notifyAny)
    {
        /// <summary>
        /// Subscribes the specified handler to the notification target.
        /// </summary>
        /// <typeparam name="TSubscribedEvent">The type of event.</typeparam>
        /// <param name="handler">The handler.</param>
        /// <param name="notificationTarget">The notification target.</param>
        /// <returns>A subscription.</returns>
        public Subscription Subscribe<TSubscribedEvent>(
            Func<TSubscribedEvent, ValueTask> handler,
            INotificationTarget notificationTarget)
            where TSubscribedEvent : TEvent
        {
#pragma warning disable SA1101
            return notifyAny.Subscribe<TSubscribedEvent>(
                (@event, _, _) => handler(@event),
                notificationTarget);
#pragma warning restore SA1101
        }

        /// <summary>
        /// Subscribes the specified handler to the notification target.
        /// </summary>
        /// <typeparam name="TSubscribedEvent">The type of event.</typeparam>
        /// <param name="handler">The handler.</param>
        /// <param name="notificationTarget">The notification target.</param>
        /// <returns>A subscription.</returns>
        public Subscription Subscribe<TSubscribedEvent>(
            Func<TSubscribedEvent, CancellationToken, ValueTask> handler,
            INotificationTarget notificationTarget)
            where TSubscribedEvent : TEvent
        {
#pragma warning disable SA1101
            return notifyAny.Subscribe<TSubscribedEvent>(
                (@event, _, cancellationToken) => handler(@event, cancellationToken),
                notificationTarget);
#pragma warning restore SA1101
        }

        /// <summary>
        /// Subscribes the specified handler to the notification target.
        /// </summary>
        /// <typeparam name="TSubscribedEvent">The type of event.</typeparam>
        /// <param name="handler">The handler.</param>
        /// <param name="notificationTarget">The notification target.</param>
        /// <returns>A subscription.</returns>
        public Subscription Subscribe<TSubscribedEvent>(
            Func<TSubscribedEvent, Subscription, ValueTask> handler,
            INotificationTarget notificationTarget)
            where TSubscribedEvent : TEvent
        {
#pragma warning disable SA1101
            return notifyAny.Subscribe<TSubscribedEvent>(
                (@event, subscription, _) => handler(@event, subscription),
                notificationTarget);
#pragma warning restore SA1101
        }
    }

    /// <summary>
    /// Subscribes the specified handler to the notification target.
    /// </summary>
    /// <typeparam name="TEvent">The type of event.</typeparam>
    /// <param name="notifyAny">The notifyAny.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="notificationTarget">The notification target.</param>
    /// <returns>A subscription.</returns>
    public static Subscription Subscribe<TEvent>(
        this INotify<TEvent> notifyAny,
        Func<TEvent, ValueTask> handler,
        INotificationTarget notificationTarget)
    {
        return notifyAny.Subscribe(
            (@event, _, _) => handler(@event),
            notificationTarget);
    }

    /// <summary>
    /// Subscribes the specified handler to the notification target.
    /// </summary>
    /// <typeparam name="TEvent">The type of event.</typeparam>
    /// <param name="notifyAny">The notifyAny.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="notificationTarget">The notification target.</param>
    /// <returns>A subscription.</returns>
    public static Subscription Subscribe<TEvent>(
        this INotify<TEvent> notifyAny,
        Func<TEvent, CancellationToken, ValueTask> handler,
        INotificationTarget notificationTarget)
    {
        return notifyAny.Subscribe(
            (@event, _, cancellationToken) => handler(@event, cancellationToken),
            notificationTarget);
    }

    /// <summary>
    /// Subscribes the specified handler to the notification target.
    /// </summary>
    /// <typeparam name="TEvent">The type of event.</typeparam>
    /// <param name="notifyAny">The notifyAny.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="notificationTarget">The notification target.</param>
    /// <returns>A subscription.</returns>
    public static Subscription Subscribe<TEvent>(
        this INotify<TEvent> notifyAny,
        Func<TEvent, Subscription, ValueTask> handler,
        INotificationTarget notificationTarget)
    {
        return notifyAny.Subscribe(
            (@event, subscription, _) => handler(@event, subscription),
            notificationTarget);
    }
}