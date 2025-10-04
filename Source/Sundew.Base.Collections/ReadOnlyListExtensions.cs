// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyListExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections;

using System;
using System.Collections.Generic;

/// <summary>
/// Extends <see cref="IReadOnlyList{TItem}"/> interface.
/// </summary>
public static class ReadOnlyListExtensions
{
    /// <summary>
    /// For loops the IList and executes action on all items.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="list">The target list.</param>
    /// <param name="action">The action.</param>
    public static void For<TItem>(this IReadOnlyList<TItem> list, Action<TItem> action)
    {
        for (var i = 0; i < list.Count; i++)
        {
            action(list[i]);
        }
    }

    /// <summary>
    /// For loops the IList and executes action on all items.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <typeparam name="TOutItem">The type of the out item.</typeparam>
    /// <param name="list">The target list.</param>
    /// <param name="getItemFunc">The get item function.</param>
    /// <param name="action">The action.</param>
    public static void For<TItem, TOutItem>(this IReadOnlyList<TItem> list, Func<TItem, TOutItem> getItemFunc, Action<TItem, TOutItem, int> action)
    {
        for (var i = 0; i < list.Count; i++)
        {
            var item = list[i];
            action(item, getItemFunc(item), i);
        }
    }

    /// <summary>
    /// For loops the IList in reverse order and executes action on all item living up to the specified predicate.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="list">The target list.</param>
    /// <param name="action">The action.</param>
    public static void ForReverse<TItem>(this IReadOnlyList<TItem> list, Action<TItem> action)
    {
        for (var i = list.Count - 1; i >= 0; i--)
        {
            action(list[i]);
        }
    }

    /// <summary>
    /// For loops the IList and executes action on all items.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <typeparam name="TOutItem">The type of the out item.</typeparam>
    /// <param name="list">The target list.</param>
    /// <param name="getItemFunc">The get item function.</param>
    /// <param name="action">The action.</param>
    public static void ForReverse<TItem, TOutItem>(this IReadOnlyList<TItem> list, Func<TItem, TOutItem> getItemFunc, Action<TItem, TOutItem, int> action)
    {
        for (var i = list.Count - 1; i >= 0; i--)
        {
            var item = list[i];
            action(item, getItemFunc(item), i);
        }
    }
}