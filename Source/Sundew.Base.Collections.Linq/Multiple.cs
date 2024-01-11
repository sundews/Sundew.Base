// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Multiple.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections.Linq;

using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents an <see cref="IEnumerable{T}"/> with multiple items.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
public sealed record Multiple<TItem> : ListCardinality<TItem>, IEnumerable<TItem>
#else
public sealed class Multiple<TItem> : ListCardinality<TItem>, IEnumerable<TItem>
#endif
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Multiple{TItem}"/> class.
    /// </summary>
    /// <param name="items">The items.</param>
    public Multiple(IEnumerable<TItem> items)
    {
        this.Items = items;
    }

    /// <summary>
    /// Gets the items.
    /// </summary>
    public IEnumerable<TItem> Items { get; }

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns>The enumerator.</returns>
    public IEnumerator<TItem> GetEnumerator()
    {
        return this.Items.GetEnumerator();
    }

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns>The enumerator.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}