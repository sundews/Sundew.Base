// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BufferExtensions.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Memory;

using System;
using System.Collections.Generic;
using Sundew.Base.Memory.Internal;

/// <summary>
/// Extends <see cref="IBuffer{TItem}"/> with easy to use methods.
/// </summary>
public static class BufferExtensions
{
    /// <summary>
    /// Adds the range.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="buffer">The buffer.</param>
    /// <param name="items">The items.</param>
    /// <returns>The current position.</returns>
    public static int Write<TItem>(this IBuffer<TItem> buffer, TItem[] items)
    {
        return buffer.Write(items.AsSpan());
    }

    /// <summary>
    /// Adds the range.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="buffer">The buffer.</param>
    /// <param name="items">The items.</param>
    /// <returns>The current position.</returns>
    public static int Write<TItem>(this IBuffer<TItem> buffer, ReadOnlyMemory<TItem> items)
    {
        return buffer.Write(items.Span);
    }

    /// <summary>
    /// Gets the buffer as memory.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="buffer">The buffer.</param>
    /// <param name="startIndex">The start index.</param>
    /// <returns>
    /// The memory.
    /// </returns>
    public static ReadOnlyMemory<TItem> AsMemory<TItem>(this IBuffer<TItem> buffer, int startIndex)
    {
        return buffer.AsMemory(startIndex, buffer.Length - startIndex);
    }

    /// <summary>
    /// Gets the buffer as a span.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="buffer">The buffer.</param>
    /// <param name="startIndex">The start index.</param>
    /// <returns>
    /// The span.
    /// </returns>
    public static ReadOnlySpan<TItem> AsSpan<TItem>(this IBuffer<TItem> buffer, int startIndex)
    {
        return buffer.AsSpan(startIndex, buffer.Length - startIndex);
    }

    /// <summary>
    /// Gets the buffer as memory.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="buffer">The buffer.</param>
    /// <param name="startIndex">The start index.</param>
    /// <returns>
    /// The memory.
    /// </returns>
    public static Memory<TItem> GetMemoryFromIndex<TItem>(this IBuffer<TItem> buffer, int startIndex)
    {
        return buffer.GetMemory(startIndex, buffer.Capacity - startIndex);
    }

    /// <summary>
    /// Gets the buffer as memory.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="buffer">The buffer.</param>
    /// <param name="startIndex">The start index.</param>
    /// <returns>
    /// The memory.
    /// </returns>
    public static Span<TItem> GetSpanFromIndex<TItem>(this IBuffer<TItem> buffer, int startIndex)
    {
        return buffer.GetSpan(startIndex, buffer.Capacity - startIndex);
    }

    /// <summary>
    /// Slices the specified start index.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="buffer">The buffer.</param>
    /// <param name="startIndex">The start index.</param>
    /// <returns>A buffer slice.</returns>
    public static IBuffer<TItem> Slice<TItem>(this IBuffer<TItem> buffer, int startIndex)
    {
        return buffer.Slice(startIndex, buffer.Length - startIndex);
    }

    /// <summary>
    /// Adds the range.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="buffer">The buffer.</param>
    /// <param name="items">The items.</param>
    /// <returns>The amount of items written.</returns>
    internal static int Write<TItem>(IBufferInternal<TItem> buffer, IEnumerable<TItem>? items)
    {
        if (items == null)
        {
            return 0;
        }

        switch (items)
        {
            case TItem[] array:
                return buffer.Write(array);
            case ICollection<TItem> list:
                return buffer.Write(list);
            case IReadOnlyList<TItem> readOnlyItems:
                return buffer.Write(readOnlyItems);
            case IReadOnlyCollection<TItem> readOnlyCollection:
                buffer.EnsureAdditionalCapacity(readOnlyCollection.Count);
                foreach (var item in items)
                {
                    buffer.WriteInternal(item);
                }

                return readOnlyCollection.Count;
        }

        var position = buffer.Position;
        foreach (var item in items)
        {
            buffer.Write(item);
        }

        return buffer.Position - position;
    }
}