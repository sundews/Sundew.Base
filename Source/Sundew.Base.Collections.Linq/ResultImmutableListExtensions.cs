// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultImmutableListExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections;

using System.Collections.Immutable;
using System.Linq;

/// <summary>
/// Extension methods for <see cref="IImmutableList{T}"/>.
/// </summary>
public static class ResultImmutableListExtensions
{
    /// <summary>
    /// Add the result value if it succeeded.
    /// </summary>
    /// <typeparam name="TList">The list type.</typeparam>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="immutableList">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static TList TryAddSuccesses<TList, TSuccess, TError>(this TList immutableList, R<All<TSuccess>, Failed<TError>> result)
        where TList : IImmutableList<TSuccess>
    {
        return result.IsSuccess ? (TList)immutableList.AddRange(result.Value) : immutableList;
    }

    /// <summary>
    /// Add the result value if it succeeded.
    /// </summary>
    /// <typeparam name="TList">The list type.</typeparam>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="immutableList">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static TList TryAddSuccesses<TList, TSuccess, TItem, TError>(this TList immutableList, R<All<TSuccess>, Failed<TItem, TError>> result)
        where TList : IImmutableList<TSuccess>
    {
        return result.IsSuccess ? (TList)immutableList.AddRange(result.Value) : immutableList;
    }

    /// <summary>
    /// Add the result error if the result failed.
    /// </summary>
    /// <typeparam name="TList">The list type.</typeparam>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="immutableList">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static TList TryAddErrors<TList, TItem>(this TList immutableList, R<Failed<TItem>> result)
        where TList : IImmutableList<TItem>
    {
        return result.IsSuccess ? immutableList : (TList)immutableList.AddRange(result.Error.GetItems().Where(x => x != null).Select(x => x!));
    }

    /// <summary>
    /// Add the result error if the result failed.
    /// </summary>
    /// <typeparam name="TList">The list type.</typeparam>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="immutableList">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static TList TryAddErrors<TList, TSuccess, TError>(this TList immutableList, R<All<TSuccess>, Failed<TError>> result)
        where TList : IImmutableList<TError>
    {
        return result.IsSuccess ? immutableList : (TList)immutableList.AddRange(result.Error.GetItems().Where(x => x != null).Select(x => x!));
    }

    /// <summary>
    /// Add the result error if the result failed.
    /// </summary>
    /// <typeparam name="TList">The list type.</typeparam>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="immutableList">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static TList TryAddErrors<TList, TSuccess, TItem, TError>(this TList immutableList, R<All<TSuccess>, Failed<TItem, TError>> result)
        where TList : IImmutableList<TError>
    {
        return result.IsSuccess ? immutableList : (TList)immutableList.AddRange(result.Error.GetErrors());
    }

    /// <summary>
    /// Add the result error if has any.
    /// </summary>
    /// <typeparam name="TList">The list type.</typeparam>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="immutableList">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static TList TryAddAnyErrors<TList, TItem>(this TList immutableList, R<Failed<TItem>> result)
        where TList : IImmutableList<TItem>
    {
        return result.HasError ? (TList)immutableList.AddRange(result.Error.GetItems().Where(x => x != null).Select(x => x!)) : immutableList;
    }

    /// <summary>
    /// Add the result error if it has any.
    /// </summary>
    /// <typeparam name="TList">The list type.</typeparam>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="immutableList">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static TList TryAddAnyErrors<TList, TSuccess, TError>(this TList immutableList, R<All<TSuccess>, Failed<TError>> result)
        where TList : IImmutableList<TError>
    {
        return result.HasError ? (TList)immutableList.AddRange(result.Error.GetItems().Where(x => x != null).Select(x => x!)) : immutableList;
    }

    /// <summary>
    /// Add the result error if it has any.
    /// </summary>
    /// <typeparam name="TList">The list type.</typeparam>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="immutableList">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static TList TryAddAnyErrors<TList, TSuccess, TItem, TError>(this TList immutableList, R<All<TSuccess>, Failed<TItem, TError>> result)
        where TList : IImmutableList<TError>
    {
        return result.HasError ? (TList)immutableList.AddRange(result.Error.GetErrors()) : immutableList;
    }
}