// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.ByCardinality.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Extends arrays with easy to use methods.
/// </summary>
public static partial class EnumerableExtensions
{
    /// <summary>
    /// Gets the cardinality of the specified <see cref="IEnumerable{T}"/> into either an empty, single or a multiple items result.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>The result.</returns>
    public static ListCardinality<TItem> ByCardinality<TItem>(this IEnumerable<TItem>? enumerable)
    {
        ListCardinality<TItem> FromList(IEnumerable<TItem> enumerable, int count)
        {
            return count switch
            {
                0 => ListCardinality<TItem>.Empty,
                1 => ListCardinality<TItem>.Single(enumerable.First()),
                _ => ListCardinality<TItem>.Multiple(enumerable),
            };
        }

        ListCardinality<TItem> FromEnumerable(IEnumerable<TItem> enumerable)
        {
            using var enumerator = enumerable.GetEnumerator();
            TItem single = default!;
            if (enumerator.MoveNext())
            {
                single = enumerator.Current;
            }
            else
            {
                return ListCardinality<TItem>.Empty;
            }

            if (enumerator.MoveNext())
            {
                return ListCardinality<TItem>.Multiple(enumerable);
            }

            return ListCardinality<TItem>.Single(single);
        }

        return enumerable switch
        {
            null => ListCardinality<TItem>.Empty,
            IReadOnlyCollection<TItem> readOnlyCollection => FromList(enumerable, readOnlyCollection.Count),
            ICollection<TItem> collection => FromList(enumerable, collection.Count),
            _ => FromEnumerable(enumerable),
        };
    }
}