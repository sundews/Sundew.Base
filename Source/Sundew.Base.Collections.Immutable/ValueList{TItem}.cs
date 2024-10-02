// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueList{TItem}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
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
    /// Gets an empty list.
    /// </summary>
    /// <returns>An empty list.</returns>
    public static ValueList<TItem> Empty => ImmutableList<TItem>.Empty;

    /// <summary>
    /// Gets a value indicating whether this array is empty.
    /// </summary>
    public bool IsEmpty => this.inner.IsEmpty();

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
    /// Checks whether the two items are equal.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns><c>true</c> if equal otherwise <c>false</c>.</returns>
    public static bool operator ==(ValueList<TItem> left, ValueList<TItem> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Checks whether the two items are inequal.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns><c>true</c> if inequal otherwise <c>false</c>.</returns>
    public static bool operator !=(ValueList<TItem> left, ValueList<TItem> right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns>The enumerator.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.inner.GetEnumerator();
    }

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns>The enumerator.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public IEnumerator<TItem> GetEnumerator()
    {
        return this.inner.GetEnumerator();
    }

    /// <summary>
    /// Gets the hashcode.
    /// </summary>
    /// <returns>The hashcode.</returns>
    public override int GetHashCode()
    {
#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
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

    /// <summary>
    /// Adds the item and returns a newly created list with containing the item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The newly created array.</returns>
    public ValueList<TItem> Add(TItem item)
    {
        return this.inner.Add(item).ToValueList();
    }

    /// <summary>
    /// Adds the items and returns a newly created array with containing the item.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <returns>The newly created array.</returns>
    public ValueList<TItem> AddRange(IEnumerable<TItem> items)
    {
        return this.inner.AddRange(items).ToValueList();
    }

    /// <summary>
    /// Removes the item and returns a newly created list.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The newly created array.</returns>
    public ValueList<TItem> Remove(TItem item)
    {
        return this.inner.Remove(item).ToValueList();
    }

    /// <summary>
    /// Removes the items and returns a newly created list.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <returns>The newly created array.</returns>
    public ValueList<TItem> RemoveRange(IEnumerable<TItem> items)
    {
        return this.inner.RemoveRange(items).ToValueList();
    }

    /// <summary>
    /// A value that indicates whether the item exists in the list.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns><c>true</c> if the item exists, otherwise <c>false</c>.</returns>
    public bool Contains(TItem item)
    {
        return this.inner.Contains(item);
    }
}