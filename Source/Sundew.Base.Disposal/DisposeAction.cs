// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposeAction.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Disposal;

using System;
using System.Reflection;

/// <summary>
/// Wraps an <see cref="Action"/> in an <see cref="IDisposable"/>.
/// </summary>
/// <seealso cref="System.IDisposable" />
public sealed class DisposeAction : IDisposable
{
    private readonly Action disposeAction;

    /// <summary>Initializes a new instance of the <see cref="DisposeAction"/> class.</summary>
    /// <param name="disposeAction">The dispose action.</param>
    public DisposeAction(Action disposeAction)
    {
        this.disposeAction = disposeAction;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        this.disposeAction();
    }

    /// <summary>Converts to string.</summary>
    /// <returns>A <see cref="string"/> that represents this instance.</returns>
    public override string ToString()
    {
        return $"DisposableAction: {this.disposeAction.Target}.{this.disposeAction.GetMethodInfo().Name}";
    }
}