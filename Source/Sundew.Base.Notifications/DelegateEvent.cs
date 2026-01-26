// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelegateEvent.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Notifications;

using System;
using System.Threading;
using System.Threading.Tasks;
using Sundew.Base.Collections;
using Sundew.Base.Collections.Concurrent;

/// <summary>
/// Represents an event to which handlers can be dynamically subscribed and unsubscribed, supporting asynchronous
/// invocation and cancellation.
/// </summary>
/// <remarks><see cref="DelegateEvent{TEvent}"/> enables event-driven programming patterns where multiple handlers can be
/// registered to respond to events of type TEvent. Handlers are invoked asynchronously and may observe cancellation via
/// a CancellationToken. This class is thread-safe and is intended for scenarios where event subscriptions and
/// notifications may occur concurrently. Dispose the instance to release all subscriptions and handlers.</remarks>
/// <typeparam name="TEvent">The type of the event data passed to event handlers.</typeparam>
public sealed class DelegateEvent<TEvent> : IDisposable
{
    private readonly Subscriptions subscriptions = new();
    private readonly ConcurrentList<Func<TEvent, CancellationToken, ValueTask>> handlers = new();

    internal Subscriptions Subscriptions => this.subscriptions;

    internal ConcurrentList<Func<TEvent, CancellationToken, ValueTask>> Handlers => this.handlers;

    /// <summary>
    /// Subscribes to the specified event.
    /// </summary>
    /// <typeparam name="TSubscribedEvent">The subscribed event type.</typeparam>
    /// <param name="handler">The handler.</param>
    /// <param name="notificationTarget">The subscription target.</param>
    /// <returns>An unsubscribe delegate.</returns>
    public Subscription Subscribe<TSubscribedEvent>(
        Func<TSubscribedEvent, CancellationToken, ValueTask> handler,
        INotificationTarget notificationTarget)
        where TSubscribedEvent : TEvent
    {
        var rawHandler = new Func<TEvent, CancellationToken, ValueTask>((TEvent @event, CancellationToken token) =>
        {
            if (@event is TSubscribedEvent subscribedEvent)
            {
                return handler(subscribedEvent, token);
            }

            return default;
        });

        this.handlers.Add(rawHandler);
        var unsubscribeViaTarget = new Subscription(Unsubscribe);
        notificationTarget.TargetSubscriptions.Add(unsubscribeViaTarget);
        this.subscriptions.Add(unsubscribeViaTarget);

        return unsubscribeViaTarget;

        void Unsubscribe(Subscription subscription)
        {
            notificationTarget.TargetSubscriptions.Remove(subscription);
            this.subscriptions.Remove(subscription);
            this.Handlers.Remove(rawHandler);
        }
    }

    /// <summary>
    /// Invokes all registered event handlers for the specified event, using the provided parallelism settings.
    /// </summary>
    /// <remarks>If cancellation is requested via the cancellation token, not all handlers may be invoked. The
    /// degree of parallelism is determined by the specified parallelism parameter.</remarks>
    /// <param name="event">The event data to pass to each handler.</param>
    /// <param name="parallelism">The parallelism options that control how event handlers are invoked concurrently.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    public void Raise(TEvent @event, Parallelism parallelism, CancellationToken cancellationToken = default)
    {
        this.RaiseAsync(@event, parallelism, cancellationToken).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Invokes all registered event handlers for the specified event, using the provided parallelism settings.
    /// </summary>
    /// <remarks>If cancellation is requested via the cancellation token, not all handlers may be invoked. The
    /// degree of parallelism is determined by the specified parallelism parameter.</remarks>
    /// <param name="event">The event data to pass to each handler.</param>
    /// <param name="parallelism">The parallelism options that control how event handlers are invoked concurrently.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A ValueTask that represents the asynchronous operation of invoking all event handlers.</returns>
    public async ValueTask RaiseAsync(TEvent @event, Parallelism parallelism, CancellationToken cancellationToken = default)
    {
        await this.handlers.ForEachAsync(parallelism, handler => handler(@event, cancellationToken));
    }

    /// <summary>
    /// Releases all resources used by the current instance.
    /// </summary>
    /// <remarks>Call this method when the instance is no longer needed to free managed resources promptly.
    /// After calling this method, the instance should not be used.</remarks>
    public void Dispose()
    {
        this.subscriptions.Dispose();
    }
}