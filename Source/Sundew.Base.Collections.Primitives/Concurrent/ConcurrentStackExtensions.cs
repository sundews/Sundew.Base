// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConcurrentStackExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections.Concurrent;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

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
    /// <returns>An <see cref="IEnumerable{T}"/> containing the popped items.</returns>
    public static IEnumerable<TItem> PopRange<TItem>(ConcurrentStack<TItem> concurrentStack)
    {
        var items = new TItem[concurrentStack.Count];
        var count = concurrentStack.TryPopRange(items);
        return items.Take(count);
    }
}