// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.WhereNotDefault.cs" company="Sundews">
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
    /// <summary>
    /// Finds the index based on the given predicate.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>The index found by the matching predicate.</returns>
    public static IEnumerable<TItem> WhereNotNull<TItem>(this IEnumerable<TItem?>? enumerable)
    {
        return enumerable == null ? [] : enumerable.Where(x => x != null).Select(x => x!);
    }

    /// <summary>
    /// Finds the index based on the given predicate.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>The index found by the matching predicate.</returns>
    public static IEnumerable<TItem> WhereNotNull<TItem>(this IEnumerable<TItem?>? enumerable)
        where TItem : struct
    {
        return enumerable == null ? [] : enumerable.Where(x => x.HasValue).Select(x => x!.Value);
    }

    /// <summary>
    /// Finds the index based on the given predicate.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>The index found by the matching predicate.</returns>
    public static IEnumerable<TItem> WhereNotDefault<TItem>(this IEnumerable<TItem?>? enumerable)
        where TItem : struct, IEquatable<TItem>
    {
        return enumerable == null ? [] : enumerable.Where(x => x.HasValue && !x.Value.Equals(default)).Select(x => x!.Value);
    }

    /// <summary>
    /// Finds the index based on the given predicate.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>The index found by the matching predicate.</returns>
    public static IEnumerable<TItem> WhereNotDefault<TItem>(this IEnumerable<TItem>? enumerable)
        where TItem : struct, IEquatable<TItem>
    {
        return enumerable == null ? [] : enumerable.Where(x => !x.Equals(default));
    }
}