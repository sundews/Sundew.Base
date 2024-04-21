// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableResultExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base;

using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Extends <see cref="IEnumerable"/> interface for the result structs <see cref="RwE{TError}"/> and <see cref="R{TValue,TError}"/>.
/// </summary>
public static class EnumerableResultExtensions
{
    /// <summary>
    /// Gets all non-successful item errors from the specified enumerable.
    /// </summary>
    /// <typeparam name="TSuccess">The error type.</typeparam>
    /// <param name="results">The results.</param>
    /// <returns>An enumerable containing all errors.</returns>
    public static IEnumerable<TSuccess> GetSuccesses<TSuccess>(this IEnumerable<RwV<TSuccess>> results)
    {
        foreach (var result in results)
        {
            if (result.IsSuccess)
            {
                yield return result.Value;
            }
        }
    }

    /// <summary>
    /// Gets all non-successful item errors from the specified enumerable.
    /// </summary>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="results">The results.</param>
    /// <returns>An enumerable containing all errors.</returns>
    public static IEnumerable<TError> GetErrors<TError>(this IEnumerable<RwE<TError>> results)
    {
        foreach (var result in results)
        {
            if (!result.IsSuccess)
            {
                yield return result.Error;
            }
        }
    }

    /// <summary>
    /// Gets all successful items from the specified enumerable.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="results">The results.</param>
    /// <returns>An enumerable containing all successful items.</returns>
    public static IEnumerable<TSuccess> GetSuccesses<TSuccess, TError>(this IEnumerable<R<TSuccess, TError>> results)
    {
        foreach (var result in results)
        {
            if (result.IsSuccess)
            {
                yield return result.Value;
            }
        }
    }

    /// <summary>
    /// Gets all non-successful item errors from the specified enumerable.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="results">The results.</param>
    /// <returns>An enumerable containing all errors.</returns>
    public static IEnumerable<TError> GetErrors<TSuccess, TError>(this IEnumerable<R<TSuccess, TError>> results)
    {
        foreach (var result in results)
        {
            if (!result.IsSuccess)
            {
                yield return result.Error;
            }
        }
    }

    /// <summary>
    /// Gets all error items from the specified enumerable, including errors for successful results.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="results">The results.</param>
    /// <returns>An enumerable containing all errors.</returns>
    public static IEnumerable<TError> GetAnyErrors<TSuccess, TError>(this IEnumerable<R<TSuccess, TError>> results)
    {
        foreach (var result in results)
        {
            if (result.HasError)
            {
                yield return result.Error;
            }
        }
    }
}