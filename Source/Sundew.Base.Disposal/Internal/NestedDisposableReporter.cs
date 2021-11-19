// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NestedDisposableReporter.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Disposal.Internal;

using System;

internal class NestedDisposableReporter : IDisposableReporter
{
    private readonly IDisposableReporter disposableReporter;
    private readonly IDisposableReporter? nextDisposableReporter;

    public NestedDisposableReporter(IDisposableReporter disposableReporter, IDisposableReporter? nextDisposableReporter)
    {
        this.disposableReporter = disposableReporter;
        this.nextDisposableReporter = nextDisposableReporter;
    }

    public void SetSource(Type target, object source)
    {
        this.disposableReporter.SetSource(target, source);
        this.nextDisposableReporter?.SetSource(target, source);
    }

    public void Disposed(object disposable)
    {
        this.disposableReporter.Disposed(disposable);
        this.nextDisposableReporter?.Disposed(disposable);
    }
}