// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelegateEventTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable SA1402
namespace Sundew.Base.Development.Tests.Notifications;

using System;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Sundew.Base.Collections;
using Sundew.Base.Notifications;
using Sundew.Base.Threading;
using Xunit;

public class DelegateEventTests
{
    [Fact]
    public async Task Subscribe_WhenEventRaised_Then_EventShouldBeReceived()
    {
        var eventSource = new DelegateEventSource();
        var notificationTarget = new DelegateNotificationTarget(eventSource);

        eventSource.Raise();

        var result1 = await notificationTarget.Event1Received.WaitAsync(TimeSpan.FromMilliseconds(1000));
        var result2 = await notificationTarget.Event2Received.WaitAsync(TimeSpan.FromMilliseconds(1000));

        result1.Should().BeTrue();
        result2.Should().BeTrue();
    }

    [Fact]
    public async Task Target_UnsubscribeAll_WhenEventRaised_Then_EventShouldNotBeReceived()
    {
        var eventSource = new DelegateEventSource();
        var notificationTarget = new DelegateNotificationTarget(eventSource);

        notificationTarget.UnsubscribeAll();
        eventSource.Raise();

        var result1 = await notificationTarget.Event1Received.WaitAsync(TimeSpan.FromMilliseconds(60));
        var result2 = await notificationTarget.Event2Received.WaitAsync(TimeSpan.FromMilliseconds(60));

        result1.Should().BeFalse();
        result2.Should().BeFalse();
        notificationTarget.TargetSubscriptions.GetUnsubscribers().Should().HaveCount(0);
        eventSource.Subscriptions.GetUnsubscribers().Should().HaveCount(0);
    }

    [Fact]
    public async Task Source_UnsubscribeAll_WhenEventRaised_Then_EventShouldNotBeReceived()
    {
        var eventSource = new DelegateEventSource();
        var notificationTarget = new DelegateNotificationTarget(eventSource);

        eventSource.UnsubscribeAll();
        eventSource.Raise();

        var result1 = await notificationTarget.Event1Received.WaitAsync(TimeSpan.FromMilliseconds(60));
        var result2 = await notificationTarget.Event2Received.WaitAsync(TimeSpan.FromMilliseconds(60));

        result1.Should().BeFalse();
        result2.Should().BeFalse();
        notificationTarget.TargetSubscriptions.GetUnsubscribers().Should().HaveCount(0);
        eventSource.Subscriptions.GetUnsubscribers().Should().HaveCount(0);
    }

    [Fact]
    public async Task SourceAndTarget_UnsubscribeAll_WhenEventRaised_Then_EventShouldNotBeReceived()
    {
        var eventSource = new DelegateEventSource();
        var notificationTarget = new DelegateNotificationTarget(eventSource);

        notificationTarget.UnsubscribeAll();
        eventSource.UnsubscribeAll();
        eventSource.Raise();

        var result1 = await notificationTarget.Event1Received.WaitAsync(TimeSpan.FromMilliseconds(60));
        var result2 = await notificationTarget.Event2Received.WaitAsync(TimeSpan.FromMilliseconds(60));

        result1.Should().BeFalse();
        result2.Should().BeFalse();
        notificationTarget.TargetSubscriptions.GetUnsubscribers().Should().HaveCount(0);
        eventSource.Subscriptions.GetUnsubscribers().Should().HaveCount(0);
    }

    [Fact]
    public async Task Target_UnsubscribeConcreteEvent_WhenEventRaised_Then_EventShouldBeReceivedOnce()
    {
        var eventSource = new DelegateEventSource();
        var notificationTarget = new DelegateNotificationTarget(eventSource);
        notificationTarget.TargetSubscriptions.GetUnsubscribers().Should().HaveCount(2);
        eventSource.Subscriptions.GetUnsubscribers().Should().HaveCount(2);

        notificationTarget.ConcreteEvent1Subscription.Unsubscribe();

        eventSource.Raise();

        var result1 = await notificationTarget.Event1Received.WaitAsync(TimeSpan.FromMilliseconds(60));
        var result2 = await notificationTarget.Event2Received.WaitAsync(TimeSpan.FromMilliseconds(60));

        result1.Should().BeFalse();
        result2.Should().BeTrue();
        notificationTarget.TargetSubscriptions.GetUnsubscribers().Should().HaveCount(1);
        eventSource.Subscriptions.GetUnsubscribers().Should().HaveCount(1);
    }
}

public class DelegateEventSource : INotify<IEvent>
{
    private readonly DelegateEvent<IEvent> delegateEvent = new();

    internal Subscriptions Subscriptions => this.delegateEvent.Subscriptions;

    public Subscription Subscribe<TSubscribedEvent>(INotificationTarget notificationTarget, Func<TSubscribedEvent, CancellationToken, ValueTask> handler)
        where TSubscribedEvent : IEvent
    {
        return this.delegateEvent.Subscribe(handler, notificationTarget);
    }

    public void Raise()
    {
        this.delegateEvent.Raise(new ConcreteEvent1(), Parallelism.None, CancellationToken.None);
        this.delegateEvent.Raise(new ConcreteEvent2(), Parallelism.None, CancellationToken.None);
    }

    public void UnsubscribeAll()
    {
        this.delegateEvent.Dispose();
    }
}

public class DelegateNotificationTarget : INotificationTarget
{
    public DelegateNotificationTarget(DelegateEventSource eventSource)
    {
        this.ConcreteEvent1Subscription = eventSource.Subscribe(this, (ConcreteEvent1 event1, CancellationToken token) =>
        {
            this.Event1Received.Set();
            return ValueTask.CompletedTask;
        });

        this.ConcreteEvent2Subscription = eventSource.Subscribe(this, (ConcreteEvent2 event2, CancellationToken token) =>
        {
            this.Event2Received.Set();
            return ValueTask.CompletedTask;
        });
    }

    public Subscription ConcreteEvent1Subscription { get; }

    public Subscription ConcreteEvent2Subscription { get; }

    public Subscriptions TargetSubscriptions { get; } = new Subscriptions();

    public AutoResetEventAsync Event1Received { get; } = new AutoResetEventAsync();

    public AutoResetEventAsync Event2Received { get; } = new AutoResetEventAsync();

    public void UnsubscribeAll()
    {
        this.TargetSubscriptions.Dispose();
    }
}

public class MultipleDelegateNotificationTarget : INotificationTarget
{
    private readonly Subscription concreteEvent1Subscription;
    private readonly Subscription concreteEvent2Subscription;

    public MultipleDelegateNotificationTarget(DelegateEventSource eventSource1, DelegateEventSource eventSource2)
    {
        this.concreteEvent1Subscription = eventSource1.Subscribe(this, (ConcreteEvent1 event1, CancellationToken token) =>
        {
            this.EventReceived.Set();
            return ValueTask.CompletedTask;
        });

        this.concreteEvent2Subscription = eventSource2.Subscribe(this, (ConcreteEvent2 event2, CancellationToken token) => ValueTask.CompletedTask);
    }

    public Subscriptions TargetSubscriptions { get; } = new Subscriptions();

    public AutoResetEventAsync EventReceived { get; } = new AutoResetEventAsync();

    public void UnsubscribeAll()
    {
        this.TargetSubscriptions.Dispose();
    }
}