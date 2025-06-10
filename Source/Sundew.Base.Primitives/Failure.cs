// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Failure.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base;

using System;
using Sundew.DiscriminatedUnions;

/// <summary>
/// Represents a failed result.
/// </summary>
/// <typeparam name="TFailure"></typeparam>
[DiscriminatedUnion]
public abstract partial record Failure<TFailure>
{
    /// <summary>
    /// Represents a failure result with an associated reason.
    /// </summary>
    /// <remarks>This type is used to encapsulate a failure scenario, providing a strongly-typed reason for
    /// the failure. It is commonly used in result-based patterns to distinguish between success and failure
    /// outcomes.</remarks>
    /// <param name="Reason">The reason.</param>
    public sealed record Failed(TFailure Reason) : Failure<TFailure>;

    /// <summary>
    /// Represents a failure state caused by a cancellation, including the reason for the cancellation.
    /// </summary>
    /// <remarks>This record is used to encapsulate cancellation-related failures, providing a specific reason
    /// for the cancellation through the <see cref="CancelReason"/> property. It is a specialized type of <see
    /// cref="Failure{TFailure}"/>.</remarks>
    /// <param name="CancelReason">The reason for the cancellation.</param>
    public sealed record Canceled(CancelReason CancelReason) : Failure<TFailure>;

    /// <summary>
    /// Represents a failure that occurred due to an exception.
    /// </summary>
    /// <remarks>This record encapsulates an exception as a failure, providing a way to represent error states
    /// caused by exceptions in a structured manner. It is typically used in scenarios where exceptions need to be
    /// propagated or logged as part of a failure result.</remarks>
    /// <param name="Exception">The exception.</param>
    public sealed record ExceptionOccured(Exception Exception) : Failure<TFailure>;
}