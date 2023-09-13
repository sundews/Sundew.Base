// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposingList.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Disposal;

using global::Disposal.Interfaces;

/// <summary>
/// An ordered list of disposables.
/// </summary>
public sealed class DisposingList : DisposingList<object>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DisposingList" /> class.
    /// </summary>
    public DisposingList()
        : this(false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisposingList" /> class.
    /// </summary>
    /// <param name="concurrentDisposal">if set to <c>true</c>  disposal will be executed concurrently.</param>
    /// <param name="disposalReporter">The disposal reporter.</param>
    public DisposingList(bool concurrentDisposal, IDisposalReporter? disposalReporter = null)
    : base(concurrentDisposal, disposalReporter)
    {
    }
}