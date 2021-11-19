// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposerBuilder.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Disposal.Internal;

using System;
using System.Collections.Generic;

internal class DisposerBuilder : IDisposerBuilder
{
    public List<object> Disposables { get; } = new();

    public IDisposerBuilder Add(IDisposable disposable)
    {
        this.Disposables.Add(disposable);
        return this;
    }

#if NETSTANDARD2_1
    public IDisposerBuilder AddAsync(IAsyncDisposable asyncDisposable)
    {
        this.Disposables.Add(asyncDisposable);
        return this;
    }
#endif
}