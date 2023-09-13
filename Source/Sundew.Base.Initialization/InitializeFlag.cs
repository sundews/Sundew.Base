// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializeFlag.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Initialization;

using System;
using System.Threading;

/// <summary>
/// Represents a thread safe initialize flag.
/// </summary>
public sealed class InitializeFlag
{
    private int flag;

    /// <summary>
    /// Occurs when [initialized].
    /// </summary>
    public event EventHandler? Initialized;

    /// <summary>
    /// Gets a value indicating whether this instance is initialized.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
    /// </value>
    public bool IsInitialized => this.flag == 1;

    /// <summary>
    /// Performs an implicit conversion from <see cref="InitializeFlag" /> to <see cref="bool" />.
    /// </summary>
    /// <param name="initializeFlag">The interlocked boolean.</param>
    /// <value>
    ///   <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
    /// </value>
    public static implicit operator bool(InitializeFlag initializeFlag)
    {
        return initializeFlag.IsInitialized;
    }

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <returns>A value indicating whether the flag was just initialized.</returns>
    public bool Initialize()
    {
        var result = Interlocked.Exchange(ref this.flag, 1) == 0;
        if (result)
        {
            this.Initialized?.Invoke(this, EventArgs.Empty);
        }

        return result;
    }
}