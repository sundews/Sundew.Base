// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Subscriptions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Notifications;

using System;
using System.Collections.Generic;
using System.Linq;
using Sundew.Base.Collections.Concurrent;

/// <summary>
/// Contains subscriptions for reactions.
/// </summary>
public sealed class Subscriptions : IDisposable
{
    private readonly ConcurrentList<Subscription> subscriptions = new();

    /// <summary>
    /// Unsubscribes from all reactions.
    /// </summary>
    public void Dispose()
    {
        this.subscriptions.Clear(x => x.Unsubscribe());
    }

    internal void Add(Subscription subscription)
    {
        this.subscriptions.Add(subscription);
    }

    internal void Remove(Subscription subscription)
    {
        this.subscriptions.Remove(subscription);
    }

    internal IReadOnlyList<Subscription> GetUnsubscribers()
    {
        return this.subscriptions.ToList();
    }
}