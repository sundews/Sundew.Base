// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrayPoolBufferResizer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Memory;

using System;
using System.Buffers;

/// <summary>
/// Implementation of <see cref="IBufferResizer{TItem}"/> that will use <see cref="ArrayPool{T}"/> for arrays larger than 10000 bytes.
/// </summary>
/// <typeparam name="TItem">The type of the item.</typeparam>
/// <seealso cref="Sundew.Base.Memory.IBufferResizer{TItem}" />
public sealed class ArrayPoolBufferResizer<TItem> : IBufferResizer<TItem>, IDisposable
{
    private readonly ArrayPool<TItem> arrayPool;
    private readonly AllocatingBufferResizer<TItem> allocatingBufferResizer = new();
    private TItem[]? currentArray;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArrayPoolBufferResizer{TItem}"/> class.
    /// </summary>
    /// <param name="arrayPool">The array pool.</param>
    public ArrayPoolBufferResizer(ArrayPool<TItem> arrayPool)
    {
        this.arrayPool = arrayPool;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArrayPoolBufferResizer{TItem}"/> class.
    /// </summary>
    public ArrayPoolBufferResizer()
        : this(ArrayPool<TItem>.Shared)
    {
    }

    /// <summary>
    /// Resizes the specified source.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="minimumCapacity">The new minimum capacity.</param>
    /// <returns>A new resized array.</returns>
    public TItem[] Resize(TItem[]? source, int minimumCapacity)
    {
        if (this.currentArray != null)
        {
            this.arrayPool.Return(this.currentArray);
        }

        if (minimumCapacity <= AllocatingBufferResizer<TItem>.MaxDoublingLimit)
        {
            this.currentArray = null;
            return this.allocatingBufferResizer.Resize(source, minimumCapacity);
        }

        return this.currentArray = this.arrayPool.Rent(minimumCapacity);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        if (this.currentArray != null)
        {
            this.arrayPool.Return(this.currentArray);
            this.currentArray = null;
        }
    }
}