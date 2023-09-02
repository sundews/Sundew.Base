// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryEqualityComparer.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Memory;

using System;
using System.Collections.Generic;
using Sundew.Base.Equality;

/// <summary>
/// Equality comparer for <see cref="Memory{T}"/>.
/// </summary>
/// <typeparam name="TItem">The type of the item.</typeparam>
/// <seealso cref="Memory{T}" />
public class MemoryEqualityComparer<TItem> : IEqualityComparer<Memory<TItem>>
    where TItem : struct, IEquatable<TItem>
{
    /// <summary>
    /// Determines whether the specified objects are equal.
    /// </summary>
    /// <param name="x">The first object of type <paramref name="x" /> to compare.</param>
    /// <param name="y">The second object of type <paramref name="y" /> to compare.</param>
    /// <returns>
    /// true if the specified objects are equal; otherwise, false.
    /// </returns>
    public bool Equals(Memory<TItem> x, Memory<TItem> y)
    {
        return x.Span.SequenceEqual(y.Span);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    public int GetHashCode(Memory<TItem> obj)
    {
        return obj.Span.GetItemsHashCode();
    }
}