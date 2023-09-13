// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposeAsyncAction.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !NETSTANDARD1_3
namespace Sundew.Base.Disposal;

using System;
using System.Reflection;
using System.Threading.Tasks;

/// <summary>
/// An action and async action which implements <see cref="IAsyncDisposable"/>.
/// </summary>
/// <seealso cref="IAsyncDisposable" />
public class DisposeAsyncAction : IAsyncDisposable
{
    private readonly Func<ValueTask> action;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisposeAsyncAction"/> class.
    /// </summary>
    /// <param name="action">The action.</param>
    public DisposeAsyncAction(Func<ValueTask> action)
    {
        this.action = action;
    }

    /// <summary>
    /// Performs the dispose action asynchronously.
    /// </summary>
    /// <returns>An async task.</returns>
    public ValueTask DisposeAsync()
    {
        return this.action();
    }

    /// <summary>Converts to string.</summary>
    /// <returns>A <see cref="string"/> that represents this instance.</returns>
    public override string ToString()
    {
        return $"DisposeAsyncAction {this.action.Target}.{this.action.GetMethodInfo().Name}";
    }
}
#endif