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
        where TEvent : class
    {
        /// <summary>
        /// Subscribes the specified handler to the notification target.
        /// </summary>
        /// <typeparam name="TSubscribedEvent">The type of event.</typeparam>
        /// <param name="notificationTarget">The notification target.</param>
        /// <param name="handler">The handler.</param>
        /// <returns>A subscription.</returns>
        public Subscription Subscribe<TSubscribedEvent>(
            INotificationTarget notificationTarget,
            Func<TSubscribedEvent, ValueTask> handler)
            where TSubscribedEvent : TEvent
        {
#pragma warning disable SA1101
            return notifyAny.Subscribe<TSubscribedEvent>(
                notificationTarget,
                (@event, _, _) => handler(@event));
#pragma warning restore SA1101
        }

        /// <summary>
        /// Subscribes the specified handler to the notification target.
        /// </summary>
        /// <typeparam name="TSubscribedEvent">The type of event.</typeparam>
        /// <param name="notificationTarget">The notification target.</param>
        /// <param name="handler">The handler.</param>
        /// <returns>A subscription.</returns>
        public Subscription Subscribe<TSubscribedEvent>(
            INotificationTarget notificationTarget,
            Func<TSubscribedEvent, CancellationToken, ValueTask> handler)
            where TSubscribedEvent : TEvent
        {
#pragma warning disable SA1101
            return notifyAny.Subscribe<TSubscribedEvent>(
                notificationTarget,
                (@event, _, cancellationToken) => handler(@event, cancellationToken));
#pragma warning restore SA1101
        }

        /// <summary>
        /// Subscribes the specified handler to the notification target.
        /// </summary>
        /// <typeparam name="TSubscribedEvent">The type of event.</typeparam>
        /// <param name="notificationTarget">The notification target.</param>
        /// <param name="handler">The handler.</param>
        /// <returns>A subscription.</returns>
        public Subscription Subscribe<TSubscribedEvent>(
            INotificationTarget notificationTarget,
            Func<TSubscribedEvent, Subscription, ValueTask> handler)
            where TSubscribedEvent : TEvent
        {
#pragma warning disable SA1101
            return notifyAny.Subscribe<TSubscribedEvent>(
                notificationTarget,
                (@event, subscription, _) => handler(@event, subscription));
#pragma warning restore SA1101
        }
    }

    /// <summary>
    /// Subscribes the specified handler to the notification target.
    /// </summary>
    /// <typeparam name="TEvent">The type of event.</typeparam>
    /// <param name="notifyAny">The notifyAny.</param>
    /// <param name="notificationTarget">The notification target.</param>
    /// <param name="handler">The handler.</param>
    /// <returns>A subscription.</returns>
    public static Subscription Subscribe<TEvent>(
        this INotify<TEvent> notifyAny,
        INotificationTarget notificationTarget,
        Func<TEvent, ValueTask> handler)
        where TEvent : class
    {
        return notifyAny.Subscribe(
            notificationTarget,
            (@event, _, _) => handler(@event));
    }

    /// <summary>
    /// Subscribes the specified handler to the notification target.
    /// </summary>
    /// <typeparam name="TEvent">The type of event.</typeparam>
    /// <param name="notifyAny">The notifyAny.</param>
    /// <param name="notificationTarget">The notification target.</param>
    /// <param name="handler">The handler.</param>
    /// <returns>A subscription.</returns>
    public static Subscription Subscribe<TEvent>(
        this INotify<TEvent> notifyAny,
        INotificationTarget notificationTarget,
        Func<TEvent, CancellationToken, ValueTask> handler)
        where TEvent : class
    {
        return notifyAny.Subscribe(
            notificationTarget,
            (@event, _, cancellationToken) => handler(@event, cancellationToken));
    }

    /// <summary>
    /// Subscribes the specified handler to the notification target.
    /// </summary>
    /// <typeparam name="TEvent">The type of event.</typeparam>
    /// <param name="notifyAny">The notifyAny.</param>
    /// <param name="notificationTarget">The notification target.</param>
    /// <param name="handler">The handler.</param>
    /// <returns>A subscription.</returns>
    public static Subscription Subscribe<TEvent>(
        this INotify<TEvent> notifyAny,
        INotificationTarget notificationTarget,
        Func<TEvent, Subscription, ValueTask> handler)
        where TEvent : class
    {
        return notifyAny.Subscribe(
            notificationTarget,
            (@event, subscription, _) => handler(@event, subscription));
    }
}