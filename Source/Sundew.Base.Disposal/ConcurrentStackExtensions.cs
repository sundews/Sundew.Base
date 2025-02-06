// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConcurrentStackExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Disposal;

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

/// <summary>
/// Extends concurrent stack with easy to use methods.
/// </summary>
public static class ConcurrentStackExtensions
{
    /// <summary>
    /// Pops all items from the concurrent stack.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="concurrentStack">The concurrent stack.</param>
    public static void DisposeAll<TItem>(ConcurrentStack<TItem> concurrentStack)
        where TItem : IDisposable
    {
        var items = new TItem[concurrentStack.Count];
        var count = concurrentStack.TryPopRange(items);
        for (int i = 0; i < count; i++)
        {
            items[i].Dispose();
        }
    }

    /// <summary>
    /// Pops all items from the concurrent stack.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="concurrentStack">The concurrent stack.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task DisposeAllAsync<TItem>(ConcurrentStack<TItem> concurrentStack)
        where TItem : IAsyncDisposable
    {
        var items = new TItem[concurrentStack.Count];
        var count = concurrentStack.TryPopRange(items);
        for (int i = 0; i < count; i++)
        {
            await items[i].DisposeAsync().ConfigureAwait(false);
        }
    }
}