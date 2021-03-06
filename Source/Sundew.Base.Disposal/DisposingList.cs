// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposingList.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Disposal;

using System;
using System.Collections.Generic;
using Sundew.Base.Disposal.Internal;
using Sundew.Base.Reporting;
using Sundew.Base.Threading;

#pragma warning disable CS1710 // XML comment has a duplicate typeparam tag
/// <summary>
/// Stores <see cref="IDisposable"/> in a list for later disposal.
/// </summary>
/// <typeparam name="TDisposable">The type of the disposable.</typeparam>
/// <seealso cref="System.IDisposable" />
public sealed partial class DisposingList<TDisposable> : IDisposable, IReportingDisposable
#pragma warning restore CS1710 // XML comment has a duplicate typeparam tag
{
    private readonly AsyncLock asyncLock = new();
    private readonly List<object> disposables = new();
    private IDisposableReporter? disposableReporter;

    /// <summary>Initializes a new instance of the <see cref="DisposingList{TDisposable}"/> class.</summary>
    public DisposingList()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="DisposingList{TDisposable}"/> class.</summary>
    /// <param name="disposableReporter">The disposable reporter.</param>
    public DisposingList(IDisposableReporter? disposableReporter)
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
    ///     Adds the specified disposable.
    /// </summary>
    /// <typeparam name="TActualDisposable">The type of the actual disposable.</typeparam>
    /// <param name="disposable">The disposable.</param>
    /// <returns>
    ///     The added disposable.
    /// </returns>
    public TActualDisposable Add<TActualDisposable>(TActualDisposable disposable)
        where TActualDisposable : TDisposable, IDisposable
    {
        using (this.asyncLock.Lock())
        {
            this.disposables.Add(disposable);
        }

        return disposable;
    }

    /// <summary>
    /// Adds the range.
    /// </summary>
    /// <param name="disposables">The disposables.</param>
    public void AddRange(IEnumerable<IDisposable> disposables)
    {
        using (this.asyncLock.Lock())
        {
            this.disposables.AddRange(disposables);
        }
    }

    /// <summary>Disposes the specified disposable.</summary>
    /// <typeparam name="TActualDisposable">The actual disposable type.</typeparam>
    /// <param name="disposable">The disposable.</param>
    public void Dispose<TActualDisposable>(TActualDisposable disposable)
        where TActualDisposable : TDisposable, IDisposable
    {
        using (this.asyncLock.Lock())
        {
            DisposableHelper.Dispose(disposable, this.disposableReporter);
            this.disposables.Remove(disposable);
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        using (this.asyncLock.Lock())
        {
            DisposableHelper.Dispose(this.disposables, this.disposableReporter);
            this.disposables.Clear();
        }

        this.asyncLock.Dispose();
    }
}