// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProgressTracker.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives.Computation;

/// <summary>
/// Interface for implmenting a process tracker.
/// </summary>
/// <typeparam name="TReport">The type of the report.</typeparam>
public interface IProgressTracker<in TReport>
    where TReport : class
{
    /// <summary>
    /// Gets the total items.
    /// </summary>
    /// <value>
    /// The total items.
    /// </value>
    int TotalItems { get; }

    /// <summary>
    /// Gets the completed items.
    /// </summary>
    /// <value>
    /// The completed items.
    /// </value>
    int CompletedItems { get; }

    /// <summary>
    /// Reports added the items.
    /// </summary>
    /// <param name="count">The count.</param>
    /// <param name="report">The report.</param>
    void AddItems(int count, TReport? report = null);

    /// <summary>
    /// Report completion of adding items.
    /// </summary>
    /// <param name="report">The report.</param>
    void CompleteAdding(TReport? report = null);

    /// <summary>
    /// Reports the specified report.
    /// </summary>
    /// <param name="report">The report.</param>
    void Report(TReport? report = null);

    /// <summary>
    /// Reports completion of an item.
    /// </summary>
    /// <param name="report">The report.</param>
    void CompleteItem(TReport? report = null);

    /// <summary>
    /// Clears the progress.
    /// </summary>
    /// <param name="report">The report.</param>
    void Clear(TReport? report = null);
}