// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Subscriptions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Reactions;

using System;
using Sundew.Base.Collections.Concurrent;

/// <summary>
/// Contains subscriptions for reactions.
/// </summary>
public sealed class Subscriptions : IDisposable
{
    private readonly ConcurrentList<Unsubscribe> subscriptions = new();

    /// <summary>
    /// Unsubscribes from all reactions.
    /// </summary>
    public void Dispose()
    {
        this.subscriptions.Clear(x => x.Invoke());
    }

    internal void Add(Unsubscribe unsubscribe)
    {
        this.subscriptions.Add(unsubscribe);
    }
}
