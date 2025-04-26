// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncEnumerableExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections;

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sundew.Base.Memory;

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

    /// <summary>
    /// Runs an asynchronous for each.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="action">The action.</param>
    /// <returns>The completion task.</returns>
    public static Task ForEachAsync<TItem>(this IEnumerable<TItem> enumerable, Func<TItem, ValueTask> action)
    {
        return Task.WhenAll(enumerable.Select(async x => await action(x).ConfigureAwait(false)));
    }

    /// <summary>
    /// Runs an asynchronous for each.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="action">The action.</param>
    /// <returns>The completion task.</returns>
    public static async Task<IReadOnlyList<TItem>> ForEachItemAsync<TItem>(this IEnumerable<TItem> enumerable, Func<TItem, Task> action)
    {
        return await Task.WhenAll(enumerable.Select(async x =>
        {
            await action(x).ConfigureAwait(false);
            return x;
        }));
    }

    /// <summary>
    /// Runs an asynchronous for each.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="action">The action.</param>
    /// <returns>The completion task.</returns>
    public static async Task<IReadOnlyList<TItem>> ForEachItemAsync<TItem>(this IEnumerable<TItem> enumerable, Func<TItem, ValueTask> action)
    {
        return await Task.WhenAll(enumerable.Select(async x =>
        {
            await action(x).ConfigureAwait(false);
            return x;
        }));
    }

    /// <summary>
    /// Asynchronously waits for the first item fulfilling the predicate or until timeout occurs.
    /// </summary>
    /// <typeparam name="TItem">The TItem type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>An async result contain the expected items or an error in case of a timeout containing the items that could be found.</returns>
    public static async Task<R<TItem>> FirstAsync<TItem>(this IEnumerable<TItem> enumerable, Func<TItem, bool> predicate, TimeSpan timeout = default)
    {
        return (await TakeIfAsync(enumerable, (item, _) => predicate.Invoke(item), 1, timeout).ConfigureAwait(false)).Map(x => x[0]);
    }

    /// <summary>
    /// Asynchronously waits for the first item fulfilling the predicate or until timeout occurs.
    /// </summary>
    /// <typeparam name="TItem">The TItem type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>An async result contain the expected items or an error in case of a timeout containing the items that could be found.</returns>
    public static async Task<R<TItem>> FirstAsync<TItem>(this IEnumerable<TItem> enumerable, Func<TItem, int, bool> predicate, TimeSpan timeout = default)
    {
        return (await TakeIfAsync(enumerable, predicate, 1, timeout).ConfigureAwait(false)).Map(x => x[0]);
    }

    /// <summary>
    /// Asynchronously waits for the first item or until timeout occurs.
    /// </summary>
    /// <typeparam name="TItem">The TItem type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>An async result contain the expected items or an error in case of a timeout containing the items that could be found.</returns>
    public static async Task<R<TItem>> FirstAsync<TItem>(this IEnumerable<TItem> enumerable, TimeSpan timeout = default)
    {
        return (await TakeIfAsync(enumerable, default(Func<TItem, int, bool>), 1, timeout).ConfigureAwait(false)).Map(x => x[0]);
    }

    /// <summary>
    /// Asynchronously waits for the second item or until timeout occurs.
    /// </summary>
    /// <typeparam name="TItem">The TItem type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>An async result contain the expected items or an error in case of a timeout containing the items that could be found.</returns>
    public static async Task<R<TItem>> SecondAsync<TItem>(this IEnumerable<TItem> enumerable, TimeSpan timeout = default)
    {
        return (await TakeIfAsync(enumerable, default(Func<TItem, int, bool>), 2, timeout).ConfigureAwait(false)).Map(x => x[1]);
    }

    /// <summary>
    /// Asynchronously waits for the second item fulfilling the predicate or until timeout occurs.
    /// </summary>
    /// <typeparam name="TItem">The TItem type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>An async result contain the expected items or an error in case of a timeout containing the items that could be found.</returns>
    public static async Task<R<TItem>> SecondAsync<TItem>(this IEnumerable<TItem> enumerable, Func<TItem, bool> predicate, TimeSpan timeout = default)
    {
        return (await TakeIfAsync(enumerable, (item, _) => predicate.Invoke(item), 2, timeout).ConfigureAwait(false)).Map(x => x[1]);
    }

    /// <summary>
    /// Asynchronously waits for the second item fulfilling the predicate or until timeout occurs.
    /// </summary>
    /// <typeparam name="TItem">The TItem type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>An async result contain the expected items or an error in case of a timeout containing the items that could be found.</returns>
    public static async Task<R<TItem>> SecondAsync<TItem>(this IEnumerable<TItem> enumerable, Func<TItem, int, bool>? predicate, TimeSpan timeout = default)
    {
        return (await TakeIfAsync(
            enumerable,
            predicate,
            2,
            timeout).ConfigureAwait(false)).Map(x => x[1]);
    }

    /// <summary>
    /// Asynchronously waits for a number of items or until a timeout occurs.
    /// </summary>
    /// <typeparam name="TItem">The TItem type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="count">The count.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>An async result contain the expected items or an error in case of a timeout containing the items that could be found.</returns>
    public static Task<R<TItem[], TItem[]>> TakeIfAsync<TItem>(this IEnumerable<TItem> enumerable, Func<TItem, bool> predicate, int count, TimeSpan timeout = default)
    {
        return TakeIfAsync(
            enumerable,
            (item, _) => predicate?.Invoke(item) ?? true,
            count,
            timeout);
    }

    /// <summary>
    /// Asynchronously waits for a number of items or until a timeout occurs.
    /// </summary>
    /// <typeparam name="TItem">The TItem type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="count">The count.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>An async result contain the expected items or an error in case of a timeout containing the items that could be found.</returns>
    public static Task<R<TItem[], TItem[]>> TakeIfAsync<TItem>(this IEnumerable<TItem> enumerable, Func<TItem, int, bool>? predicate, int count, TimeSpan timeout = default)
    {
        var numberOfItems = 0;
        return TakeIfAsync(
            enumerable,
            (item, index) =>
            {
                if (predicate?.Invoke(item, index) ?? true)
                {
                    return Interlocked.Increment(ref numberOfItems) < count ? TakeAction.Take : TakeAction.TakeAndEnd;
                }

                return TakeAction.Skip;
            },
            timeout);
    }

    /// <summary>
    /// Asynchronously waits for a number of items or until a timeout occurs.
    /// </summary>
    /// <typeparam name="TItem">The TItem type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="count">The count.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>An async result contain the expected items or an error in case of a timeout containing the items that could be found.</returns>
    public static Task<R<TItem[], TItem[]>> TakeAsync<TItem>(this IEnumerable<TItem> enumerable, int count, TimeSpan timeout = default)
    {
        var numberOfItems = 0;
        return TakeIfAsync(
            enumerable,
            (item, index) => Interlocked.Increment(ref numberOfItems) < count ? TakeAction.Take : TakeAction.TakeAndEnd,
            timeout);
    }

    /// <summary>
    /// Asynchronously waits for items fulfilling the condition until timeout occurs.
    /// </summary>
    /// <typeparam name="TItem">The TItem type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>An async result contain the expected items or an error in case of a timeout containing the items that could be found.</returns>
    public static Task<R<TItem[], TItem[]>> TakeIfAsync<TItem>(this IEnumerable<TItem> enumerable, Func<TItem, TakeAction> predicate, TimeSpan timeout = default)
    {
        return TakeIfAsync(enumerable, (item, _) => predicate.Invoke(item), timeout);
    }

    /// <summary>
    /// Asynchronously waits for items fulfilling the predicate or the timeout occurs.
    /// </summary>
    /// <typeparam name="TItem">The TItem type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>An async result contain the expected items or an error in case of a timeout containing the items that could be found.</returns>
    public static async Task<R<TItem[], TItem[]>> TakeIfAsync<TItem>(this IEnumerable<TItem> enumerable, Func<TItem, int, TakeAction> predicate, TimeSpan timeout = default)
    {
        timeout = timeout == TimeSpan.Zero ? Timeout.InfiniteTimeSpan : timeout;
        var index = 0;
        var buffer = new Buffer<TItem>();
        foreach (var item in enumerable)
        {
            if (ProcessTakeOrSkipOrEnd(item))
            {
                return R.Success(buffer.ToFinalArray());
            }
        }

        var cancellationTokenSource = new CancellationTokenSource(timeout);
        var taskCompletionSource = new TaskCompletionSource<R<TItem[], TItem[]>>();
        var cancellationRegistration = cancellationTokenSource.Token.Register(_ => taskCompletionSource.TrySetResult(R.Error(buffer.ToFinalArray())), __._);
        if (enumerable is INotifyCollectionChanged collection)
        {
            collection.CollectionChanged += OnCollectionChanged;
#if NETSTANDARD2_0
            _ = taskCompletionSource.Task.ContinueWith(
                _ =>
                {
                    collection.CollectionChanged -= OnCollectionChanged;
                    cancellationRegistration.Dispose();
                },
                CancellationToken.None);
#else
            _ = taskCompletionSource.Task.ContinueWith(
                async _ =>
                {
                    collection.CollectionChanged -= OnCollectionChanged;
                    await cancellationRegistration.DisposeAsync().ConfigureAwait(false);
                },
                CancellationToken.None);
#endif

            void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    if (e.NewItems == null)
                    {
                        return;
                    }

                    var firstItem = e.NewItems.Cast<TItem>();
                    foreach (var item in firstItem)
                    {
                        if (ProcessTakeOrSkipOrEnd(item))
                        {
                            taskCompletionSource.TrySetResult(R.Success(buffer.ToFinalArray()));
                            return;
                        }
                    }
                }
            }

            return await taskCompletionSource.Task;
        }

        if (enumerable is Array)
        {
            return R.Error(buffer.ToFinalArray());
        }

        var synchronizationContext = SynchronizationContext.Current;
        if (synchronizationContext != null)
        {
            synchronizationContext.Post(_ => Reprocess(), __._);
            return await taskCompletionSource.Task;
        }

