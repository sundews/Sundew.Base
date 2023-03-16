// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAttempt.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives.Computation;

/// <summary>
/// Indicates the current attempt running out of the maximum attempts.
/// </summary>
public interface IAttempt
{
    /// <summary>
    /// Gets the maximum attempts.
    /// </summary>
    /// <value>
    /// The maximum attempts.
    /// </value>
    int MaxAttempts { get; }

    /// <summary>
    /// Gets the current attempt.
    /// </summary>
    /// <value>
    /// The current attempt.
    /// </value>
    int CurrentAttempt { get; }
}