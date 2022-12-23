// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.SelectAll.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
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
    public static SelectAllResult<TItem> SelectAll<TItem>(this IEnumerable<TItem?> enumerable)
        where TItem : struct
    {
        return enumerable.SelectAll(SelectItem<TItem>.From);
    }

    /// <summary>
    /// Ensures that all items could be selected and otherwise returns a error result.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>A discriminated union of the ensured items or the error result.</returns>
    public static SelectAllResult<TItem> SelectAll<TItem>(this IEnumerable<TItem?> enumerable)
        where TItem : class
    {
        return enumerable.SelectAll(SelectItem<TItem>.From);
    }

    /// <summary>
    /// Ensures that all items could be selected and otherwise returns a error result.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="selector">the selector.</param>
    /// <returns>A discriminated union of the ensured items or the error result.</returns>
    public static SelectAllResult<TResult, TError> SelectAll<TItem, TError, TResult>(this IEnumerable<TItem> enumerable, Func<TItem, SelectItem<TResult, TError>> selector)
    {
        static SelectAllResult<TResult, TError> EnsureEnumerableWithCount(IEnumerable<TItem> enumerable, int count, Func<TItem, SelectItem<TResult, TError>> selector)
        {
            var result = new TResult[count];
            var index = 0;
            foreach (var item in enumerable)
            {
                var itemResult = selector(item);
                if (itemResult.IsValid)
                {
                    result[index++] = itemResult.SuccessItem;
                }
                else
                {
                    return All<TResult, TError>.None(itemResult.ErrorItem, index);
                }
            }

            return All<TResult, TError>.All(result);
        }

        static SelectAllResult<TResult, TError> EnsureEnumerable(IEnumerable<TItem> enumerable, Func<TItem, SelectItem<TResult, TError>> selector)
        {
            var buffer = new Buffer<TResult>();
            foreach (var item in enumerable)
            {
                var itemResult = selector(item);
                if (itemResult.IsValid)
                {
                    buffer.Write(itemResult.SuccessItem);
                }
                else
                {
                    return All<TResult, TError>.None(itemResult.ErrorItem, buffer.Position);
                }
            }

            return All<TResult, TError>.All(buffer.ToFinalArray());
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
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="selector">the selector.</param>
    /// <returns>A discriminated union of the ensured items or the error result.</returns>
    public static SelectAllResult<TResult> SelectAll<TItem, TResult>(this IEnumerable<TItem> enumerable, Func<TItem, SelectItem<TResult>> selector)
    {
        static SelectAllResult<TResult> EnsureEnumerableWithCount(IEnumerable<TItem> enumerable, int count, Func<TItem, SelectItem<TResult>> selector)
        {
            var result = new TResult[count];
            var index = 0;
            foreach (var item in enumerable)
            {
                var itemResult = selector(item);
                if (itemResult.IsValid)
                {
                    result[index++] = itemResult.SuccessItem;
                }
                else
                {
                    return All<TResult>.None(index);
                }
            }

            return All<TResult>.All(result);
        }

        static SelectAllResult<TResult> EnsureEnumerable(IEnumerable<TItem> enumerable, Func<TItem, SelectItem<TResult>> selector)
        {
            var buffer = new Buffer<TResult>();
            foreach (var item in enumerable)
            {
                var itemResult = selector(item);
                if (itemResult.IsValid)
                {
                    buffer.Write(itemResult.SuccessItem);
                }
                else
                {
                    return All<TResult>.None(buffer.Position);
                }
            }

            return All<TResult>.All(buffer.ToFinalArray());
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