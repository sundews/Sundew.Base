// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyArray.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections.Internal;

using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Provides read only access to arrays.
/// </summary>
/// <typeparam name="TItem">The type of the item.</typeparam>
internal sealed class ReadOnlyArray<TItem> : IReadOnlyList<TItem>
{
    private readonly TItem[] array;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyArray{TItem}"/> class.
    /// </summary>
    /// <param name="array">The array.</param>
    public ReadOnlyArray(TItem[] array)
    {
        this.array = array;
    }

    /// <summary>
    /// Gets the count.
    /// </summary>
    /// <value>
    /// The count.
    /// </value>
    public int Count => this.array.Length;

    /// <summary>
    /// Gets the TItem with the specified error.
    /// </summary>
    /// <value>
    /// The TItem.
    /// </value>
    /// <param name="index">The index.</param>
    /// <returns>The item as the specified index.</returns>
    public TItem this[int index] => this.array[index];

    /// <summary>
    /// Copies to the items to the specified array.
    /// </summary>
    /// <param name="sourceIndex">Index of the source.</param>
    /// <param name="targetArray">The target array.</param>
    /// <param name="targetIndex">Index of the target.</param>
    /// <param name="count">The count.</param>
    public void CopyTo(int sourceIndex, TItem[] targetArray, int targetIndex, int count)
    {
        this.array.CopyTo(sourceIndex, targetArray, targetIndex, count);
    }

    /// <summary>
    /// Copies to the items to the specified array.
    /// </summary>
    /// <param name="sourceIndex">Index of the source.</param>
    /// <param name="targetSpan">The target span.</param>
    /// <param name="targetIndex">Index of the target.</param>
    /// <param name="count">The count.</param>
    public void CopyTo(int sourceIndex, Span<TItem> targetSpan, int targetIndex, int count)
    {
        this.array.AsSpan(sourceIndex, count).CopyTo(targetSpan.Slice(targetIndex));
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// An enumerator that can be used to iterate through the collection.
    /// </returns>
    public IEnumerator<TItem> GetEnumerator()
    {
        return ((IEnumerable<TItem>)this.array).GetEnumerator();
    }

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns>
    /// An enumerator that can be used to iterate through the collection.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}