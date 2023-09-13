// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReferenceEqualityComparer.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Equality;

using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <summary>
/// Implementation of <see cref="IEqualityComparer{TObject}"/> that uses reference equality.
/// </summary>
/// <typeparam name="TObject">The type of the object.</typeparam>
public sealed class ReferenceEqualityComparer<TObject> : IEqualityComparer<TObject>
{
    private ReferenceEqualityComparer()
    {
    }

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static ReferenceEqualityComparer<TObject> Instance { get; } = new();

    /// <summary>
    /// Determines whether the specified objects are reference equal.
    /// </summary>
    /// <param name="x">The x object.</param>
    /// <param name="y">The y object.</param>
    /// <returns><c>true</c>, if the specified objects are reference equal.</returns>
    public bool Equals(TObject x, TObject y)
    {
        return ReferenceEquals(x, y);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    public int GetHashCode(TObject obj)
    {
        return RuntimeHelpers.GetHashCode(obj);
    }
}