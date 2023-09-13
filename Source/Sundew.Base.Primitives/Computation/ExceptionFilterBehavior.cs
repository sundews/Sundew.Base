// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionFilterBehavior.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives.Computation;

/// <summary>
/// Enum to specify whether a list of exception types for an <see cref="ExceptionFilter"/> are consider handled or unhandled.
/// </summary>
public enum ExceptionFilterBehavior
{
    /// <summary>
    /// Specifies that the filter handles the specified exceptions.
    /// </summary>
    Include,

    /// <summary>
    /// Specifies that the filter handles all exceptions except the specified ones.
    /// </summary>
    Exclude,
}