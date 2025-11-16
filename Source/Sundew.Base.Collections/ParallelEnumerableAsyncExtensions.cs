// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParallelEnumerableAsyncExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Sundew.Base.Collections.Concurrent;
using Sundew.Base.Numeric;

/// <summary>
/// Extends <see cref="IEnumerable{T}"/> with async methods.
/// </summary>
public static class ParallelEnumerableAsyncExtensions
{
    /// <summary>
    /// Runs an asynchronous for each.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="parallelism">The parallelism.</param>
    /// <param name="action">The action.</param>
    /// <returns>The completion task.</returns>
    [OverloadResolutionPriority(0)]
    public static Task ForEachAsync<TItem>(
        this IEnumerable<TItem> enumerable,
        Parallelism parallelism,
        Func<TItem, Task> action)
    {
        return enumerable.SelectAsync(
            parallelism,
            async (item, _, _) =>
            {
                await action(item).ConfigureAwait(false);
                return item;
            });
    }

    /// <summary>
    /// Runs an asynchronous for each.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="parallelism">The parallelism.</param>
    /// <param name="action">The action.</param>
    /// <returns>The completion task.</returns>
    [OverloadResolutionPriority(1)]
    public static Task ForEachAsync<TItem>(
        this IEnumerable<TItem> enumerable,
        Parallelism parallelism,
        Func<TItem, ValueTask> action)
    {
        return enumerable.SelectAsync(
            parallelism,
            async (item, _, _) =>
            {
                await action(item).ConfigureAwait(false);
                return item;
            });
    }

    /// <summary>
    /// Runs an asynchronous for each.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="parallelism">The parallelism.</param>
    /// <param name="action">The action.</param>
    /// <returns>The completion task.</returns>
    [OverloadResolutionPriority(0)]
    public static Task ForEachAsync<TItem>(
        this IEnumerable<TItem> enumerable,
        Parallelism parallelism,
        Func<TItem, CancellationToken, Task> action)
    {
        return enumerable.SelectAsync(
            parallelism,
            async (item, _, cancellationToken) =>
            {
                await action(item, cancellationToken).ConfigureAwait(false);
                return item;
            });
    }

    /// <summary>
    /// Runs an asynchronous for each.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="parallelism">The parallelism.</param>
    /// <param name="action">The action.</param>
    /// <returns>The completion task.</returns>
    [OverloadResolutionPriority(1)]
    public static Task ForEachAsync<TItem>(
        this IEnumerable<TItem> enumerable,
        Parallelism parallelism,
        Func<TItem, CancellationToken, ValueTask> action)
    {
        return enumerable.SelectAsync(
            parallelism,
            async (item, _, cancellationToken) =>
            {
                await action(item, cancellationToken).ConfigureAwait(false);
                return item;
            });
    }

    /// <summary>
    /// Runs an asynchronous for each.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="parallelism">The parallelism.</param>
    /// <param name="action">The action.</param>
    /// <returns>The completion task.</returns>
    [OverloadResolutionPriority(0)]
    public static Task ForEachAsync<TItem>(
        this IEnumerable<TItem> enumerable,
        Parallelism parallelism,
        Func<TItem, int, CancellationToken, Task> action)
    {
        return enumerable.SelectAsync(
            parallelism,
            async (item, index, cancellationToken) =>
            {
                await action(item, index, cancellationToken).ConfigureAwait(false);
                return item;
            });
    }

    /// <summary>
    /// Runs an asynchronous for each.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="parallelism">The parallelism.</param>
    /// <param name="action">The action.</param>
    /// <returns>The completion task.</returns>
    [OverloadResolutionPriority(1)]
    public static Task ForEachAsync<TItem>(
        this IEnumerable<TItem> enumerable,
        Parallelism parallelism,
        Func<TItem, int, CancellationToken, ValueTask> action)
    {
        return enumerable.SelectAsync(
            parallelism,
            async (item, index, cancellationToken) =>
            {
                await action(item, index, cancellationToken).ConfigureAwait(false);
                return item;
            });
    }

    /// <summary>
    /// Runs an asynchronous for each.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="parallelism">The parallelism.</param>
    /// <param name="action">The action.</param>
    /// <returns>The completion task.</returns>
    [OverloadResolutionPriority(0)]
    public static Task<IReadOnlyList<TItem>> ForEachItemAsync<TItem>(
        this IEnumerable<TItem> enumerable,
        Parallelism parallelism,
        Func<TItem, Task> action)
    {
        return enumerable.SelectAsync(
            parallelism,
            async (item, _, _) =>
            {
                await action(item).ConfigureAwait(false);
                return item;
            });
    }

