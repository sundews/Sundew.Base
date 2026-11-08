// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReactiveEventSubscriber.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Notifications.Reactive;

using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sundew.Base.Notifications;

/// <summary>
/// Implements reactive reactions.
/// </summary>
/// <remarks>
/// The observable chain and any source side <see cref="Subscriptions"/> only reference the handler through a weak link, so the
/// notification target is not kept alive by the observable. The subscription stays active for as long as the target is reachable,
/// or until it is unsubscribed. When the target has been collected, the link disposes the observable subscription and removes
/// itself from the source side subscriptions the next time an event arrives.
/// </remarks>
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
    /// <param name="notificationOwner">The notification owner.</param>
    /// <param name="subscriptionsList">The source side subscriptions, whose UnsubscribeAll ends the subscription from the source.</param>
    /// <returns>A subscription.</returns>
    public static Subscription Subscribe<TSubscribedEvent>(
        IObservable<TEvent> observable,
        Func<TSubscribedEvent, Subscription, CancellationToken, ValueTask> handler,
        INotificationOwner notificationOwner,
        params IReadOnlyList<Subscriptions> subscriptionsList)
        where TSubscribedEvent : TEvent
    {
        return SubscribeCore(observable.OfType<TSubscribedEvent>(), handler, notificationOwner.TargetSubscriptions, subscriptionsList);
    }

    /// <summary>
    /// Subscribes to the specified event.
    /// </summary>
    /// <param name="observable">The observable.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="notificationOwner">The notification owner.</param>
    /// <param name="subscriptionsList">The source side subscriptions, whose UnsubscribeAll ends the subscription from the source.</param>
    /// <returns>A subscription.</returns>
    public static Subscription Subscribe(
        IObservable<TEvent> observable,
        Func<TEvent, Subscription, CancellationToken, ValueTask> handler,
        INotificationOwner notificationOwner,
        params IReadOnlyList<Subscriptions> subscriptionsList)
    {
        return SubscribeCore(observable, handler, notificationOwner.TargetSubscriptions, subscriptionsList);
    }

    private static Subscription SubscribeCore<TSubscribedEvent>(
        IObservable<TSubscribedEvent> observable,
        Func<TSubscribedEvent, Subscription, CancellationToken, ValueTask> handler,
        Subscriptions targetSubscriptions,
        IReadOnlyList<Subscriptions> subscriptionsList)
    {
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        var link = new WeakLink<TSubscribedEvent>(subscriptionsList);
        var registration = new Registration<TSubscribedEvent>(handler, targetSubscriptions, link);
        link.Connect(observable, registration);
        targetSubscriptions.Add(registration.Subscription);
        foreach (var subscriptions in subscriptionsList)
        {
            subscriptions.Add(link.SourceSubscription);
        }

        return registration.Subscription;
    }

    /// <summary>
    /// A registration is referenced strongly only by the <see cref="Subscriptions"/> of its notification target (as the
    /// unsubscriber of its subscription). It therefore lives exactly as long as its target.
    /// </summary>
    /// <typeparam name="TSubscribedEvent">The subscribed event type.</typeparam>
    private sealed class Registration<TSubscribedEvent> : IUnsubscriber
    {
        private readonly WeakLink<TSubscribedEvent> link;

        public Registration(Func<TSubscribedEvent, Subscription, CancellationToken, ValueTask> handler, Subscriptions targetSubscriptions, WeakLink<TSubscribedEvent> link)
        {
            this.Handler = handler;
            this.link = link;
            this.Subscription = new Subscription(this, targetSubscriptions);
        }

        public Func<TSubscribedEvent, Subscription, CancellationToken, ValueTask> Handler { get; }

        public Subscription Subscription { get; }

        /// <inheritdoc/>
        void IUnsubscriber.Unsubscribe(Subscription subscription)
        {
            this.link.Disconnect();
        }
    }

    /// <summary>
    /// The link is what the observable chain and the source side subscriptions reference. It only holds the registration weakly,
    /// so it never keeps the notification target alive.
    /// </summary>
    /// <typeparam name="TSubscribedEvent">The subscribed event type.</typeparam>
    private sealed class WeakLink<TSubscribedEvent> : IUnsubscriber
    {
#if NET9_0_OR_GREATER
        private readonly Lock @lock = new();
#else
        private readonly object @lock = new();
#endif
        private readonly IReadOnlyList<Subscriptions> sourceSubscriptionsList;
        private WeakReference<Registration<TSubscribedEvent>>? weakRegistration;
        private IDisposable? disposable;
        private bool isDisconnected;

        public WeakLink(IReadOnlyList<Subscriptions> sourceSubscriptionsList)
        {
            this.sourceSubscriptionsList = sourceSubscriptionsList;
            this.SourceSubscription = new Subscription(this, null);
        }

        public Subscription SourceSubscription { get; }

        public void Connect(IObservable<TSubscribedEvent> observable, Registration<TSubscribedEvent> registration)
        {
            this.weakRegistration = new WeakReference<Registration<TSubscribedEvent>>(registration);
            var observableSubscription = observable.Select(this.Project).Concat().Subscribe();
            lock (this.@lock)
            {
                if (!this.isDisconnected)
                {
                    this.disposable = observableSubscription;
                    return;
                }
            }

            observableSubscription.Dispose();
        }

        public void Disconnect()
        {
            foreach (var subscriptions in this.sourceSubscriptionsList)
            {
                subscriptions.Remove(this.SourceSubscription);
            }

            IDisposable? observableSubscription;
            lock (this.@lock)
            {
                this.isDisconnected = true;
                observableSubscription = this.disposable;
                this.disposable = null;
            }

            observableSubscription?.Dispose();
        }

        /// <inheritdoc/>
        void IUnsubscriber.Unsubscribe(Subscription subscription)
        {
            if (this.weakRegistration is { } weakRegistration && weakRegistration.TryGetTarget(out var registration))
            {
                registration.Subscription.Unsubscribe();
                return;
            }

            this.Disconnect();
        }

        private IObservable<Unit> Project(TSubscribedEvent @event)
        {
            return Observable.FromAsync(cancellationToken => this.InvokeAsync(@event, cancellationToken));
        }

        private async Task InvokeAsync(TSubscribedEvent @event, CancellationToken cancellationToken)
        {
            if (this.weakRegistration is { } weakRegistration && weakRegistration.TryGetTarget(out var registration))
            {
                await registration.Handler(@event, registration.Subscription, cancellationToken).ConfigureAwait(false);
                return;
            }

            this.Disconnect();
        }
    }
}
