// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableOptionExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives.Computation;

using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Extends <see cref="IEnumerable"/> interface for the optional values.
/// </summary>
public static class EnumerableOptionExtensions
{
    /// <summary>
    /// Gets all successful items from the specified enumerable.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="options">The options.</param>
    /// <returns>An enumerable containing all successful items.</returns>
    public static IEnumerable<TValue> GetValues<TValue>(this IEnumerable<TValue?> options)
        where TValue : class
    {
        foreach (var result in options)
        {
            if (result.HasValue())
            {
                yield return result;
            }
        }
    }

    /// <summary>
    /// Gets all successful items from the specified enumerable.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="options">The options.</param>
    /// <returns>An enumerable containing all successful items.</returns>
    public static IEnumerable<TValue> GetValues<TValue>(this IEnumerable<TValue?> options)
        where TValue : struct
    {
        foreach (var result in options)
        {
            if (result.HasValue)
            {
                yield return result.Value;
            }
        }
    }
}