    /// <summary>
    /// Runs an asynchronous for each.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="parallelism">The parallelism.</param>
    /// <param name="action">The action.</param>
    /// <returns>The completion task.</returns>
    [OverloadResolutionPriority(1)]
    public static Task<IReadOnlyList<TItem>> ForEachItemAsync<TItem>(
        this IEnumerable<TItem> enumerable,
        Parallelism parallelism,
        Func<TItem, ValueTask> action)
    {
        return enumerable.SelectAsync(
            parallelism,
            async (item, _, _) =>
            {
                await action(item).ConfigureAwait(false);
                return item;
            });
    }

    /// <summary>
    /// Runs an asynchronous for each.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="parallelism">The parallelism.</param>
    /// <param name="action">The action.</param>
    /// <returns>The completion task.</returns>
    [OverloadResolutionPriority(0)]
    public static Task<IReadOnlyList<TItem>> ForEachItemAsync<TItem>(
        this IEnumerable<TItem> enumerable,
        Parallelism parallelism,
        Func<TItem, CancellationToken, Task> action)
    {
        return enumerable.SelectAsync(
            parallelism,
            async (item, _, cancellationToken) =>
            {
                await action(item, cancellationToken).ConfigureAwait(false);
                return item;
            });
    }

    /// <summary>
    /// Runs an asynchronous for each.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="parallelism">The parallelism.</param>
    /// <param name="action">The action.</param>
    /// <returns>The completion task.</returns>
    [OverloadResolutionPriority(1)]
    public static Task<IReadOnlyList<TItem>> ForEachItemAsync<TItem>(
        this IEnumerable<TItem> enumerable,
        Parallelism parallelism,
        Func<TItem, CancellationToken, ValueTask> action)
    {
        return enumerable.SelectAsync(
            parallelism,
            async (item, _, cancellationToken) =>
            {
                await action(item, cancellationToken).ConfigureAwait(false);
                return item;
            });
    }

    /// <summary>
    /// Runs an asynchronous for each.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="parallelism">The parallelism.</param>
    /// <param name="action">The action.</param>
    /// <returns>The completion task.</returns>
    [OverloadResolutionPriority(0)]
    public static Task<IReadOnlyList<TItem>> ForEachItemAsync<TItem>(
        this IEnumerable<TItem> enumerable,
        Parallelism parallelism,
        Func<TItem, int, CancellationToken, Task> action)
    {
        return enumerable.SelectAsync(
            parallelism,
            async (item, index, cancellationToken) =>
            {
                await action(item, index, cancellationToken).ConfigureAwait(false);
                return item;
            });
    }

    /// <summary>
    /// Runs an asynchronous for each.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="parallelism">The parallelism.</param>
    /// <param name="action">The action.</param>
    /// <returns>The completion task.</returns>
    [OverloadResolutionPriority(1)]
    public static Task<IReadOnlyList<TItem>> ForEachItemAsync<TItem>(
        this IEnumerable<TItem> enumerable,
        Parallelism parallelism,
        Func<TItem, int, CancellationToken, ValueTask> action)
    {
        return enumerable.SelectAsync(
            parallelism,
            async (item, index, cancellationToken) =>
            {
                await action(item, index, cancellationToken).ConfigureAwait(false);
                return item;
            });
    }

    /// <summary>
    /// Processes the source in parallel.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TOutItem">The out item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="parallelism">The parallelism.</param>
    /// <param name="func">The func.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    [OverloadResolutionPriority(0)]
    public static Task<IReadOnlyList<TOutItem>> SelectAsync<TItem, TOutItem>(
        this IEnumerable<TItem> enumerable,
        Parallelism parallelism,
        Func<TItem, Task<TOutItem>> func)
    {
        return enumerable.SelectAsync(
            parallelism,
            (item, _, _) => func(item));
    }

    /// <summary>
    /// Processes the source in parallel.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TOutItem">The out item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="parallelism">The parallel options.</param>
    /// <param name="func">The func.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    [OverloadResolutionPriority(1)]
    public static Task<IReadOnlyList<TOutItem>> SelectAsync<TItem, TOutItem>(
        this IEnumerable<TItem> enumerable,
        Parallelism parallelism,
        Func<TItem, ValueTask<TOutItem>> func)
    {
        return enumerable.SelectAsync(
            parallelism,
            (item, _, _) => func(item));
    }

