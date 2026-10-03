// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INotifyAny.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Notifications;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Interface for implementing an event source.
/// </summary>
/// <typeparam name="TEvent">The type of event.</typeparam>
public interface INotifyAny<in TEvent>
{
    /// <summary>
    /// Subscribes to the subscribed event.
    /// </summary>
    /// <typeparam name="TSubscribedEvent">The subscribed event type.</typeparam>
    /// <param name="handler">The handler.</param>
    /// <param name="notificationTarget">The subscription target.</param>
    /// <returns>A subscription.</returns>
    Subscription Subscribe<TSubscribedEvent>(
        Func<TSubscribedEvent, Subscription, CancellationToken, ValueTask> handler,
        INotificationTarget notificationTarget)
        where TSubscribedEvent : TEvent;
}