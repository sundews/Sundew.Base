// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INotify.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Reactions;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Interface for implementing an event source.
/// </summary>
/// <typeparam name="TEvent">The type of event.</typeparam>
public interface INotify<in TEvent>
    where TEvent : class
{
    /// <summary>
    /// Subscribes to the subscribed event.
    /// </summary>
    /// <typeparam name="TSubscribedEvent">The subscribed event type.</typeparam>
    /// <param name="subscriptionTarget">The subscription target.</param>
    /// <param name="handler">The handler.</param>
    /// <returns>An unsubscribe delegate.</returns>
    Unsubscribe Subscribe<TSubscribedEvent>(
        ISubscriptionTarget subscriptionTarget,
        Func<TSubscribedEvent, CancellationToken, ValueTask> handler)
        where TSubscribedEvent : TEvent;
}