// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListExtensions.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections;

using System.Collections.Generic;

/// <summary>
/// Defines extension methods for the generic IList interface.
/// </summary>
public static class ListExtensions
{
    /// <summary>
    /// Adds the range.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="list">The list to be added to.</param>
    /// <param name="enumerable">The enumerable.</param>
    public static void AddRange<TItem>(this IList<TItem> list, IEnumerable<TItem> enumerable)
    {
        if (list is List<TItem> realList)
        {
            realList.AddRange(enumerable);
            return;
        }

        foreach (var item in enumerable)
        {
            list.Add(item);
        }
    }
}