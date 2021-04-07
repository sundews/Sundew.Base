// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDisposerBuilder.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Disposal
{
    using System;

    /// <summary>Interface for implementing a disposer builder.</summary>
    public interface IDisposerBuilder
    {
        /// <summary>Adds the specified disposable.</summary>
        /// <param name="disposable">The disposable.</param>
        /// <returns>This instance.</returns>
        IDisposerBuilder Add(IDisposable disposable);

#if NETSTANDARD2_1
        /// <summary>Adds the specified async disposable.</summary>
        /// <param name="asyncDisposable">The async disposable.</param>
        /// <returns>This instance.</returns>
        IDisposerBuilder AddAsync(IAsyncDisposable asyncDisposable);
#endif
    }
}