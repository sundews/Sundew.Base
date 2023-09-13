// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposingDictionary.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Disposal;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using global::Disposal.Interfaces;

/// <summary>
/// An ordered collection of disposables that allows disposing by a key.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
public sealed class DisposingDictionary<TKey> :
#if !NETSTANDARD1_3
IDisposable, IAsyncDisposable
#else
IDisposable
#endif
    where TKey : notnull
{
    private readonly IDisposalReporter? disposableReporter;
    private IImmutableList<Item> disposables = ImmutableList<Item>.Empty;

    /// <summary>Initializes a new instance of the <see cref="DisposingDictionary{TKey}"/> class.</summary>
    public DisposingDictionary()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="DisposingDictionary{TKey}"/> class.</summary>
    /// <param name="disposableReporter">The disposer reporter.</param>
    public DisposingDictionary(IDisposalReporter? disposableReporter)
    {
        this.disposableReporter = disposableReporter;
    }

    /// <summary>
    /// Adds the specified item.
    /// </summary>
    /// <typeparam name="TActualKey">The type of the actual key.</typeparam>
    /// <param name="key">The item.</param>
    /// <param name="disposable">The disposer.</param>
    /// <param name="tryDisposeKey">if set to <c>true</c> the key will also be disposed when disposing (If it implements <see cref="IDisposable" />).</param>
    /// <returns>The added key.</returns>
    public TActualKey Add<TActualKey>(TActualKey key, IDisposable disposable, TryDisposeKey tryDisposeKey)
        where TActualKey : TKey, IDisposable
    {
        return this.PrivateAdd(key, new Disposer.Synchronous(disposable), tryDisposeKey);
    }

    /// <summary>
    /// Adds the specified item.
    /// </summary>
    /// <typeparam name="TActualKey">The type of the actual key.</typeparam>
    /// <param name="key">The item.</param>
    /// <param name="disposable">The disposer.</param>
    /// <returns>The added key.</returns>
    public TActualKey Add<TActualKey>(TActualKey key, IDisposable disposable)
        where TActualKey : TKey
    {
        return this.PrivateAdd(key, new Disposer.Synchronous(disposable), TryDisposeKey.No);
    }

#if !NETSTANDARD1_3
    /// <summary>
    /// Adds the specified item.
    /// </summary>
    /// <typeparam name="TActualKey">The type of the actual key.</typeparam>
    /// <param name="key">The item.</param>
    /// <param name="disposable">The disposer.</param>
    /// <param name="tryDisposeKey">if set to <c>true</c> the key will also be disposed when disposing (If it implements <see cref="IDisposable" />).</param>
    /// <returns>The added key.</returns>
    public TActualKey AddAsync<TActualKey>(TActualKey key, IAsyncDisposable disposable, TryDisposeKey tryDisposeKey)
        where TActualKey : TKey
    {
        return this.PrivateAdd(key, new Disposer.Asynchronous(disposable), tryDisposeKey);
    }

    /// <summary>
    /// Adds the specified item.
    /// </summary>
    /// <typeparam name="TActualKey">The type of the actual key.</typeparam>
    /// <param name="key">The item.</param>
    /// <param name="disposable">The disposer.</param>
    /// <returns>The added key.</returns>
    public TActualKey AddAsync<TActualKey>(TActualKey key, IAsyncDisposable disposable)
        where TActualKey : TKey
    {
        return this.PrivateAdd(key, new Disposer.Asynchronous(disposable), TryDisposeKey.No);
    }
#endif

    /// <summary>
    /// Clears the disposables.
    /// </summary>
    public void Clear()
    {
        this.ReplaceList(disposables => disposables.Clear());
    }

    /// <summary>
    /// Disposes the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    public void Dispose(TKey key)
    {
        var (index, item) = this.FindAndRemove(this.disposables, key);
        if (index > -1)
        {
            this.ReplaceList(disposables => disposables.RemoveAt(index));
            foreach (var disposer in GetDisposers(item))
            {
                disposer.Dispose(this.disposableReporter);
            }
        }
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    public void Dispose()
    {
        var disposables = this.disposables;
        this.Clear();
        foreach (var disposer in GetDisposers(disposables))
        {
            disposer.Dispose(this.disposableReporter);
        }
    }

#if !NETSTANDARD1_3
    /// <summary>
    /// Disposes the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>An async task.</returns>
    public async ValueTask DisposeAsync(TKey key)
    {
        var (index, item) = this.FindAndRemove(this.disposables, key);
        if (index > -1)
        {
            this.ReplaceList(disposables => disposables.RemoveAt(index));
            foreach (var disposer in GetDisposers(item))
            {
                await disposer.DisposeAsync(this.disposableReporter).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <returns>An async task.</returns>
    public async ValueTask DisposeAsync()
    {
        var disposables = this.disposables;
        this.Clear();
        foreach (var disposer in GetDisposers(disposables))
        {
            await disposer.DisposeAsync(this.disposableReporter).ConfigureAwait(false);
        }
    }
#endif

    internal IEnumerable<Disposer> GetDisposers()
    {
        return GetDisposers(this.disposables);
    }

    private static IEnumerable<Disposer> GetDisposers(IImmutableList<Item> disposers)
    {
        return disposers.SelectMany(GetDisposers);
    }

    private static IEnumerable<Disposer> GetDisposers(Item item)
    {
        switch (item.TryDisposeKey)
        {
            case TryDisposeKey.BeforeValue:
                var disposerBeforeValue = Disposer.TryGet(item.Key);
                if (disposerBeforeValue != null)
                {
                    yield return disposerBeforeValue;
                }

                yield return item.Disposer;
                break;
            case TryDisposeKey.AfterValue:
                yield return item.Disposer;
                var disposerAfterValue = Disposer.TryGet(item.Key);
                if (disposerAfterValue != null)
                {
                    yield return disposerAfterValue;
                }

                break;
            default:
                yield return item.Disposer;
                break;
        }
    }

    private (int Index, Item Item) FindAndRemove(IImmutableList<Item> immutableList, TKey key)
    {
        for (var index = 0; index < immutableList.Count; index++)
        {
            var item = immutableList[index];
            if (Equals(item.Key, key))
            {
                return (index, item);
            }
        }

        return (-1, default);
    }

    private TActualKey PrivateAdd<TActualKey>(TActualKey key, Disposer disposer, TryDisposeKey tryDisposeKey)
        where TActualKey : TKey
    {
        var item = new Item(key, disposer, tryDisposeKey);
        this.ReplaceList(disposables => disposables.Add(item));
        return key;
    }

    private void ReplaceList(Func<IImmutableList<Item>, IImmutableList<Item>> newListFunc)
    {
        var disposables = this.disposables;
        var newList = newListFunc(disposables);
        Interlocked.CompareExchange(ref this.disposables, newList, disposables);
        while (!ReferenceEquals(this.disposables, newList))
        {
            disposables = this.disposables;
            newList = newListFunc(disposables);
            Interlocked.CompareExchange(ref this.disposables, newList, disposables);
        }
    }

    private readonly struct Item
    {
        public Item(TKey key, Disposer disposer, TryDisposeKey tryDisposeKey)
        {
            this.Key = key;
            this.Disposer = disposer;
            this.TryDisposeKey = tryDisposeKey;
        }

        public TKey Key { get; }

        public Disposer Disposer { get; }

        public TryDisposeKey TryDisposeKey { get; }
    }
}