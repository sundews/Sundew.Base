// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FailedAttempts.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives.Computation;

using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A failed attempt.
/// </summary>
/// <typeparam name="TError">The error type.</typeparam>
public readonly struct FailedAttempts<TError> : IReadOnlyList<FailedAttempt<TError>>
    where TError : notnull
{
    internal FailedAttempts(IReadOnlyList<FailedAttempt<TError>> attempts)
    {
        this.Attempts = attempts;
    }

    /// <summary>
    /// Gets the attempts.
    /// </summary>
    public IReadOnlyList<FailedAttempt<TError>> Attempts { get; }

    /// <summary>
    /// Gets the amount of attempts.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// The failed attempt at the given index.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>The failed attempt.</returns>
    public FailedAttempt<TError> this[int index] => this.Attempts[index];

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns>The enumerator.</returns>
    public IEnumerator<FailedAttempt<TError>> GetEnumerator()
    {
        return this.Attempts.GetEnumerator();
    }

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns>The enumerator.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}