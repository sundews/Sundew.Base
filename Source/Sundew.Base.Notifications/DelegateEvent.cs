// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelegateEvent.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Notifications;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sundew.Base.Collections;
using Sundew.Base.Disposal;

/// <summary>
/// Represents an event to which handlers can be dynamically subscribed and unsubscribed, supporting asynchronous
/// invocation and cancellation, and which never keeps its subscribers alive.
/// </summary>
/// <remarks>
/// <see cref="DelegateEvent{TEvent}"/> enables event-driven programming patterns where multiple handlers can be
/// registered to respond to events of type TEvent. Handlers are invoked asynchronously and may observe cancellation via
/// a CancellationToken. This class is thread-safe and is intended for scenarios where event subscriptions and
/// notifications may occur concurrently.
/// The event holds only weak references to its subscriptions. A handler is stored on its <see cref="Subscription"/>, which is
/// kept alive only by the <see cref="Subscriptions"/> of the notification owner it was subscribed with, so a handler lives
/// exactly as long as its subscription. Subscribers may therefore be garbage collected while the event is still alive, even
/// though their handlers reference them; their entries are dropped the next time the event is raised or subscribed to.
/// The subscriptions in turn reference the event only weakly, so an event that its source has dropped is collected even while
/// its subscribers are alive. Nothing needs to be disposed in either direction. Call <see cref="UnsubscribeAll"/> to unsubscribe
/// all handlers deterministically.
/// The event implements both <see cref="INotify{TEvent}"/> and <see cref="INotifyAny{TEvent}"/>, whose extension methods would make a call
/// with a shorter handler ambiguous, so the event declares those overloads itself.
/// </remarks>
/// <typeparam name="TEvent">The type of the event data passed to event handlers.</typeparam>
public sealed class DelegateEvent<TEvent> : IEvents<TEvent>, IEvent<TEvent>, IUnsubscriber
{
#if NET9_0_OR_GREATER
    private readonly Lock lockObject = new();
#else
    private readonly object lockObject = new();
#endif
    private readonly WeakUnsubscriber unsubscriber;
    private WeakReference<Subscription>[] subscriptions = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="DelegateEvent{TEvent}"/> class.
    /// </summary>
    public DelegateEvent()
    {
        this.unsubscriber = new WeakUnsubscriber(this);
    }

    /// <summary>
    /// Subscribes to the specified event.
    /// </summary>
    /// <remarks>
    /// The event does not keep the target alive. The subscription stays active for as long as the target subscriptions are reachable,
    /// or until it is unsubscribed. Pass an owner whose <see cref="Subscriptions"/> are dedicated to the purpose to control the lifetime explicitly.
    /// </remarks>
    /// <param name="handler">The handler.</param>
    /// <param name="notificationOwner">The notification owner.</param>
    /// <returns>A subscription.</returns>
    public Subscription Subscribe(Func<TEvent, Subscription, CancellationToken, ValueTask> handler, INotificationOwner notificationOwner)
    {
        return this.Subscribe<TEvent>(handler, notificationOwner);
    }

    /// <summary>
    /// Subscribes to the specified event.
    /// </summary>
    /// <remarks>
    /// The event does not keep the target alive. The subscription stays active for as long as the target subscriptions are reachable,
    /// or until it is unsubscribed. Pass an owner whose <see cref="Subscriptions"/> are dedicated to the purpose to control the lifetime explicitly.
    /// </remarks>
    /// <typeparam name="TSubscribedEvent">The subscribed event type.</typeparam>
    /// <param name="handler">The handler.</param>
    /// <param name="notificationOwner">The notification owner.</param>
    /// <returns>A subscription.</returns>
    public Subscription Subscribe<TSubscribedEvent>(
        Func<TSubscribedEvent, Subscription, CancellationToken, ValueTask> handler,
        INotificationOwner notificationOwner)
        where TSubscribedEvent : TEvent
    {
        var rawHandler = DelegateEventHelper.Adapt<TEvent, TSubscribedEvent>(handler);
        var subscription = new Subscription(this.unsubscriber, notificationOwner.TargetSubscriptions, rawHandler);
        lock (this.lockObject)
        {
            Volatile.Write(ref this.subscriptions, Rebuild(this.subscriptions, new WeakReference<Subscription>(subscription), null));
        }

        notificationOwner.TargetSubscriptions.Add(subscription);
        return subscription;
    }

