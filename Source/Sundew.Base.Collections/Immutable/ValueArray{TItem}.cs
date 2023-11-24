// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueArray{TItem}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections.Immutable;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

/// <summary>
/// Represents in immutable array that implements value semantics.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public readonly struct ValueArray<TItem> : IReadOnlyList<TItem>, IEquatable<ValueArray<TItem>>
{
    private readonly ImmutableArray<TItem> inner;

    internal ValueArray(ImmutableArray<TItem> inner)
    {
        this.inner = inner;
    }

    /// <summary>
    /// Gets an empty list.
    /// </summary>
    /// <returns>An empty list.</returns>
    public static ValueArray<TItem> Empty { get; } = ImmutableArray<TItem>.Empty;

    /// <summary>
    /// Gets the count.
    /// </summary>
    public int Count => this.inner.Length;

    /// <summary>
    /// Gets a value indicating whether this array is default.
    /// </summary>
    public bool IsDefault => this.inner.IsDefault;

    /// <summary>
    /// Gets the index at the specified index.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>The item.</returns>
    public TItem this[int index] => this.inner[index];

    /// <summary>
    /// Converts from an array to a <see cref="ValueArray{TItem}"/>.
    /// </summary>
    /// <param name="array">An array.</param>
    public static implicit operator ValueArray<TItem>(TItem[] array)
    {
        return new ValueArray<TItem>(array.ToImmutableArray());
    }

    /// <summary>
    /// Converts from an <see cref="ImmutableArray{T}"/> to a <see cref="ValueArray{TItem}"/>.
    /// </summary>
    /// <param name="immutableArray">The immutable array.</param>
    public static implicit operator ValueArray<TItem>(ImmutableArray<TItem> immutableArray)
    {
        return new ValueArray<TItem>(immutableArray);
    }

    /// <summary>
    /// Converts from a  <see cref="ValueArray{TItem}"/> to an <see cref="ImmutableArray{T}"/>.
    /// </summary>
    /// <param name="valueArray">The value array.</param>
    public static implicit operator ImmutableArray<TItem>(ValueArray<TItem> valueArray)
    {
        return valueArray.inner;
    }

    /// <summary>
    /// Checks if the two sides are equal.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns><c>true</c>, if the sides are equal otherwise <c>false</c>.</returns>
    public static bool operator ==(ValueArray<TItem> left, ValueArray<TItem> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Checks if the two sides differ.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns><c>false</c>, if the sides are equal otherwise <c>true</c>.</returns>
    public static bool operator !=(ValueArray<TItem> left, ValueArray<TItem> right)
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
        return ((IEnumerable<TItem>)this.inner).GetEnumerator();
    }

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns>The enumerator.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public ImmutableArray<TItem>.Enumerator GetEnumerator()
    {
        return this.inner.GetEnumerator();
    }

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns>The enumerator.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    IEnumerator<TItem> IEnumerable<TItem>.GetEnumerator()
    {
        return ((IEnumerable<TItem>)this.inner).GetEnumerator();
    }

    /// <summary>
    /// Gets the hashcode.
    /// </summary>
    /// <returns>The hashcode.</returns>
    public override int GetHashCode()
    {
        return StructuralComparisons.StructuralEqualityComparer.GetHashCode(this.inner);
    }

    /// <summary>
    /// Gets a value indicating whether this instance is equal to other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c>, if the values are equal otherwise false.</returns>
    public bool Equals(ValueArray<TItem> other)
    {
        return StructuralComparisons.StructuralEqualityComparer.Equals(this.inner, other.inner);
    }

    /// <summary>
    /// Gets a value indicating whether this instance is equal to other.
    /// </summary>
    /// <param name="obj">The obj.</param>
    /// <returns><c>true</c>, if the values are equal otherwise false.</returns>
    public override bool Equals(object? obj)
    {
        return obj is ValueArray<TItem> other && this.Equals(other);
    }
}