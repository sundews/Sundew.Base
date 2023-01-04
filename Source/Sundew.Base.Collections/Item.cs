// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Item.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections;

/// <summary>
/// Represents the result of selecting an ensured item.
/// </summary>
public static class Item
{
    /// <summary>
    /// Converts the item into an item result.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="item">The item.</param>
    /// <returns>An Item result.</returns>
    public static Item<TItem> PassIfNotNull<TItem>(TItem? item)
    {
        return new Item<TItem>(item, !Equals(item, default));
    }

    /// <summary>
    /// Converts the item into an item result.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="item">The item.</param>
    /// <returns>An Item result.</returns>
    public static Item<TItem> PassIfNotNull<TItem>(TItem? item)
        where TItem : struct
    {
        var hasValue = item.HasValue;
        return new Item<TItem>(item.GetValueOrDefault(default), hasValue);
    }

    /// <summary>
    /// Creates a valid result with the passed item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <returns>The result.</returns>
    public static Item<TItem> Pass<TItem>(TItem item)
    {
        return new Item<TItem>(item, true);
    }

    /// <summary>
    /// Creates a valid result with the passed item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <returns>The result.</returns>
    public static Item<TItem, TError> Pass<TItem, TError>(TItem item)
    {
        return new Item<TItem, TError>(item, default, true);
    }

    /// <summary>
    /// Create an error result.
    /// </summary>
    /// <returns>The result.</returns>
    public static FailedItem Fail()
    {
        return default;
    }

    /// <summary>
    /// Create an error result.
    /// </summary>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="error">The error.</param>
    /// <returns>The result.</returns>
    public static FailedItem<TError> Fail<TError>(TError error)
    {
        return new FailedItem<TError>(error);
    }

    /// <summary>
    /// Create an error result.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="error">The error.</param>
    /// <returns>The result.</returns>
    public static Item<TResult, TError> Fail<TResult, TError>(TError error)
    {
        return new Item<TResult, TError>(default, error, false);
    }

    /// <summary>
    /// Represents a failed item.
    /// </summary>
    public readonly struct FailedItem
    {
    }

    /// <summary>
    /// Represents a failed item.
    /// </summary>
    /// <typeparam name="TError">The error type.</typeparam>
    public readonly struct FailedItem<TError>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedItem{TError}"/> struct.
        /// </summary>
        /// <param name="error">The error.</param>
        public FailedItem(TError error)
        {
            this.Error = error;
        }

        /// <summary>
        /// Gets the error.
        /// </summary>
        public TError Error { get; }
    }
}