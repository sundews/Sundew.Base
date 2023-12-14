// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.DistinctInOrder.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections;

using System.Collections.Generic;

/// <summary>
/// Extends the generic IEnumerable interface with functions.
/// </summary>
public static partial class EnumerableExtensions
{
    /// <summary>
    /// Gets the distinct items from the <see cref="IEnumerable{T}"/> while preserving the order.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>A enumerable containing items only once.</returns>
    public static IEnumerable<TItem> DistinctInOrder<TItem>(this IEnumerable<TItem> enumerable)
    {
        return enumerable.DistinctInOrder(EqualityComparer<TItem>.Default);
    }

    /// <summary>
    /// Gets the distinct items from the <see cref="IEnumerable{T}"/> while preserving the order.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="equalityComparer">The equality comparer.</param>
    /// <returns>A enumerable containing items only once.</returns>
    public static IEnumerable<TItem> DistinctInOrder<TItem>(this IEnumerable<TItem> enumerable, IEqualityComparer<TItem> equalityComparer)
    {
        var hashSet = new HashSet<TItem>(equalityComparer);
        foreach (var item in enumerable)
        {
            if (hashSet.Add(item))
            {
                yield return item;
            }
        }
    }
}