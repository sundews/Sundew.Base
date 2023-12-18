// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImmutableHashSetExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections.Immutable;

using System.Collections.Immutable;

/// <summary>
/// Extension methods fro <see cref="ImmutableHashSet{TItem}"/>.
/// </summary>
public static class ImmutableHashSetExtensions
{
    /// <summary>
    /// Tries to a the item and returns the new immutable hash set and a value indicating whether the value was added.
    /// </summary>
    /// <typeparam name="TSet">The set type.</typeparam>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="immutableHashSet">The immutable hashset.</param>
    /// <param name="item">The item.</param>
    /// <returns>The new immutable hashset and a value indicating whether the value was added.</returns>
    public static (TSet Result, bool WasAdded) TryAdd<TSet, TItem>(this TSet immutableHashSet, TItem item)
        where TSet : IImmutableSet<TItem>
    {
        var result = immutableHashSet.Add(item);
        return ((TSet)result, !Equals(immutableHashSet, result));
    }

    /// <summary>
    /// Tries to add the option item if it has any.
    /// </summary>
    /// <typeparam name="TSet">The set type.</typeparam>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="immutableSet">The immutable array.</param>
    /// <param name="option">The option.</param>
    /// <returns>The resulting array.</returns>
    public static TSet AddIfHasValue<TSet, TItem>(this TSet immutableSet, TItem? option)
        where TSet : IImmutableSet<TItem>
        where TItem : struct
    {
        return option.HasValue ? (TSet)immutableSet.Add(option.Value) : immutableSet;
    }

    /// <summary>
    /// Tries to add the option item if it has any.
    /// </summary>
    /// <typeparam name="TSet">The set type.</typeparam>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="immutableSet">The immutable array.</param>
    /// <param name="option">The option.</param>
    /// <returns>The resulting array.</returns>
    public static TSet AddIfHasValue<TSet, TItem>(this TSet immutableSet, TItem? option)
        where TSet : IImmutableSet<TItem>
        where TItem : class
    {
        return option.HasValue() ? (TSet)immutableSet.Add(option) : immutableSet;
    }

    /// <summary>
    /// Tries to add the result item if it has any.
    /// </summary>
    /// <typeparam name="TSet">The set type.</typeparam>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="immutableSet">The immutable array.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static TSet TryAdd<TSet, TItem>(this TSet immutableSet, R<TItem> result)
        where TSet : IImmutableSet<TItem>
    {
        return result.IsSuccess ? immutableSet : (TSet)immutableSet.Add(result.Error);
    }
}