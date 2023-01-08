// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.Classify.cs" company="Hukano">
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
    /// Classifies the specified <see cref="IEnumerable{T}"/> into either an empty, single or a multiple items result.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>The result.</returns>
    public static ListClassification<TItem> Classify<TItem>(this IEnumerable<TItem>? enumerable)
    {
        ListClassification<TItem> FromList(IEnumerable<TItem> enumerable, int count)
        {
            return count switch
            {
                0 => ListClassification<TItem>.Empty(),
                1 => ListClassification<TItem>.Single(enumerable.First()),
                _ => ListClassification<TItem>.Multiple(enumerable),
            };
        }

        ListClassification<TItem> FromEnumerable(IEnumerable<TItem> enumerable)
        {
            using var enumerator = enumerable.GetEnumerator();
            TItem single = default!;
            if (enumerator.MoveNext())
            {
                single = enumerator.Current;
            }
            else
            {
                return ListClassification<TItem>.Empty();
            }

            if (enumerator.MoveNext())
            {
                return ListClassification<TItem>.Multiple(enumerable);
            }

            return ListClassification<TItem>.Single(single);
        }

        return enumerable switch
        {
            null => ListClassification<TItem>.Empty(),
            IReadOnlyCollection<TItem> readOnlyCollection => FromList(enumerable, readOnlyCollection.Count),
            ICollection<TItem> collection => FromList(enumerable, collection.Count),
            _ => FromEnumerable(enumerable),
        };
    }
}