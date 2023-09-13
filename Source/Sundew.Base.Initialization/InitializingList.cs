// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializingList.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Initialization;

using global::Initialization.Interfaces;

/// <summary>
/// An ordered list of <see cref="IInitializable"/> or <see cref="IAsyncInitializable"/>.
/// </summary>
public sealed class InitializingList : InitializingList<object>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InitializingList" /> class.
    /// </summary>
    public InitializingList()
     : this(false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InitializingList" /> class.
    /// </summary>
    /// <param name="concurrentInitialization">if set to <c>true</c>  initialization will be executed concurrently.</param>
    /// <param name="initializationReporter">The initializable reporter.</param>
    public InitializingList(bool concurrentInitialization, IInitializationReporter? initializationReporter = null)
        : base(concurrentInitialization, initializationReporter)
    {
    }
}