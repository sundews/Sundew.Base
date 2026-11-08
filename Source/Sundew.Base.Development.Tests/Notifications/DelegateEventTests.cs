// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelegateEventTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable SA1402
namespace Sundew.Base.Development.Tests.Notifications;

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Sundew.Base.Collections;
using Sundew.Base.Notifications;
using Sundew.Base.Threading;

public class DelegateEventTests
{
    [Test]
    public async Task Subscribe_WhenEventRaised_Then_EventShouldBeReceived()
    {
        var eventSource = new DelegateEventSource();
        var notificationOwner = new DelegateNotificationOwner(eventSource);

        eventSource.Raise();

        var result1 = await notificationOwner.Event1Received.WaitAsync(TimeSpan.FromMilliseconds(1000));
        var result2 = await notificationOwner.Event2Received.WaitAsync(TimeSpan.FromMilliseconds(1000));

        result1.Should().BeTrue();
        result2.Should().BeTrue();
    }

    [Test]
    public async Task Target_UnsubscribeAll_WhenEventRaised_Then_EventShouldNotBeReceived()
    {
        var eventSource = new DelegateEventSource();
        var notificationOwner = new DelegateNotificationOwner(eventSource);

        notificationOwner.UnsubscribeAll();
        eventSource.Raise();

        var result1 = await notificationOwner.Event1Received.WaitAsync(TimeSpan.FromMilliseconds(60));
        var result2 = await notificationOwner.Event2Received.WaitAsync(TimeSpan.FromMilliseconds(60));

        result1.Should().BeFalse();
        result2.Should().BeFalse();
        INotificationOwner iDelegateNotificationOwner = notificationOwner;
        iDelegateNotificationOwner.TargetSubscriptions.GetUnsubscribers().Should().HaveCount(0);
        eventSource.Subscriptions.Should().HaveCount(0);
    }

    [Test]
    public async Task Source_UnsubscribeAll_WhenEventRaised_Then_EventShouldNotBeReceived()
    {
        var eventSource = new DelegateEventSource();
        var notificationOwner = new DelegateNotificationOwner(eventSource);

        eventSource.UnsubscribeAll();
        eventSource.Raise();

        var result1 = await notificationOwner.Event1Received.WaitAsync(TimeSpan.FromMilliseconds(60));
        var result2 = await notificationOwner.Event2Received.WaitAsync(TimeSpan.FromMilliseconds(60));

        result1.Should().BeFalse();
        result2.Should().BeFalse();
        INotificationOwner iDelegateNotificationOwner = notificationOwner;
        iDelegateNotificationOwner.TargetSubscriptions.GetUnsubscribers().Should().HaveCount(0);
        eventSource.Subscriptions.Should().HaveCount(0);
    }

    [Test]
    public async Task SourceAndTarget_UnsubscribeAll_WhenEventRaised_Then_EventShouldNotBeReceived()
    {
        var eventSource = new DelegateEventSource();
        var notificationOwner = new DelegateNotificationOwner(eventSource);

        notificationOwner.UnsubscribeAll();
        eventSource.UnsubscribeAll();
        eventSource.Raise();

        var result1 = await notificationOwner.Event1Received.WaitAsync(TimeSpan.FromMilliseconds(60));
        var result2 = await notificationOwner.Event2Received.WaitAsync(TimeSpan.FromMilliseconds(60));

        result1.Should().BeFalse();
        result2.Should().BeFalse();
        INotificationOwner iDelegateNotificationOwner = notificationOwner;
        iDelegateNotificationOwner.TargetSubscriptions.GetUnsubscribers().Should().HaveCount(0);
        eventSource.Subscriptions.Should().HaveCount(0);
    }

