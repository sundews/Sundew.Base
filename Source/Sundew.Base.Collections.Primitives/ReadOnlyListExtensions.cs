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
    /// Extends TValue with option-like functionality.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    extension<TItem>(IReadOnlyCollection<TItem> list)
    {
        /// <summary>
        /// Gets a value indicating whether the list has any items.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the specified source array has any; otherwise, <c>false</c>.
        /// </returns>
#pragma warning disable SA1101
        public bool HasAny => list.Count > 0;
#pragma warning restore SA1101

        /// <summary>
        /// Gets a value indicating whether the list is empty.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the specified list is empty; otherwise, <c>false</c>.
        /// </returns>
#pragma warning disable SA1101
        public bool IsEmpty => list.Count == 0;
#pragma warning restore SA1101
    }

    /// <summary>
    /// Gets an <see cref="IReadOnlyList{TItem}"/> from an item.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="item">The item.</param>
    /// <returns>An <see cref="IReadOnlyList{TItem}"/>.</returns>
    public static IReadOnlyList<TItem> ToReadOnlyList<TItem>(this TItem? item)
        where TItem : struct
    {
        if (item != null)
        {
            return new[] { item.Value };
        }

        return Arrays.Empty<TItem>();
    }

    /// <summary>
    /// Gets an <see cref="IReadOnlyList{TItem}"/> from an item.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="item">The item.</param>
    /// <returns>An <see cref="IReadOnlyList{TItem}"/>.</returns>
    public static IReadOnlyList<TItem> ToReadOnlyList<TItem>(this TItem? item)
        where TItem : class
    {
        if (item != null)
        {
            return [item];
        }

        return Arrays.Empty<TItem>();
    }

    /// <summary>
    /// Copies items from the specified list to the specified array.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="list">The read only list.</param>
    /// <param name="sourceIndex">Index of the source.</param>
    /// <param name="targetArray">The target array.</param>
    /// <param name="targetIndex">Index of the target.</param>
    /// <param name="count">The count.</param>
    public static void CopyTo<TItem>(
        this IReadOnlyList<TItem> list,
        int sourceIndex,
        TItem[] targetArray,
        int targetIndex,
        int count)
    {
        for (int i = sourceIndex; i < count; i++)
        {
            targetArray[targetIndex++] = list[i];
        }
    }

    /// <summary>
    /// Copies items from the specified list to the specified array.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="list">The read only list.</param>
    /// <param name="sourceIndex">Index of the source.</param>
    /// <param name="targetSpan">The target array.</param>
    /// <param name="targetIndex">Index of the target.</param>
    /// <param name="count">The count.</param>
    public static void CopyTo<TItem>(
        this IReadOnlyList<TItem> list,
        int sourceIndex,
        Span<TItem> targetSpan,
        int targetIndex,
        int count)
    {
        for (int i = sourceIndex; i < count; i++)
        {
            targetSpan[targetIndex++] = list[i];
        }
    }

    /// <summary>
    /// Converts all items into a new array.
    /// </summary>
    /// <typeparam name="TInItem">The type of the input array items.</typeparam>
    /// <typeparam name="TOutItem">The type of the output array item.</typeparam>
    /// <param name="sourceArray">The source array.</param>
    /// <param name="converter">The converter.</param>
    /// <returns>The converted array.</returns>
    public static TOutItem[] ConvertAll<TInItem, TOutItem>(this IReadOnlyList<TInItem>? sourceArray, Func<TInItem, TOutItem> converter)
    {
        if (sourceArray == null)
        {
            return [];
        }

        if (converter == null)
        {
            throw new ArgumentNullException(nameof(converter));
        }

        var result = new TOutItem[sourceArray.Count];
        for (var index = 0; index < sourceArray.Count; index++)
        {
            result[index] = converter(sourceArray[index]);
        }

        return result;
    }
}