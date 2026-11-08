// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelegateEventSource.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Notifications;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sundew.Base.Collections;
using Sundew.Base.Notifications;

public sealed class DelegateEventSource : INotifyAny<IEvent>
{
    private readonly DelegateEvent<IEvent> delegateEvent = new();

    public IEnumerable<Subscription> Subscriptions => this.delegateEvent.GetSubscriptions();

    public Subscription Subscribe<TSubscribedEvent>(Func<TSubscribedEvent, Subscription, CancellationToken, ValueTask> handler, INotificationOwner notificationOwner)
        where TSubscribedEvent : IEvent
    {
        return this.delegateEvent.Subscribe(handler, notificationOwner);
    }

    public void Raise(Parallelism parallelism = default)
    {
        this.delegateEvent.Raise(new ConcreteEvent1(), parallelism, CancellationToken.None);
        this.delegateEvent.Raise(new ConcreteEvent2(), parallelism, CancellationToken.None);
    }

    public void UnsubscribeAll()
    {
        this.delegateEvent.UnsubscribeAll();
    }
}
