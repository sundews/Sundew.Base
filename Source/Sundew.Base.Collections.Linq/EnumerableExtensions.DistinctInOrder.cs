// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.DistinctByInOrder.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections.Linq;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Extends the generic IEnumerable interface with functions.
/// </summary>
public static partial class EnumerableExtensions
{
#if !NET6_0_OR_GREATER
    /// <summary>
    /// Gets the distinct items from the <see cref="IEnumerable{T}"/> while preserving the order.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TComparand">The comparand type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="comparandFunc">The comparand func.</param>
    /// <returns>A enumerable containing items only once.</returns>
    public static IEnumerable<TItem> DistinctBy<TItem, TComparand>(this IEnumerable<TItem> enumerable, Func<TItem, TComparand> comparandFunc)
    {
        return enumerable.Distinct(new FuncEqualityComparer<TItem, TComparand>(comparandFunc));
    }
#endif

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

    /// <summary>
    /// Gets the distinct items from the <see cref="IEnumerable{T}"/> while preserving the order.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TComparand">The comparand type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="comparandFunc">The comparand func.</param>
    /// <returns>A enumerable containing items only once.</returns>
    public static IEnumerable<TItem> DistinctByInOrder<TItem, TComparand>(this IEnumerable<TItem> enumerable, Func<TItem, TComparand> comparandFunc)
    {
        return enumerable.DistinctInOrder(new FuncEqualityComparer<TItem, TComparand>(comparandFunc));
    }

    private sealed class FuncEqualityComparer<TItem, TComparand> : IEqualityComparer<TItem>
    {
        private readonly Func<TItem, TComparand> comparandFunc;

        public FuncEqualityComparer(Func<TItem, TComparand> comparandFunc)
        {
            this.comparandFunc = comparandFunc;
        }

        public bool Equals(TItem? x, TItem? y)
        {
            return (x == null && y == null) || (x != null && y != null && Equals(this.comparandFunc(x), this.comparandFunc(y)));
        }

        public int GetHashCode(TItem obj)
        {
            return this.comparandFunc(obj)?.GetHashCode() ?? 0;
        }
    }
}