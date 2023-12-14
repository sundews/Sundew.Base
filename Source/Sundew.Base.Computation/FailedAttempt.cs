// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FailedAttempt.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Computation;

using System;

/// <summary>
/// A failed attempt.
/// </summary>
public readonly struct FailedAttempt
{
    internal FailedAttempt(int attempt, int maxAttempts, Exception exception)
    {
        this.Attempt = attempt;
        this.MaxAttempts = maxAttempts;
        this.Exception = exception;
    }

    /// <summary>
    /// Gets the attempt number.
    /// </summary>
    public int Attempt { get; }

    /// <summary>
    /// Gets the max attempt.
    /// </summary>
    public int MaxAttempts { get; }

    /// <summary>
    /// Gets the exception.
    /// </summary>
    public Exception Exception { get; }
}