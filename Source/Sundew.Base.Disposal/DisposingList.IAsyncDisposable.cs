// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposingList.IAsyncDisposable.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Disposal;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sundew.Base.Disposal.Internal;

#pragma warning disable CS1710 // XML comment has a duplicate typeparam tag
/// <summary>
/// Stores <see cref="IDisposable"/> in a list for later disposal.
/// </summary>
/// <typeparam name="TDisposable">The type of the disposable.</typeparam>
/// <seealso cref="System.IAsyncDisposable" />
public partial class DisposingList<TDisposable> : IAsyncDisposable
#pragma warning restore CS1710 // XML comment has a duplicate typeparam tag
{
    /// <summary>
    /// Adds the specified disposable.
    /// </summary>
    /// <typeparam name="TActualDisposable">The type of the actual disposable.</typeparam>
    /// <param name="disposable">The disposable.</param>
    /// <returns>
    ///     The added disposable.
    /// </returns>
    public async Task<TActualDisposable> AddAsync<TActualDisposable>(TActualDisposable disposable)
        where TActualDisposable : TDisposable, IAsyncDisposable
    {
        using (await this.asyncLock.LockAsync().ConfigureAwait(false))
        {
            this.disposables.Add(disposable);
        }

        return disposable;
    }

    /// <summary>
    /// Adds the range.
    /// </summary>
    /// <param name="disposables">The disposables.</param>
    /// <returns>A task.</returns>
    public async Task AddRangeAsync(IEnumerable<IAsyncDisposable> disposables)
    {
        using (await this.asyncLock.LockAsync().ConfigureAwait(false))
        {
            this.disposables.AddRange(disposables);
        }
    }

    /// <summary>
    /// Disposes the disposable asynchronously.
    /// </summary>
    /// <typeparam name="TActualDisposable">The type of the disposable.</typeparam>
    /// <param name="disposable">The disposable.</param>
    /// <returns>A value task.</returns>
    public async ValueTask DisposeAsync<TActualDisposable>(TActualDisposable disposable)
        where TActualDisposable : TDisposable, IAsyncDisposable
    {
        using (await this.asyncLock.LockAsync().ConfigureAwait(false))
        {
            await DisposableHelper.DisposeAsync(disposable, this.disposableReporter).ConfigureAwait(false);
            this.disposables.Remove(disposable);
        }
    }

    /// <summary>
    /// Disposes the items asynchronously.
    /// </summary>
    /// <returns>A value task.</returns>
    public async ValueTask DisposeAsync()
    {
        using (await this.asyncLock.LockAsync().ConfigureAwait(false))
        {
            await DisposableHelper.DisposeAsync(this.disposables, this.disposableReporter).ConfigureAwait(false);
            this.disposables.Clear();
        }
    }
}