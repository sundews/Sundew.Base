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
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Sundew.Base.Notifications;
using Sundew.Base.Notifications.Reactive;
using Sundew.Base.Threading;
using Xunit;

public class ReactiveEventTests
{
    [Fact]
    public async Task Subscribe_WhenEventRaised_Then_EventShouldBeReceived()
    {
        var eventSource = new ReactiveEventSource();
        var subscriberTarget = new ReactiveNotificationTarget(eventSource);

        eventSource.Raise();

        var result = await subscriberTarget.EventReceived.WaitAsync(TimeSpan.FromSeconds(1));

        result.Should().BeTrue();
    }

    [Fact]
    public async Task Target_UnsubscribeAll_WhenEventRaised_Then_EventShouldNotBeReceived()
    {
        var eventSource = new ReactiveEventSource();
        var subscriberTarget = new ReactiveNotificationTarget(eventSource);

        subscriberTarget.UnsubscribeAll();
        eventSource.Raise();

        var result = await subscriberTarget.EventReceived.WaitAsync(TimeSpan.FromSeconds(1));

        result.Should().BeFalse();
        subscriberTarget.TargetSubscriptions.GetUnsubscribers().Should().HaveCount(0);
        eventSource.ReactiveSubscriptions.GetUnsubscribers().Should().HaveCount(0);
    }

    [Fact]
    public async Task Source_UnsubscribeAll_WhenEventRaised_Then_EventShouldNotBeReceived()
    {
        var eventSource = new ReactiveEventSource();
        var subscriberTarget = new ReactiveNotificationTarget(eventSource);

        eventSource.UnsubscribeAll();
        eventSource.Raise();

        var result = await subscriberTarget.EventReceived.WaitAsync(TimeSpan.FromSeconds(1));

        result.Should().BeFalse();
        subscriberTarget.TargetSubscriptions.GetUnsubscribers().Should().HaveCount(0);
        eventSource.ReactiveSubscriptions.GetUnsubscribers().Should().HaveCount(0);
    }

    [Fact]
    public async Task SourceAndTarget_UnsubscribeAll_WhenEventRaised_Then_EventShouldNotBeReceived()
    {
        var eventSource = new ReactiveEventSource();
        var subscriberTarget = new ReactiveNotificationTarget(eventSource);

        subscriberTarget.UnsubscribeAll();
        eventSource.UnsubscribeAll();
        eventSource.Raise();

        var result = await subscriberTarget.EventReceived.WaitAsync(TimeSpan.FromSeconds(1));

        result.Should().BeFalse();
        subscriberTarget.TargetSubscriptions.GetUnsubscribers().Should().HaveCount(0);
        eventSource.ReactiveSubscriptions.GetUnsubscribers().Should().HaveCount(0);
    }
}

#pragma warning disable SA1201

public class ReactiveEventSource : INotify<IEvent>
{
    private readonly Subject<IEvent> subject = new();
    private readonly Subscriptions subscriptions = new();

    internal Subscriptions ReactiveSubscriptions => this.subscriptions;

    public Subscription Subscribe<TSubscribedEvent>(INotificationTarget notificationTarget, Func<TSubscribedEvent, CancellationToken, ValueTask> handler)
        where TSubscribedEvent : IEvent
    {
        return ReactiveEventSubscriber<IEvent>.Subscribe(this.subject, handler, notificationTarget, this.subscriptions);
    }

    public void Raise()
    {
        this.subject.OnNext(new ConcreteEvent1());
    }

    public void UnsubscribeAll()
    {
        this.subscriptions.Dispose();
    }
}

public class ReactiveNotificationTarget : INotificationTarget
{
    public ReactiveNotificationTarget(ReactiveEventSource reactiveEventSource)
    {
        reactiveEventSource.Subscribe(this, (ConcreteEvent1 event1, CancellationToken token) =>
        {
            this.EventReceived.Set();
            return ValueTask.CompletedTask;
        });
    }

    public Subscriptions TargetSubscriptions { get; } = new Subscriptions();

    public AutoResetEventAsync EventReceived { get; } = new AutoResetEventAsync();

    public void UnsubscribeAll()
    {
        this.TargetSubscriptions.Dispose();
    }
}