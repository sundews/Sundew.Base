// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueListExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections.Immutable;

/// <summary>
/// Extension methods for <see cref="ValueList{T}"/>.
/// </summary>
public static class ValueListExtensions
{
    /// <summary>
    /// Add the option item if it has any.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="valueList">The value list.</param>
    /// <param name="option">The option.</param>
    /// <returns>The resulting list.</returns>
    public static ValueList<TItem> AddIfHasValue<TItem>(this ValueList<TItem> valueList, TItem? option)
        where TItem : struct
    {
        return option.HasValue ? valueList.Add(option.Value) : valueList;
    }

    /// <summary>
    /// Add the option item if it has any.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="valueList">The value list.</param>
    /// <param name="option">The option.</param>
    /// <returns>The resulting list.</returns>
    public static ValueList<TItem> AddIfHasValue<TItem>(this ValueList<TItem> valueList, TItem? option)
        where TItem : class
    {
        return option.HasValue ? valueList.Add(option) : valueList;
    }

    /// <summary>
    /// Add the result value if it succeeded.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="valueList">The value list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ValueList<TSuccess> AddIfSuccess<TSuccess, TError>(this ValueList<TSuccess> valueList, R<TSuccess, TError> result)
    {
        return result.IsSuccess ? valueList.Add(result.Value) : valueList;
    }

    /// <summary>
    /// Add the result error if the result failed.
    /// </summary>
    /// <typeparam name="TSuccess">The value type.</typeparam>
    /// <param name="valueList">The value list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ValueList<TSuccess> AddIfSuccess<TSuccess>(this ValueList<TSuccess> valueList, R<TSuccess> result)
    {
        return result.IsSuccess ? valueList.Add(result.Value) : valueList;
    }

    /// <summary>
    /// Add the result error if the result failed.
    /// </summary>
    /// <typeparam name="TError">The item type.</typeparam>
    /// <param name="valueList">The value list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ValueList<TError> AddIfError<TError>(this ValueList<TError> valueList, RoE<TError> result)
    {
        return result.IsSuccess ? valueList : valueList.Add(result.Error);
    }

    /// <summary>
    /// Add the result error if the result failed.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The item type.</typeparam>
    /// <param name="valueList">The value list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ValueList<TError> AddIfError<TSuccess, TError>(this ValueList<TError> valueList, R<TSuccess, TError> result)
    {
        return result.IsSuccess ? valueList : valueList.Add(result.Error);
    }

    /// <summary>
    /// Add the result error if it has any.
    /// </summary>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="valueList">The value list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ValueList<TError> AddIfAnyError<TError>(this ValueList<TError> valueList, RoE<TError> result)
    {
        return result.HasError ? valueList.Add(result.Error) : valueList;
    }

    /// <summary>
    /// Add the result error if it has any.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="valueList">The value list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ValueList<TError> AddIfAnyError<TSuccess, TError>(this ValueList<TError> valueList, R<TSuccess, TError> result)
    {
        return result.HasError ? valueList.Add(result.Error) : valueList;
    }
}