// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelegateEventRaiseBenchmark.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.PerformanceBenchmark.Notifications;

using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Sundew.Base.Collections;
using Sundew.Base.Notifications;

/// <summary>
/// Compares raising the <see cref="DelegateEvent{TEvent}"/> with a plain .NET event and a strong reference event.
/// </summary>
/// <remarks>
/// Handlers are no-ops, so the numbers show the cost of reaching the handlers, not of running them. The plain event is the
/// familiar baseline: a multicast delegate invoked synchronously, which is why its handlers return nothing. The strong reference
/// event is the asynchronous floor, an array of handlers raised with the same loop as the delegate event, so that the difference
/// to the delegate event is the weak reference per subscription and nothing else.
/// </remarks>
[MemoryDiagnoser]
public class DelegateEventRaiseBenchmark
{
    private readonly NotificationOwner notificationOwner = new();
    private readonly DelegateEvent<object> delegateEvent = new();
    private readonly StrongReferenceEvent<object> strongEvent = new();
    private readonly object @event = new();

    /// <summary>
    /// Occurs when the plain event is raised.
    /// </summary>
    public event Action<object>? PlainEventRaised;

    /// <summary>
    /// Gets or sets the number of subscribed handlers.
    /// </summary>
    [Params(1, 10, 100)]
    public int Handlers { get; set; }

    /// <summary>
    /// Subscribes the handlers.
    /// </summary>
    [GlobalSetup]
    public void Setup()
    {
        for (var i = 0; i < this.Handlers; i++)
        {
            this.delegateEvent.Subscribe((_, _, _) => default, this.notificationOwner);
            this.strongEvent.Subscribe((_, _) => default);
            this.PlainEventRaised += _ => { };
        }
    }

    /// <summary>
    /// Raises the plain .NET event.
    /// </summary>
    [Benchmark(Baseline = true)]
    public void PlainEvent()
    {
        this.PlainEventRaised?.Invoke(this.@event);
    }

    /// <summary>
    /// Raises the strong reference event.
    /// </summary>
    /// <returns>The completion.</returns>
    [Benchmark]
    public ValueTask StrongReference()
    {
        return this.strongEvent.RaiseAsync(this.@event);
    }

    /// <summary>
    /// Raises the delegate event.
    /// </summary>
    /// <returns>The completion.</returns>
    [Benchmark]
    public ValueTask DelegateEvent()
    {
        return this.delegateEvent.RaiseAsync(this.@event, Parallelism.None);
    }

    public sealed class NotificationOwner : INotificationOwner
    {
        public Subscriptions TargetSubscriptions { get; } = new Subscriptions();
    }
}