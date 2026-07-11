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
        Func<TSubscribedEvent, Subscription, CancellationToken, ValueTask> handler,
        INotificationTarget notificationTarget,
        params IReadOnlyList<Subscriptions> subscriptionsList)
        where TSubscribedEvent : TEvent
    {
#if NET9_0_OR_GREATER
        var @lock = new Lock();
#else
        var @lock = new object();
#endif
        IDisposable? disposable = null;
        var subscription = new Subscription(Unsubscribe);
        lock (@lock)
        {
            disposable = observable.OfType<TSubscribedEvent>()
                .Select(x =>
                    Observable.FromAsync(async cancellationToken =>
                    {
                        await handler(x, subscription, cancellationToken).ConfigureAwait(false);
                    }))
                .Concat()
                .Subscribe();
        }

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
            lock (@lock)
            {
                disposable?.Dispose();
            }
        }
    }

    /// <summary>
    /// Subscribes to the specified event.
    /// </summary>
    /// <param name="observable">The observable.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="notificationTarget">The subscription target.</param>
    /// <param name="subscriptionsList">The subscriptions.</param>
    /// <returns>A subscription.</returns>
    public static Subscription Subscribe(
        IObservable<TEvent> observable,
        Func<TEvent, Subscription, CancellationToken, ValueTask> handler,
        INotificationTarget notificationTarget,
        params IReadOnlyList<Subscriptions> subscriptionsList)
    {
#if NET9_0_OR_GREATER
        var @lock = new Lock();
#else
        var @lock = new object();
#endif
        IDisposable? disposable = null;
        var subscription = new Subscription(Unsubscribe);
        lock (@lock)
        {
            disposable = observable
                .Select(x =>
                    Observable.FromAsync(async cancellationToken =>
                    {
                        await handler(x, subscription, cancellationToken).ConfigureAwait(false);
                    }))
                .Concat()
                .Subscribe();
        }

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
            lock (@lock)
            {
                disposable?.Dispose();
            }
        }
    }
}