// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgressTracker.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives.Computation
{
    using System;

    /// <summary>
    /// Thread-safe implementation of <see cref="IProgressTracker{TReport}"/>.
    /// </summary>
    /// <typeparam name="TReport">The type of the report.</typeparam>
    /// <seealso cref="IProgressTracker{TMessage}" />
    public class ProgressTracker<TReport> : IProgressTracker<TReport>
        where TReport : class
    {
        private readonly object lockObject = new object();
        private readonly IProgressReporter<TReport>? progressReporter;
        private TReport? currentReport;
        private bool hasCompletedAdding;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressTracker{TReport}"/> class.
        /// </summary>
        public ProgressTracker()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressTracker{TReport}"/> class.
        /// </summary>
        /// <param name="progressReporter">The progress reporter.</param>
        public ProgressTracker(IProgressReporter<TReport>? progressReporter)
        {
            this.progressReporter = progressReporter;
        }

        /// <summary>
        /// Occurs when progress is reported.
        /// </summary>
        public event EventHandler<Progress<TReport>>? ProgressReported;

        /// <summary>
        /// Gets the total items.
        /// </summary>
        /// <value>
        /// The total items.
        /// </value>
        public int TotalItems { get; private set; }

        /// <summary>
        /// Gets the completed items.
        /// </summary>
        /// <value>
        /// The completed items.
        /// </value>
        public int CompletedItems { get; private set; }

        /// <summary>
        /// Adds the specified count to total items.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="report">The report.</param>
        public void AddItems(int count, TReport? report = default)
        {
            this.Notify(() => this.TotalItems += count, report, ProgressType.ItemsAdded);
        }

        /// <summary>
        /// Report completion of adding items.
        /// </summary>
        /// <param name="report">The report.</param>
        public void CompleteAdding(TReport? report = default)
        {
            this.Notify(() => this.hasCompletedAdding = true, report, ProgressType.CompletedAdding);
        }

        /// <summary>
        /// Reports the specified report.
        /// </summary>
        /// <param name="report">The report.</param>
        public void Report(TReport? report)
        {
            this.Notify(null, report, ProgressType.Message);
        }

        /// <summary>
        /// Completes an item.
        /// </summary>
        /// <param name="report">The report.</param>
        public void CompleteItem(TReport? report)
        {
            this.Notify(() => this.CompletedItems++, report, ProgressType.ItemCompleted);
        }

        /// <summary>
        /// Clears the progress.
        /// </summary>
        /// <param name="report">The report.</param>
        public void Clear(TReport? report = null)
        {
            this.Notify(
                () =>
                {
                    this.hasCompletedAdding = false;
                    this.CompletedItems = 0;
                    this.TotalItems = 0;
                },
                report,
                ProgressType.Cleared);
        }

        private void Notify(Action? action, TReport? report, ProgressType progressType)
        {
            var state = this.LockedAction(action, report);
            var aggregatedReport = report as IAggregatedReport<TReport>;
            aggregatedReport?.OnReporting(state.PreviousReport);
            var progress = new Progress<TReport>(state.CurrentReport, state.HasCompletedAdding, state.CompletedItems, state.TotalItems, progressType);
            this.progressReporter?.Report(progress);
            this.ProgressReported?.Invoke(this, progress);
            aggregatedReport?.OnReported(state.PreviousReport);
        }

        private State LockedAction(Action? action, TReport? report)
        {
            lock (this.lockObject)
            {
                action?.Invoke();
                var previousReport = this.currentReport;
                this.currentReport = report;
                return new State(this.TotalItems, this.hasCompletedAdding, this.CompletedItems, previousReport, this.currentReport);
            }
        }

        private class State
        {
            public State(int totalItems, bool hasCompletedAdding, int completedItems, TReport? previousReport, TReport? currentReport)
            {
                this.TotalItems = totalItems;
                this.HasCompletedAdding = hasCompletedAdding;
                this.CompletedItems = completedItems;
                this.PreviousReport = previousReport;
                this.CurrentReport = currentReport;
            }

            public int TotalItems { get; }

            public int CompletedItems { get; }

            public bool HasCompletedAdding { get; }

            public TReport? PreviousReport { get; }

            public TReport? CurrentReport { get; }
        }
    }
}