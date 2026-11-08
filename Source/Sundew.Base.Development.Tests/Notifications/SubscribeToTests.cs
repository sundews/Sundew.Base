// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubscribeToTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Notifications;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Sundew.Base.Notifications;

#pragma warning disable SA1402 // File may only contain a single type

/// <summary>
/// Tests the SubscribeTo methods that Sundew.Base.Notifications.Generator adds to partial notification owners.
/// </summary>
public class SubscribeToTests
{
    [Test]
    public void SubscribeTo_WhenNotifyAny_Then_OnlySubscribedEventTypeShouldBeReceived()
    {
        var eventSource = new DelegateEventSource();
        var owner = new SubscribingNotificationOwner();

        var subscription = owner.SubscribeToAny(eventSource);
        eventSource.Raise();

        owner.ReceivedCount.Should().Be(1);
        subscription.IsSubscribed.Should().BeTrue();
        owner.SubscriptionCount.Should().Be(1);
        eventSource.Subscriptions.Should().HaveCount(1);
    }

    [Test]
    public void SubscribeTo_WhenNotifyAnyWithCancellationToken_Then_EventShouldBeReceived()
    {
        var eventSource = new DelegateEventSource();
        var owner = new SubscribingNotificationOwner();

        owner.SubscribeToAnyWithCancellationToken(eventSource);
        eventSource.Raise();

        owner.ReceivedCount.Should().Be(1);
        owner.SubscriptionCount.Should().Be(1);
    }

    [Test]
    public void SubscribeTo_WhenNotifyAnyWithSubscription_Then_HandlerShouldBeAbleToUnsubscribe()
    {
        var eventSource = new DelegateEventSource();
        var owner = new SubscribingNotificationOwner();

        owner.SubscribeToAnyWithSubscription(eventSource);
        eventSource.Raise();
        eventSource.Raise();

        owner.ReceivedCount.Should().Be(1);
        owner.SubscriptionCount.Should().Be(0);
        eventSource.Subscriptions.Should().HaveCount(0);
    }

    [Test]
    public void SubscribeTo_WhenNotifyAnyWithEvent_Then_EventShouldBeReceived()
    {
        var eventSource = new DelegateEventSource();
        var owner = new SubscribingNotificationOwner();

        owner.SubscribeToAnyWithEvent(eventSource);
        eventSource.Raise();

        owner.ReceivedCount.Should().Be(1);
        owner.SubscriptionCount.Should().Be(1);
    }

    [Test]
    public void SubscribeTo_WhenNotify_Then_EventShouldBeReceived()
    {
        var delegateEvent = new DelegateEvent<ConcreteEvent1>();
        var owner = new SubscribingNotificationOwner();

        var subscription = owner.SubscribeToNotify(delegateEvent);
        delegateEvent.Raise(new ConcreteEvent1());

        owner.ReceivedCount.Should().Be(1);
        subscription.IsSubscribed.Should().BeTrue();
        owner.SubscriptionCount.Should().Be(1);
        delegateEvent.GetSubscriptions().Count().Should().Be(1);
    }

    [Test]
    public void SubscribeTo_WhenNotifyWithCancellationToken_Then_EventShouldBeReceived()
    {
        var delegateEvent = new DelegateEvent<ConcreteEvent1>();
        var owner = new SubscribingNotificationOwner();

        owner.SubscribeToNotifyWithCancellationToken(delegateEvent);
        delegateEvent.Raise(new ConcreteEvent1());

        owner.ReceivedCount.Should().Be(1);
        owner.SubscriptionCount.Should().Be(1);
    }

    [Test]
    public void SubscribeTo_WhenNotifyWithSubscription_Then_HandlerShouldBeAbleToUnsubscribe()
    {
        var delegateEvent = new DelegateEvent<ConcreteEvent1>();
        var owner = new SubscribingNotificationOwner();

        owner.SubscribeToNotifyWithSubscription(delegateEvent);
        delegateEvent.Raise(new ConcreteEvent1());
        delegateEvent.Raise(new ConcreteEvent1());

        owner.ReceivedCount.Should().Be(1);
        owner.SubscriptionCount.Should().Be(0);
        delegateEvent.GetSubscriptions().Count().Should().Be(0);
    }

