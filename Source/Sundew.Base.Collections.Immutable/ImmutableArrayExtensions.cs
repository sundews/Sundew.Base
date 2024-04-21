// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImmutableArrayExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections.Immutable;

using System.Collections.Immutable;

/// <summary>
/// Extension methods for <see cref="ImmutableArray{T}"/>.
/// </summary>
public static class ImmutableArrayExtensions
{
    /// <summary>
    /// Converts the specified <see cref="ImmutableArray{T}"/> to a <see cref="ValueArray{TItem}"/>.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="immutableArray">The immutable array.</param>
    /// <returns>The value array.</returns>
    public static ValueArray<TItem> ToValueArray<TItem>(this ImmutableArray<TItem> immutableArray)
    {
        return new ValueArray<TItem>(immutableArray);
    }

    /// <summary>
    /// Tries to add the option item if it has any.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="immutableArray">The immutable array.</param>
    /// <param name="option">The option.</param>
    /// <returns>The resulting array.</returns>
    public static ImmutableArray<TItem> AddIfHasValue<TItem>(this ImmutableArray<TItem> immutableArray, TItem? option)
        where TItem : struct
    {
        return option.HasValue ? immutableArray.Add(option.Value) : immutableArray;
    }

    /// <summary>
    /// Tries to add the option item if it has any.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="immutableArray">The immutable array.</param>
    /// <param name="option">The option.</param>
    /// <returns>The resulting array.</returns>
    public static ImmutableArray<TItem> AddIfHasValue<TItem>(this ImmutableArray<TItem> immutableArray, TItem? option)
        where TItem : class
    {
        return option.HasValue() ? immutableArray.Add(option) : immutableArray;
    }

    /// <summary>
    /// Tries to add the result item if it is successful.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="immutableArray">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ImmutableArray<TSuccess> AddIfSuccess<TSuccess, TError>(this ImmutableArray<TSuccess> immutableArray, R<TSuccess, TError> result)
    {
        return result.IsSuccess ? immutableArray.Add(result.Value) : immutableArray;
    }

    /// <summary>
    /// Adds the result error if the result failed.
    /// </summary>
    /// <typeparam name="TSuccess">The error type.</typeparam>
    /// <param name="immutableArray">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ImmutableArray<TSuccess> AddIfSuccess<TSuccess>(this ImmutableArray<TSuccess> immutableArray, RwV<TSuccess> result)
    {
        return result.IsSuccess ? immutableArray.Add(result.Value) : immutableArray;
    }

    /// <summary>
    /// Adds the result error if the result failed.
    /// </summary>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="immutableArray">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ImmutableArray<TError> AddIfError<TError>(this ImmutableArray<TError> immutableArray, RwE<TError> result)
    {
        return result.IsSuccess ? immutableArray : immutableArray.Add(result.Error);
    }

    /// <summary>
    /// Adds the result error if the result failed.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="immutableArray">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ImmutableArray<TError> AddIfError<TSuccess, TError>(this ImmutableArray<TError> immutableArray, R<TSuccess, TError> result)
    {
        return result.IsSuccess ? immutableArray : immutableArray.Add(result.Error);
    }

    /// <summary>
    /// Adds the result error if there are any.
    /// </summary>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="immutableArray">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ImmutableArray<TError> AddIfAnyError<TError>(this ImmutableArray<TError> immutableArray, RwE<TError> result)
    {
        return result.HasError ? immutableArray.Add(result.Error) : immutableArray;
    }

    /// <summary>
    /// Adds the result error if there are any.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="immutableArray">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ImmutableArray<TError> AddIfAnyError<TSuccess, TError>(this ImmutableArray<TError> immutableArray, R<TSuccess, TError> result)
    {
        return result.HasError ? immutableArray.Add(result.Error) : immutableArray;
    }
}