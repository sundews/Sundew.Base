// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplitContext.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Memory.Internal;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents a context for splitting <see cref="ReadOnlyMemory{T}"/>.
/// </summary>
/// <typeparam name="TItem">The type of the item.</typeparam>
internal sealed class SplitContext<TItem> : ISplitContextInternal<TItem>
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
    /// Gets the input.
    /// </summary>
    /// <value>
    /// The input.
    /// </value>
    public ReadOnlyMemory<TItem> Input => this.input;

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

    /// <summary>
    /// Gets or sets a value indicating whether this instance is ignoring.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is ignoring; otherwise, <c>false</c>.
    /// </value>
    public bool IsIgnoring { get; set; }

    /// <summary>
    /// Gets the next item or default.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>The next item or default.</returns>
    public TItem? GetNextOrDefault(int index)
    {
        var nextIndex = index + 1;
        return nextIndex > -1 && nextIndex < this.input.Length ? this.input.Span[nextIndex] : default;
    }

    /// <summary>
    /// Gets the previous item or default.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>The previous item or default.</returns>
    public TItem? GetPreviousOrDefault(int index)
    {
        var previousIndex = index - 1;
        return previousIndex > -1 && previousIndex < this.input.Length ? this.input.Span[previousIndex] : default;
    }

    /// <summary>
    /// Tries to get the next item or default.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <param name="item">The item.</param>
    /// <returns>
    ///   <c>true</c> if next is a valid item.
    /// </returns>
    public bool TryGetNext(int index, out TItem? item)
    {
        var nextIndex = index + 1;
        if (nextIndex > -1 && nextIndex < this.input.Length)
        {
            item = this.input.Span[nextIndex];
            return true;
        }

        item = default;
        return false;
    }

    /// <summary>
    /// Tries to get the previous item or default.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <param name="item">The item.</param>
    /// <returns>
    ///   <c>true</c> if previous is a valid item.
    /// </returns>
    public bool TryGetPrevious(int index, out TItem? item)
    {
        var previousIndex = index - 1;
        if (previousIndex > -1 && previousIndex < this.input.Length)
        {
            item = this.input.Span[previousIndex];
            return true;
        }

        item = default;
        return false;
    }

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

    /// <summary>
    /// Gets the section and reset.
    /// </summary>
    /// <returns>The readonly memory.</returns>
    public ReadOnlyMemory<TItem> GetSectionAndReset()
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

    /// <summary>
    /// Starts the including.
    /// </summary>
    /// <param name="startIndex">The start index.</param>
    public void StartIncluding(int startIndex)
    {
        this.StartIndex = startIndex;
        this.IsIgnoring = false;
    }

    /// <summary>
    /// Includes the specified item.
    /// </summary>
    /// <param name="item">The item.</param>
    public void Include(TItem item)
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