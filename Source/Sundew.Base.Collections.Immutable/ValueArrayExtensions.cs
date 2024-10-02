// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueArray{TItem}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections.Immutable;

/// <summary>
/// Represents in value array that implements value semantics.
/// </summary>
public static class ValueArrayExtensions
{
    /// <summary>
    /// Tries to add the option item if it has any.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="valueArray">The value array.</param>
    /// <param name="option">The option.</param>
    /// <returns>The resulting array.</returns>
    public static ValueArray<TItem> AddIfHasValue<TItem>(this ValueArray<TItem> valueArray, TItem? option)
        where TItem : struct
    {
        return option.HasValue ? valueArray.Add(option.Value) : valueArray;
    }

    /// <summary>
    /// Tries to add the option item if it has any.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="valueArray">The value array.</param>
    /// <param name="option">The option.</param>
    /// <returns>The resulting array.</returns>
    public static ValueArray<TItem> AddIfHasValue<TItem>(this ValueArray<TItem> valueArray, TItem? option)
        where TItem : class
    {
        return option.HasValue() ? valueArray.Add(option) : valueArray;
    }

    /// <summary>
    /// Tries to add the result item if it is successful.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="valueArray">The value list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ValueArray<TSuccess> AddIfSuccess<TSuccess, TError>(this ValueArray<TSuccess> valueArray, R<TSuccess, TError> result)
    {
        return result.IsSuccess ? valueArray.Add(result.Value) : valueArray;
    }

    /// <summary>
    /// Adds the result error if the result failed.
    /// </summary>
    /// <typeparam name="TSuccess">The error type.</typeparam>
    /// <param name="valueArray">The value list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ValueArray<TSuccess> AddIfSuccess<TSuccess>(this ValueArray<TSuccess> valueArray, R<TSuccess> result)
    {
        return result.IsSuccess ? valueArray.Add(result.Value) : valueArray;
    }

    /// <summary>
    /// Adds the result error if the result failed.
    /// </summary>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="valueArray">The value list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ValueArray<TError> AddIfError<TError>(this ValueArray<TError> valueArray, RoE<TError> result)
    {
        return result.IsSuccess ? valueArray : valueArray.Add(result.Error);
    }

    /// <summary>
    /// Adds the result error if the result failed.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="valueArray">The value list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ValueArray<TError> AddIfError<TSuccess, TError>(this ValueArray<TError> valueArray, R<TSuccess, TError> result)
    {
        return result.IsSuccess ? valueArray : valueArray.Add(result.Error);
    }

    /// <summary>
    /// Adds the result error if there are any.
    /// </summary>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="valueArray">The value list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ValueArray<TError> AddIfAnyError<TError>(this ValueArray<TError> valueArray, RoE<TError> result)
    {
        return result.HasError ? valueArray.Add(result.Error) : valueArray;
    }

    /// <summary>
    /// Adds the result error if there are any.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="valueArray">The value list.</param>
    /// <param name="result">The result.</param>
    /// <returns> The resulting list.</returns>
    public static ValueArray<TError> AddIfAnyError<TSuccess, TError>(this ValueArray<TError> valueArray, R<TSuccess, TError> result)
    {
        return result.HasError ? valueArray.Add(result.Error) : valueArray;
    }
}