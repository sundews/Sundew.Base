// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAggregatedReport.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives.Computation
{
    /// <summary>
    /// Interface for implementing message actions before and after reporting progress.
    /// </summary>
    /// <typeparam name="TReport">The type of the report.</typeparam>
    public interface IAggregatedReport<in TReport>
        where TReport : class
    {
        /// <summary>
        /// Called when before the progress has been reported.
        /// </summary>
        /// <param name="previousReport">The previous report.</param>
        void OnReporting(TReport? previousReport);

        /// <summary>
        /// Called when after the progress has been reported.
        /// </summary>
        /// <param name="previousReport">The previous report.</param>
        void OnReported(TReport? previousReport);
    }
}
