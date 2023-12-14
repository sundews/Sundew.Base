// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FailedAttempt{TError}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Computation;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// A failed attempt.
/// </summary>
/// <typeparam name="TError">The error type.</typeparam>
public readonly struct FailedAttempt<TError>
{
    internal FailedAttempt(int attempt, int maxAttempts, TError? error, Exception? exception)
    {
        this.Attempt = attempt;
        this.MaxAttempts = maxAttempts;
        this.Error = error;
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
    /// Gets the error.
    /// </summary>
    public TError? Error { get; }

    /// <summary>
    /// Gets the exception.
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    /// Gets a value indicating whether the failure was due to an exception.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Error))]
    [MemberNotNullWhen(true, nameof(Exception))]
    public bool IsException => this.Exception != null;
}