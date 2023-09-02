// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.AllOrFailed.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections;

using System;
using System.Collections;
using System.Collections.Generic;
using Sundew.Base.Memory;

/// <summary>
/// Extends arrays with easy to use methods.
/// </summary>
public static partial class EnumerableExtensions
{
    /// <summary>
    /// Ensures that all items could be selected and otherwise returns a error result.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>A discriminated union of the ensured items or the error result.</returns>
    public static AllOrFailed<TItem?, TItem> AllOrFailed<TItem>(this IEnumerable<TItem?> enumerable)
        where TItem : struct
    {
        return enumerable.AllOrFailed(Item.PassIfNotNull);
    }

    /// <summary>
    /// Ensures that all items could be selected and otherwise returns a error result.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>A discriminated union of the ensured items or the error result.</returns>
    public static AllOrFailed<TItem?, TItem> AllOrFailed<TItem>(this IEnumerable<TItem?> enumerable)
        where TItem : class
    {
        return enumerable.AllOrFailed(Item.PassIfNotNull);
    }

    /// <summary>
    /// Ensures that all items could be selected and otherwise returns a error result.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="selector">the selector.</param>
    /// <returns>A discriminated union of the ensured items or the error result.</returns>
    public static AllOrFailed<TItem, TResult> AllOrFailed<TItem, TResult>(this IEnumerable<TItem> enumerable, Func<TItem, Item<TResult>> selector)
    {
        static AllOrFailed<TItem, TResult> EnsureEnumerableWithCount(IEnumerable<TItem> enumerable, int count, Func<TItem, Item<TResult>> selector)
        {
            var result = new TResult[count];
            var failedItems = new Buffer<FailedItem<TItem>>();
            var index = 0;
            foreach (var item in enumerable)
            {
                var itemResult = selector(item);
                if (itemResult.IsValid)
                {
                    result[index] = itemResult.SelectedItem;
                }
                else
                {
                    failedItems.Write(new FailedItem<TItem>(index, item));
                }

                index += 1;
            }

            if (failedItems.Length > 0)
            {
                return Collections.AllOrFailed<TItem, TResult>.Failed(failedItems.ToFinalArray());
            }

            return Collections.AllOrFailed<TItem, TResult>.All(result);
        }

        static AllOrFailed<TItem, TResult> EnsureEnumerable(IEnumerable<TItem> enumerable, Func<TItem, Item<TResult>> selector)
        {
            var buffer = new Buffer<TResult>();
            var failedIndices = new Buffer<FailedItem<TItem>>();
            var index = 0;
            foreach (var item in enumerable)
            {
                var itemResult = selector(item);
                if (itemResult.IsValid)
                {
                    buffer.Write(itemResult.SelectedItem);
                }
                else
                {
                    failedIndices.Write(new FailedItem<TItem>(index, item));
                }

                index += 1;
            }

            if (failedIndices.Length > 0)
            {
                return Collections.AllOrFailed<TItem, TResult>.Failed(failedIndices.ToFinalArray());
            }

            return Collections.AllOrFailed<TItem, TResult>.All(buffer.ToFinalArray());
        }

        return enumerable switch
        {
            IReadOnlyCollection<TItem> readOnlyCollection => EnsureEnumerableWithCount(readOnlyCollection, readOnlyCollection.Count, selector),
            ICollection<TItem> collectionOfT => EnsureEnumerableWithCount(collectionOfT, collectionOfT.Count, selector),
            ICollection collection => EnsureEnumerableWithCount(enumerable, collection.Count, selector),
            _ => EnsureEnumerable(enumerable, selector),
        };
    }

    /// <summary>
    /// Ensures that all items could be selected and otherwise returns a error result.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TError">The failed item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="selector">the selector.</param>
    /// <returns>A discriminated union of the ensured items or the error result.</returns>
    public static AllOrFailed<TItem, TResult, TError> AllOrFailed<TItem, TResult, TError>(
        this IEnumerable<TItem> enumerable, Func<TItem, Item<TResult, TError>> selector)
    {
        static AllOrFailed<TItem, TResult, TError> EnsureEnumerableWithCount(IEnumerable<TItem> enumerable, int count, Func<TItem, Item<TResult, TError>> selector)
        {
            var result = new TResult[count];
            var failedItems = new Buffer<FailedItem<TItem, TError>>();
            var index = 0;
            foreach (var item in enumerable)
            {
                var itemResult = selector(item);
                if (itemResult.IsValid)
                {
                    result[index] = itemResult.SelectedItem;
                }
                else
                {
                    failedItems.Write(new FailedItem<TItem, TError>(index, item, itemResult.ErrorItem));
                }

                index += 1;
            }

            if (failedItems.Length > 0)
            {
                return Collections.AllOrFailed<TItem, TResult, TError>.Failed(failedItems.ToFinalArray());
            }

            return Collections.AllOrFailed<TItem, TResult, TError>.All(result);
        }

        static AllOrFailed<TItem, TResult, TError> EnsureEnumerable(IEnumerable<TItem> enumerable, Func<TItem, Item<TResult, TError>> selector)
        {
            var buffer = new Buffer<TResult>();
            var failedItems = new Buffer<FailedItem<TItem, TError>>();
            var index = 0;
            foreach (var item in enumerable)
            {
                var itemResult = selector(item);
                if (itemResult.IsValid)
                {
                    buffer.Write(itemResult.SelectedItem);
                }
                else
                {
                    failedItems.Write(new FailedItem<TItem, TError>(index, item, itemResult.ErrorItem));
                }

                index += 1;
            }

            if (failedItems.Length > 0)
            {
                return Collections.AllOrFailed<TItem, TResult, TError>.Failed(failedItems.ToFinalArray());
            }

            return Collections.AllOrFailed<TItem, TResult, TError>.All(buffer.ToFinalArray());
        }

        return enumerable switch
        {
            IReadOnlyCollection<TItem> readOnlyCollection => EnsureEnumerableWithCount(readOnlyCollection, readOnlyCollection.Count, selector),
            ICollection<TItem> collectionOfT => EnsureEnumerableWithCount(collectionOfT, collectionOfT.Count, selector),
            ICollection collection => EnsureEnumerableWithCount(enumerable, collection.Count, selector),
            _ => EnsureEnumerable(enumerable, selector),
        };
    }
}