    [Test]
    public async Task Target_UnsubscribeConcreteEvent_WhenEventRaised_Then_EventShouldBeReceivedOnce()
    {
        var eventSource = new DelegateEventSource();
        var notificationOwner = new DelegateNotificationOwner(eventSource);
        INotificationOwner inotificationOwner = notificationOwner;
        inotificationOwner.TargetSubscriptions.GetUnsubscribers().Should().HaveCount(2);
        eventSource.Subscriptions.Should().HaveCount(2);

        notificationOwner.ConcreteEvent1Subscription.Unsubscribe();

        eventSource.Raise();

        var result1 = await notificationOwner.Event1Received.WaitAsync(TimeSpan.FromMilliseconds(60));
        var result2 = await notificationOwner.Event2Received.WaitAsync(TimeSpan.FromMilliseconds(60));

        result1.Should().BeFalse();
        result2.Should().BeTrue();
        inotificationOwner.TargetSubscriptions.GetUnsubscribers().Should().HaveCount(1);
        eventSource.Subscriptions.Should().HaveCount(1);
    }

    [Test]
    public async Task Subscribe_WhenSubscribedToMultipleSources_Then_EventsFromBothSourcesShouldBeReceived()
    {
        var eventSource1 = new DelegateEventSource();
        var eventSource2 = new DelegateEventSource();
        var notificationOwner = new MultipleDelegateNotificationOwner(eventSource1, eventSource2);

        eventSource1.Raise();
        eventSource2.Raise();

        var result1 = await notificationOwner.Event1Received.WaitAsync(TimeSpan.FromMilliseconds(60));
        var result2 = await notificationOwner.Event2Received.WaitAsync(TimeSpan.FromMilliseconds(60));

        result1.Should().BeTrue();
        result2.Should().BeTrue();
        INotificationOwner inotificationOwner = notificationOwner;
        inotificationOwner.TargetSubscriptions.GetUnsubscribers().Should().HaveCount(2);
        eventSource1.Subscriptions.Should().HaveCount(1);
        eventSource2.Subscriptions.Should().HaveCount(1);
    }

    [Test]
    public async Task Target_UnsubscribeAll_WhenSubscribedToMultipleSources_Then_BothSourcesShouldBeUnsubscribed()
    {
        var eventSource1 = new DelegateEventSource();
        var eventSource2 = new DelegateEventSource();
        var notificationOwner = new MultipleDelegateNotificationOwner(eventSource1, eventSource2);

        notificationOwner.UnsubscribeAll();
        eventSource1.Raise();
        eventSource2.Raise();

        var result1 = await notificationOwner.Event1Received.WaitAsync(TimeSpan.FromMilliseconds(60));
        var result2 = await notificationOwner.Event2Received.WaitAsync(TimeSpan.FromMilliseconds(60));

        result1.Should().BeFalse();
        result2.Should().BeFalse();
        INotificationOwner inotificationOwner = notificationOwner;
        inotificationOwner.TargetSubscriptions.GetUnsubscribers().Should().HaveCount(0);
        eventSource1.Subscriptions.Should().HaveCount(0);
        eventSource2.Subscriptions.Should().HaveCount(0);
    }

    [Test]
    public void Subscribe_WhenPlainTargetIsAlive_Then_EventShouldBeReceived()
    {
        var eventSource = new DelegateEventSource();
        var counter = new StrongBox<int>();
        var notificationOwner = new PlainNotificationOwner(eventSource, counter);
        CollectGarbage();

        eventSource.Raise();

        counter.Value.Should().Be(1);
        eventSource.Subscriptions.Should().HaveCount(1);
        GC.KeepAlive(notificationOwner);
    }

    [Test]
    public void Subscriptions_UnsubscribeAll_WhenPlainTarget_Then_EventShouldNotBeReceived()
    {
        var eventSource = new DelegateEventSource();
        var counter = new StrongBox<int>();
        var notificationOwner = new PlainNotificationOwner(eventSource, counter);

        Subscriptions.UnsubscribeAll(notificationOwner);
        eventSource.Raise();

        counter.Value.Should().Be(0);
        eventSource.Subscriptions.Should().HaveCount(0);
        INotificationOwner inotificationOwner = notificationOwner;
        inotificationOwner.TargetSubscriptions.GetUnsubscribers().Should().HaveCount(0);
    }

