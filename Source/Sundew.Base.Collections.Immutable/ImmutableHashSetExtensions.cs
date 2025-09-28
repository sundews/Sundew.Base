// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImmutableHashSetExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections.Immutable;

using System.Collections.Immutable;

/// <summary>
/// Extension methods for <see cref="ImmutableHashSet{TItem}"/>.
/// </summary>
public static class ImmutableHashSetExtensions
{
    /// <summary>
    /// Tries to add the item and returns the new immutable hash set and a value indicating whether the value was added.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="immutableHashSet">The immutable hashset.</param>
    /// <param name="item">The item.</param>
    /// <returns>The new immutable hashset and a value indicating whether the value was added.</returns>
    public static (ImmutableHashSet<TItem> Result, bool WasAdded) TryAdd<TItem>(this ImmutableHashSet<TItem> immutableHashSet, TItem item)
    {
        var result = immutableHashSet.Add(item);
        return (result, !Equals(immutableHashSet, result));
    }

    /// <summary>
    /// Tries to add the option item if it has any.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="immutableSet">The immutable array.</param>
    /// <param name="option">The option.</param>
    /// <returns>The resulting array.</returns>
    public static ImmutableHashSet<TItem> AddIfHasValue<TItem>(this ImmutableHashSet<TItem> immutableSet, TItem? option)
        where TItem : struct
    {
        return option.HasValue ? immutableSet.Add(option.Value) : immutableSet;
    }

    /// <summary>
    /// Tries to add the option item if it has any.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="immutableSet">The immutable array.</param>
    /// <param name="option">The option.</param>
    /// <returns>The resulting array.</returns>
    public static ImmutableHashSet<TItem> AddIfHasValue<TItem>(this ImmutableHashSet<TItem> immutableSet, TItem? option)
        where TItem : class
    {
        return option.HasValue ? immutableSet.Add(option) : immutableSet;
    }

    /// <summary>
    /// Tries to add the result item if it has any.
    /// </summary>
    /// <typeparam name="TSuccess">The value type.</typeparam>
    /// <param name="immutableSet">The immutable array.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ImmutableHashSet<TSuccess> AddIfSuccess<TSuccess>(this ImmutableHashSet<TSuccess> immutableSet, R<TSuccess> result)
    {
        return result.IsSuccess ? immutableSet.Add(result.Value) : immutableSet;
    }

    /// <summary>
    /// Tries to add the result item if it has any.
    /// </summary>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="immutableSet">The immutable array.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ImmutableHashSet<TError> AddIfError<TError>(this ImmutableHashSet<TError> immutableSet, RoE<TError> result)
    {
        return result.IsSuccess ? immutableSet : immutableSet.Add(result.Error);
    }

    /// <summary>
    /// Tries to add the result item if it has any.
    /// </summary>
    /// <typeparam name="TSuccess">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="immutableSet">The immutable array.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ImmutableHashSet<TSuccess> AddIfSuccess<TSuccess, TError>(this ImmutableHashSet<TSuccess> immutableSet, R<TSuccess, TError> result)
    {
        return result.IsSuccess ? immutableSet.Add(result.Value) : immutableSet;
    }

    /// <summary>
    /// Tries to add the result item if it has any.
    /// </summary>
    /// <typeparam name="TSuccess">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="immutableSet">The immutable array.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ImmutableHashSet<TError> AddIfError<TSuccess, TError>(this ImmutableHashSet<TError> immutableSet, R<TSuccess, TError> result)
    {
        return result.IsSuccess ? immutableSet : immutableSet.Add(result.Error);
    }
}