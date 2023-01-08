// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListCardinality.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections;

using System.Collections.Generic;

/// <summary>
/// Represents a discriminated union of either none, a single or multiple items.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract class ListCardinality<TItem>
{
    /// <summary>
    /// Creates an empty result.
    /// </summary>
    /// <returns>A empty result.</returns>
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(Empty<>))]
    public static ListCardinality<TItem> Empty() => new Empty<TItem>();

    /// <summary>
    /// Create a single result.
    /// </summary>
    /// <param name="item">The single item.</param>
    /// <returns>A single result.</returns>
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(Single<>))]
    public static ListCardinality<TItem> Single(TItem item) => new Single<TItem>(item);

    /// <summary>
    /// Creates a multiple result.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <returns>A multiple result.</returns>
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(Multiple<>))]
    public static ListCardinality<TItem> Multiple(IEnumerable<TItem> items) => new Multiple<TItem>(items);
}