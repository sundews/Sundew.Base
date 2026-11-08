// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Subscription.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Notifications;

using System;
using System.Threading;
using Sundew.Base.Disposal;

/// <summary>
/// Represents a subscription to an event, which can be unsubscribed.
/// </summary>
/// <remarks>
/// A subscription is owned by the <see cref="Subscriptions"/> of the notification owner it was created for, and is what
/// keeps the handler alive: the handler is stored on the subscription and an event only reaches it through the subscription, so
/// when the subscription is no longer reachable, the handler is released as well. Unsubscribing releases the handler at once.
/// The subscription reaches its source only through a link the source provides,
/// which <see cref="DelegateEvent{TEvent}"/> makes weak, so holding a subscription never keeps that event alive. Unsubscribing
/// removes the subscription from its owner and, if the source is still alive, from the source.
/// </remarks>
public sealed class Subscription
{
    private readonly Subscriptions? subscriptions;
    private IUnsubscriber? unsubscriber;
    private Delegate? handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="Subscription"/> class.
    /// </summary>
    /// <param name="unsubscriber">The source to remove the subscription from when unsubscribing. Cannot be null.</param>
    /// <param name="subscriptions">The subscriptions of the owner to remove the subscription from when unsubscribing, if any.</param>
    /// <param name="handler">The handler the subscription keeps alive for its source, if any.</param>
    internal Subscription(IUnsubscriber unsubscriber, Subscriptions? subscriptions, Delegate? handler = null)
    {
        this.unsubscriber = unsubscriber ?? throw new ArgumentNullException(nameof(unsubscriber));
        this.subscriptions = subscriptions;
        this.handler = handler;
    }

    /// <summary>
    /// Gets a value indicating whether the subscription is still active.
    /// </summary>
    public bool IsSubscribed => Volatile.Read(ref this.unsubscriber) is not null;

    /// <summary>
    /// Gets the handler, or <see langword="null"/> if the subscription has been unsubscribed or has no handler of its own.
    /// </summary>
    internal Delegate? Handler => Volatile.Read(ref this.handler);

    /// <summary>
    /// Unsubscribes from the associated event or notification, preventing further callbacks or notifications from being
    /// received.
    /// </summary>
    /// <remarks>After calling this method, subsequent invocations have no effect. This method is typically
    /// used to release resources or stop receiving updates when they are no longer needed.</remarks>
    public void Unsubscribe()
    {
        var unsubscriber = Interlocked.Exchange(ref this.unsubscriber, null);
        if (unsubscriber is null)
        {
            return;
        }

        Volatile.Write(ref this.handler, null);
        this.subscriptions?.Remove(this);
        unsubscriber.Unsubscribe(this);
    }

    /// <summary>
    /// Returns an <see cref="IDisposable"/> that unsubscribes from the current subscription when disposed.
    /// </summary>
    /// <remarks>Use this method to integrate the subscription with <see langword="using"/> statements or
    /// other disposal patterns, ensuring that unsubscription occurs automatically when the returned object is
    /// disposed.</remarks>
    /// <returns>An <see cref="IDisposable"/> that, when disposed, unsubscribes from the subscription. If the subscription has
    /// already been unsubscribed, disposing the returned object has no effect.</returns>
    public IDisposable AsDisposable()
    {
        return new DisposeAction(this.Unsubscribe);
    }
}