    [Test]
    public void SubscribeTo_WhenNotifyWithEvent_Then_EventShouldBeReceived()
    {
        var delegateEvent = new DelegateEvent<ConcreteEvent1>();
        var owner = new SubscribingNotificationOwner();

        owner.SubscribeToNotifyWithEvent(delegateEvent);
        delegateEvent.Raise(new ConcreteEvent1());

        owner.ReceivedCount.Should().Be(1);
        owner.SubscriptionCount.Should().Be(1);
    }

    [Test]
    public void SubscribeTo_WhenOwnerUnsubscribesAll_Then_EventShouldNotBeReceived()
    {
        var eventSource = new DelegateEventSource();
        var owner = new SubscribingNotificationOwner();
        owner.SubscribeToAny(eventSource);
        owner.SubscribeToAnyWithEvent(eventSource);

        owner.UnsubscribeAll();
        eventSource.Raise();

        owner.ReceivedCount.Should().Be(0);
        owner.SubscriptionCount.Should().Be(0);
        eventSource.Subscriptions.Should().HaveCount(0);
    }
}

/// <summary>
/// A partial owner, so that the generator adds SubscribeTo, which this class calls on itself in one public method per handler shape.
/// </summary>
public partial class SubscribingNotificationOwner : INotificationOwner
{
    public int ReceivedCount { get; private set; }

    public int SubscriptionCount => ((INotificationOwner)this).TargetSubscriptions.Count;

    public Subscription SubscribeToAny(INotifyAny<IEvent> notifyAny)
    {
        return this.SubscribeTo(notifyAny, (ConcreteEvent1 @event, Subscription subscription, CancellationToken cancellationToken) => this.Receive());
    }

    public Subscription SubscribeToAnyWithCancellationToken(INotifyAny<IEvent> notifyAny)
    {
        return this.SubscribeTo(notifyAny, (ConcreteEvent1 @event, CancellationToken cancellationToken) => this.Receive());
    }

    public Subscription SubscribeToAnyWithSubscription(INotifyAny<IEvent> notifyAny)
    {
        return this.SubscribeTo(notifyAny, (ConcreteEvent2 @event, Subscription subscription) => this.ReceiveAndUnsubscribe(subscription));
    }

    public Subscription SubscribeToAnyWithEvent(INotifyAny<IEvent> notifyAny)
    {
        return this.SubscribeTo(notifyAny, (ConcreteEvent2 @event) => this.Receive());
    }

    public Subscription SubscribeToNotify(INotify<ConcreteEvent1> notify)
    {
        return this.SubscribeTo(notify, (ConcreteEvent1 @event, Subscription subscription, CancellationToken cancellationToken) => this.Receive());
    }

    public Subscription SubscribeToNotifyWithCancellationToken(INotify<ConcreteEvent1> notify)
    {
        return this.SubscribeTo(notify, (ConcreteEvent1 @event, CancellationToken cancellationToken) => this.Receive());
    }

    public Subscription SubscribeToNotifyWithSubscription(INotify<ConcreteEvent1> notify)
    {
        return this.SubscribeTo(notify, (ConcreteEvent1 @event, Subscription subscription) => this.ReceiveAndUnsubscribe(subscription));
    }

    public Subscription SubscribeToNotifyWithEvent(INotify<ConcreteEvent1> notify)
    {
        return this.SubscribeTo(notify, (ConcreteEvent1 @event) => this.Receive());
    }

    public void UnsubscribeAll()
    {
        Subscriptions.UnsubscribeAll(this);
    }

    private ValueTask Receive()
    {
        this.ReceivedCount++;
        return ValueTask.CompletedTask;
    }

    private ValueTask ReceiveAndUnsubscribe(Subscription subscription)
    {
        this.ReceivedCount++;
        subscription.Unsubscribe();
        return ValueTask.CompletedTask;
    }
}
