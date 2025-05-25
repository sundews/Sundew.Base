// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportAttribute.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Reporting;

using System;

/// <summary>
/// Specifies metadata for a report, including a message and its relative level.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public class ReportAttribute : Attribute
{
    /// <summary>
    /// Gets the message associated with the report.
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// Gets the level of the report within the scope of a <see cref="IReporter"/> or <see cref="IReporter{TSource}"/>.
    /// </summary>
    public RelativeImportance RelativeImportance { get; init; } = RelativeImportance.Default;
}