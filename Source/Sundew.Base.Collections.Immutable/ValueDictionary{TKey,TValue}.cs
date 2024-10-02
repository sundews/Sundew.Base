// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueDictionary{TKey,TValue}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections.Immutable;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

/// <summary>
/// Represents in immutable array that implements value semantics.
/// </summary>
/// <typeparam name="TKey">The key type.</typeparam>
/// <typeparam name="TValue">The value type.</typeparam>
public readonly struct ValueDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>, IEquatable<ValueDictionary<TKey, TValue>>
    where TKey : IEquatable<TKey>
{
    private readonly IImmutableDictionary<TKey, TValue> inner;

    internal ValueDictionary(IImmutableDictionary<TKey, TValue> inner)
    {
        this.inner = inner;
    }

    /// <summary>
    /// Gets an empty dictionary.
    /// </summary>
    /// <returns>An empty dictionary.</returns>
    public static ValueDictionary<TKey, TValue> Empty { get; } = ImmutableDictionary<TKey, TValue>.Empty;

    /// <summary>
    /// Gets the count.
    /// </summary>
    public int Count => this.inner.Count;

    /// <summary>
    /// Gets a value indicating whether this array is empty.
    /// </summary>
    public bool IsEmpty => this.inner.IsEmpty();

    /// <summary>
    /// Gets the keys.
    /// </summary>
    /// <returns>The keys.</returns>
    public IEnumerable<TKey> Keys => this.inner.Keys;

    /// <summary>
    /// Gets the values.
    /// </summary>
    /// <returns>The values.</returns>
    public IEnumerable<TValue> Values => this.inner.Values;

    /// <summary>
    /// The value corresponding to the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The value.</returns>
    public TValue this[TKey key] => this.inner[key];

    /// <summary>
    /// Converts an <see cref="ImmutableDictionary{TKey,TValue}"/> to a <see cref="ValueDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="immutableDictionary">The immutable dictionary.</param>
    public static implicit operator ValueDictionary<TKey, TValue>(System.Collections.Immutable.ImmutableDictionary<TKey, TValue> immutableDictionary)
    {
        return new ValueDictionary<TKey, TValue>(immutableDictionary);
    }

    /// <summary>
    /// Converts a <see cref="ValueDictionary{TKey,TValue}"/> to an <see cref="ImmutableDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="valueDictionary">The value dictionary.</param>
    public static implicit operator ImmutableDictionary<TKey, TValue>(ValueDictionary<TKey, TValue> valueDictionary)
    {
        if (valueDictionary.inner is ImmutableDictionary<TKey, TValue> dictionary)
        {
            return dictionary;
        }

        return valueDictionary.inner.ToImmutableDictionary();
    }

    /// <summary>
    /// Checks whether the left and right hand side are equal.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns><c>true</c>, if the values are equal, otherwise <c>false</c>.</returns>
    public static bool operator ==(ValueDictionary<TKey, TValue> left, ValueDictionary<TKey, TValue> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Checks whether the left and right hand side differ.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns><c>true</c>, if the values differ, otherwise <c>false</c>.</returns>
    public static bool operator !=(ValueDictionary<TKey, TValue> left, ValueDictionary<TKey, TValue> right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Checks whether the specified key is present.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns><c>true</c>, if the key is present, otherwise <c>false</c>.</returns>
    public bool ContainsKey(TKey key)
    {
        return this.inner.ContainsKey(key);
    }

    /// <summary>
    /// Tries to get the value..
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns>A value indicating whether the value was found.</returns>
#pragma warning disable CS8767
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
#pragma warning restore CS8767
    {
        return this.inner.TryGetValue(key, out value);
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
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
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
        foreach (var pair in this.inner)
        {
            hashCode.Add(pair.Key);
            hashCode.Add(pair.Value);
        }

        return hashCode.ToHashCode();
#else

        static int CombineHashCode(int hashCode1, int hashcode2)
        {
            const int orderingHashPrime = 397;
            unchecked
            {
                return (hashCode1 * orderingHashPrime) ^ hashcode2;
            }
        }

        return Equality.Equality.GetItemsHashCode(this.inner.Select(x => CombineHashCode(x.Key?.GetHashCode() ?? 0, x.Value?.GetHashCode() ?? 0)));
#endif
    }

    /// <summary>
    /// Gets a value indicating whether this instance is equal to other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c>, if the values are equal otherwise false.</returns>
    public bool Equals(ValueDictionary<TKey, TValue> other)
    {
        if (this.inner.Count != other.Count)
        {
            return false;
        }

        var isEqual = true;
        foreach (var pair in this.inner)
        {
            if (other.TryGetValue(pair.Key, out var value))
            {
                if (!Equals(pair.Value, value))
                {
                    isEqual = false;
                    break;
                }
            }
            else
            {
                isEqual = false;
                break;
            }
        }

        return isEqual;
    }

    /// <summary>
    /// Gets a value indicating whether this instance is equal to other.
    /// </summary>
    /// <param name="obj">The obj.</param>
    /// <returns><c>true</c>, if the values are equal otherwise false.</returns>
    public override bool Equals(object? obj)
    {
        return obj is ValueDictionary<TKey, TValue> other && this.Equals(other);
    }

    /// <summary>
    /// Adds the value for the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns>The new value dictionary.</returns>
    public ValueDictionary<TKey, TValue> Add(TKey key, TValue value)
    {
        return new ValueDictionary<TKey, TValue>(this.inner.Add(key, value));
    }

    /// <summary>
    /// Adds the pairs.
    /// </summary>
    /// <param name="pairs">The pairs.</param>
    /// <returns>The new value dictionary.</returns>
    public ValueDictionary<TKey, TValue> AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
    {
        return new ValueDictionary<TKey, TValue>(this.inner.AddRange(pairs));
    }

    /// <summary>
    /// Removes the key and returns a newly created array.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The newly created array.</returns>
    public ValueDictionary<TKey, TValue> Remove(TKey key)
    {
        return this.inner.Remove(key).ToValueDictionary();
    }

    /// <summary>
    /// Removes the keys and returns a newly created array.
    /// </summary>
    /// <param name="keys">The keys.</param>
    /// <returns>The newly created array.</returns>
    public ValueDictionary<TKey, TValue> RemoveRange(IEnumerable<TKey> keys)
    {
        return this.inner.RemoveRange(keys).ToValueDictionary();
    }
}