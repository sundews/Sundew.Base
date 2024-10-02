// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.OnlyOneOrDefault.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections.Linq;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Extends the generic IEnumerable interface with functions.
/// </summary>
public static partial class EnumerableExtensions
{
    /// <summary>
    /// Gets a value indicating whether only one item exists in the enumerable and gets it.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="item">The item.</param>
    /// <returns><c>true</c> if exactly one item exists otherwise <c>false</c>.</returns>
    public static bool TryGetOnlyOne<TItem>(this IEnumerable<TItem?>? enumerable, [NotNullWhen(true)] out TItem? item)
        where TItem : class
    {
        item = OnlyOneOrDefault(enumerable);
        return item != null;
    }

    /// <summary>
    /// Gets a value indicating whether only one item exists in the enumerable and gets it.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="item">The item.</param>
    /// <returns><c>true</c> if exactly one item exists otherwise <c>false</c>.</returns>
    public static bool TryGetOnlyOne<TItem>(this IEnumerable<TItem?>? enumerable, out TItem item)
        where TItem : struct
    {
        if (enumerable == null)
        {
            item = default;
            return false;
        }

        if (TryGetOnlyOneItem(enumerable, out var item2))
        {
            item = item2.GetValueOrDefault();
            return item2.HasValue;
        }

        item = default;
        return false;
    }

    /// <summary>
    /// Gets a value indicating whether only one item exists in the enumerable and gets it.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="item">The item.</param>
    /// <returns><c>true</c> if exactly one item exists otherwise <c>false</c>.</returns>
    public static bool TryGetOnlyOneValue<TItem>(this IEnumerable<TItem>? enumerable, out TItem item)
        where TItem : struct
    {
        if (enumerable == null)
        {
            item = default;
            return false;
        }

        return TryGetOnlyOneItem(enumerable, out item);
    }

    /// <summary>
    /// Gets a value indicating whether only one item exists in the enumerable and gets it.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="item">The item.</param>
    /// <returns><c>true</c> if exactly one item exists otherwise <c>false</c>.</returns>
    public static bool TryGetOnlyOne<TItem>(this IEnumerable<TItem?>? enumerable, Func<TItem?, bool> predicate, [NotNullWhen(true)] out TItem? item)
    {
        item = OnlyOneOrDefault(enumerable, predicate);
        return !EqualityComparer<TItem?>.Default.Equals(item, default);
    }

    /// <summary>
    /// Gets the item if the enumerable contains exactly one item and otherwise returns the default value.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>The item or the default value.</returns>
    public static TItem? OnlyOneOrDefault<TItem>(this IEnumerable<TItem?>? enumerable)
        where TItem : class
    {
        if (enumerable == null)
        {
            return default;
        }

        return TryGetOnlyOneItem(enumerable, out var item) ? item : default;
    }

    /// <summary>
    /// Gets the item if the enumerable contains exactly one item and otherwise returns the default value.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>The item or the default value.</returns>
    public static TItem? OnlyOneOrDefault<TItem>(this IEnumerable<TItem?>? enumerable)
        where TItem : struct
    {
        if (enumerable == null)
        {
            return default;
        }

        return TryGetOnlyOneItem(enumerable, out var item) ? item : default;
    }

    /// <summary>
    /// Gets the item if the enumerable contains exactly one item and otherwise returns the default value.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>The item or the default value.</returns>
    public static TItem OnlyOneOrDefaultValue<TItem>(this IEnumerable<TItem>? enumerable)
        where TItem : struct
    {
        if (enumerable == null)
        {
            return default;
        }

        return TryGetOnlyOneItem(enumerable, out var result) ? result : default;
    }

    /// <summary>
    /// Gets the item if the enumerable contains exactly one item and otherwise returns the default value.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>The item or the default value.</returns>
    public static TItem? OnlyOneOrDefault<TItem>(this IEnumerable<TItem?>? enumerable, Func<TItem?, bool> predicate)
    {
        if (enumerable == null)
        {
            return default;
        }

        TItem? result = default;
        var count = 0;
        foreach (var item in enumerable)
        {
            if (!predicate(item))
            {
                continue;
            }

            if (count > 0)
            {
                result = default;
                break;
            }

            result = item;
            count++;
        }

        return result;
    }

    private static bool TryGetOnlyOneItem<TItem>(IEnumerable<TItem?> enumerable, out TItem? item)
    {
        using var enumerator = enumerable.GetEnumerator();
        (item, var result) = enumerator.MoveNext() ? (enumerator.Current, true) : (default, false);
        if (!result)
        {
            item = default;
            return false;
        }

        (item, result) = enumerator.MoveNext() ? (default, false) : (item, true);
        return result;
    }
}