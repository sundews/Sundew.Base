// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelativeImportance.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Reporting;

/// <summary>
/// Represents the relative level of a value, typically used to indicate priority or importance.
/// </summary>
public enum RelativeImportance
{
    /// <summary>
    /// Represents reports that are of low priority or importance.
    /// </summary>
    Low = -1,

    /// <summary>
    /// Represents reports that are of default priority or importance.
    /// </summary>
    Default = 0,

    /// <summary>
    /// Represents reports that are of high priority or importance.
    /// </summary>
    High = 1,
}