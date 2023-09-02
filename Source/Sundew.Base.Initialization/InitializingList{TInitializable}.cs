// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializingList{TInitializable}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Initialization;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using global::Initialization.Interfaces;

/// <summary>
/// An ordered list of <see cref="IInitializable"/> or <see cref="IAsyncInitializable"/>.
/// </summary>
/// <typeparam name="TInitializable">The initializable type.</typeparam>
public class InitializingList<TInitializable> : IInitializable, IAsyncInitializable
{
    private static readonly Task<bool> CompletedTrueTask = Task.FromResult(true);
    private readonly bool concurrentInitialization;
    private readonly IInitializationReporter? initializationReporter;
    private IImmutableList<Initializer> initializers = ImmutableList<Initializer>.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="InitializingList{TInitializable}" /> class.
    /// </summary>
    public InitializingList()
     : this(false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InitializingList{TInitializable}" /> class.
    /// </summary>
    /// <param name="concurrentInitialization">if set to <c>true</c>  initialization will be executed concurrently.</param>
    /// <param name="initializationReporter">The initialization reporter.</param>
    public InitializingList(bool concurrentInitialization, IInitializationReporter? initializationReporter = null)
    {
        this.concurrentInitialization = concurrentInitialization;
        this.initializationReporter = initializationReporter;
    }

    /// <summary>
    /// Adds the specified initializable.
    /// </summary>
    /// <typeparam name="TActualInitializable">The actual type of the initializable.</typeparam>
    /// <param name="initializable">The initializable.</param>
    /// <returns>
    ///     The added initializable.
    /// </returns>
    public TActualInitializable Add<TActualInitializable>(TActualInitializable initializable)
        where TActualInitializable : IInitializable
    {
        this.PrivateAdd(new Initializer.Synchronous(initializable));
        return initializable;
    }

    /// <summary>
    /// Adds the range.
    /// </summary>
    /// <param name="initializables">The initializables.</param>
    public void AddRange(IEnumerable<IInitializable> initializables)
    {
        this.PrivateAdd(new Initializer.SynchronousInitializables(initializables));
    }

    /// <summary>
    ///     Adds the specified disposable.
    /// </summary>
    /// <typeparam name="TActualInitializable">The actual type of the initializable.</typeparam>
    /// <param name="initializable">The initializable.</param>
    /// <returns>
    ///     The added disposable.
    /// </returns>
    public TActualInitializable AddAsync<TActualInitializable>(TActualInitializable initializable)
        where TActualInitializable : IAsyncInitializable
    {
        this.PrivateAdd(new Initializer.Asynchronous(initializable));
        return initializable;
    }

    /// <summary>
    /// Adds the range.
    /// </summary>
    /// <param name="initializables">The initializables.</param>
    public void AddAsyncRange(IEnumerable<IAsyncInitializable> initializables)
    {
        this.PrivateAdd(new Initializer.AsynchronousInitializables(initializables));
    }

    /// <summary>
    /// Tries to add the specified object, if it is an initializable.
    /// </summary>
    /// <param name="object">The object.</param>
    /// <returns>A value indicating whether the object was added.</returns>
    public bool TryAdd(object @object)
    {
        var initializer = Initializer.TryGet(@object);
        if (initializer != null)
        {
            this.PrivateAdd(initializer);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Initializes the collection synchronously.
    /// </summary>
    public void Initialize()
    {
        if (this.concurrentInitialization)
        {
            Parallel.ForEach(this.initializers, x => x.Initialize(this.initializationReporter));
        }
        else
        {
            foreach (var initializable in this.initializers)
            {
                initializable.Initialize(this.initializationReporter);
            }
        }
    }

    /// <summary>
    /// Initializes the collection asynchronously.
    /// </summary>
    /// <returns>
    /// An async task.
    /// </returns>
    public async ValueTask InitializeAsync()
    {
        if (this.concurrentInitialization)
        {
            await Task.WhenAll(this.initializers.Select(x =>
            {
                var valueTask = x.InitializeAsync(this.initializationReporter);
                if (valueTask.IsCompleted)
                {
                    return CompletedTrueTask;
                }

                return valueTask.AsTask();
            })).ConfigureAwait(false);
        }
        else
        {
            foreach (var initializer in this.initializers)
            {
                await initializer.InitializeAsync(this.initializationReporter).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Clear the initializing list.
    /// </summary>
    public void Clear()
    {
        this.ReplaceList(initializers => initializers.Clear());
    }

    internal IEnumerable<Initializer> GetInitializers()
    {
        return this.initializers;
    }

    private void PrivateAdd(Initializer initializer)
    {
        this.ReplaceList(initializers => initializers.Add(initializer));
    }

    private void ReplaceList(Func<IImmutableList<Initializer>, IImmutableList<Initializer>> newListFunc)
    {
        var disposers = this.initializers;
        var newList = newListFunc(disposers);
        Interlocked.CompareExchange(ref this.initializers, newList, disposers);
        while (!ReferenceEquals(this.initializers, newList))
        {
            disposers = this.initializers;
            newList = newListFunc(disposers);
            Interlocked.CompareExchange(ref this.initializers, newList, disposers);
        }
    }
}