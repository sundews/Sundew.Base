// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NonDeferredEnumerable.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections.Internal;

using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Provides read only access to <see cref="List{TItem}"/>.
/// </summary>
/// <typeparam name="TItem">The type of the item.</typeparam>
internal sealed class NonDeferredEnumerable<TItem> : IReadOnlyCollection<TItem>
{
    private readonly IEnumerable<TItem> source;

    /// <summary>
    /// Initializes a new instance of the <see cref="NonDeferredEnumerable{TItem}" /> class.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="count">The count.</param>
    public NonDeferredEnumerable(IEnumerable<TItem> enumerable, int count)
    {
        this.Count = count;
        this.source = enumerable;
    }

    /// <summary>
    /// Gets the count.
    /// </summary>
    /// <value>
    /// The count.
    /// </value>
    public int Count { get; }

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns>
    /// An enumerator that can be used to iterate through the collection.
    /// </returns>
    public IEnumerator<TItem> GetEnumerator()
    {
        return this.source.GetEnumerator();
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