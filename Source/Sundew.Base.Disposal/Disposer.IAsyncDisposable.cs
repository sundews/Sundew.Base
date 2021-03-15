// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Disposer.IAsyncDisposable.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Disposal
{
    using System;
    using System.Threading.Tasks;
    using Sundew.Base.Disposal.Internal;

    /// <summary>
    /// An implementation of <see cref="IDisposable"/> and <see cref="IAsyncDisposable"/> that disposes an list of <see cref="IDisposable"/> and <see cref="IAsyncDisposable"/>.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <seealso cref="System.IAsyncDisposable" />
    public partial class Disposer : IAsyncDisposable
    {
        /// <summary>Disposes the asynchronous.</summary>
        /// <returns>A value task.</returns>
        public ValueTask DisposeAsync()
        {
            var localDisposables = this.disposables;
            this.disposables = null;
            return DisposableHelper.DisposeAsync(localDisposables, this.disposableReporter);
        }
    }
}