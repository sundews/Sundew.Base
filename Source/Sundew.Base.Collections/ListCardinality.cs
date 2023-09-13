// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListCardinality.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections;

/// <summary>
/// Represents a discriminated union of either none, a single or multiple items.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract partial class ListCardinality<TItem>
{
}