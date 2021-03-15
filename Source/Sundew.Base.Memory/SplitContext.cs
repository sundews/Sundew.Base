// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplitContext.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Memory
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a context for splitting <see cref="ReadOnlyMemory{T}"/>.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public sealed class SplitContext<TItem>
    {
        /// <summary>
        /// The section not started index.
        /// </summary>
        public const int SectionNotStartedIndex = -1;

        private readonly ReadOnlyMemory<TItem> input;

        private int length;

        private Buffer<TItem>? buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SplitContext{TItem}" /> class.
        /// </summary>
        /// <param name="input">The input.</param>
        internal SplitContext(ReadOnlyMemory<TItem> input)
        {
            this.input = input;
            this.StartIndex = SectionNotStartedIndex;
            this.length = 1;
            this.IsIgnoring = false;
            this.buffer = null;
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public int Length => (this.StartIndex > SectionNotStartedIndex ? this.length : 0) + (this.buffer?.Length ?? 0);

        /// <summary>
        /// Gets the start index.
        /// </summary>
        /// <value>
        /// The start index.
        /// </value>
        public int StartIndex { get; private set; }

        internal bool IsIgnoring { get; set; }

        /// <summary>
        /// Appends the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Append(TItem item)
        {
            this.EnsureBuffer(1);
            this.buffer!.Write(item);
        }

        /// <summary>
        /// Appends the specified span.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        public void Append(IEnumerable<TItem> enumerable)
        {
            this.EnsureBuffer(4);
            this.buffer!.WriteRange(enumerable);
        }

        /// <summary>
        /// Appends the specified span.
        /// </summary>
        /// <param name="readOnlyItems">The read only items.</param>
        public void Append(IReadOnlyList<TItem> readOnlyItems)
        {
            this.EnsureBuffer(readOnlyItems.Count);
            this.buffer!.Write(readOnlyItems);
        }

        /// <summary>
        /// Appends the specified span.
        /// </summary>
        /// <param name="span">The span.</param>
        public void Append(ReadOnlySpan<TItem> span)
        {
            this.EnsureBuffer(span.Length);
            this.buffer!.Write(span);
        }

        internal ReadOnlyMemory<TItem> GetSectionAndReset()
        {
            var memory = ReadOnlyMemory<TItem>.Empty;
            if (this.StartIndex > SectionNotStartedIndex)
            {
                memory = this.input.Slice(this.StartIndex, this.length);
                this.StartIndex = SectionNotStartedIndex;
                this.length = 1;
                this.IsIgnoring = false;
            }

            if (this.buffer != null)
            {
                this.buffer.Write(memory);
                memory = this.buffer.AsMemory();
                this.buffer = null;
                return memory;
            }

            return memory;
        }

        internal void StartIncluding(int startIndex)
        {
            this.StartIndex = startIndex;
            this.IsIgnoring = false;
        }

        internal void Include(TItem item)
        {
            if (this.IsIgnoring)
            {
                this.Append(item);
            }
            else
            {
                this.length++;
            }
        }

        private void EnsureBuffer(int additionalLength)
        {
            if (this.buffer == null)
            {
                this.buffer = new Buffer<TItem>(this.length + additionalLength);
            }

            if (this.StartIndex > SectionNotStartedIndex)
            {
                this.buffer.Write(this.input.Slice(this.StartIndex, this.length));
                this.StartIndex = SectionNotStartedIndex;
                this.length = 1;
            }
        }
    }
}