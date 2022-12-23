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
    /// Create an error result.
    /// </summary>
    /// <returns>The result.</returns>
    public static FailedItem Fail()
    {
        return default;
    }

    /// <summary>
    /// Represents a failed item.
    /// </summary>
    public readonly struct FailedItem
    {
    }
}