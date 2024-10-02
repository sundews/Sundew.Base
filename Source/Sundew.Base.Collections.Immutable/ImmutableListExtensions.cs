// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImmutableListExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections.Immutable;

using System.Collections.Immutable;

/// <summary>
/// Extension methods for <see cref="IImmutableList{T}"/>.
/// </summary>
public static class ImmutableListExtensions
{
    /// <summary>
    /// Converts the specified <see cref="IImmutableList{T}"/> to a <see cref="ValueList{TItem}"/>.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="immutableList">The immutable list.</param>
    /// <returns>The value list.</returns>
    public static ValueList<TItem> ToValueList<TItem>(this IImmutableList<TItem>? immutableList)
    {
        return new ValueList<TItem>(immutableList ?? ImmutableList<TItem>.Empty);
    }

    /// <summary>
    /// Add the option item if it has any.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="immutableList">The immutable list.</param>
    /// <param name="option">The option.</param>
    /// <returns>The resulting list.</returns>
    public static ImmutableList<TItem> AddIfHasValue<TItem>(this ImmutableList<TItem> immutableList, TItem? option)
        where TItem : struct
    {
        return option.HasValue ? immutableList.Add(option.Value) : immutableList;
    }

    /// <summary>
    /// Add the option item if it has any.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="immutableList">The immutable list.</param>
    /// <param name="option">The option.</param>
    /// <returns>The resulting list.</returns>
    public static ImmutableList<TItem> AddIfHasValue<TItem>(this ImmutableList<TItem> immutableList, TItem? option)
        where TItem : class
    {
        return option.HasValue() ? immutableList.Add(option) : immutableList;
    }

    /// <summary>
    /// Add the result value if it succeeded.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="immutableList">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ImmutableList<TSuccess> AddIfSuccess<TSuccess, TError>(this ImmutableList<TSuccess> immutableList, R<TSuccess, TError> result)
    {
        return result.IsSuccess ? immutableList.Add(result.Value) : immutableList;
    }

    /// <summary>
    /// Add the result error if the result failed.
    /// </summary>
    /// <typeparam name="TSuccess">The value type.</typeparam>
    /// <param name="immutableList">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ImmutableList<TSuccess> AddIfSuccess<TSuccess>(this ImmutableList<TSuccess> immutableList, R<TSuccess> result)
    {
        return result.IsSuccess ? immutableList.Add(result.Value) : immutableList;
    }

    /// <summary>
    /// Add the result error if the result failed.
    /// </summary>
    /// <typeparam name="TError">The item type.</typeparam>
    /// <param name="immutableList">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ImmutableList<TError> AddIfError<TError>(this ImmutableList<TError> immutableList, RoE<TError> result)
    {
        return result.IsSuccess ? immutableList : immutableList.Add(result.Error);
    }

    /// <summary>
    /// Add the result error if the result failed.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The item type.</typeparam>
    /// <param name="immutableList">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ImmutableList<TError> AddIfError<TSuccess, TError>(this ImmutableList<TError> immutableList, R<TSuccess, TError> result)
    {
        return result.IsSuccess ? immutableList : immutableList.Add(result.Error);
    }

    /// <summary>
    /// Add the result error if it has any.
    /// </summary>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="immutableList">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ImmutableList<TError> AddIfAnyError<TError>(this ImmutableList<TError> immutableList, RoE<TError> result)
    {
        return result.HasError ? immutableList.Add(result.Error) : immutableList;
    }

    /// <summary>
    /// Add the result error if it has any.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="immutableList">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ImmutableList<TError> AddIfAnyError<TSuccess, TError>(this ImmutableList<TError> immutableList, R<TSuccess, TError> result)
    {
        return result.HasError ? immutableList.Add(result.Error) : immutableList;
    }
}