    [Test]
    public void Subscribe_WhenTargetIsSubscriptions_Then_UnsubscribingAllShouldUnsubscribe()
    {
        var eventSource = new DelegateEventSource();
        var counter = new StrongBox<int>();
        var notificationOwner = new NotificationOwner();
        eventSource.Subscribe(
            (ConcreteEvent1 event1, CancellationToken token) =>
            {
                counter.Value++;
                return ValueTask.CompletedTask;
            },
            notificationOwner);
        CollectGarbage();

        eventSource.Raise();
        notificationOwner.TargetSubscriptions.UnsubscribeAll();
        eventSource.Raise();

        counter.Value.Should().Be(1);
        eventSource.Subscriptions.Should().HaveCount(0);
    }

    [Test]
    public void Subscribe_WhenHandlerUnsubscribesItself_Then_EventShouldBeReceivedOnce()
    {
        var eventSource = new DelegateEventSource();
        var counter = new StrongBox<int>();
        var notificationOwner = new NotificationOwner();
        eventSource.Subscribe(
            (ConcreteEvent1 event1, Subscription subscription) =>
            {
                counter.Value++;
                subscription.Unsubscribe();
                return ValueTask.CompletedTask;
            },
            notificationOwner);

        eventSource.Raise();
        eventSource.Raise();

        counter.Value.Should().Be(1);
        eventSource.Subscriptions.Should().HaveCount(0);
        notificationOwner.TargetSubscriptions.Count.Should().Be(0);
    }

    [Test]
    public void Subscribe_WhenShortHandlerForTheEventTypeItself_Then_EventShouldBeReceived()
    {
        var delegateEvent = new DelegateEvent<IEvent>();
        var counter = new StrongBox<int>();
        var notificationOwner = new NotificationOwner();
        delegateEvent.Subscribe((IEvent @event, CancellationToken token) => Count(counter), notificationOwner);
        delegateEvent.Subscribe((IEvent @event, Subscription subscription) => Count(counter), notificationOwner);
        delegateEvent.Subscribe((IEvent @event) => Count(counter), notificationOwner);

        delegateEvent.Raise(new ConcreteEvent1());

        counter.Value.Should().Be(3);
        notificationOwner.TargetSubscriptions.Count.Should().Be(3);
    }

    [Test]
    public void Raise_WhenParallel_Then_AllHandlersShouldBeInvoked()
    {
        var eventSource = new DelegateEventSource();
        var counter = new StrongBox<int>();
        var notificationOwner = new NotificationOwner();
        for (var i = 0; i < 10; i++)
        {
            eventSource.Subscribe(
                (ConcreteEvent1 event1, CancellationToken token) =>
                {
                    Interlocked.Increment(ref counter.Value);
                    return ValueTask.CompletedTask;
                },
                notificationOwner);
        }

        eventSource.Raise(new Parallelism(4));

        counter.Value.Should().Be(10);
        eventSource.Subscriptions.Should().HaveCount(10);
        GC.KeepAlive(notificationOwner);
    }

    [Test]
    public void Subscribe_WhenPlainTargetIsCollected_Then_HandlerShouldBeRemovedOnRaise()
    {
        var eventSource = new DelegateEventSource();
        var counter = new StrongBox<int>();
        SubscribePlainTarget(eventSource, counter);
        CollectGarbage();

        eventSource.Raise();

        counter.Value.Should().Be(0);
        eventSource.Subscriptions.Should().HaveCount(0);
    }

    [Test]
    public void Subscribe_WhenManyCollectedTargets_Then_OnlyLiveRegistrationsShouldRemain()
    {
        var eventSource = new DelegateEventSource();
        var counter = new StrongBox<int>();
        for (var i = 0; i < 40; i++)
        {
            SubscribePlainTarget(eventSource, counter);
        }

        CollectGarbage();
        var notificationOwner = new PlainNotificationOwner(eventSource, counter);

        eventSource.Subscriptions.Should().HaveCount(1);
        GC.KeepAlive(notificationOwner);
    }

    [Test]
    public void Subscribe_WhenEventIsCollected_Then_UnsubscribeShouldRemoveSubscriptionFromOwner()
    {
        var notificationOwner = new NotificationOwner();
        var (weakEvent, subscription) = SubscribeToTransientEvent(notificationOwner);
        CollectGarbage();

        IsAlive(weakEvent).Should().BeFalse();
        subscription.IsSubscribed.Should().BeTrue();
        notificationOwner.TargetSubscriptions.Count.Should().Be(1);

        subscription.Unsubscribe();

        subscription.IsSubscribed.Should().BeFalse();
        notificationOwner.TargetSubscriptions.Count.Should().Be(0);
    }

