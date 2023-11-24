// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Item{TResult}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents the result of selecting an ensured item.
/// </summary>
/// <typeparam name="TResult">The item type.</typeparam>
public readonly record struct Item<TResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Item{TResult}" /> class.
    /// Use <see cref="Item"/> class to create <see cref="Item{TResult}"/>.
    /// </summary>
    /// <param name="selectedItem">The success item.</param>
    /// <param name="isValid">The value indicating whether the result is valid.</param>
    internal Item(TResult? selectedItem, bool isValid)
    {
        this.SelectedItem = selectedItem;
        this.IsValid = isValid;
    }

    /// <summary>
    /// Gets the success item.
    /// </summary>
    public TResult? SelectedItem { get; }

    /// <summary>
    /// Gets a value indicating whether the result is valid.
    /// </summary>
    [MemberNotNullWhen(true, nameof(SelectedItem))]
    public bool IsValid { get; }
    /*
    /// <summary>
    /// Converts a failed item to a failed select item.
    /// </summary>
    /// <param name="option">The option.</param>
    public static implicit operator Item<TResult>(TResult option)
    {
        return Item.From(!Equals(option, default), option);
    }*/

    /// <summary>
    /// Converts a failed item to a failed select item.
    /// </summary>
    /// <param name="failedItem">The failed item.</param>
    public static implicit operator Item<TResult>(Item.FailedItem failedItem)
    {
        return new Item<TResult>(default, false);
    }
}