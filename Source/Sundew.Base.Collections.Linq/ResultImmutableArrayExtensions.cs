// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultImmutableArrayExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections.Linq;

using System.Collections.Immutable;
using System.Linq;

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
    public static ImmutableArray<TSuccess> AddAllIfSuccess<TSuccess, TError>(this ImmutableArray<TSuccess> immutableArray, in R<All<TSuccess>, Failed<TError>> result)
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
    public static ImmutableArray<TSuccess> AddAllIfSuccess<TSuccess, TItem, TError>(this ImmutableArray<TSuccess> immutableArray, in R<All<TSuccess>, Failed<TItem, TError>> result)
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
    public static ImmutableArray<TError> AddFailedIfError<TError>(this ImmutableArray<TError> immutableArray, in RwE<Failed<TError>> result)
    {
        return result.IsSuccess ? immutableArray : immutableArray.AddRange(result.Error.GetItems().Where(x => x != null).Select(x => x!));
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
    public static ImmutableArray<TError> AddFailedIfError<TSuccess, TItem, TError>(this ImmutableArray<TError> immutableArray, in R<All<TSuccess>, Failed<TError>> result)
    {
        return result.IsSuccess ? immutableArray : immutableArray.AddRange(result.Error.GetItems().Where(x => x != null).Select(x => x!));
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
    public static ImmutableArray<TError> AddFailedIfError<TSuccess, TItem, TError>(this ImmutableArray<TError> immutableArray, in R<All<TSuccess>, Failed<TItem, TError>> result)
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
    public static ImmutableArray<TError> AddFailedIfAnyError<TError>(this ImmutableArray<TError> immutableArray, in RwE<Failed<TError>> result)
    {
        return result.HasError ? immutableArray.AddRange(result.Error.GetItems().Where(x => x != null).Select(x => x!)) : immutableArray;
    }

    /// <summary>
    /// Adds the result error if there are any.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="immutableArray">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ImmutableArray<TError> AddFailedIfAnyError<TSuccess, TError>(this ImmutableArray<TError> immutableArray, in R<All<TSuccess>, Failed<TError>> result)
    {
        return result.HasError ? immutableArray.AddRange(result.Error.GetItems().Where(x => x != null).Select(x => x!)) : immutableArray;
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
    public static ImmutableArray<TError> AddFailedIfAnyError<TSuccess, TItem, TError>(this ImmutableArray<TError> immutableArray, in R<All<TSuccess>, Failed<TItem, TError>> result)
    {
        return result.HasError ? immutableArray.AddRange(result.Error.GetErrors()) : immutableArray;
    }
}