// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposingDictionary.IAsyncDisposable.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Disposal
{
    using System;
    using System.Threading.Tasks;
    using Sundew.Base.Disposal.Internal;

#pragma warning disable CS1710 // XML comment has a duplicate typeparam tag
    /// <summary>
    /// Maps a key to a disposable for cleaning up unmanaged resources.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    public partial class DisposingDictionary<TKey> : IAsyncDisposable
        where TKey : notnull
#pragma warning restore CS1710 // XML comment has a duplicate typeparam tag
    {
        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <typeparam name="TActualKey">The type of the actual key.</typeparam>
        /// <param name="key">The item.</param>
        /// <param name="disposable">The disposable.</param>
        /// <param name="disposeKey">if set to <c>true</c> the key will also be disposed when disposing (If it implements <see cref="IDisposable" />).</param>
        /// <returns>
        /// The added key.
        /// </returns>
        public Task<TActualKey> AddAsyncKeyAsync<TActualKey>(TActualKey key, IAsyncDisposable disposable, DisposeKey disposeKey = DisposeKey.No)
            where TActualKey : TKey, IDisposable
        {
            return this.PrivateAddAsync(key, disposable, disposeKey);
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <typeparam name="TActualKey">The type of the actual key.</typeparam>
        /// <param name="key">The item.</param>
        /// <param name="disposable">The disposable.</param>
        /// <param name="disposeKey">if set to <c>true</c> the key will also be disposed when disposing (If it implements <see cref="IDisposable" />).</param>
        /// <returns>
        /// The added key.
        /// </returns>
        public TActualKey AddAsyncKey<TActualKey>(TActualKey key, IDisposable disposable, DisposeKey disposeKey = DisposeKey.No)
            where TActualKey : TKey, IAsyncDisposable
        {
            return this.PrivateAdd(key, disposable, disposeKey);
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <typeparam name="TActualKey">The type of the actual key.</typeparam>
        /// <param name="key">The item.</param>
        /// <param name="disposable">The disposable.</param>
        /// <param name="disposeKey">if set to <c>true</c> the key will also be disposed when disposing (If it implements <see cref="IDisposable" />).</param>
        /// <returns>
        /// The added key.
        /// </returns>
        public Task<TActualKey> AddAsync<TActualKey>(TActualKey key, IAsyncDisposable disposable, DisposeKey disposeKey)
            where TActualKey : TKey, IAsyncDisposable
        {
            return this.PrivateAddAsync(key, disposable, disposeKey);
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <typeparam name="TActualKey">The type of the actual key.</typeparam>
        /// <param name="key">The item.</param>
        /// <param name="disposable">The disposable.</param>
        /// <returns>
        /// The added key.
        /// </returns>
        public Task<TActualKey> AddAsync<TActualKey>(TActualKey key, IAsyncDisposable disposable)
            where TActualKey : TKey
        {
            return this.PrivateAddAsync(key, disposable);
        }

        /// <summary>
        /// Disposes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>A value task.</returns>
        public async ValueTask DisposeAsync(TKey key)
        {
            using (await this.asyncLock.LockAsync().ConfigureAwait(false))
            {
                var itemIndex = this.disposables.FindIndex(x => Equals(x.Key, key));
                if (itemIndex > -1)
                {
                    var item = this.disposables[itemIndex];
                    await DisposableHelper.DisposeAsync(item, this.disposableReporter).ConfigureAwait(false);
                    this.disposables.RemoveAt(itemIndex);
                }
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <returns>A value task.</returns>
        public async ValueTask DisposeAsync()
        {
            using (await this.asyncLock.LockAsync().ConfigureAwait(false))
            {
                await DisposableHelper.DisposeAsync(GetDisposables(this.disposables), this.disposableReporter).ConfigureAwait(false);
                this.disposables.Clear();
            }
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <typeparam name="TActualKey">The type of the actual key.</typeparam>
        /// <param name="key">The item.</param>
        /// <param name="disposable">The disposable.</param>
        /// <param name="disposeKey">if set to <c>true</c> the key will also be disposed when disposing (If it implements <see cref="IDisposable" />).</param>
        /// <returns>
        /// The added key.
        /// </returns>
        private async Task<TActualKey> PrivateAddAsync<TActualKey>(TActualKey key, IAsyncDisposable disposable, DisposeKey disposeKey = DisposeKey.No)
            where TActualKey : TKey
        {
            using (await this.asyncLock.LockAsync().ConfigureAwait(false))
            {
                this.disposables.Add(new Item(key, disposable, disposeKey));
            }

            return key;
        }
    }
}