    private static ValueTask Count(StrongBox<int> counter)
    {
        counter.Value++;
        return ValueTask.CompletedTask;
    }

    private static void CollectGarbage()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void SubscribePlainTarget(DelegateEventSource eventSource, StrongBox<int> counter)
    {
        _ = new PlainNotificationOwner(eventSource, counter);
    }

    /// <summary>
    /// Checks the weak reference in a frame of its own, so that the target does not linger in a temporary of the test method.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool IsAlive<T>(WeakReference<T> weakReference)
        where T : class
    {
        return weakReference.TryGetTarget(out _);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static (WeakReference<DelegateEvent<IEvent>> Event, Subscription Subscription) SubscribeToTransientEvent(INotificationOwner notificationOwner)
    {
        var delegateEvent = new DelegateEvent<IEvent>();
        var subscription = delegateEvent.Subscribe((_, _, _) => ValueTask.CompletedTask, notificationOwner);
        return (new WeakReference<DelegateEvent<IEvent>>(delegateEvent), subscription);
    }
}

public partial class DelegateNotificationOwner : INotificationOwner
{
    public DelegateNotificationOwner(DelegateEventSource eventSource)
    {
        this.ConcreteEvent1Subscription = this.SubscribeTo(
            eventSource,
            (ConcreteEvent1 event1, CancellationToken token) =>
            {
                this.Event1Received.Set();
                return ValueTask.CompletedTask;
            });

        this.ConcreteEvent2Subscription = this.SubscribeTo(
            eventSource,
            (ConcreteEvent2 event2, CancellationToken token) =>
            {
                this.Event2Received.Set();
                return ValueTask.CompletedTask;
            });
    }

    public Subscription ConcreteEvent1Subscription { get; }

    public Subscription ConcreteEvent2Subscription { get; }

    public AutoResetEventAsync Event1Received { get; } = new AutoResetEventAsync();

    public AutoResetEventAsync Event2Received { get; } = new AutoResetEventAsync();

    public void UnsubscribeAll()
    {
        Subscriptions.UnsubscribeAll(this);
    }
}

public partial class MultipleDelegateNotificationOwner : INotificationOwner
{
    private readonly Subscription concreteEvent1Subscription;
    private readonly Subscription concreteEvent2Subscription;

    public MultipleDelegateNotificationOwner(DelegateEventSource eventSource1, DelegateEventSource eventSource2)
    {
        this.concreteEvent1Subscription = this.SubscribeTo(
            eventSource1,
            (ConcreteEvent1 event1, CancellationToken token) =>
            {
                this.Event1Received.Set();
                return ValueTask.CompletedTask;
            });

        this.concreteEvent2Subscription = this.SubscribeTo(
            eventSource2,
            (ConcreteEvent2 event2, CancellationToken token) =>
            {
                this.Event2Received.Set();
                return ValueTask.CompletedTask;
            });
    }

    public AutoResetEventAsync Event1Received { get; } = new AutoResetEventAsync();

    public AutoResetEventAsync Event2Received { get; } = new AutoResetEventAsync();

    public void UnsubscribeAll()
    {
        Subscriptions.UnsubscribeAll(this);
    }
}

public sealed class PlainNotificationOwner : INotificationOwner
{
    private readonly StrongBox<int> counter;

    public PlainNotificationOwner(DelegateEventSource eventSource, StrongBox<int> counter)
    {
        this.counter = counter;
        eventSource.Subscribe(
            (ConcreteEvent1 event1, CancellationToken token) =>
            {
                this.counter.Value++;
                return ValueTask.CompletedTask;
            },
            this);
    }

    public Subscriptions TargetSubscriptions { get; } = new Subscriptions();
}

internal sealed class NotificationOwner : INotificationOwner
{
    public Subscriptions TargetSubscriptions { get; } = new Subscriptions();
}