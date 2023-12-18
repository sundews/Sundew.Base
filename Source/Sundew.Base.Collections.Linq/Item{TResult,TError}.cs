// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Item{TResult,TError}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections.Linq;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents the result of selecting an ensured item.
/// </summary>
/// <typeparam name="TResult">The item type.</typeparam>
/// <typeparam name="TError">The error type.</typeparam>
public readonly record struct Item<TResult, TError>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Item{TResult, TError}" /> class.
    /// Use <see cref="Item"/> class to create <see cref="Item{TResult, TError}"/>.
    /// </summary>
    /// <param name="selectedItem">The success item.</param>
    /// <param name="errorItem">The error item.</param>
    /// <param name="isValid">The value indicating whether the result is valid.</param>
    internal Item(TResult? selectedItem, TError? errorItem, bool isValid)
    {
        this.SelectedItem = selectedItem;
        this.ErrorItem = errorItem;
        this.IsValid = isValid;
    }

    /// <summary>
    /// Gets the success item.
    /// </summary>
    public TResult? SelectedItem { get; }

    /// <summary>
    /// Gets the error item.
    /// </summary>
    public TError? ErrorItem { get; }

    /// <summary>
    /// Gets a value indicating whether the result is valid.
    /// </summary>
    [MemberNotNullWhen(true, nameof(SelectedItem))]
    [MemberNotNullWhen(false, nameof(ErrorItem))]
    public bool IsValid { get; }

    /// <summary>
    /// Gets a value indicating whether this <see cref="R"/> has an error.
    /// </summary>
    /// <value>
    ///   <c>true</c> if error is non default; otherwise, <c>false</c>.
    /// </value>
    [MemberNotNullWhen(true, nameof(ErrorItem))]
    public bool HasError => !Equals(this.ErrorItem, default(TError)) || !this.IsValid;

    /// <summary>
    /// Converts a result to an item.
    /// </summary>
    /// <param name="result">The result.</param>
    public static implicit operator Item<TResult, TError>(R<TResult, TError> result)
    {
        return new Item<TResult, TError>(result.Value, result.Error, result.IsSuccess);
    }

    /// <summary>
    /// Converts a failed item to a failed select item.
    /// </summary>
    /// <param name="failedItem">The failed item.</param>
    public static implicit operator Item<TResult, TError>(Item.FailedItem<TError> failedItem)
    {
        return new Item<TResult, TError>(default, failedItem.Error, false);
    }

    /// <summary>
    /// Converts a failed item to a failed select item.
    /// </summary>
    /// <param name="item">The item.</param>
    public static implicit operator Item<TResult, TError>(Item<TResult> item)
    {
        return new Item<TResult, TError>(item.SelectedItem, default, item.IsValid);
    }
}