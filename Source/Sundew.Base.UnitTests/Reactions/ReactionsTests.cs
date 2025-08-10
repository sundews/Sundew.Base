// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReactionsTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable SA1402
namespace Sundew.Base.UnitTests.Reactions;

using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Sundew.Base.Reactions;
using Sundew.Base.Threading;
using Xunit;

public class ReactionsTests
{
    [Fact]
    public async Task Subscribe_WhenEventRaised_Then_EventShouldBeReceived()
    {
        var eventSource = new EventSource();
        var subscriberTarget = new SubscriberTarget(eventSource);

        eventSource.Raise();

        var result = await subscriberTarget.EventReceived.WaitAsync(TimeSpan.FromSeconds(1));

        result.Should().BeTrue();
    }

    [Fact]
    public async Task Target_UnsubscribeAll_WhenEventRaised_Then_EventShouldNotBeReceived()
    {
        var eventSource = new EventSource();
        var subscriberTarget = new SubscriberTarget(eventSource);

        subscriberTarget.UnsubscribeAll();
        eventSource.Raise();

        var result = await subscriberTarget.EventReceived.WaitAsync(TimeSpan.FromSeconds(1));

        result.Should().BeFalse();
    }

    [Fact]
    public async Task Source_UnsubscribeAll_WhenEventRaised_Then_EventShouldNotBeReceived()
    {
        var eventSource = new EventSource();
        var subscriberTarget = new SubscriberTarget(eventSource);

        eventSource.UnsubscribeAll();
        eventSource.Raise();

        var result = await subscriberTarget.EventReceived.WaitAsync(TimeSpan.FromSeconds(1));

        result.Should().BeFalse();
    }

    [Fact]
    public async Task SourceAndTarget_UnsubscribeAll_WhenEventRaised_Then_EventShouldNotBeReceived()
    {
        var eventSource = new EventSource();
        var subscriberTarget = new SubscriberTarget(eventSource);

        subscriberTarget.UnsubscribeAll();
        eventSource.UnsubscribeAll();
        eventSource.Raise();

        var result = await subscriberTarget.EventReceived.WaitAsync(TimeSpan.FromSeconds(1));

        result.Should().BeFalse();
    }
}

#pragma warning disable SA1201
public interface IEvent
#pragma warning restore SA1201
{
}

public class ConcreteEvent : IEvent
{
}

public class EventSource : INotify<IEvent>
{
    private readonly Subject<IEvent> subject = new();
    private readonly Subscriptions subscriptions = new();

    public Unsubscribe Subscribe<TSubscribedEvent>(ISubscriptionTarget subscriptionTarget, Func<TSubscribedEvent, CancellationToken, ValueTask> handler)
        where TSubscribedEvent : IEvent
    {
        return ReactiveEventSubscriber<IEvent>.Subscribe(this.subject, handler, subscriptionTarget, this.subscriptions);
    }

    public void Raise()
    {
        this.subject.OnNext(new ConcreteEvent());
    }

    public void UnsubscribeAll()
    {
        this.subscriptions.Dispose();
    }
}

public class SubscriberTarget : ISubscriptionTarget
{
    public SubscriberTarget(EventSource eventSource)
    {
        eventSource.Subscribe(this, (ConcreteEvent @event, CancellationToken token) =>
        {
            this.EventReceived.Set();
            return ValueTask.CompletedTask;
        });
    }

    public Subscriptions ReactionSubscriptions { get; } = new Subscriptions();

    public AutoResetEventAsync EventReceived { get; } = new AutoResetEventAsync();

    public void UnsubscribeAll()
    {
        this.ReactionSubscriptions.Dispose();
    }
}