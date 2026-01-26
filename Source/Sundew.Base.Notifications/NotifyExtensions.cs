// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotifyExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Notifications;

using System;
using System.Threading.Tasks;

/// <summary>
/// Provides extension methods for the <see cref="INotify{TEvent}"/> interface.
/// </summary>
public static class NotifyExtensions
{
    /// <summary>
    /// Subscribes the specified handler to the notification target.
    /// </summary>
    /// <typeparam name="TEvent">The type of event.</typeparam>
    /// <param name="notify">The notify.</param>
    /// <param name="notificationTarget">The notification target.</param>
    /// <param name="handler">The handler.</param>
    /// <returns>A subscription.</returns>
    public static Subscription Subscribe<TEvent>(
        this INotify<TEvent> notify,
        INotificationTarget notificationTarget,
        Func<TEvent, ValueTask> handler)
        where TEvent : class
    {
        return notify.Subscribe<TEvent>(
            notificationTarget,
            (evt, ct) => handler(evt));
    }
}