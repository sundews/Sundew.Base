// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImmutableArrayExtensions.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections.Immutable;

using System.Collections.Immutable;

/// <summary>
/// Extension methods for <see cref="ImmutableArray{T}"/>.
/// </summary>
public static class ImmutableArrayExtensions
{
    /// <summary>
    /// Converts the specified <see cref="ImmutableArray{T}"/> to a <see cref="ValueArray{TItem}"/>.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="immutableArray">The immutable array.</param>
    /// <returns>The value array.</returns>
    public static ValueArray<TItem> ToValueArray<TItem>(this ImmutableArray<TItem> immutableArray)
    {
        return new ValueArray<TItem>(immutableArray);
    }
}