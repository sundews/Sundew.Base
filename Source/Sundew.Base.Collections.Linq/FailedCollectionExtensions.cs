// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FailedCollectionExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections;

using System.Collections.Generic;

/// <summary>
/// Defines extension methods for the generic ICollection interface.
/// </summary>
public static class FailedCollectionExtensions
{
    /// <summary>
    /// Tries to add all the result errors if there are any.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="results">The results.</param>
    /// <returns><c>true</c>, if the error was added, otherwise <c>false</c>.</returns>
    public static bool TryAddAnyErrors<TValue, TError>(this ICollection<TError> collection, R<TValue, Failed<TError>> results)
    {
        var count = collection.Count;
        if (results.HasError)
        {
            collection.AddRange(results.Error.GetItems().WhereNotNull());
        }

        return count < collection.Count;
    }

    /// <summary>
    /// Tries to add all the result errors if there are any.
    /// </summary>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="results">The results.</param>
    /// <returns><c>true</c>, if the error was added, otherwise <c>false</c>.</returns>
    public static bool TryAddAnyErrors<TError>(this ICollection<TError> collection, R<Failed<TError>> results)
    {
        var count = collection.Count;
        if (results.HasError)
        {
            collection.AddRange(results.Error.GetItems().WhereNotNull());
        }

        return count < collection.Count;
    }
}