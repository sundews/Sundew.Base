// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBufferReporter.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Memory;

using Sundew.Base.Reporting;

/// <summary>
/// Interface for implementing a buffer reporter.
/// </summary>
/// <seealso cref="Sundew.Base.Reporting.IReporter" />
public interface IBufferReporter : IReporter
{
    /// <summary>
    /// Called when a buffer gets expanded.
    /// </summary>
    /// <param name="oldCapacity">The old capacity.</param>
    /// <param name="newCapacity">The new capacity.</param>
    [Report(RelativeImportance = RelativeImportance.Low)]
    void OnExpanding(int oldCapacity, int newCapacity);
}