#if NETSTANDARD2_0
        await Task.Delay(timeout - TimeSpan.FromMilliseconds(timeout.TotalMilliseconds / 3.0), CancellationToken.None).ConfigureAwait(false);
#else
        await Task.Delay(timeout - (timeout / 3.0), CancellationToken.None).ConfigureAwait(false);
#endif
        foreach (var item in enumerable.Skip(index))
        {
            if (ProcessTakeOrSkipOrEnd(item))
            {
                return R.Success(buffer.ToFinalArray());
            }
        }

        return await taskCompletionSource.Task;

        void Reprocess()
        {
            try
            {
                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    foreach (var item in enumerable.Skip(index))
                    {
                        if (ProcessTakeOrSkipOrEnd(item))
                        {
                            taskCompletionSource.TrySetResult(R.Success(buffer.ToFinalArray()));
                            return;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        bool ProcessTakeOrSkipOrEnd(TItem item)
        {
            var takeAction = predicate.Invoke(item, index++);
            return takeAction switch
            {
                TakeAction.Take => Write(item, false),
                TakeAction.Skip => false,
                TakeAction.End => true,
                TakeAction.TakeAndEnd => Write(item, true),
            };
        }

        bool Write(TItem item, bool end)
        {
            buffer.Write(item);
            return end;
        }
    }
}