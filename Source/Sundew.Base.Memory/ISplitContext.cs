// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISplitContext.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Memory
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface for split context.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public interface ISplitContext<TItem>
    {
        /// <summary>
        /// Gets the input.
        /// </summary>
        /// <value>
        /// The input.
        /// </value>
        ReadOnlyMemory<TItem> Input { get; }

        /// <summary>
        /// Appends the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        void Append(TItem item);

        /// <summary>
        /// Appends the specified enumerable.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        void Append(IEnumerable<TItem> enumerable);

        /// <summary>
        /// Appends the specified read only items.
        /// </summary>
        /// <param name="readOnlyItems">The read only items.</param>
        void Append(IReadOnlyList<TItem> readOnlyItems);

        /// <summary>
        /// Appends the specified span.
        /// </summary>
        /// <param name="span">The span.</param>
        void Append(ReadOnlySpan<TItem> span);

        /// <summary>
        /// Gets the next or default.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The next item or default.</returns>
        TItem? GetNextOrDefault(int index);

        /// <summary>
        /// Gets the previous or default.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The previous item or default.</returns>
        TItem? GetPreviousOrDefault(int index);

        /// <summary>
        /// Tries to get the next item or default.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if next is a valid item.</returns>
        bool TryGetNext(int index, out TItem? item);

        /// <summary>
        /// Tries to get the previous item or default.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if previous is a valid item.</returns>
        bool TryGetPrevious(int index, out TItem? item);
    }
}