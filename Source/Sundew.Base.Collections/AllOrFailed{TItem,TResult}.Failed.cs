// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllOrFailed{TItem,TResult}.Failed.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections;

using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents the error causing items not to become ensured.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
/// <typeparam name="TResult">The result type.</typeparam>
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Discriminated union")]
public sealed class Failed<TItem, TResult> : AllOrFailed<TItem, TResult>, IReadOnlyList<FailedItem<TItem>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Failed{TItem, TResult}" /> class.
    /// </summary>
    /// <param name="items">The failed items.</param>
    public Failed(IReadOnlyList<FailedItem<TItem>> items)
    {
        this.Items = items;
    }

    /// <summary>
    /// Gets the failed items.
    /// </summary>
    public IReadOnlyList<FailedItem<TItem>> Items { get; }

    /// <summary>
    /// Gets the count.
    /// </summary>
    public int Count => this.Items.Count;

    /// <summary>
    /// Gets the value at the specified index.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>The value.</returns>
    public FailedItem<TItem> this[int index] => this.Items[index];

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <returns>The items.</returns>
    public IReadOnlyList<TItem?> GetItems() => this.Items.ToArray(x => x.Item);

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns>The enumerator.</returns>
    public IEnumerator<FailedItem<TItem>> GetEnumerator()
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