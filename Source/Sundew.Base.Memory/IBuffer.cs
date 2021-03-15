// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBuffer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Memory
{
    using System;
    using System.Buffers;
    using System.Collections.Generic;

    /// <summary>
    /// Interface for implementing a buffer.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <seealso cref="System.Buffers.IBufferWriter{TItem}" />
    public interface IBuffer<TItem> : IBufferWriter<TItem>
    {
        /// <summary>
        /// Gets the start index of the underlying memory.
        /// </summary>
        /// <value>
        /// The start index.
        /// </value>
        int StartIndex { get; }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        int Length { get; }

        /// <summary>
        /// Gets the capacity.
        /// </summary>
        /// <value>
        /// The capacity.
        /// </value>
        int Capacity { get; }

        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        int Position { get; }

        /// <summary>
        /// Gets or sets the item at the specified index.
        /// </summary>
        /// <value>
        /// The item.
        /// </value>
        /// <param name="index">The index.</param>
        TItem this[int index] { get; set; }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The amount of item written.</returns>
        int Write(TItem item);

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>The amount of item written.</returns>
        int Write(ICollection<TItem> items);

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>The amount of item written.</returns>
        int Write(IReadOnlyList<TItem> items);

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>The amount of item written.</returns>
        int Write(TItem[] items);

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>The amount of item written.</returns>
        int Write(ReadOnlySpan<TItem> items);

        /// <summary>
        /// Writes the specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>The amount of item written.</returns>
        int WriteRange(IEnumerable<TItem> items);

        /// <summary>
        /// Gets the buffer as memory.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The length.</param>
        /// <returns>The memory.</returns>
        ReadOnlyMemory<TItem> AsMemory(int startIndex, int length);

        /// <summary>
        /// Gets the buffer as a span.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The length.</param>
        /// <returns>The span.</returns>
        ReadOnlySpan<TItem> AsSpan(int startIndex, int length);

        /// <summary>
        /// Gets the buffer as memory.
        /// </summary>
        /// <returns>The memory.</returns>
        ReadOnlyMemory<TItem> AsMemory();

        /// <summary>
        /// Gets the buffer as a span.
        /// </summary>
        /// <returns>The span.</returns>
        ReadOnlySpan<TItem> AsSpan();

        /// <summary>
        /// Gets the buffer as memory.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The length.</param>
        /// <returns>The memory.</returns>
        Memory<TItem> GetMemory(int startIndex, int length);

        /// <summary>
        /// Gets the buffer as a span.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The length.</param>
        /// <returns>The span.</returns>
        Span<TItem> GetSpan(int startIndex, int length);

        /// <summary>
        /// Gets the buffer as memory.
        /// </summary>
        /// <returns>The memory.</returns>
        Memory<TItem> GetMemory();

        /// <summary>
        /// Gets the buffer as a span.
        /// </summary>
        /// <returns>The span.</returns>
        Span<TItem> GetSpan();

        /// <summary>
        /// Slices the specified start index.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="capacity">The capacity.</param>
        /// <returns>
        /// A buffer slice.
        /// </returns>
        IBuffer<TItem> Slice(int startIndex, int capacity);

        /// <summary>
        /// Slices the specified relative to.
        /// </summary>
        /// <param name="relativeTo">The relative to.</param>
        /// <param name="capacityBefore">The capacity before.</param>
        /// <param name="capacityAfter">The capacity after.</param>
        /// <returns>A sliced buffer.</returns>
        IBuffer<TItem> Slice(IBuffer<TItem> relativeTo, int capacityBefore, int capacityAfter);

        /// <summary>
        /// Slices the specified relative to.
        /// </summary>
        /// <param name="relativeTo">The relative to.</param>
        /// <param name="slice">The slice.</param>
        /// <param name="capacity">The capacity.</param>
        /// <param name="includeBuffer">if set to <c>true</c> [include buffer].</param>
        /// <returns>A sliced buffer.</returns>
        IBuffer<TItem> Slice(IBuffer<TItem> relativeTo, Slice slice, int capacity, bool includeBuffer = false);

        /// <summary>
        /// Creates the default slice.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        /// <param name="defaultSlice">The default slice.</param>
        /// <returns>
        /// A buffer slice.
        /// </returns>
        IBuffer<TItem> SliceDefault(int capacity, DefaultSlice defaultSlice = DefaultSlice.Append);

        /// <summary>
        /// Slices the content within the content (start - length range).
        /// </summary>
        /// <returns>A buffer slice.</returns>
        IBuffer<TItem> Slice();

        /// <summary>Converts to array.</summary>
        /// <returns>The new array.</returns>
        TItem[] ToArray();
    }
}