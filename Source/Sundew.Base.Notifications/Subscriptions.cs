// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Subscriptions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Notifications;

using System;
using System.Collections.Generic;
using Sundew.Base.Collections.Concurrent;
using Sundew.Base.Disposal;

/// <summary>
/// Contains the subscriptions owned by a notification target.
/// </summary>
/// <remarks>
/// A <see cref="Subscriptions"/> instance owns the subscriptions of a notification target, which lets the target unsubscribe everything
/// it owns. For an event that does not keep its subscribers alive, such as <see cref="DelegateEvent{TEvent}"/>, it is also what keeps a
/// subscription alive, and through it the handler: such an event reaches its handlers only through the subscription, so the subscription
/// stays active exactly as long as the <see cref="Subscriptions"/> that owns it is reachable, which means that the notification target can
/// be garbage collected even though its handlers reference it, and that its subscriptions end when it is collected. An owner whose
/// <see cref="Subscriptions"/> serve no other purpose controls the lifetime of its subscriptions explicitly.
/// Call <see cref="UnsubscribeAll()"/> to unsubscribe deterministically.
/// </remarks>
public sealed class Subscriptions
{
    private readonly ConcurrentList<Subscription> subscriptions = new();

    /// <summary>
    /// Gets the number of subscriptions.
    /// </summary>
    public int Count => this.subscriptions.Count;

    /// <summary>
    /// Unsubscribes all subscriptions owned by the specified notification target.
    /// </summary>
    /// <param name="notificationOwner">The notification target.</param>
    public static void UnsubscribeAll(INotificationOwner notificationOwner)
    {
        notificationOwner.TargetSubscriptions.UnsubscribeAll();
    }

    /// <summary>
    /// Unsubscribes all subscriptions.
    /// </summary>
    /// <remarks>Subscriptions made afterwards are unaffected, so the instance can keep being used as a notification target.</remarks>
    public void UnsubscribeAll()
    {
        this.subscriptions.Clear(x => x.Unsubscribe());
    }

    /// <summary>
    /// Returns an <see cref="IDisposable"/> that unsubscribes all subscriptions when disposed.
    /// </summary>
    /// <remarks>Use this method to integrate the subscriptions with <see langword="using"/> statements or other disposal patterns.</remarks>
    /// <returns>An <see cref="IDisposable"/> that calls <see cref="UnsubscribeAll()"/> when disposed.</returns>
    public IDisposable AsDisposable()
    {
        return new DisposeAction(this.UnsubscribeAll);
    }

    internal void Add(Subscription subscription)
    {
        this.subscriptions.Add(subscription);
    }

    internal void Remove(Subscription subscription)
    {
        this.subscriptions.Remove(subscription);
    }

    internal IReadOnlyList<Subscription> GetUnsubscribers()
    {
        return [.. this.subscriptions];
    }
}