    /// <summary>
    /// Processes the source in parallel.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TOutItem">The out item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="parallelism">The parallelism.</param>
    /// <param name="func">The func.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    [OverloadResolutionPriority(0)]
    public static Task<IReadOnlyList<TOutItem>> SelectAsync<TItem, TOutItem>(
        this IEnumerable<TItem> enumerable,
        Parallelism parallelism,
        Func<TItem, CancellationToken, Task<TOutItem>> func)
    {
        return enumerable.SelectAsync(
            parallelism,
            (item, _, cancellationToken) => func(item, cancellationToken));
    }

    /// <summary>
    /// Processes the source in parallel.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TOutItem">The out item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="parallelism">The parallel options.</param>
    /// <param name="func">The func.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    [OverloadResolutionPriority(1)]
    public static Task<IReadOnlyList<TOutItem>> SelectAsync<TItem, TOutItem>(
        this IEnumerable<TItem> enumerable,
        Parallelism parallelism,
        Func<TItem, CancellationToken, ValueTask<TOutItem>> func)
    {
        return enumerable.SelectAsync(
            parallelism,
            (item, _, cancellationToken) => func(item, cancellationToken));
    }

    /// <summary>
    /// Processes the source in parallel.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TOutItem">The out item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="parallelism">The parallelism.</param>
    /// <param name="func">The func.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    [OverloadResolutionPriority(0)]
    public static Task<IReadOnlyList<TOutItem>> SelectAsync<TItem, TOutItem>(
        this IEnumerable<TItem> enumerable,
        Parallelism parallelism,
        Func<TItem, int, CancellationToken, Task<TOutItem>> func)
    {
        return enumerable.SelectAsync(
            parallelism,
            async (item, index, cancellationToken) => await func(item, index, cancellationToken).ConfigureAwait(false));
    }

    /// <summary>
    /// Processes the source in parallel.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TOutItem">The out item type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="parallelism">The parallelism.</param>
    /// <param name="func">The func.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    [OverloadResolutionPriority(1)]
    public static Task<IReadOnlyList<TOutItem>> SelectAsync<TItem, TOutItem>(
        this IEnumerable<TItem> enumerable,
        Parallelism parallelism,
        Func<TItem, int, CancellationToken, ValueTask<TOutItem>> func)
    {
        var maxDegreeOfParallelism = Interval.From(1, Environment.ProcessorCount).Limit(parallelism.MaxDegreeOfParallelism);
        var cancellationToken = parallelism.Cancellation.Token;
        var scheduler = parallelism.TaskScheduler ?? TaskScheduler.Current;
        var enumerator = enumerable.GetEnumerator();
        var enabler = parallelism.Cancellation.EnableCancellation();
        var semaphore = new SemaphoreSlim(1, 1);
        var workerTasks = new Task[maxDegreeOfParallelism];
        var concurrentList = new ConcurrentList<TOutItem>();
        var index = 0;
        for (var i = 0; i < maxDegreeOfParallelism; i++)
        {
            workerTasks[i] = Task.Factory.StartNew(
                    async () =>
                    {
                        (TItem? Item, int Index) itemPair = (default, 0);
                        try
                        {
                            while (enabler.ContinueOrThrowIfCancellationRequested())
                            {
                                await semaphore.WaitAsync().ConfigureAwait(true);
                                try
                                {
                                    if (!enumerator.MoveNext())
                                    {
                                        break;
                                    }

                                    itemPair = (enumerator.Current, index++);
                                }
                                finally
                                {
                                    semaphore.Release();
                                }

                                concurrentList[itemPair.Index] = await func(itemPair.Item, itemPair.Index, enabler.Token).ConfigureAwait(true);
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            enabler.Cancel();
                            throw;
                        }
                        catch (Exception)
                        {
                            enabler.Cancel();
                            throw;
                        }
                    },
                    CancellationToken.None,
                    TaskCreationOptions.DenyChildAttach,
                    scheduler)
                .Unwrap();
        }

        return Task.WhenAll(workerTasks).ContinueWith(
            _ =>
            {
                using (enumerator)
                using (enabler)
                using (semaphore)
                {
                }

                return Task.FromResult((IReadOnlyList<TOutItem>)concurrentList);
            },
            CancellationToken.None,
            TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default).Unwrap();
    }
}