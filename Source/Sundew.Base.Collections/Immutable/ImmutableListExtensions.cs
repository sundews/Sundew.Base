// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImmutableListExtensions.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
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
    public static ValueList<TItem> ToValueList<TItem>(this IImmutableList<TItem> immutableList)
    {
        return new ValueList<TItem>(immutableList);
    }
}