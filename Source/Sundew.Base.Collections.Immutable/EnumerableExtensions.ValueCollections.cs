// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.ValueCollections.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections;

using System.Collections.Generic;
using System.Collections.Immutable;

/// <summary>
/// Extends the generic IEnumerable interface with functions.
/// </summary>
public static partial class EnumerableExtensions
{
    /// <summary>
    /// Converts the specified <see cref="ImmutableArray{T}"/> to a <see cref="ValueArray{TItem}"/>.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>The value array.</returns>
    public static ValueArray<TItem> ToValueArray<TItem>(this IEnumerable<TItem> enumerable)
    {
        return new ValueArray<TItem>(enumerable.ToImmutableArray());
    }

    /// <summary>
    /// Converts the specified <see cref="IImmutableList{T}"/> to a <see cref="ValueList{TItem}"/>.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>The value list.</returns>
    public static ValueList<TItem> ToValueList<TItem>(this IEnumerable<TItem> enumerable)
    {
        return new ValueList<TItem>(enumerable.ToImmutableList());
    }
}