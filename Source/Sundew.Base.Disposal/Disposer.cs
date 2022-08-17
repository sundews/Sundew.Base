// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Disposer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Disposal;

using System;
using System.Collections.Generic;
using System.Linq;
using Sundew.Base.Collections;
using Sundew.Base.Disposal.Internal;
using Sundew.Base.Reporting;

/// <summary>
/// An implementation of <see cref="IDisposable"/> that disposes an list of <see cref="IDisposable"/>.
/// </summary>
/// <seealso cref="System.IDisposable" />
public sealed partial class Disposer : IDisposable, IReportingDisposable
{
    private IReadOnlyCollection<object>? disposables;
    private IDisposableReporter? disposableReporter;

    /// <summary>
    /// Initializes a new instance of the <see cref="Disposer"/> class.
    /// </summary>
    public Disposer()
        : this(Enumerable.Empty<IDisposable>())
    {
    }

    /// <summary>Initializes a new instance of the <see cref="Disposer"/> class.</summary>
    /// <param name="disposableReporter">The disposable reporter.</param>
    public Disposer(IDisposableReporter disposableReporter)
        : this(Enumerable.Empty<IDisposable>(), disposableReporter)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Disposer"/> class.
    /// </summary>
    /// <param name="disposables">The disposables.</param>
    public Disposer(params IDisposable[] disposables)
        : this((IEnumerable<IDisposable>)disposables)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="Disposer"/> class.</summary>
    /// <param name="disposableReporter">The disposable reporter.</param>
    /// <param name="disposables">The disposables.</param>
    public Disposer(IDisposableReporter disposableReporter, params IDisposable[] disposables)
        : this(disposables, disposableReporter)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="Disposer"/> class.</summary>
    /// <param name="disposables">The disposables.</param>
    /// <param name="disposableReporter">The disposable reporter.</param>
    public Disposer(IEnumerable<IDisposable> disposables, IDisposableReporter? disposableReporter = null)
    {
        this.disposableReporter = disposableReporter;
        this.disposables = disposables.ToReadOnly();
        this.disposableReporter?.SetSource(this);
    }

#if NETSTANDARD2_1
    /// <summary>
    /// Initializes a new instance of the <see cref="Disposer"/> class.
    /// </summary>
    /// <param name="disposables">The disposables.</param>
    public Disposer(params IAsyncDisposable[] disposables)
        : this((IEnumerable<IAsyncDisposable>)disposables)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="Disposer"/> class.</summary>
    /// <param name="disposableReporter">The disposable reporter.</param>
    /// <param name="disposables">The disposables.</param>
    public Disposer(IDisposableReporter disposableReporter, params IAsyncDisposable[] disposables)
        : this(disposables, disposableReporter)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="Disposer"/> class.</summary>
    /// <param name="disposables">The disposables.</param>
    /// <param name="disposableReporter">The disposable reporter.</param>
    public Disposer(IEnumerable<IAsyncDisposable> disposables, IDisposableReporter? disposableReporter = null)
    {
        this.disposableReporter = disposableReporter;
        this.disposables = disposables.ToReadOnly();
        this.disposableReporter?.SetSource(this);
    }
#endif

    /// <summary>Initializes a new instance of the <see cref="Disposer"/> class.</summary>
    /// <param name="disposables">The disposables.</param>
    /// <param name="disposableReporter">The disposable reporter.</param>
    private Disposer(IEnumerable<object> disposables, IDisposableReporter? disposableReporter = null)
    {
        this.disposableReporter = disposableReporter;
        this.disposables = disposables.ToReadOnly();
        this.disposableReporter?.SetSource(this);
    }

    /// <summary>Creates the specified disposer builder function.</summary>
    /// <param name="disposerBuilderFunc">The disposer builder function.</param>
    /// <param name="disposableReporter">The disposable reporter.</param>
    /// <returns>A new Disposer.</returns>
    public static Disposer Create(Action<IDisposerBuilder> disposerBuilderFunc, IDisposableReporter? disposableReporter = null)
    {
        var disposerBuilder = new DisposerBuilder();
        if (disposerBuilderFunc == null)
        {
            throw new ArgumentNullException(nameof(disposerBuilderFunc));
        }

        disposerBuilderFunc(disposerBuilder);
        return new Disposer(disposerBuilder.Disposables, disposableReporter);
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
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        DisposableHelper.Dispose(this.disposables, this.disposableReporter);
        this.disposables = null;
    }
}