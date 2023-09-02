// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializeAction.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Initialization;

using System;
using System.Reflection;
using global::Initialization.Interfaces;

/// <summary>
/// An action and async action which implements <see cref="IInitializable"/>.
/// </summary>
/// <seealso cref="IInitializable" />
public class InitializeAction : IInitializable
{
    private readonly Action action;

    /// <summary>
    /// Initializes a new instance of the <see cref="InitializeAction"/> class.
    /// </summary>
    /// <param name="action">The action.</param>
    public InitializeAction(Action action)
    {
        this.action = action;
    }

    /// <summary>
    /// Initializes the .
    /// </summary>
    public void Initialize()
    {
        this.action();
    }

    /// <summary>Converts to string.</summary>
    /// <returns>A <see cref="string"/> that represents this instance.</returns>
    public override string ToString()
    {
        return $"InitializeAction {this.action.Target}.{this.action.GetMethodInfo().Name}";
    }
}