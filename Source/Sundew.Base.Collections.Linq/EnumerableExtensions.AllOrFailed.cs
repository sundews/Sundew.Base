// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.AllOrFailed.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections.Linq;

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
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>A result of the ensured items or the error result.</returns>
    public static R<All<TItem>, Failed<R<TItem, TError>, TError>> AllOrFailed<TItem, TError>(this IEnumerable<R<TItem, TError>> enumerable)
    {
        return enumerable.AllOrFailed(Item.PassIfSuccess);
    }

    /// <summary>
    /// Ensures that all items could be selected and otherwise returns a error result.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>A result of the ensured items or the error result.</returns>
    public static R<All<TItem>, Failed<TItem?>> AllOrFailed<TItem>(this IEnumerable<TItem?> enumerable)
        where TItem : struct
    {
        return enumerable.AllOrFailed(Item.PassIfHasValue);
    }

    /// <summary>
    /// Ensures that all items could be selected and otherwise returns a error result.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>A result of the ensured items or the error result.</returns>
    public static R<All<TItem>, Failed<TItem?>> AllOrFailed<TItem>(this IEnumerable<TItem?> enumerable)
        where TItem : class
    {
        return enumerable.AllOrFailed(Item.PassIfHasValue);
    }

    /// <summary>
    /// Ensures that all items could be selected and otherwise returns a error result.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="selector">the selector.</param>
    /// <returns>A result of the ensured items or the error result.</returns>
    public static R<All<TResult>, Failed<TItem>> AllOrFailed<TItem, TResult>(this IEnumerable<TItem> enumerable, Func<TItem, Item<TResult>> selector)
    {
        static R<All<TResult>, Failed<TItem>> EnsureEnumerableWithCount(IEnumerable<TItem> enumerable, int count, Func<TItem, Item<TResult>> selector)
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
                return R.Error(new Failed<TItem>(failedItems.ToFinalArray()));
            }

            return R.From(true, new All<TResult>(result), new Failed<TItem>(failedItems.ToFinalArray()));
        }

        static R<All<TResult>, Failed<TItem>> EnsureEnumerable(IEnumerable<TItem> enumerable, Func<TItem, Item<TResult>> selector)
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
                return R.Error(new Failed<TItem>(failedIndices.ToFinalArray()));
            }

            return R.From(true, new All<TResult>(buffer.ToFinalArray()), new Failed<TItem>(failedIndices.ToFinalArray()));
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
    /// <returns>A result of the ensured items or the error result.</returns>
    public static R<All<TResult>, Failed<TItem, TError>> AllOrFailed<TItem, TResult, TError>(
        this IEnumerable<TItem> enumerable, Func<TItem, Item<TResult, TError>> selector)
    {
        static R<All<TResult>, Failed<TItem, TError>> EnsureEnumerableWithCount(IEnumerable<TItem> enumerable, int count, Func<TItem, Item<TResult, TError>> selector)
        {
            var result = new TResult[count];
            var failedItems = new Buffer<FailedItem<TItem, TError>>();
            var index = 0;
            var success = 0;
            foreach (var item in enumerable)
            {
                var itemResult = selector(item);
                if (itemResult.IsValid)
                {
                    result[index] = itemResult.SelectedItem;
                    success++;
                }

                if (itemResult.HasError)
                {
                    failedItems.Write(new FailedItem<TItem, TError>(index, item, itemResult.ErrorItem));
                }

                index += 1;
            }

            if (success == index)
            {
                return R.From(true, new All<TResult>(result), new Failed<TItem, TError>(failedItems.ToFinalArray()));
            }

            return R.Error(new Failed<TItem, TError>(failedItems.ToFinalArray()));
        }

        static R<All<TResult>, Failed<TItem, TError>> EnsureEnumerable(IEnumerable<TItem> enumerable, Func<TItem, Item<TResult, TError>> selector)
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

                if (itemResult.HasError)
                {
                    failedItems.Write(new FailedItem<TItem, TError>(index, item, itemResult.ErrorItem));
                }

                index += 1;
            }

            if (buffer.Length == index)
            {
                return R.From(true, new All<TResult>(buffer.ToFinalArray()), new Failed<TItem, TError>(failedItems.ToFinalArray()));
            }

            return R.Error(new Failed<TItem, TError>(failedItems.ToFinalArray()));
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