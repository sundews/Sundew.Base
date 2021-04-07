// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyListExtensions.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Extends <see cref="IReadOnlyList{TItem}"/> interface.
    /// </summary>
    public static class ReadOnlyListExtensions
    {
        /// <summary>
        /// Copies items from the specified list to the specified array.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="list">The read only list.</param>
        /// <param name="sourceIndex">Index of the source.</param>
        /// <param name="targetArray">The target array.</param>
        /// <param name="targetIndex">Index of the target.</param>
        /// <param name="count">The count.</param>
        public static void CopyTo<TItem>(
            this IReadOnlyList<TItem> list,
            int sourceIndex,
            TItem[] targetArray,
            int targetIndex,
            int count)
        {
            for (int i = sourceIndex; i < count; i++)
            {
                targetArray[targetIndex++] = list[i];
            }
        }

        /// <summary>
        /// Copies items from the specified list to the specified array.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="list">The read only list.</param>
        /// <param name="sourceIndex">Index of the source.</param>
        /// <param name="targetSpan">The target array.</param>
        /// <param name="targetIndex">Index of the target.</param>
        /// <param name="count">The count.</param>
        public static void CopyTo<TItem>(
            this IReadOnlyList<TItem> list,
            int sourceIndex,
            Span<TItem> targetSpan,
            int targetIndex,
            int count)
        {
            for (int i = sourceIndex; i < count; i++)
            {
                targetSpan[targetIndex++] = list[i];
            }
        }

        /// <summary>
        /// For loops the IList and executes action on all items.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="list">The target list.</param>
        /// <param name="action">The action.</param>
        public static void For<TItem>(this IReadOnlyList<TItem> list, Action<TItem> action)
        {
            for (var i = 0; i < list.Count; i++)
            {
                action(list[i]);
            }
        }

        /// <summary>
        /// For loops the IList and executes action on all items.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <typeparam name="TOutItem">The type of the out item.</typeparam>
        /// <param name="list">The target list.</param>
        /// <param name="getItemFunc">The get item function.</param>
        /// <param name="action">The action.</param>
        public static void For<TItem, TOutItem>(this IReadOnlyList<TItem> list, Func<TItem, TOutItem> getItemFunc, Action<TItem, TOutItem, int> action)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var item = list[i];
                action(item, getItemFunc(item), i);
            }
        }

        /// <summary>
        /// For loops the IList in reverse order and executes action on all item living up to the specified predicate.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="list">The target list.</param>
        /// <param name="action">The action.</param>
        public static void ForReverse<TItem>(this IReadOnlyList<TItem> list, Action<TItem> action)
        {
            for (var i = list.Count - 1; i >= 0; i--)
            {
                action(list[i]);
            }
        }

        /// <summary>
        /// For loops the IList and executes action on all items.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <typeparam name="TOutItem">The type of the out item.</typeparam>
        /// <param name="list">The target list.</param>
        /// <param name="getItemFunc">The get item function.</param>
        /// <param name="action">The action.</param>
        public static void ForReverse<TItem, TOutItem>(this IReadOnlyList<TItem> list, Func<TItem, TOutItem> getItemFunc, Action<TItem, TOutItem, int> action)
        {
            for (var i = list.Count - 1; i >= 0; i--)
            {
                var item = list[i];
                action(item, getItemFunc(item), i);
            }
        }

        /// <summary>
        /// Converts all items into a new array.
        /// </summary>
        /// <typeparam name="TInItem">The type of the input array items.</typeparam>
        /// <typeparam name="TOutItem">The type of the output array item.</typeparam>
        /// <param name="sourceArray">The source array.</param>
        /// <param name="converter">The converter.</param>
        /// <returns>The converted array.</returns>
        public static TOutItem[] ConvertAll<TInItem, TOutItem>(this IReadOnlyList<TInItem>? sourceArray, Func<TInItem, TOutItem> converter)
        {
            if (sourceArray == null)
            {
#if NETSTANDARD2_1
                return Array.Empty<TOutItem>();
#else
                return new TOutItem[0];
#endif
            }

            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            var result = new TOutItem[sourceArray.Count];
            for (var index = 0; index < sourceArray.Count; index++)
            {
                result[index] = converter(sourceArray[index]);
            }

            return result;
        }

        /// <summary>
        /// Determines whether this instance has any.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>
        ///   <c>true</c> if the specified source array has any; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAny<TItem>(this IReadOnlyCollection<TItem> list)
        {
            return list.Count > 0;
        }

        /// <summary>
        /// Determines whether the specified list is empty.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>
        ///   <c>true</c> if the specified list is empty; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty<TItem>(this IReadOnlyCollection<TItem> list)
        {
            return list.Count == 0;
        }
    }
}