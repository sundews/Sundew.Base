// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BufferSlice.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Memory.Internal;

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

internal sealed class BufferSlice<TItem> : IBufferInternal<TItem>
{
    private readonly Buffer<TItem> parentBuffer;
    private int position;
    private int length;

    public BufferSlice(Buffer<TItem> parentBuffer, int startIndex, int capacity, int length)
    {
        this.parentBuffer = parentBuffer;
        this.StartIndex = startIndex;
        this.Capacity = capacity;
        this.Length = length;
    }

    public int Length
    {
        get => this.length;
        private set => this.length = Math.Max(value, this.Length);
    }

    public int Capacity { get; private set; }

    public int Position
    {
        get => this.position;

        private set
        {
            this.position = value;
            this.Length = this.position;
        }
    }

    public int StartIndex { get; }

    public TItem this[int index]
    {
        get
        {
            this.EnsureCapacity(index);
            return this.parentBuffer[this.StartIndex + index];
        }

        set
        {
            this.EnsureCapacity(index + 1);
            this.parentBuffer[this.StartIndex + index] = value;
            this.Length = index;
        }
    }

    public void EnsureAdditionalCapacity(int requiredAdditionalCapacity)
    {
        this.EnsureCapacity(this.Position + requiredAdditionalCapacity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IBufferInternal<TItem>.WriteInternal(TItem item)
    {
        this.PrivateWrite(item);
    }

    public void EnsureCapacity(int requiredCapacity)
    {
        if (this.Capacity < requiredCapacity)
        {
            this.Capacity = this.parentBuffer.EnsureSliceCapacity(this, requiredCapacity);
        }
    }

    public int Write(TItem item)
    {
        this.EnsureAdditionalCapacity(1);
        return this.PrivateWrite(item);
    }

    public int Write(ICollection<TItem> items)
    {
        if (items.Count == 0)
        {
            return 0;
        }

        this.EnsureAdditionalCapacity(items.Count);
        items.CopyTo(this.parentBuffer.InternalBuffer, this.StartIndex + this.Position);
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

    public int Write(IReadOnlyList<TItem> items)
    {
        if (items.Count == 0)
        {
            return 0;
        }

        this.EnsureAdditionalCapacity(items.Count);
        var targetIndex = this.Position;
        var span = this.parentBuffer.GetSpan(this.StartIndex + this.Position, items.Count);
        for (int i = 0; i < items.Count; i++)
        {
            span[targetIndex++] = items[i];
        }

        this.Position += items.Count;
        return items.Count;
    }

    public int Write(ReadOnlySpan<TItem> items)
    {
        if (items.IsEmpty)
        {
            return 0;
        }

        this.EnsureAdditionalCapacity(items.Length);
        items.CopyTo(this.parentBuffer.GetSpan(this.StartIndex + this.Position, items.Length));
        this.Position += items.Length;
        return items.Length;
    }

    public int WriteRange(IEnumerable<TItem> items)
    {
        return BufferExtensions.Write(this, items);
    }

    public Memory<TItem> GetMemory(int startIndex, int length)
    {
        this.EnsureCapacity(startIndex + length);
        this.Length = length;
        return this.parentBuffer.GetMemory(this.StartIndex + startIndex, length);
    }

    public Span<TItem> GetSpan(int startIndex, int length)
    {
        this.EnsureCapacity(startIndex + length);
        this.Length = length;
        return this.parentBuffer.GetSpan(this.StartIndex + startIndex, length);
    }

    public Memory<TItem> GetMemory()
    {
        return this.GetMemory(0, this.Capacity);
    }

    public Span<TItem> GetSpan()
    {
        return this.GetSpan(0, this.Capacity);
    }

    public ReadOnlyMemory<TItem> AsMemory(int startIndex, int length)
    {
        this.EnsureCapacity(startIndex + length);
        this.Length = length;
        return this.parentBuffer.AsMemory(this.StartIndex + startIndex, length);
    }

    public ReadOnlySpan<TItem> AsSpan(int startIndex, int length)
    {
        this.EnsureCapacity(startIndex + length);
        return this.parentBuffer.AsSpan(this.StartIndex + startIndex, length);
    }

    public ReadOnlyMemory<TItem> AsMemory()
    {
        return this.AsMemory(0, this.Length);
    }

    public ReadOnlySpan<TItem> AsSpan()
    {
        return this.AsSpan(0, this.Length);
    }

    public IBuffer<TItem> Slice(int startIndex, int capacity)
    {
        return this.parentBuffer.Slice(this.StartIndex + startIndex, capacity);
    }

    public IBuffer<TItem> Slice(IBuffer<TItem> relativeTo, int capacityBefore, int capacityAfter)
    {
        return this.parentBuffer.Slice(relativeTo, capacityBefore, capacityAfter);
    }

    public IBuffer<TItem> Slice(IBuffer<TItem> relativeTo, Slice slice, int capacity, bool includeBuffer = false)
    {
        return this.parentBuffer.Slice(relativeTo, slice, capacity, includeBuffer);
    }

    public IBuffer<TItem> SliceDefault(int capacity, DefaultSlice defaultSlice = DefaultSlice.Append)
    {
        return defaultSlice switch
        {
            DefaultSlice.Content => this.parentBuffer.Slice(this, 0, capacity),
            DefaultSlice.Append => this.parentBuffer.Slice(this, Memory.Slice.After, capacity),
            _ => throw new ArgumentOutOfRangeException(nameof(defaultSlice), defaultSlice, null),
        };
    }

    public IBuffer<TItem> Slice()
    {
        return this.parentBuffer.Slice(this.StartIndex, this.Length);
    }

    /// <summary>Converts to array.</summary>
    /// <returns>The new array.</returns>
    public TItem[] ToArray()
    {
        var newArray = new TItem[this.Length];
        Array.Copy(this.parentBuffer.InternalBuffer, this.StartIndex, newArray, 0, this.Length);
        return newArray;
    }

    void IBufferWriter<TItem>.Advance(int count)
    {
        this.position += count;
        this.EnsureCapacity(this.Length);
    }

    Memory<TItem> IBufferWriter<TItem>.GetMemory(int sizeHint)
    {
        this.EnsureCapacity(this.Position + sizeHint);
        return this.GetMemory(this.Position, sizeHint);
    }

    Span<TItem> IBufferWriter<TItem>.GetSpan(int sizeHint)
    {
        this.EnsureCapacity(this.Position + sizeHint);
        return this.GetSpan(this.Position, sizeHint);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int PrivateWrite(TItem item)
    {
        this[this.Position++] = item;
        return 1;
    }
}