    /// <summary>
    /// Subscribes to the specified event.
    /// </summary>
    /// <remarks>
    /// The event does not keep the target alive. The subscription stays active for as long as the target subscriptions are reachable,
    /// or until it is unsubscribed. Pass an owner whose <see cref="Subscriptions"/> are dedicated to the purpose to control the lifetime explicitly.
    /// </remarks>
    /// <typeparam name="TSubscribedEvent">The subscribed event type.</typeparam>
    /// <param name="handler">The handler.</param>
    /// <param name="notificationOwner">The notification owner.</param>
    /// <returns>A subscription.</returns>
    public Subscription Subscribe<TSubscribedEvent>(Func<TSubscribedEvent, CancellationToken, ValueTask> handler, INotificationOwner notificationOwner)
        where TSubscribedEvent : TEvent
    {
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        return this.Subscribe<TSubscribedEvent>((@event, _, cancellationToken) => handler(@event, cancellationToken), notificationOwner);
    }

    /// <summary>
    /// Subscribes to the specified event.
    /// </summary>
    /// <remarks>
    /// The event does not keep the target alive. The subscription stays active for as long as the target subscriptions are reachable,
    /// or until it is unsubscribed. Pass an owner whose <see cref="Subscriptions"/> are dedicated to the purpose to control the lifetime explicitly.
    /// </remarks>
    /// <typeparam name="TSubscribedEvent">The subscribed event type.</typeparam>
    /// <param name="handler">The handler.</param>
    /// <param name="notificationOwner">The notification owner.</param>
    /// <returns>A subscription.</returns>
    public Subscription Subscribe<TSubscribedEvent>(Func<TSubscribedEvent, Subscription, ValueTask> handler, INotificationOwner notificationOwner)
        where TSubscribedEvent : TEvent
    {
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        return this.Subscribe<TSubscribedEvent>((@event, subscription, _) => handler(@event, subscription), notificationOwner);
    }

    /// <summary>
    /// Subscribes to the specified event.
    /// </summary>
    /// <remarks>
    /// The event does not keep the target alive. The subscription stays active for as long as the target subscriptions are reachable,
    /// or until it is unsubscribed. Pass an owner whose <see cref="Subscriptions"/> are dedicated to the purpose to control the lifetime explicitly.
    /// </remarks>
    /// <typeparam name="TSubscribedEvent">The subscribed event type.</typeparam>
    /// <param name="handler">The handler.</param>
    /// <param name="notificationOwner">The notification owner.</param>
    /// <returns>A subscription.</returns>
    public Subscription Subscribe<TSubscribedEvent>(Func<TSubscribedEvent, ValueTask> handler, INotificationOwner notificationOwner)
        where TSubscribedEvent : TEvent
    {
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        return this.Subscribe<TSubscribedEvent>((@event, _, _) => handler(@event), notificationOwner);
    }

