// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImmutableDictionaryExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections.Immutable;

using System;
using System.Collections.Immutable;

/// <summary>
/// Extension methods for <see cref="ImmutableDictionary{TKey, TValue}"/>.
/// </summary>
public static class ImmutableDictionaryExtensions
{
    /// <summary>
    /// Converts the specified <see cref="ImmutableDictionary{TKey, TValue}"/> to a <see cref="ValueDictionary{TKey, TValue}"/>.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="immutableDictionary">The immutable array.</param>
    /// <returns>The value array.</returns>
    public static ValueDictionary<TKey, TValue> ToValueDictionary<TKey, TValue>(this IImmutableDictionary<TKey, TValue> immutableDictionary)
        where TKey : IEquatable<TKey>
    {
        return new ValueDictionary<TKey, TValue>(immutableDictionary);
    }
}