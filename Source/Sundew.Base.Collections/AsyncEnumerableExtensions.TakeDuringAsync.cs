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
public static partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// Asynchronously waits for the first item fulfilling the predicate or until timeout occurs.
    /// </summary>
    /// <typeparam name="TItem">The TItem type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellation">The cancellation token.</param>
    /// <returns>An async result contain the expected items or an error in case of a timeout containing the items that could be found.</returns>
    public static async Task<R<TItem>> FirstAsync<TItem>(this IEnumerable<TItem> enumerable, Func<TItem, bool> predicate, Cancellation cancellation = default)
    {
        return (await TakeDuringAsync(enumerable, (item, _) => predicate.Invoke(item), 1, cancellation).ConfigureAwait(false)).Map(x => x[0]);
    }

    /// <summary>
    /// Asynchronously waits for the first item fulfilling the predicate or until timeout occurs.
    /// </summary>
    /// <typeparam name="TItem">The TItem type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellation">The cancellation.</param>
    /// <returns>An async result contain the expected items or an error in case of a timeout containing the items that could be found.</returns>
    public static async Task<R<TItem>> FirstAsync<TItem>(this IEnumerable<TItem> enumerable, Func<TItem, int, bool> predicate, Cancellation cancellation = default)
    {
        return (await TakeDuringAsync(enumerable, predicate, 1, cancellation).ConfigureAwait(false)).Map(x => x[0]);
    }

    /// <summary>
    /// Asynchronously waits for the first item or until timeout occurs.
    /// </summary>
    /// <typeparam name="TItem">The TItem type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="cancellation">The cancellation.</param>
    /// <returns>An async result contain the expected items or an error in case of a timeout containing the items that could be found.</returns>
    public static async Task<R<TItem>> FirstAsync<TItem>(this IEnumerable<TItem> enumerable, Cancellation cancellation = default)
    {
        return (await TakeDuringAsync(enumerable, default(Func<TItem, int, bool>), 1, cancellation).ConfigureAwait(false)).Map(x => x[0]);
    }

    /// <summary>
    /// Asynchronously waits for the second item or until timeout occurs.
    /// </summary>
    /// <typeparam name="TItem">The TItem type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="cancellation">The cancellation.</param>
    /// <returns>An async result contain the expected items or an error in case of a timeout containing the items that could be found.</returns>
    public static async Task<R<TItem>> SecondAsync<TItem>(this IEnumerable<TItem> enumerable, Cancellation cancellation = default)
    {
        return (await TakeDuringAsync(enumerable, default(Func<TItem, int, bool>), 2, cancellation).ConfigureAwait(false)).Map(x => x[1]);
    }

    /// <summary>
    /// Asynchronously waits for the second item fulfilling the predicate or until timeout occurs.
    /// </summary>
    /// <typeparam name="TItem">The TItem type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellation">The cancellation.</param>
    /// <returns>An async result contain the expected items or an error in case of a timeout containing the items that could be found.</returns>
    public static async Task<R<TItem>> SecondAsync<TItem>(this IEnumerable<TItem> enumerable, Func<TItem, bool> predicate, Cancellation cancellation = default)
    {
        return (await TakeDuringAsync(enumerable, (item, _) => predicate.Invoke(item), 2, cancellation).ConfigureAwait(false)).Map(x => x[1]);
    }

    /// <summary>
    /// Asynchronously waits for the second item fulfilling the predicate or until timeout occurs.
    /// </summary>
    /// <typeparam name="TItem">The TItem type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellation">The cancellation.</param>
    /// <returns>An async result contain the expected items or an error in case of a timeout containing the items that could be found.</returns>
    public static async Task<R<TItem>> SecondAsync<TItem>(this IEnumerable<TItem> enumerable, Func<TItem, int, bool>? predicate, Cancellation cancellation = default)
    {
        return (await TakeDuringAsync(
            enumerable,
            predicate,
            2,
            cancellation).ConfigureAwait(false)).Map(x => x[1]);
    }

    /// <summary>
    /// Asynchronously waits for a number of items or until a timeout occurs.
    /// </summary>
    /// <typeparam name="TItem">The TItem type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="count">The count.</param>
    /// <param name="cancellation">The cancellation.</param>
    /// <returns>An async result contain the expected items or an error in case of a timeout containing the items that could be found.</returns>
    public static Task<R<TItem[], TItem[]>> TakeDuringAsync<TItem>(this IEnumerable<TItem> enumerable, Func<TItem, bool> predicate, int count, Cancellation cancellation = default)
    {
        return TakeDuringAsync(
            enumerable,
            (item, _) => predicate?.Invoke(item) ?? true,
            count,
            cancellation);
    }

    /// <summary>
    /// Asynchronously waits for a number of items or until a timeout occurs.
    /// </summary>
    /// <typeparam name="TItem">The TItem type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="count">The count.</param>
    /// <param name="cancellation">The cancellation.</param>
    /// <returns>An async result contain the expected items or an error in case of a timeout containing the items that could be found.</returns>
    public static Task<R<TItem[], TItem[]>> TakeDuringAsync<TItem>(this IEnumerable<TItem> enumerable, Func<TItem, int, bool>? predicate, int count, Cancellation cancellation = default)
    {
        var numberOfItems = 0;
        return TakeDuringAsync(
            enumerable,
            (item, index) =>
            {
                if (predicate?.Invoke(item, index) ?? true)
                {
                    return Interlocked.Increment(ref numberOfItems) < count ? TakeAction.Take : TakeAction.TakeAndEnd;
                }

                return TakeAction.Skip;
            },
            cancellation);
    }

    /// <summary>
    /// Asynchronously waits for a number of items or until a timeout occurs.
    /// </summary>
    /// <typeparam name="TItem">The TItem type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="count">The count.</param>
    /// <param name="cancellation">The cancellation.</param>
    /// <returns>An async result contain the expected items or an error in case of a timeout containing the items that could be found.</returns>
    public static Task<R<TItem[], TItem[]>> TakeDuringAsync<TItem>(this IEnumerable<TItem> enumerable, int count, Cancellation cancellation = default)
    {
        var numberOfItems = 0;
        return TakeDuringAsync(
            enumerable,
            (item, index) => Interlocked.Increment(ref numberOfItems) < count ? TakeAction.Take : TakeAction.TakeAndEnd,
            cancellation);
    }

    /// <summary>
    /// Asynchronously waits for items fulfilling the condition until timeout occurs.
    /// </summary>
    /// <typeparam name="TItem">The TItem type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellation">The cancellation.</param>
    /// <returns>An async result contain the expected items or an error in case of a timeout containing the items that could be found.</returns>
    public static Task<R<TItem[], TItem[]>> TakeDuringAsync<TItem>(this IEnumerable<TItem> enumerable, Func<TItem, TakeAction> predicate, Cancellation cancellation = default)
    {
        return TakeDuringAsync(enumerable, (item, _) => predicate.Invoke(item), cancellation);
    }

    /// <summary>
    /// Asynchronously waits for items fulfilling the predicate or the timeout occurs.
    /// </summary>
    /// <typeparam name="TItem">The TItem type.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellation">The cancellation.</param>
    /// <returns>An async result contain the expected items or an error in case of a timeout containing the items that could be found.</returns>
    public static async Task<R<TItem[], TItem[]>> TakeDuringAsync<TItem>(this IEnumerable<TItem> enumerable, Func<TItem, int, TakeAction> predicate, Cancellation cancellation = default)
    {
        var index = 0;
        var buffer = new Buffer<TItem>();
        foreach (var item in enumerable)
        {
            if (ProcessTakeOrSkipOrEnd(item))
            {
                return R.Success(buffer.ToFinalArray());
            }
        }

        if (enumerable is Array)
        {
            return R.Error(buffer.ToFinalArray());
        }

        var enabler = cancellation.EnableCancellation();
        var taskCompletionSource = new TaskCompletionSource<R<TItem[], TItem[]>>();
        cancellation.Register(_ => taskCompletionSource.TrySetResult(R.Error(buffer.ToFinalArray())), __._);
        if (enumerable is INotifyCollectionChanged collection)
        {
            collection.CollectionChanged += OnCollectionChanged;

            _ = taskCompletionSource.Task.ContinueWith(
                _ =>
                {
                    collection.CollectionChanged -= OnCollectionChanged;
                    enabler.Dispose();
                },
                CancellationToken.None);

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

        var retryDelay = cancellation.Timeout > TimeSpan.FromSeconds(1)
            ? TimeSpan.FromSeconds(1)
            : GetRetryDelay(cancellation.Timeout);

        var synchronizationContext = SynchronizationContext.Current;
        if (synchronizationContext != null)
        {
            synchronizationContext.Post(_ => Reprocess(), __._);
            return await taskCompletionSource.Task;
        }

        await Task.Delay(retryDelay, CancellationToken.None).ConfigureAwait(false);

        foreach (var item in enumerable.Skip(index))
        {
            if (ProcessTakeOrSkipOrEnd(item))
            {
                taskCompletionSource.TrySetResult(R.Success(buffer.ToFinalArray()));
            }
        }

        return await taskCompletionSource.Task;

        TimeSpan GetRetryDelay(TimeSpan cancellationTokenTimeout)
        {
#if NETSTANDARD2_0
            return cancellationTokenTimeout - TimeSpan.FromMilliseconds(cancellationTokenTimeout.TotalMilliseconds / 3.0);
#else
            return cancellationTokenTimeout - (cancellationTokenTimeout / 3.0);
#endif
        }

        async void Reprocess()
        {
            try
            {
                while (!enabler.IsCancellationRequested)
                {
                    foreach (var item in enumerable.Skip(index))
                    {
                        if (ProcessTakeOrSkipOrEnd(item))
                        {
                            taskCompletionSource.TrySetResult(R.Success(buffer.ToFinalArray()));
                            return;
                        }
                    }

                    await Task.Delay(retryDelay, CancellationToken.None).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch
            {
                taskCompletionSource.SetCanceled();
            }
        }

        bool ProcessTakeOrSkipOrEnd(TItem item)
        {
            var takeAction = predicate.Invoke(item, index++);
            return takeAction switch
            {
                TakeAction.Take => TakeItem(item, false),
                TakeAction.Skip => false,
                TakeAction.End => true,
                TakeAction.TakeAndEnd => TakeItem(item, true),
            };
        }

        bool TakeItem(TItem item, bool end)
        {
            buffer.Write(item);
            return end;
        }
    }
}