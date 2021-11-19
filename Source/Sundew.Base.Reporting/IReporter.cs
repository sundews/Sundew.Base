// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReporter.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Reporting;

using System;

/// <summary>
/// Interface for implementing a reporter.
/// </summary>
public interface IReporter
{
    /// <summary>
    /// Sets the source.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="source">The source.</param>
    void SetSource(Type target, object source);
}