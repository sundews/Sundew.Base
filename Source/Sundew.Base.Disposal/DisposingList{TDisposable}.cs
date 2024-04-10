// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposingList{TDisposable}.cs" company="Sundews">
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
/// An ordered list of disposables.
/// </summary>
/// <typeparam name="TDisposable">The disposable type.</typeparam>
public class DisposingList<TDisposable> :
#if !NETSTANDARD1_3
IDisposable, IAsyncDisposable
#else
IDisposable
#endif
{
    private static readonly Task<bool> CompletedTrueTask = Task.FromResult(true);
    private readonly bool concurrentDisposal;
    private readonly IDisposalReporter? disposalReporter;
    private IImmutableList<Disposer> disposers = ImmutableList<Disposer>.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisposingList{TItem}" /> class.
    /// </summary>
    public DisposingList()
     : this(false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisposingList{TItem}" /> class.
    /// </summary>
    /// <param name="concurrentDisposal">if set to <c>true</c>  disposal will be parallelized.</param>
    /// <param name="disposalReporter">The disposal reporter.</param>
    public DisposingList(bool concurrentDisposal, IDisposalReporter? disposalReporter = null)
    {
        this.concurrentDisposal = concurrentDisposal;
        this.disposalReporter = disposalReporter;
    }

    /// <summary>
    ///     Adds the specified disposable.
    /// </summary>
    /// <typeparam name="TActualDisposable">The actual type of the disposable.</typeparam>
    /// <param name="disposable">The disposable.</param>
    /// <returns>The added disposable.</returns>
    public TActualDisposable Add<TActualDisposable>(TActualDisposable disposable)
        where TActualDisposable : TDisposable, IDisposable
    {
        this.PrivateAdd(new Disposer.Synchronous(disposable));
        return disposable;
    }

    /// <summary>
    /// Adds the range.
    /// </summary>
    /// <typeparam name="TActualDisposable">The actual type of the disposable.</typeparam>
    /// <param name="disposables">The disposables.</param>
    public void AddRange<TActualDisposable>(IEnumerable<TActualDisposable> disposables)
        where TActualDisposable : TDisposable, IDisposable
    {
        this.PrivateAdd(new Disposer.SynchronousDisposables((IEnumerable<IDisposable>)disposables));
    }

#if !NETSTANDARD1_3
    /// <summary>
    /// Adds the specified disposable.
    /// </summary>
    /// <typeparam name="TActualDisposable">The actual type of the disposable.</typeparam>
    /// <param name="disposable">The disposable.</param>
    /// <returns>The added disposable.</returns>
    public TActualDisposable AddAsync<TActualDisposable>(TActualDisposable disposable)
        where TActualDisposable : IAsyncDisposable
    {
        this.PrivateAdd(new Disposer.Asynchronous(disposable));
        return disposable;
    }

    /// <summary>
    /// Adds the range.
    /// </summary>
    /// <typeparam name="TActualDisposable">The actual type of the disposable.</typeparam>
    /// <param name="disposables">The disposables.</param>
    public void AddAsyncRange<TActualDisposable>(IEnumerable<TActualDisposable> disposables)
        where TActualDisposable : TDisposable, IAsyncDisposable
    {
        this.PrivateAdd(new Disposer.AsynchronousDisposables((IEnumerable<IAsyncDisposable>)disposables));
    }
#endif

    /// <summary>
    /// Tries to add the specified object, if it is an disposable.
    /// </summary>
    /// <param name="object">The object.</param>
    /// <returns>A value indicating whether the object was added.</returns>
    public bool TryAdd(object @object)
    {
        var disposer = Disposer.TryGet(@object);
        if (disposer != null)
        {
            this.PrivateAdd(disposer);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Dispose the list.
    /// </summary>
    /// <typeparam name="TActualDisposable">The actual type of the disposable.</typeparam>
    /// <param name="disposable">The disposable.</param>
    public void Dispose<TActualDisposable>(TActualDisposable disposable)
        where TActualDisposable : TDisposable, IDisposable
    {
        this.PrivateRemove(new Disposer.Synchronous(disposable));
        disposable.Dispose();
        this.disposalReporter?.Disposed(this.GetType(), disposable);
    }

#if !NETSTANDARD1_3
    /// <summary>
    /// Disposes the list asynchronously.
    /// </summary>
    /// <typeparam name="TActualDisposable">The actual type of the disposable.</typeparam>
    /// <param name="asyncDisposable">The async disposable.</param>
    /// <returns>An async task.</returns>
    public async ValueTask DisposeAsync<TActualDisposable>(TActualDisposable asyncDisposable)
        where TActualDisposable : TDisposable, IAsyncDisposable
    {
        this.PrivateRemove(new Disposer.Asynchronous(asyncDisposable));
        await asyncDisposable.DisposeAsync();
        this.disposalReporter?.Disposed(this.GetType(), asyncDisposable);
    }
#endif

    /// <summary>
    /// Dispose the list.
    /// </summary>
    public void Dispose()
    {
        var disposers = this.disposers;
        this.Clear();
        if (this.concurrentDisposal)
        {
            Parallel.ForEach(disposers, x => x.Dispose(this.disposalReporter));
        }
        else
        {
            foreach (var disposer in disposers)
            {
                disposer.Dispose(this.disposalReporter);
            }
        }
    }

    /// <summary>
    /// Disposes the list asynchronously.
    /// </summary>
    /// <returns>An async task.</returns>
    public async ValueTask DisposeAsync()
    {
        var disposers = this.disposers;
        this.Clear();
        if (this.concurrentDisposal)
        {
            await Task.WhenAll(disposers.Select(x =>
            {
                var valueTask = x.DisposeAsync(this.disposalReporter);
                if (valueTask.IsCompleted)
                {
                    return CompletedTrueTask;
                }

                return valueTask.AsTask();
            })).ConfigureAwait(false);
        }
        else
        {
            foreach (var disposer in disposers)
            {
                await disposer.DisposeAsync(this.disposalReporter).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Clear the initializing list.
    /// </summary>
    public void Clear()
    {
        this.ReplaceList(disposers => disposers.Clear());
    }

    internal IEnumerable<Disposer> GetDisposers()
    {
        return this.disposers;
    }

    private void PrivateAdd(Disposer disposer)
    {
        this.ReplaceList(disposers => disposers.Add(disposer));
    }

    private void PrivateRemove(Disposer disposer)
    {
        this.ReplaceList(disposers => disposers.Remove(disposer));
    }

    private void ReplaceList(Func<IImmutableList<Disposer>, IImmutableList<Disposer>> newListFunc)
    {
        var disposers = this.disposers;
        var newList = newListFunc(disposers);
        Interlocked.CompareExchange(ref this.disposers, newList, disposers);
        while (!ReferenceEquals(this.disposers, newList))
        {
            disposers = this.disposers;
            newList = newListFunc(disposers);
            Interlocked.CompareExchange(ref this.disposers, newList, disposers);
        }
    }
}