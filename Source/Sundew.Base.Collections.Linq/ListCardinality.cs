// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListCardinality.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections.Linq;

/// <summary>
/// Represents a discriminated union of either none, a single or multiple items.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
[Sundew.DiscriminatedUnions.DiscriminatedUnion]
#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
public abstract partial record ListCardinality<TItem>
#else
public abstract partial class ListCardinality<TItem>

#endif
{
}