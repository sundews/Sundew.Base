﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="All{TResult}.All.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections.Linq;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Represents ensured items.
/// </summary>
/// <typeparam name="TResult">The result type.</typeparam>
public readonly record struct All<TResult> : IReadOnlyList<TResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="All{TResult}" /> struct.
    /// </summary>
    /// <param name="items">The items.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1642:Constructor summary documentation should begin with standard text", Justification = "Bug")]
    public All(TResult[] items)
    {
        this.Items = items;
    }

    /// <summary>
    /// Gets the items.
    /// </summary>
    public TResult[] Items { get; }

    /// <summary>
    /// Gets the count.
    /// </summary>
    public int Count => this.Items.Length;

    /// <summary>
    /// Gets the item at the specified index.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>The item.</returns>
    public TResult this[int index] => this.Items[index];

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns>The enumerator.</returns>
    public IEnumerator<TResult> GetEnumerator()
    {
        return this.Items.Cast<TResult>().GetEnumerator();
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