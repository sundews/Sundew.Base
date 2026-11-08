// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReactiveEventTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable SA1402
namespace Sundew.Base.Development.Tests.Notifications;

using System;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Sundew.Base.Notifications;
using Sundew.Base.Notifications.Reactive;
using Sundew.Base.Threading;

public class ReactiveEventTests
{
    [Test]
    public async Task Subscribe_WhenEventRaised_Then_EventShouldBeReceived()
    {
        var eventSource = new ReactiveEventSource();
        var notificationOwner = new ReactiveNotificationOwner(eventSource);

        eventSource.Raise();

        var result = await notificationOwner.EventReceived.WaitAsync(TimeSpan.FromSeconds(1));

        result.Should().BeTrue();
    }

    [Test]
    public async Task Target_UnsubscribeAll_WhenEventRaised_Then_EventShouldNotBeReceived()
    {
        var eventSource = new ReactiveEventSource();
        var notificationOwner = new ReactiveNotificationOwner(eventSource);

        notificationOwner.UnsubscribeAll();
        eventSource.Raise();

        var result = await notificationOwner.EventReceived.WaitAsync(TimeSpan.FromSeconds(1));

        result.Should().BeFalse();
        ((INotificationOwner)notificationOwner).TargetSubscriptions.GetUnsubscribers().Should().HaveCount(0);
        eventSource.ReactiveSubscriptions.GetUnsubscribers().Should().HaveCount(0);
    }

    [Test]
    public async Task Source_UnsubscribeAll_WhenEventRaised_Then_EventShouldNotBeReceived()
    {
        var eventSource = new ReactiveEventSource();
        var notificationOwner = new ReactiveNotificationOwner(eventSource);

        eventSource.UnsubscribeAll();
        eventSource.Raise();

        var result = await notificationOwner.EventReceived.WaitAsync(TimeSpan.FromSeconds(1));

        result.Should().BeFalse();
        ((INotificationOwner)notificationOwner).TargetSubscriptions.GetUnsubscribers().Should().HaveCount(0);
        eventSource.ReactiveSubscriptions.GetUnsubscribers().Should().HaveCount(0);
    }

    [Test]
    public async Task SourceAndTarget_UnsubscribeAll_WhenEventRaised_Then_EventShouldNotBeReceived()
    {
        var eventSource = new ReactiveEventSource();
        var notificationOwner = new ReactiveNotificationOwner(eventSource);

        notificationOwner.UnsubscribeAll();
        eventSource.UnsubscribeAll();
        eventSource.Raise();

        var result = await notificationOwner.EventReceived.WaitAsync(TimeSpan.FromSeconds(1));

        result.Should().BeFalse();
        ((INotificationOwner)notificationOwner).TargetSubscriptions.GetUnsubscribers().Should().HaveCount(0);
        eventSource.ReactiveSubscriptions.GetUnsubscribers().Should().HaveCount(0);
    }

    [Test]
    public async Task Subscribe_WhenPlainTargetIsCollected_Then_SourceSubscriptionShouldBeRemovedOnRaise()
    {
        var eventSource = new ReactiveEventSource();
        var counter = new StrongBox<int>();
        SubscribePlainTarget(eventSource, counter);
        CollectGarbage();

        eventSource.Raise();
        await Task.Delay(50);

        counter.Value.Should().Be(0);
        eventSource.ReactiveSubscriptions.Count.Should().Be(0);
    }

    [Test]
    public async Task Subscribe_WhenPlainTargetIsAlive_Then_EventShouldBeReceived()
    {
        var eventSource = new ReactiveEventSource();
        var counter = new StrongBox<int>();
        var notificationOwner = new PlainReactiveNotificationOwner(eventSource, counter);
        CollectGarbage();

        eventSource.Raise();
        await Task.Delay(50);

        counter.Value.Should().Be(1);
        eventSource.ReactiveSubscriptions.Count.Should().Be(1);
        GC.KeepAlive(notificationOwner);
    }

    [Test]
    public async Task Subscriptions_UnsubscribeAll_WhenPlainTarget_Then_EventShouldNotBeReceived()
    {
        var eventSource = new ReactiveEventSource();
        var counter = new StrongBox<int>();
        var notificationOwner = new PlainReactiveNotificationOwner(eventSource, counter);

        Subscriptions.UnsubscribeAll(notificationOwner);
        eventSource.Raise();
        await Task.Delay(50);

        counter.Value.Should().Be(0);
        eventSource.ReactiveSubscriptions.Count.Should().Be(0);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void SubscribePlainTarget(ReactiveEventSource eventSource, StrongBox<int> counter)
    {
        _ = new PlainReactiveNotificationOwner(eventSource, counter);
    }

    private static void CollectGarbage()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }
}

#pragma warning disable SA1201

public class ReactiveEventSource : INotifyAny<IEvent>
{
    private readonly Subject<IEvent> subject = new();
    private readonly Subscriptions subscriptions = new();

    internal Subscriptions ReactiveSubscriptions => this.subscriptions;

    public Subscription Subscribe<TSubscribedEvent>(Func<TSubscribedEvent, Subscription, CancellationToken, ValueTask> handler, INotificationOwner notificationOwner)
        where TSubscribedEvent : IEvent
    {
        return ReactiveEventSubscriber<IEvent>.Subscribe(this.subject, handler, notificationOwner, this.subscriptions);
    }

    public void Raise()
    {
        this.subject.OnNext(new ConcreteEvent1());
    }

    public void UnsubscribeAll()
    {
        this.subscriptions.UnsubscribeAll();
    }
}

public partial class ReactiveNotificationOwner : INotificationOwner
{
    public ReactiveNotificationOwner(ReactiveEventSource reactiveEventSource)
    {
        reactiveEventSource.Subscribe(
            (ConcreteEvent1 event1, CancellationToken token) =>
            {
                this.EventReceived.Set();
                return ValueTask.CompletedTask;
            },
            this);
    }

    public AutoResetEventAsync EventReceived { get; } = new AutoResetEventAsync();

    public void UnsubscribeAll()
    {
        Subscriptions.UnsubscribeAll(this);
    }
}

public sealed partial class PlainReactiveNotificationOwner : INotificationOwner
{
    private readonly StrongBox<int> counter;

    public PlainReactiveNotificationOwner(ReactiveEventSource eventSource, StrongBox<int> counter)
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
}
