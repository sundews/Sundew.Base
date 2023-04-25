// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueList.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections.Immutable;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

/// <summary>
/// Represents in immutable list that implements value semantics.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public readonly struct ValueList<TItem> : IReadOnlyList<TItem>, IEquatable<ValueList<TItem>>
{
    private readonly System.Collections.Immutable.IImmutableList<TItem> inner;

    internal ValueList(System.Collections.Immutable.IImmutableList<TItem> inner)
    {
        this.inner = inner;
    }

    /// <summary>
    /// Gets the count.
    /// </summary>
    public int Count => this.inner.Count;

    /// <summary>
    /// Gets the index at the specified index.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>The item.</returns>
    public TItem this[int index] => this.inner[index];

    /// <summary>
    /// Converts from a  <see cref="ImmutableArray{T}"/> to an <see cref="ValueList{T}"/>.
    /// </summary>
    /// <param name="immutableArray">The immutable array.</param>
    public static implicit operator ValueList<TItem>(System.Collections.Immutable.ImmutableArray<TItem> immutableArray)
    {
        return new ValueList<TItem>(immutableArray);
    }

    /// <summary>
    /// Converts from a  <see cref="ImmutableList{T}"/> to an <see cref="ValueList{T}"/>.
    /// </summary>
    /// <param name="immutableList">The immutable list.</param>
    public static implicit operator ValueList<TItem>(System.Collections.Immutable.ImmutableList<TItem> immutableList)
    {
        return new ValueList<TItem>(immutableList);
    }

    /// <summary>
    /// Converts a <see cref="ValueList{TItem}"/> to an <see cref="ImmutableList{TItem}"/>.
    /// </summary>
    /// <param name="valueList">The value dictionary.</param>
    public static implicit operator ImmutableList<TItem>(ValueList<TItem> valueList)
    {
        if (valueList.inner is ImmutableList<TItem> immutableList)
        {
            return immutableList;
        }

        return valueList.inner.ToImmutableList();
    }

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns>The enumerator.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.inner.GetEnumerator();
    }

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns>The enumerator.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator<TItem> IEnumerable<TItem>.GetEnumerator()
    {
        return this.inner.GetEnumerator();
    }

    /// <summary>
    /// Gets the hashcode.
    /// </summary>
    /// <returns>The hashcode.</returns>
    public override int GetHashCode()
    {
#if NETSTANDARD1_3_OR_GREATER
        var hashCode = default(HashCode);
        foreach (var item in this.inner)
        {
            hashCode.Add(item?.GetHashCode() ?? 0);
        }

        return hashCode.ToHashCode();
#else
        return Equality.Equality.GetItemsHashCode(this.inner.Select(x => x?.GetHashCode() ?? 0));
#endif
    }

    /// <summary>
    /// Gets a value indicating whether this instance is equal to other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c>, if the values are equal otherwise false.</returns>
    public bool Equals(ValueList<TItem> other)
    {
        if (this.inner == null && other.inner == null)
        {
            return true;
        }

        return this.inner?.SequenceEqual(other.inner) ?? false;
    }

    /// <summary>
    /// Gets a value indicating whether this instance is equal to other.
    /// </summary>
    /// <param name="obj">The obj.</param>
    /// <returns><c>true</c>, if the values are equal otherwise false.</returns>
    public override bool Equals(object? obj)
    {
        return obj is ValueList<TItem> other && this.Equals(other);
    }
}