// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProgressReporter.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives.Computation;

/// <summary>
/// Interface for implementing a progress reporter.
/// </summary>
/// <typeparam name="TReport">The type of the report.</typeparam>
public interface IProgressReporter<TReport>
    where TReport : class
{
    /// <summary>
    /// Reports the specified progress.
    /// </summary>
    /// <param name="progress">The progress.</param>
    void Report(Progress<TReport> progress);
}