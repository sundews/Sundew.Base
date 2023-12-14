// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections;

using System;
using System.Collections.Generic;

/// <summary>
/// Extends the generic IEnumerable interface with functions.
/// </summary>
public static partial class EnumerableExtensions
{
    /// <summary>
    /// Iterates through the enumerable and executes the specified action on all items.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="action">The action.</param>
    public static void ForEach<TItem>(this IEnumerable<TItem> enumerable, Action<TItem, int> action)
    {
        var i = 0;
        foreach (var item in enumerable)
        {
            action(item, i++);
        }
    }

    /// <summary>
    /// Iterates through the enumerable and executes the specified action on all items.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="action">The action.</param>
    public static void ForEach<TItem>(this IEnumerable<TItem> enumerable, Action<TItem> action)
    {
        foreach (var item in enumerable)
        {
            action(item);
        }
    }

    /// <summary>
    /// Iterates through the enumerable and executes the specified action on all items.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="action">The action.</param>
    /// <returns>The original enumerable.</returns>
    public static IEnumerable<TItem> ForEachItem<TItem>(this IEnumerable<TItem> enumerable, Action<TItem, int> action)
    {
        var i = 0;
        foreach (var item in enumerable)
        {
            action(item, i++);
            yield return item;
        }
    }

    /// <summary>
    /// Iterates through the enumerable and executes the specified action on all items.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="action">The action.</param>
    /// <returns>The original enumerable.</returns>
    public static IEnumerable<TItem> ForEachItem<TItem>(this IEnumerable<TItem> enumerable, Action<TItem> action)
    {
        foreach (var item in enumerable)
        {
            action(item);
            yield return item;
        }
    }
}