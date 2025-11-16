// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableAsyncExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

/// <summary>
/// Extends <see cref="IEnumerable{T}"/> with async methods.
/// </summary>
public static class EnumerableAsyncExtensions
{
    /// <summary>
    /// Runs an asynchronous for each.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="action">The action.</param>
    /// <returns>The completion task.</returns>
    [OverloadResolutionPriority(0)]
    public static async Task ForEachAsync<TItem>(this IEnumerable<TItem> enumerable, Func<TItem, Task> action)
    {
        foreach (var item in enumerable)
        {
            await action(item).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Runs an asynchronous for each.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="action">The action.</param>
    /// <returns>The completion task.</returns>
    [OverloadResolutionPriority(1)]
    public static async Task ForEachAsync<TItem>(this IEnumerable<TItem> enumerable, Func<TItem, ValueTask> action)
    {
        foreach (var item in enumerable)
        {
            await action(item).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Runs an asynchronous for each.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="action">The action.</param>
    /// <returns>The completion task.</returns>
    [OverloadResolutionPriority(0)]
    public static async Task ForEachAsync<TItem>(this IEnumerable<TItem> enumerable, Func<TItem, int, Task> action)
    {
        var index = 0;
        foreach (var item in enumerable)
        {
            await action(item, index++).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Runs an asynchronous for each.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="action">The action.</param>
    /// <returns>The completion task.</returns>
    [OverloadResolutionPriority(1)]
    public static async Task ForEachAsync<TItem>(this IEnumerable<TItem> enumerable, Func<TItem, int, ValueTask> action)
    {
        var index = 0;
        foreach (var item in enumerable)
        {
            await action(item, index++).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Runs an asynchronous for each.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="action">The action.</param>
    /// <returns>The completion task.</returns>
    [OverloadResolutionPriority(0)]
    public static async Task<IReadOnlyList<TItem>> ForEachItemAsync<TItem>(this IEnumerable<TItem> enumerable, Func<TItem, Task> action)
    {
        var items = new List<TItem>();
        foreach (var inItem in enumerable)
        {
            await action(inItem).ConfigureAwait(false);
            items.Add(inItem);
        }

        return items;
    }

    /// <summary>
    /// Runs an asynchronous for each.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="action">The action.</param>
    /// <returns>The completion task.</returns>
    [OverloadResolutionPriority(1)]
    public static async Task<IReadOnlyList<TItem>> ForEachItemAsync<TItem>(this IEnumerable<TItem> enumerable, Func<TItem, ValueTask> action)
    {
        var items = new List<TItem>();
        foreach (var inItem in enumerable)
        {
            await action(inItem).ConfigureAwait(false);
            items.Add(inItem);
        }

        return items;
    }

    /// <summary>
    /// Runs an asynchronous for each.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="action">The action.</param>
    /// <returns>The completion task.</returns>
    [OverloadResolutionPriority(0)]
    public static async Task<IReadOnlyList<TItem>> ForEachItemAsync<TItem>(this IEnumerable<TItem> enumerable, Func<TItem, int, Task> action)
    {
        var items = new List<TItem>();
        var index = 0;
        foreach (var item in enumerable)
        {
            await action(item, index).ConfigureAwait(false);
            items.Add(item);
        }

        return items;
    }

    /// <summary>
    /// Runs an asynchronous for each.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="action">The action.</param>
    /// <returns>The completion task.</returns>
    [OverloadResolutionPriority(1)]
    public static async Task<IReadOnlyList<TItem>> ForEachItemAsync<TItem>(this IEnumerable<TItem> enumerable, Func<TItem, int, ValueTask> action)
    {
        var items = new List<TItem>();
        var index = 0;
        foreach (var item in enumerable)
        {
            await action(item, index++).ConfigureAwait(false);
            items.Add(item);
        }

        return items;
    }

    /// <summary>
    /// Performs a select asynchronously.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <typeparam name="TOutItem">The type of the out item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="selectFunc">The select function.</param>
    /// <returns>The selected items.</returns>
    [OverloadResolutionPriority(0)]
    public static async Task<IReadOnlyList<TOutItem>> SelectAsync<TItem, TOutItem>(
        this IEnumerable<TItem> enumerable,
        Func<TItem, Task<TOutItem>> selectFunc)
    {
        var items = new List<TOutItem>();
        foreach (var item in enumerable)
        {
            items.Add(await selectFunc(item).ConfigureAwait(false));
        }

        return items;
    }

    /// <summary>
    /// Performs a select asynchronously.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <typeparam name="TOutItem">The type of the out item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="selectFunc">The select function.</param>
    /// <returns>The selected items.</returns>
    [OverloadResolutionPriority(1)]
    public static async Task<IReadOnlyList<TOutItem>> SelectAsync<TItem, TOutItem>(
        this IEnumerable<TItem> enumerable,
        Func<TItem, ValueTask<TOutItem>> selectFunc)
    {
        var items = new List<TOutItem>();
        foreach (var item in enumerable)
        {
            items.Add(await selectFunc(item).ConfigureAwait(false));
        }

        return items;
    }

    /// <summary>
    /// Performs a select asynchronously.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <typeparam name="TOutItem">The type of the out item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="selectFunc">The select function.</param>
    /// <returns>The selected items.</returns>
    [OverloadResolutionPriority(0)]
    public static async Task<IReadOnlyList<TOutItem>> SelectAsync<TItem, TOutItem>(
        this IEnumerable<TItem> enumerable,
        Func<TItem, int, Task<TOutItem>> selectFunc)
    {
        var items = new List<TOutItem>();
        var index = 0;
        foreach (var item in enumerable)
        {
            items.Add(await selectFunc(item, index++).ConfigureAwait(false));
        }

        return items;
    }

    /// <summary>
    /// Performs a select asynchronously.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <typeparam name="TOutItem">The type of the out item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="selectFunc">The select function.</param>
    /// <returns>The selected items.</returns>
    [OverloadResolutionPriority(1)]
    public static async Task<IReadOnlyList<TOutItem>> SelectAsync<TItem, TOutItem>(
        this IEnumerable<TItem> enumerable,
        Func<TItem, int, ValueTask<TOutItem>> selectFunc)
    {
        var items = new List<TOutItem>();
        var index = 0;
        foreach (var item in enumerable)
        {
            items.Add(await selectFunc(item, index++).ConfigureAwait(false));
        }

        return items;
    }
}