// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReporterExtensions.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Reporting
{
    /// <summary>
    /// Extends <see cref="IReporter"/> with easy to use methods.
    /// </summary>
    public static class ReporterExtensions
    {
        /// <summary>
        /// Sets the source.
        /// </summary>
        /// <typeparam name="TReporter">The type of the reporter.</typeparam>
        /// <param name="reporter">The reporter.</param>
        /// <param name="source">The source.</param>
        public static void SetSource<TReporter>(this TReporter reporter, object source)
            where TReporter : IReporter
        {
            reporter.SetSource(typeof(TReporter), source);
        }
    }
}