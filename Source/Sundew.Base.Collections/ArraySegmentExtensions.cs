// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArraySegmentExtensions.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections;

using System;

/// <summary>
/// Extends <see cref="ArraySegment{TItem}"/> with easy to use methods.
/// </summary>
public static class ArraySegmentExtensions
{
    /// <summary>
    /// Gets the item at specified index.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="arraySegment">The array segment.</param>
    /// <param name="index">The index.</param>
    /// <returns>The requested item.</returns>
    public static TItem GetValue<TItem>(this ArraySegment<TItem> arraySegment, int index)
    {
        return arraySegment.Array[arraySegment.Offset + index];
    }

    /// <summary>
    /// Gets the item at specified index.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="arraySegment">The array segment.</param>
    /// <param name="index">The index.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The requested item.
    /// </returns>
    public static TItem SetValue<TItem>(this ArraySegment<TItem> arraySegment, int index, TItem value)
    {
        return arraySegment.Array[arraySegment.Offset + index] = value;
    }

    /// <summary>
    /// Gets the segment.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="arraySegment">The array segment.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="count">The count.</param>
    /// <returns>The new <see cref="ArraySegment{TItem}"/>.</returns>
    public static ArraySegment<TItem> GetSegment<TItem>(this ArraySegment<TItem> arraySegment, int offset, int count)
    {
        return new ArraySegment<TItem>(arraySegment.Array, arraySegment.Offset + offset, count);
    }

    /// <summary>
    /// Copies the source array segment to the specified target array.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="sourceArraySegment">The source array.</param>
    /// <param name="sourceIndex">Index of the source.</param>
    /// <param name="targetArray">The target array.</param>
    /// <param name="targetIndex">Index of the target.</param>
    /// <param name="count">The count.</param>
    public static void CopyTo<TItem>(this ArraySegment<TItem> sourceArraySegment, int sourceIndex, TItem[] targetArray, int targetIndex, int count)
    {
        sourceArraySegment.Array.CopyTo(sourceArraySegment.Offset + sourceIndex, targetArray, targetIndex, count);
    }

    /// <summary>
    /// Create an array from the specified array segment..
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="arraySegment">The array segment.</param>
    /// <returns>The new array.</returns>
    public static TItem[] ToArray<TItem>(this ArraySegment<TItem> arraySegment)
    {
        var array = new TItem[arraySegment.Count];
        arraySegment.CopyTo(0, array, 0, arraySegment.Count);
        return array;
    }
}