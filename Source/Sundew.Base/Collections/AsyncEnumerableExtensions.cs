// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncEnumerableExtensions.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Extends <see cref="IEnumerable{T}"/> with async methods.
    /// </summary>
    public static class AsyncEnumerableExtensions
    {
        /// <summary>
        /// Performs a select asynchronously.
        /// </summary>
        /// <typeparam name="TInItem">The type of the in item.</typeparam>
        /// <typeparam name="TOutItem">The type of the out item.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="selectFunc">The select function.</param>
        /// <returns>The selected items.</returns>
        public static Task<TOutItem[]> SelectAsync<TInItem, TOutItem>(
            this IEnumerable<TInItem> enumerable,
            Func<TInItem, Task<TOutItem>> selectFunc)
        {
            return Task.WhenAll(enumerable.Select(async item => await selectFunc(item).ConfigureAwait(false)));
        }

        /// <summary>
        /// Runs an asynchronous for each.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="action">The action.</param>
        /// <returns>The completion task.</returns>
        public static Task ForEachAsync<TItem>(this IEnumerable<TItem> enumerable, Func<TItem, Task> action)
        {
            return Task.WhenAll(enumerable.Select(async x => await action(x).ConfigureAwait(false)));
        }
    }
}