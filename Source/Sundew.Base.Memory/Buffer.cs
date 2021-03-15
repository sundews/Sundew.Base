// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Buffer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Memory
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Sundew.Base.Memory.Internal;
    using Sundew.Base.Numeric;
    using Sundew.Base.Reporting;

    /// <summary>
    /// Represents a buffer that can grow in size as needed.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public class Buffer<TItem> : IBufferInternal<TItem>
    {
        private readonly IBufferResizer<TItem> bufferResizer;
        private readonly int minimumCapacity;
        private readonly IBufferReporter? bufferReporter;
        private int startIndex = -1;
        private int position;
        private int length;
        private BufferSlice<TItem>? lastBufferSlice;

        /// <summary>
        /// Initializes a new instance of the <see cref="Buffer{TItem}" /> class.
        /// </summary>
        /// <param name="minimumCapacity">The capacity.</param>
        /// <param name="bufferReporter">The buffer reporter.</param>
        public Buffer(int minimumCapacity = 16, IBufferReporter? bufferReporter = null)
        : this(new AllocatingBufferResizer<TItem>(), minimumCapacity, bufferReporter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Buffer{TItem}" /> class.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="bufferReporter">The buffer reporter.</param>
        public Buffer(TItem[] items, IBufferReporter? bufferReporter = null)
            : this(items.Length, bufferReporter)
        {
            this.InternalBuffer = items;
            this.startIndex = 0;
            this.Length = items.Length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Buffer{TItem}" /> class.
        /// </summary>
        /// <param name="bufferResizer">The array provider.</param>
        /// <param name="minimumCapacity">The minimum capacity.</param>
        /// <param name="bufferReporter">The buffer reporter.</param>
        public Buffer(IBufferResizer<TItem> bufferResizer, int minimumCapacity = 16, IBufferReporter? bufferReporter = null)
        {
            this.bufferResizer = bufferResizer;
            this.minimumCapacity = minimumCapacity;
            this.bufferReporter = bufferReporter;
            this.bufferReporter?.SetSource(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Buffer{TItem}" /> class.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="bufferResizer">The buffer resizer.</param>
        /// <param name="bufferReporter">The buffer reporter.</param>
        public Buffer(TItem[] items, IBufferResizer<TItem> bufferResizer, IBufferReporter? bufferReporter = null)
            : this(bufferResizer, items.Length, bufferReporter)
        {
            this.InternalBuffer = items;
        }

        /// <summary>
        /// Gets the start index.
        /// </summary>
        /// <value>
        /// The start index.
        /// </value>
        public int StartIndex
        {
            get => this.startIndex;
            private set
            {
                if (this.startIndex == -1 || this.startIndex > value)
                {
                    this.startIndex = value;
                }
            }
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public int Length
        {
            get => this.length;
            internal set => this.length = Math.Max(this.Length, value);
        }

        /// <summary>
        /// Gets the capacity.
        /// </summary>
        /// <value>
        /// The capacity.
        /// </value>
        public int Capacity => this.InternalBuffer?.Length ?? this.minimumCapacity;

        /// <summary>
        /// Gets the current position.
        /// </summary>
        /// <value>
        /// The current position.
        /// </value>
        public int Position
        {
            get => this.position;
            private set
            {
                this.ProposeBounds(this.position, this.startIndex == -1 ? value : value - this.startIndex);
                this.position = value;
            }
        }

        internal TItem[]? InternalBuffer { get; private set; }

        /// <summary>
        /// Gets or sets the item at the specified index.
        /// </summary>
        /// <value>
        /// The item.
        /// </value>
        /// <param name="index">The index.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1065:Do not raise exceptions in unexpected locations", Justification = "IndexOfOutRangeException not listed as invalid for indexer get.")]
        public TItem this[int index]
        {
            get
            {
                if (this.InternalBuffer == null)
                {
                    if (index < this.minimumCapacity)
                    {
                        return default!;
                    }
                }
                else if (index < this.InternalBuffer.Length)
                {
                    return this.InternalBuffer[index];
                }

                throw new IndexOutOfRangeException($"The index {index} is out of range. Capacity: {this.Capacity}");
            }

            set
            {
                VerifyIndex(index);
                this.StartIndex = index;
                this.InternalBuffer = this.PrivateEnsureCapacity(index + 1);
                this.InternalBuffer[index] = value;
            }
        }

        /// <summary>
        /// Ensures the additional capacity.
        /// </summary>
        /// <param name="requiredAdditionalCapacity">The required additional capacity.</param>
        public void EnsureAdditionalCapacity(int requiredAdditionalCapacity)
        {
            this.InternalBuffer = this.PrivateEnsureAdditionalCapacity(requiredAdditionalCapacity);
        }

        /// <summary>
        /// Expects the additional items.
        /// </summary>
        /// <param name="requiredCapacity">The capacity.</param>
        public void EnsureCapacity(int requiredCapacity)
        {
            this.InternalBuffer = this.PrivateEnsureCapacity(requiredCapacity);
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The current position.</returns>
        public int Write(TItem item)
        {
            this.EnsureAdditionalCapacity(1);
            return this.PrivateWrite(item);
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>The current position.</returns>
        public int Write(ICollection<TItem> items)
        {
            if (items.Count == 0)
            {
                return 0;
            }

            this.InternalBuffer = this.PrivateEnsureAdditionalCapacity(items.Count);
            items.CopyTo(this.InternalBuffer, this.Position);
            this.Position += items.Count;
            return items.Count;
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>The current position.</returns>
        public int Write(IReadOnlyList<TItem> items)
        {
            if (items.Count == 0)
            {
                return 0;
            }

            this.InternalBuffer = this.PrivateEnsureAdditionalCapacity(items.Count);
            var targetIndex = this.Position;
            for (int i = 0; i < items.Count; i++)
            {
                this.InternalBuffer[targetIndex++] = items[i];
            }

            this.Position += items.Count;
            return items.Count;
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>The current position.</returns>
        public int Write(TItem[] items)
        {
            return this.Write(items.AsSpan());
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>The current position.</returns>
        public int Write(ReadOnlySpan<TItem> items)
        {
            if (items.IsEmpty)
            {
                return 0;
            }

            this.InternalBuffer = this.PrivateEnsureAdditionalCapacity(items.Length);
            items.CopyTo(this.InternalBuffer.AsSpan(this.Position, items.Length));
            this.Position += items.Length;
            return items.Length;
        }

        /// <summary>
        /// Writes the specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>The current position.</returns>
        public int WriteRange(IEnumerable<TItem> items)
        {
            return BufferExtensions.Write(this, items);
        }

        /// <summary>
        /// Advances the specified count.
        /// </summary>
        /// <param name="count">The count.</param>
        public void Advance(int count)
        {
            this.position += count;
        }

        /// <summary>
        /// Gets the memory.
        /// </summary>
        /// <param name="sizeHint">The size hint.</param>
        /// <returns>The requested memory.</returns>
        public Memory<TItem> GetMemory(int sizeHint)
        {
            this.StartIndex = this.position;
            this.EnsureCapacity(this.Position + sizeHint);
            return this.GetMemory(this.position, sizeHint);
        }

        /// <summary>
        /// Gets the span.
        /// </summary>
        /// <param name="sizeHint">The size hint.</param>
        /// <returns>The requested span.</returns>
        public Span<TItem> GetSpan(int sizeHint)
        {
            this.StartIndex = this.position;
            this.EnsureCapacity(this.Position + sizeHint);
            return this.GetSpan(this.position, sizeHint);
        }

        /// <summary>
        /// Gets the memory.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The length.</param>
        /// <returns>The memory.</returns>
        public Memory<TItem> GetMemory(int startIndex, int length)
        {
            VerifyIndex(startIndex);
            var requiredCapacity = startIndex + length;
            this.ProposeBounds(startIndex, requiredCapacity);
            this.InternalBuffer = this.PrivateEnsureCapacity(requiredCapacity);
            return this.InternalBuffer.AsMemory(startIndex, length);
        }

        /// <summary>
        /// Gets the span.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The length.</param>
        /// <returns>The span.</returns>
        public Span<TItem> GetSpan(int startIndex, int length)
        {
            VerifyIndex(startIndex);
            var requiredCapacity = startIndex + length;
            this.ProposeBounds(startIndex, requiredCapacity);
            this.InternalBuffer = this.PrivateEnsureCapacity(requiredCapacity);
            return this.InternalBuffer.AsSpan(startIndex, length);
        }

        /// <summary>
        /// Gets the memory.
        /// </summary>
        /// <returns>The memory.</returns>
        public Memory<TItem> GetMemory()
        {
            VerifyIndex(this.StartIndex);
            return this.InternalBuffer.AsMemory(this.StartIndex, this.Capacity);
        }

        /// <summary>
        /// Gets the span.
        /// </summary>
        /// <returns>The span.</returns>
        public Span<TItem> GetSpan()
        {
            VerifyIndex(this.StartIndex);
            return this.InternalBuffer.AsSpan(this.StartIndex, this.Capacity);
        }

        /// <summary>
        /// Gets the buffer as memory.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The length.</param>
        /// <returns>The memory.</returns>
        public ReadOnlyMemory<TItem> AsMemory(int startIndex, int length)
        {
            VerifyIndex(startIndex);
            this.InternalBuffer = this.PrivateEnsureCapacity(startIndex + length);
            return this.InternalBuffer.AsMemory(startIndex, length);
        }

        /// <summary>
        /// Gets the buffer as memory.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The length.</param>
        /// <returns>The memory.</returns>
        public ReadOnlySpan<TItem> AsSpan(int startIndex, int length)
        {
            VerifyIndex(startIndex);
            this.InternalBuffer = this.PrivateEnsureCapacity(startIndex + length);
            return this.InternalBuffer.AsSpan(startIndex, length);
        }

        /// <summary>
        /// Gets the buffer as memory.
        /// </summary>
        /// <returns>The memory.</returns>
        public ReadOnlyMemory<TItem> AsMemory()
        {
            return this.AsMemory(this.StartIndex, this.Length);
        }

        /// <summary>
        /// Gets the buffer as a span.
        /// </summary>
        /// <returns>The span.</returns>
        public ReadOnlySpan<TItem> AsSpan()
        {
            return this.AsSpan(this.StartIndex, this.Length);
        }

        /// <summary>
        /// Slices the specified start index.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The length.</param>
        /// <returns>A sliced buffer.</returns>
        public IBuffer<TItem> Slice(int startIndex, int length)
        {
            VerifyIndex(startIndex);
            var sliceInterval = Interval.FromMinAndLength(startIndex, length);
            var bufferInterval = Interval.FromMinAndLength(this.StartIndex, this.Length);
            this.EnsureCapacity(sliceInterval.Max);
            var sliceLength = sliceInterval.Overlaps(bufferInterval) ? Math.Max(0, Math.Min(bufferInterval.Max, sliceInterval.Max) - startIndex) : 0;
            this.ProposeBounds(startIndex, bufferInterval.Max);
            var bufferSlice = new BufferSlice<TItem>(this, startIndex, length, sliceLength);
            if (this.lastBufferSlice == null ||
                this.lastBufferSlice.StartIndex + this.lastBufferSlice.Capacity < bufferSlice.StartIndex + bufferSlice.Capacity)
            {
                this.lastBufferSlice = bufferSlice;
            }

            return bufferSlice;
        }

        /// <summary>
        /// Slices the specified relative to.
        /// </summary>
        /// <param name="relativeTo">The relative to.</param>
        /// <param name="capacityBefore">The capacity before.</param>
        /// <param name="capacityAfter">The capacity after.</param>
        /// <returns>A sliced buffer.</returns>
        public IBuffer<TItem> Slice(IBuffer<TItem> relativeTo, int capacityBefore, int capacityAfter)
        {
            this.VerifyRelativeTo(relativeTo);
            return this.Slice(relativeTo.StartIndex - capacityBefore, capacityBefore + relativeTo.Capacity + capacityAfter);
        }

        /// <summary>
        /// Slices the specified relative to.
        /// </summary>
        /// <param name="relativeTo">The relative to.</param>
        /// <param name="slice">The slice.</param>
        /// <param name="capacity">The capacity.</param>
        /// <param name="includeBuffer">if set to <c>true</c> [include buffer].</param>
        /// <returns>A sliced buffer.</returns>
        public IBuffer<TItem> Slice(IBuffer<TItem> relativeTo, Slice slice, int capacity, bool includeBuffer = false)
        {
            this.VerifyRelativeTo(relativeTo);
            return slice switch
            {
                Memory.Slice.Before => this.Slice(relativeTo.StartIndex - capacity, capacity + (includeBuffer ? relativeTo.Capacity : 0)),
                Memory.Slice.After => this.Slice(relativeTo.StartIndex + (includeBuffer ? 0 : relativeTo.Capacity), capacity),
                _ => throw new ArgumentOutOfRangeException(nameof(slice), slice, $"Invalid slice value {slice}"),
            };
        }

        /// <summary>
        /// Slices the buffer in the middle if unused and otherwise after the length.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        /// <param name="defaultSlice">The default slice.</param>
        /// <returns>
        /// A sliced buffer.
        /// </returns>
        public IBuffer<TItem> SliceDefault(int capacity, DefaultSlice defaultSlice = DefaultSlice.Append)
        {
            if (this.StartIndex == -1)
            {
                var minimumCapacity = Math.Max(this.Capacity, capacity);
                return this.Slice(BufferHelper.StartIndexInterval.Limit(Math.Abs(Math.Min((minimumCapacity / 2) - capacity, capacity * 2))), capacity);
            }

            switch (defaultSlice)
            {
                case DefaultSlice.Content:
                    return this.Slice(this.StartIndex, Math.Max(this.Length, capacity));
                case DefaultSlice.Append:
                    if (this.lastBufferSlice != null)
                    {
                        return this.Slice(this.lastBufferSlice, Memory.Slice.After, capacity);
                    }

                    return this.Slice(this.StartIndex + this.Length, capacity);
                default:
                    throw new ArgumentOutOfRangeException(nameof(defaultSlice), defaultSlice, $"Invalid DefaultSlice {defaultSlice} value.");
            }
        }

        /// <summary>
        /// Slices to the content (StartIndex - Length).
        /// </summary>
        /// <returns>A sliced buffer.</returns>
        public IBuffer<TItem> Slice()
        {
            return this.Slice(this.StartIndex, this.Length);
        }

        /// <summary>Converts to array.</summary>
        /// <returns>The new array.</returns>
        public TItem[] ToArray()
        {
            var newArray = new TItem[this.Length];
            Array.Copy(this.InternalBuffer, 0, newArray, 0, this.Length);
            return newArray;
        }

        /// <summary>
        /// Writes the item without capacity checking.
        /// </summary>
        /// <param name="item">The item.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void IBufferInternal<TItem>.WriteInternal(TItem item)
        {
            this.PrivateWrite(item);
        }

        internal int EnsureSliceCapacity(BufferSlice<TItem> bufferSlice, int requiredCapacity)
        {
            if (this.lastBufferSlice == bufferSlice)
            {
                this.EnsureCapacity(bufferSlice.StartIndex + requiredCapacity);
                return requiredCapacity;
            }

            throw new NotSupportedException($"Cannot grow buffer at: {bufferSlice.StartIndex}, capacity: {bufferSlice.Capacity} to the required capacity: {requiredCapacity}");
        }

        private static void VerifyIndex(int startIndex)
        {
            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, $"Start index: {startIndex} cannot be negative.");
            }
        }

        private void VerifyRelativeTo(IBuffer<TItem> relativeTo)
        {
            if (!MemoryMarshal.TryGetArray(relativeTo.AsMemory(), out var arraySegment) || arraySegment.Array != this.InternalBuffer)
            {
                throw new ArgumentException(
                    $"Cannot slice relative to a slice {relativeTo} that is not a part of this buffer.",
                    nameof(relativeTo));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int PrivateWrite(TItem item)
        {
            this.InternalBuffer![this.Position++] = item;
            return 1;
        }

        private void ProposeBounds(int startIndex, int endIndex)
        {
            this.StartIndex = startIndex;
            this.Length = endIndex - this.StartIndex;
        }

        private TItem[] PrivateEnsureAdditionalCapacity(int requiredAdditionalCapacity)
        {
            return this.PrivateEnsureCapacity(this.Length + requiredAdditionalCapacity);
        }

        private TItem[] PrivateEnsureCapacity(in int requiredCapacity)
        {
            if (this.InternalBuffer == null || this.InternalBuffer.Length < requiredCapacity)
            {
                var newMinimumCapacity = Math.Max(this.minimumCapacity, requiredCapacity);
                if (this.InternalBuffer != null)
                {
                    this.bufferReporter?.OnExpanding(this.InternalBuffer.Length, newMinimumCapacity);
                }

                return this.bufferResizer.Resize(this.InternalBuffer, newMinimumCapacity);
            }

            return this.InternalBuffer;
        }
    }
}