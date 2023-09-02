// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializeAsyncAction.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Initialization;

using System;
using System.Reflection;
using System.Threading.Tasks;
using global::Initialization.Interfaces;

/// <summary>
/// An action and async action which implements <see cref="IInitializable"/>.
/// </summary>
/// <seealso cref="IInitializable" />
public class InitializeAsyncAction : IAsyncInitializable
{
    private readonly Func<ValueTask> action;

    /// <summary>
    /// Initializes a new instance of the <see cref="InitializeAsyncAction"/> class.
    /// </summary>
    /// <param name="action">The action.</param>
    public InitializeAsyncAction(Func<ValueTask> action)
    {
        this.action = action;
    }

    /// <summary>
    /// Initializes the asynchronous.
    /// </summary>
    /// <returns>
    /// An async task.
    /// </returns>
    public ValueTask InitializeAsync()
    {
        return this.action();
    }

    /// <summary>Converts to string.</summary>
    /// <returns>A <see cref="string"/> that represents this instance.</returns>
    public override string ToString()
    {
        return $"InitializeAsyncAction {this.action.Target}.{this.action.GetMethodInfo().Name}";
    }
}