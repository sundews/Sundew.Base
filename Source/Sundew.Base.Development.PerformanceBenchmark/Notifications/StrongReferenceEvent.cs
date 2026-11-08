// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StrongReferenceEvent.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.PerformanceBenchmark.Notifications;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A strong reference event for comparison with <see cref="Sundew.Base.Notifications.DelegateEvent{TEvent}"/>.
/// </summary>
/// <remarks>
/// Handlers are kept in a copy-on-write array and raised with the same synchronous loop as the library event, so the only
/// difference measured is holding the handlers directly instead of through weak references to their subscriptions.
/// </remarks>
/// <typeparam name="TEvent">The type of event.</typeparam>
public sealed class StrongReferenceEvent<TEvent>
{
    private readonly object lockObject = new();
    private Func<TEvent, CancellationToken, ValueTask>[] handlers = [];

    /// <summary>
    /// Subscribes the specified handler.
    /// </summary>
    /// <param name="handler">The handler.</param>
    public void Subscribe(Func<TEvent, CancellationToken, ValueTask> handler)
    {
        lock (this.lockObject)
        {
            var current = this.handlers;
            var updated = new Func<TEvent, CancellationToken, ValueTask>[current.Length + 1];
            Array.Copy(current, updated, current.Length);
            updated[current.Length] = handler;
            Volatile.Write(ref this.handlers, updated);
        }
    }

    /// <summary>
    /// Invokes all handlers.
    /// </summary>
    /// <param name="event">The event.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that completes when all handlers have been invoked.</returns>
    public ValueTask RaiseAsync(TEvent @event, CancellationToken cancellationToken = default)
    {
        var snapshot = Volatile.Read(ref this.handlers);
        for (var i = 0; i < snapshot.Length; i++)
        {
            var valueTask = snapshot[i](@event, cancellationToken);
            if (!valueTask.IsCompletedSuccessfully)
            {
                return AwaitRemainingAsync(valueTask, snapshot, i + 1, @event, cancellationToken);
            }

            valueTask.GetAwaiter().GetResult();
        }

        return default;
    }

    private static async ValueTask AwaitRemainingAsync(ValueTask pending, Func<TEvent, CancellationToken, ValueTask>[] snapshot, int next, TEvent @event, CancellationToken cancellationToken)
    {
        await pending.ConfigureAwait(false);
        for (var i = next; i < snapshot.Length; i++)
        {
            await snapshot[i](@event, cancellationToken).ConfigureAwait(false);
        }
    }
}
