// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Progress.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives.Computation;

/// <summary>
/// Contains information about the current progress being made.
/// </summary>
/// <typeparam name="TReport">The type of the report.</typeparam>
public class Progress<TReport>
    where TReport : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Progress{TReport}" /> class.
    /// </summary>
    /// <param name="report">The report.</param>
    /// <param name="hasCompletedAdding">if set to <c>true</c> [has completed adding].</param>
    /// <param name="completedItems">The completed items.</param>
    /// <param name="totalItems">The total items.</param>
    /// <param name="progressType">Type of the progress.</param>
    public Progress(TReport? report, bool hasCompletedAdding, int completedItems, int totalItems, ProgressType progressType)
    {
        this.Report = report;
        this.HasCompletedAdding = hasCompletedAdding;
        this.CompletedItems = completedItems;
        this.TotalItems = totalItems;
        this.ProgressType = progressType;
    }

    /// <summary>
    /// Gets the report.
    /// </summary>
    /// <value>
    /// The report.
    /// </value>
    public TReport? Report { get; }

    /// <summary>
    /// Gets a value indicating whether this instance has completed adding.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance has completed adding; otherwise, <c>false</c>.
    /// </value>
    public bool HasCompletedAdding { get; }

    /// <summary>
    /// Gets the completed items.
    /// </summary>
    /// <value>
    /// The completed items.
    /// </value>
    public int CompletedItems { get; }

    /// <summary>
    /// Gets the total items.
    /// </summary>
    /// <value>
    /// The total items.
    /// </value>
    public int TotalItems { get; }

    /// <summary>
    /// Gets or sets the type of the progress.
    /// </summary>
    /// <value>
    /// The type of the progress.
    /// </value>
    public ProgressType ProgressType { get; set; }

    /// <summary>
    /// Gets a value indicating whether has progress changed.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance has progress changed; otherwise, <c>false</c>.
    /// </value>
    public bool HasProgressChanged => this.ProgressType == ProgressType.ItemCompleted || this.ProgressType == ProgressType.ItemsAdded;

    /// <summary>
    /// Gets a value indicating whether is completed.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is completed; otherwise, <c>false</c>.
    /// </value>
    public bool IsCompleted => this.HasCompletedAdding && this.TotalItems > 0 && this.TotalItems == this.CompletedItems;

    /// <summary>
    /// Gets the percentage.
    /// </summary>
    /// <value>
    /// The percentage.
    /// </value>
    public double Percentage => (double)this.CompletedItems / this.TotalItems;
}