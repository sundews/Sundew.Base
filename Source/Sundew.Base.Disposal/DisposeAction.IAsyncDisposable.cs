// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposeAction.IAsyncDisposable.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Disposal
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Wraps an <see cref="Action"/> in an <see cref="IDisposable"/>.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public partial class DisposeAction : IAsyncDisposable
    {
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <returns>A value task.</returns>
        public async ValueTask DisposeAsync()
        {
            switch (this.disposeAction)
            {
                case Func<ValueTask> func:
                    await func().ConfigureAwait(false);
                    break;
                case Action action:
                    action();
                    break;
            }
        }
    }
}