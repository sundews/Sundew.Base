// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Empty.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections;

using System.Collections.Generic;

/// <summary>
/// Represents an empty <see cref="IEnumerable{T}"/>.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public sealed class Empty<TItem> : ListCardinality<TItem>
{
}