// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Item.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections.Linq;

using System.Runtime.CompilerServices;

/// <summary>
/// Represents the result of selecting an ensured item.
/// </summary>
public static class Item
{
    /// <summary>
    /// Converts the item into an item result.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">The result.</param>
    /// <returns>An Item result.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static Item<TSuccess, TError> PassIfSuccess<TSuccess, TError>(R<TSuccess, TError> result)
    {
        return result.ToItem();
    }

    /// <summary>
    /// Converts the item into an item result.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="item">The item.</param>
    /// <returns>An Item result.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static Item<TItem> PassIfHasValue<TItem>(TItem? item)
    {
        return new Item<TItem>(item, !Equals(item, default));
    }

    /// <summary>
    /// Converts the item into an item result.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="item">The item.</param>
    /// <returns>An Item result.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static Item<TItem> PassIfHasValue<TItem>(TItem? item)
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
    [MethodImpl((MethodImplOptions)0x300)]
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
    [MethodImpl((MethodImplOptions)0x300)]
    public static Item<TItem, TError> Pass<TItem, TError>(TItem item)
    {
        return new Item<TItem, TError>(item, default, true);
    }

    /// <summary>
    /// Creates an item from the specified values.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="isValid">A value that indicates whether the item is valid.</param>
    /// <param name="item">The item.</param>
    /// <returns>The created item.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static Item<TItem> From<TItem>(bool isValid, TItem item)
    {
        return new Item<TItem>(item, isValid);
    }

    /// <summary>
    /// Creates an item from the specified values.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="isValid">A value that indicates whether the item is valid.</param>
    /// <param name="item">The item.</param>
    /// <param name="error">The error.</param>
    /// <returns>The created item.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static Item<TItem, TError> From<TItem, TError>(bool isValid, TItem item, TError error)
    {
        return new Item<TItem, TError>(item, error, isValid);
    }

    /// <summary>
    /// Create an error result.
    /// </summary>
    /// <returns>The result.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
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
    [MethodImpl((MethodImplOptions)0x300)]
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
    [MethodImpl((MethodImplOptions)0x300)]
    public static Item<TResult, TError> Fail<TResult, TError>(TError error)
    {
        return new Item<TResult, TError>(default, error, false);
    }

    /// <summary>
    /// Converts the option to an item.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="option">The option.</param>
    /// <returns>The new item.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static Item<TValue> ToItem<TValue>(this TValue? option)
        where TValue : struct
    {
        return new Item<TValue>(option.GetValueOrDefault(default), option.HasValue);
    }

    /// <summary>
    /// Converts the option to an item.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="option">The option.</param>
    /// <returns>The new item.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static Item<TValue> ToItem<TValue>(this TValue? option)
        where TValue : class
    {
        return new Item<TValue>(option, option.HasValue());
    }

    /// <summary>
    /// Converts the option to an item.
    /// </summary>
    /// <typeparam name="TResult">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">The result.</param>
    /// <returns>The new item.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static Item<TResult, TError> ToItem<TResult, TError>(this R<TResult, TError> result)
    {
        return new Item<TResult, TError>(result.Value, result.Error, result.IsSuccess);
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
    /// <remarks>
    /// Initializes a new instance of the <see cref="FailedItem{TError}"/> struct.
    /// </remarks>
    /// <param name="error">The error.</param>
    public readonly struct FailedItem<TError>(TError error)
    {
        /// <summary>
        /// Gets the error.
        /// </summary>
        public TError Error { get; } = error;

        /// <summary>
        /// Converts the <see cref="FailedItem{TError}"/> into a failed <see cref="Item{TResult,TError}"/>.
        /// </summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <returns>The failed item.</returns>
        public Item<TResult, TError> Omits<TResult>()
        {
            return new Item<TResult, TError>(default, this.Error, false);
        }
    }
}