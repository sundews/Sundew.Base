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
    /// <typeparam name="TList">The list type.</typeparam>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="immutableList">The immutable list.</param>
    /// <param name="option">The option.</param>
    /// <returns>The resulting list.</returns>
    public static TList AddIfHasValue<TList, TItem>(this TList immutableList, TItem? option)
        where TList : IImmutableList<TItem>
        where TItem : struct
    {
        return option.HasValue ? (TList)immutableList.Add(option.Value) : immutableList;
    }

    /// <summary>
    /// Add the option item if it has any.
    /// </summary>
    /// <typeparam name="TList">The list type.</typeparam>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="immutableList">The immutable list.</param>
    /// <param name="option">The option.</param>
    /// <returns>The resulting list.</returns>
    public static TList AddIfHasValue<TList, TItem>(this TList immutableList, TItem? option)
        where TList : IImmutableList<TItem>
        where TItem : class
    {
        return option.HasValue() ? (TList)immutableList.Add(option) : immutableList;
    }

    /// <summary>
    /// Add the result value if it succeeded.
    /// </summary>
    /// <typeparam name="TList">The list type.</typeparam>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="immutableList">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static TList AddIfSuccess<TList, TSuccess, TError>(this TList immutableList, R<TSuccess, TError> result)
        where TList : IImmutableList<TSuccess>
    {
        return result.IsSuccess ? (TList)immutableList.Add(result.Value) : immutableList;
    }

    /// <summary>
    /// Add the result error if the result failed.
    /// </summary>
    /// <typeparam name="TList">The list type.</typeparam>
    /// <typeparam name="TSuccess">The value type.</typeparam>
    /// <param name="immutableList">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static TList AddIfSuccess<TList, TSuccess>(this TList immutableList, RwV<TSuccess> result)
        where TList : IImmutableList<TSuccess>
    {
        return result.IsSuccess ? (TList)immutableList.Add(result.Value) : immutableList;
    }

    /// <summary>
    /// Add the result error if the result failed.
    /// </summary>
    /// <typeparam name="TList">The list type.</typeparam>
    /// <typeparam name="TError">The item type.</typeparam>
    /// <param name="immutableList">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static TList AddIfError<TList, TError>(this TList immutableList, RwE<TError> result)
        where TList : IImmutableList<TError>
    {
        return result.IsSuccess ? immutableList : (TList)immutableList.Add(result.Error);
    }

    /// <summary>
    /// Add the result error if the result failed.
    /// </summary>
    /// <typeparam name="TList">The list type.</typeparam>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The item type.</typeparam>
    /// <param name="immutableList">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static TList AddIfError<TList, TSuccess, TError>(this TList immutableList, R<TSuccess, TError> result)
        where TList : IImmutableList<TError>
    {
        return result.IsSuccess ? immutableList : (TList)immutableList.Add(result.Error);
    }

    /// <summary>
    /// Add the result error if it has any.
    /// </summary>
    /// <typeparam name="TList">The list type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="immutableList">The immutable list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static TList AddIfAnyError<TList, TError>(this TList immutableList, RwE<TError> result)
        where TList : IImmutableList<TError>
    {
        return result.HasError ? (TList)immutableList.Add(result.Error) : immutableList;
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
    public static TList AddIfAnyError<TList, TSuccess, TError>(this TList immutableList, R<TSuccess, TError> result)
        where TList : IImmutableList<TError>
    {
        return result.HasError ? (TList)immutableList.Add(result.Error) : immutableList;
    }
}