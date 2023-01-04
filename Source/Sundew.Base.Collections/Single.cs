// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Single.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections;

/// <summary>
/// Represents a single item.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public sealed class Single<TItem> : EmptyOrSingleOrMultiple<TItem>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Single{TItem}"/> class.
    /// </summary>
    /// <param name="item">The item.</param>
    internal Single(TItem item)
    {
        this.Item = item;
    }

    /// <summary>
    /// Gets the item.
    /// </summary>
    public TItem Item { get; }
}