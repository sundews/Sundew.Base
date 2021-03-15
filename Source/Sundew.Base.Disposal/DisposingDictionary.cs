// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposingDictionary.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Disposal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sundew.Base.Disposal.Internal;
    using Sundew.Base.Reporting;
    using Sundew.Base.Threading;

#pragma warning disable CS1710 // XML comment has a duplicate typeparam tag
    /// <summary>
    /// Maps a key to a disposable for cleaning up unmanaged resources.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    public sealed partial class DisposingDictionary<TKey> : IDisposable, IReportingDisposable
        where TKey : notnull
#pragma warning restore CS1710 // XML comment has a duplicate typeparam tag
    {
        private readonly AsyncLock asyncLock = new();
        private readonly List<Item> disposables = new();
        private IDisposableReporter? disposableReporter;

        /// <summary>Initializes a new instance of the <see cref="DisposingDictionary{TKey}"/> class.</summary>
        public DisposingDictionary()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="DisposingDictionary{TKey}"/> class.</summary>
        /// <param name="disposableReporter">The disposable reporter.</param>
        public DisposingDictionary(IDisposableReporter? disposableReporter)
        {
            this.disposableReporter = disposableReporter;
            this.disposableReporter?.SetSource(this);
        }

        /// <summary>Sets the reporter.</summary>
        /// <param name="disposableReporter">The disposable reporter.</param>
        void IReportingDisposable.SetReporter(IDisposableReporter? disposableReporter)
        {
            this.disposableReporter = this.disposableReporter != null
                ? new NestedDisposableReporter(this.disposableReporter, disposableReporter)
                : disposableReporter;
            this.disposableReporter?.SetSource(this);
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
        public TActualKey Add<TActualKey>(TActualKey key, IDisposable disposable, DisposeKey disposeKey)
            where TActualKey : TKey, IDisposable
        {
            return this.PrivateAdd(key, disposable, disposeKey);
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
        public TActualKey Add<TActualKey>(TActualKey key, IDisposable disposable)
            where TActualKey : TKey
        {
            return this.PrivateAdd(key, disposable, DisposeKey.No);
        }

        /// <summary>
        /// Disposes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Dispose(TKey key)
        {
            using (this.asyncLock.Lock())
            {
                var itemIndex = this.disposables.FindIndex(x => Equals(x.Key, key));
                if (itemIndex > -1)
                {
                    var item = this.disposables[itemIndex];
                    DisposableHelper.Dispose(GetDisposables(item), this.disposableReporter);
                    this.disposables.RemoveAt(itemIndex);
                }
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            using (this.asyncLock.Lock())
            {
                DisposableHelper.Dispose(GetDisposables(this.disposables), this.disposableReporter);
                this.disposables.Clear();
            }

            this.asyncLock.Dispose();
        }

        private static IEnumerable<object> GetDisposables(List<Item> items)
        {
            return items.SelectMany(GetDisposables);
        }

        private static IEnumerable<object> GetDisposables(Item item)
        {
            switch (item.DisposeKey)
            {
                case DisposeKey.BeforeValue:
                    yield return item.Key;
                    yield return item.Disposable;
                    break;
                case DisposeKey.AfterValue:
                    yield return item.Disposable;
                    yield return item.Key;
                    break;
                default:
                    yield return item.Disposable;
                    break;
            }
        }

        private TActualKey PrivateAdd<TActualKey>(TActualKey key, IDisposable disposable, DisposeKey disposeKey)
            where TActualKey : TKey
        {
            using (this.asyncLock.Lock())
            {
                this.disposables.Add(new Item(key, disposable, disposeKey));
            }

            return key;
        }

        private readonly struct Item
        {
            public Item(TKey key, object disposable, DisposeKey disposeKey)
            {
                this.Key = key;
                this.Disposable = disposable;
                this.DisposeKey = disposeKey;
            }

            public TKey Key { get; }

            public object Disposable { get; }

            public DisposeKey DisposeKey { get; }
        }
    }
}