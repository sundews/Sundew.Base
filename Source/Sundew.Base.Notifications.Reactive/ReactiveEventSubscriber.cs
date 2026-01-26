// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReactiveEventSubscriber.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Notifications.Reactive;

using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sundew.Base.Notifications;

/// <summary>
/// Implements reactive reactions.
/// </summary>
/// <typeparam name="TEvent">The type of event.</typeparam>
public class ReactiveEventSubscriber<TEvent>
    where TEvent : class
{
    /// <summary>
    /// Subscribes to the specified event.
    /// </summary>
    /// <typeparam name="TSubscribedEvent">The subscribed event type.</typeparam>
    /// <param name="observable">The observable.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="notificationTarget">The subscription target.</param>
    /// <param name="subscriptionsList">The subscriptions.</param>
    /// <returns>A subscription.</returns>
    public static Subscription Subscribe<TSubscribedEvent>(
        IObservable<TEvent> observable,
        Func<TSubscribedEvent, CancellationToken, ValueTask> handler,
        INotificationTarget notificationTarget,
        params IReadOnlyList<Subscriptions> subscriptionsList)
        where TSubscribedEvent : TEvent
    {
        var disposable = observable.OfType<TSubscribedEvent>()
            .Select(x =>
                Observable.FromAsync(async cancellationToken =>
                    await handler(x, cancellationToken).ConfigureAwait(false)))
            .Concat()
            .Subscribe();

        var subscription = new Subscription(Unsubscribe);
        notificationTarget.TargetSubscriptions.Add(subscription);
        foreach (var subscriptions in subscriptionsList)
        {
            subscriptions.Add(subscription);
        }

        return subscription;

        void Unsubscribe(Subscription subscription)
        {
            foreach (var subscriptions in subscriptionsList)
            {
                subscriptions.Remove(subscription);
            }

            notificationTarget.TargetSubscriptions.Remove(subscription);
            disposable.Dispose();
        }
    }
}