    /// <summary>
    /// Invokes all registered event handlers for the specified event, using the provided parallelism settings.
    /// </summary>
    /// <remarks>If cancellation is requested via the cancellation token, not all handlers may be invoked. The
    /// degree of parallelism is determined by the specified parallelism parameter.</remarks>
    /// <param name="event">The event data to pass to each handler.</param>
    /// <param name="parallelism">The parallelism options that control how event handlers are invoked concurrently.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    public void Raise(TEvent @event, Parallelism parallelism = default, CancellationToken cancellationToken = default)
    {
        this.RaiseAsync(@event, parallelism, cancellationToken).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Invokes all registered event handlers for the specified event, using the provided parallelism settings.
    /// </summary>
    /// <remarks>If cancellation is requested via the cancellation token, not all handlers may be invoked. The
    /// degree of parallelism is determined by the specified parallelism parameter. The handlers that are live when the
    /// event is raised are invoked; subscriptions whose owner has been garbage collected are skipped and dropped.
    /// With a sequential parallelism, the default, the handlers are invoked one after the other on the calling context, and
    /// the returned task completes synchronously when every handler does.</remarks>
    /// <param name="event">The event data to pass to each handler.</param>
    /// <param name="parallelism">The parallelism options that control how event handlers are invoked concurrently.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A ValueTask that represents the asynchronous operation of invoking all event handlers.</returns>
    public ValueTask RaiseAsync(TEvent @event, Parallelism parallelism = default, CancellationToken cancellationToken = default)
    {
        var snapshot = Volatile.Read(ref this.subscriptions);
        if (snapshot.Length == 0)
        {
            return default;
        }

        if (!DelegateEventHelper.IsSequential(parallelism))
        {
            return this.RaiseInParallelAsync(snapshot, @event, parallelism, cancellationToken);
        }

        var dead = 0;
        try
        {
            for (var i = 0; i < snapshot.Length; i++)
            {
                if (!snapshot[i].TryGetTarget(out var subscription))
                {
                    dead++;
                    continue;
                }

                if (subscription.Handler is Func<TEvent, Subscription, CancellationToken, ValueTask> handler)
                {
                    var valueTask = handler(@event, subscription, cancellationToken);
                    if (!valueTask.IsCompletedSuccessfully)
                    {
                        return this.AwaitRemainingAsync(valueTask, snapshot, i + 1, dead, @event, cancellationToken);
                    }

                    valueTask.GetAwaiter().GetResult();
                }
            }
        }
        catch (Exception e)
        {
            return new ValueTask(Task.FromException(e));
        }

        if (dead > 0)
        {
            this.Compact(snapshot);
        }

        return default;
    }

    /// <summary>
    /// Unsubscribes all live subscriptions.
    /// </summary>
    /// <remarks>Subscriptions made afterwards are unaffected, so the event can keep being used.</remarks>
    public void UnsubscribeAll()
    {
        foreach (var subscription in this.GetSubscriptions())
        {
            subscription.Unsubscribe();
        }
    }

    /// <summary>
    /// Returns an <see cref="IDisposable"/> that unsubscribes all subscriptions when disposed.
    /// </summary>
    /// <remarks>Use this method to integrate the event with <see langword="using"/> statements or other disposal patterns.</remarks>
    /// <returns>An <see cref="IDisposable"/> that calls <see cref="UnsubscribeAll"/> when disposed.</returns>
    public IDisposable AsDisposable()
    {
        return new DisposeAction(this.UnsubscribeAll);
    }

    /// <inheritdoc/>
    void IUnsubscriber.Unsubscribe(Subscription subscription)
    {
        lock (this.lockObject)
        {
            Volatile.Write(ref this.subscriptions, Rebuild(this.subscriptions, null, subscription));
        }
    }

    internal IEnumerable<Subscription> GetSubscriptions()
    {
        foreach (var reference in Volatile.Read(ref this.subscriptions))
        {
            if (reference.TryGetTarget(out var subscription))
            {
                yield return subscription;
            }
        }
    }

    /// <summary>
    /// Creates a copy of the subscriptions without the collected ones, without the specified subscription and with the specified addition.
    /// </summary>
    private static WeakReference<Subscription>[] Rebuild(WeakReference<Subscription>[] current, WeakReference<Subscription>? addition, Subscription? removal)
    {
        var result = new WeakReference<Subscription>[current.Length + (addition is null ? 0 : 1)];
        var count = 0;
        foreach (var reference in current)
        {
            if (reference.TryGetTarget(out var subscription) && !ReferenceEquals(subscription, removal))
            {
                result[count++] = reference;
            }
        }

        if (addition is not null)
        {
            result[count++] = addition;
        }

        if (count != result.Length)
        {
            Array.Resize(ref result, count);
        }

        return result;
    }

    private async ValueTask AwaitRemainingAsync(ValueTask pending, WeakReference<Subscription>[] snapshot, int next, int dead, TEvent @event, CancellationToken cancellationToken)
    {
        await pending.ConfigureAwait(false);
        for (var i = next; i < snapshot.Length; i++)
        {
            if (!snapshot[i].TryGetTarget(out var subscription))
            {
                dead++;
                continue;
            }

            if (subscription.Handler is Func<TEvent, Subscription, CancellationToken, ValueTask> handler)
            {
                await handler(@event, subscription, cancellationToken).ConfigureAwait(false);
            }
        }

        if (dead > 0)
        {
            this.Compact(snapshot);
        }
    }

    private async ValueTask RaiseInParallelAsync(WeakReference<Subscription>[] snapshot, TEvent @event, Parallelism parallelism, CancellationToken cancellationToken)
    {
        var live = new List<Subscription>(snapshot.Length);
        foreach (var reference in snapshot)
        {
            if (reference.TryGetTarget(out var subscription))
            {
                live.Add(subscription);
            }
        }

        if (live.Count != snapshot.Length)
        {
            this.Compact(snapshot);
        }

        await live.ForEachAsync(
            parallelism,
            subscription => subscription.Handler is Func<TEvent, Subscription, CancellationToken, ValueTask> handler
                ? handler(@event, subscription, cancellationToken)
                : default).ConfigureAwait(false);
    }

    /// <summary>
    /// Drops the collected subscriptions, unless the subscriptions have changed since the snapshot was taken, in which case the change did.
    /// </summary>
    private void Compact(WeakReference<Subscription>[] snapshot)
    {
        lock (this.lockObject)
        {
            if (ReferenceEquals(this.subscriptions, snapshot))
            {
                Volatile.Write(ref this.subscriptions, Rebuild(snapshot, null, null));
            }
        }
    }
}
