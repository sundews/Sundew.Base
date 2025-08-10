// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReactiveEventSubscriber.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Reactions;

using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

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
    /// <param name="subscriptionTarget">The subscription target.</param>
    /// <param name="subscriptions">The subscriptions.</param>
    /// <returns>An unsubscribe delegate.</returns>
    public static Unsubscribe Subscribe<TSubscribedEvent>(IObservable<TEvent> observable, Func<TSubscribedEvent, CancellationToken, ValueTask> handler, ISubscriptionTarget subscriptionTarget, params IEnumerable<Subscriptions> subscriptions)
        where TSubscribedEvent : TEvent
    {
        var disposable = observable.OfType<TSubscribedEvent>()
            .Select(x => Observable.FromAsync(async cancellationToken => await handler(x, cancellationToken).ConfigureAwait(false)))
            .Concat()
            .Subscribe();

        subscriptionTarget.ReactionSubscriptions.Add(disposable.Dispose);
        foreach (var target in subscriptions)
        {
            target.Add(disposable.Dispose);
        }

        return disposable.Dispose;
    }
}