// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.OnlyOneOrDefault.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections;

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
    {
        item = OnlyOneOrDefault(enumerable);
        return !EqualityComparer<TItem?>.Default.Equals(item, default);
    }

    /// <summary>
    /// Gets a value indicating whether only one item exists in the enumerable and gets it.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="item">The item.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns><c>true</c> if exactly one item exists otherwise <c>false</c>.</returns>
    public static bool TryGetOnlyOne<TItem>(this IEnumerable<TItem?>? enumerable, [NotNullWhen(true)] out TItem? item, Func<TItem?, bool> predicate)
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
    {
        if (enumerable == null)
        {
            return default;
        }

        using var enumerator = enumerable.GetEnumerator();
        var item = enumerator.MoveNext() ? enumerator.Current : default;
        item = enumerator.MoveNext() ? default : item;
        return item;
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
}