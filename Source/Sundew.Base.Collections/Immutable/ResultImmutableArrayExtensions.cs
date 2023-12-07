// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultImmutableArrayExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections.Immutable;

using System.Collections.Immutable;
using Sundew.Base.Primitives;

/// <summary>
/// Extension methods for <see cref="ImmutableArray{T}"/>.
/// </summary>
public static class ResultImmutableArrayExtensions
{
    /// <summary>
    /// Tries to add the result item if it is successful.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="immutableArray">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ImmutableArray<TSuccess> TryAddAll<TSuccess, TError>(this ImmutableArray<TSuccess> immutableArray, R<All<TSuccess>, Failed<TError>> result)
    {
        return result.IsSuccess ? immutableArray.AddRange(result.Value) : immutableArray;
    }

    /// <summary>
    /// Tries to add the result item if it is successful.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="immutableArray">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ImmutableArray<TSuccess> TryAddAll<TSuccess, TItem, TError>(this ImmutableArray<TSuccess> immutableArray, R<All<TSuccess>, Failed<TItem, TError>> result)
    {
        return result.IsSuccess ? immutableArray.AddRange(result.Value) : immutableArray;
    }

    /// <summary>
    /// Adds the result error if the result failed.
    /// </summary>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="immutableArray">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ImmutableArray<TError> TryAddErrors<TError>(this ImmutableArray<TError> immutableArray, R<Failed<TError>> result)
    {
        return result.IsSuccess ? immutableArray : immutableArray.AddRange(result.Error.GetItems().WhereNotNull());
    }

    /// <summary>
    /// Adds the result error if the result failed.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="immutableArray">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ImmutableArray<TError> TryAddErrors<TSuccess, TItem, TError>(this ImmutableArray<TError> immutableArray, R<All<TSuccess>, Failed<TError>> result)
    {
        return result.IsSuccess ? immutableArray : immutableArray.AddRange(result.Error.GetItems().WhereNotNull());
    }

    /// <summary>
    /// Adds the result error if the result failed.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="immutableArray">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ImmutableArray<TError> TryAddErrors<TSuccess, TItem, TError>(this ImmutableArray<TError> immutableArray, R<All<TSuccess>, Failed<TItem, TError>> result)
    {
        return result.IsSuccess ? immutableArray : immutableArray.AddRange(result.Error.GetErrors());
    }

    /// <summary>
    /// Adds the result error if there are any.
    /// </summary>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="immutableArray">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ImmutableArray<TError> TryAddAnyErrors<TError>(this ImmutableArray<TError> immutableArray, R<Failed<TError>> result)
    {
        return result.HasError ? immutableArray.AddRange(result.Error.GetItems().WhereNotNull()) : immutableArray;
    }

    /// <summary>
    /// Adds the result error if there are any.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="immutableArray">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ImmutableArray<TError> TryAddAnyErrors<TSuccess, TError>(this ImmutableArray<TError> immutableArray, R<All<TSuccess>, Failed<TError>> result)
    {
        return result.HasError ? immutableArray.AddRange(result.Error.GetItems().WhereNotNull()) : immutableArray;
    }

    /// <summary>
    /// Adds the result error if there are any.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="immutableArray">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ImmutableArray<TError> TryAddAnyErrors<TSuccess, TItem, TError>(this ImmutableArray<TError> immutableArray, R<All<TSuccess>, Failed<TItem, TError>> result)
    {
        return result.HasError ? immutableArray.AddRange(result.Error.GetErrors()) : immutableArray;
    }
}