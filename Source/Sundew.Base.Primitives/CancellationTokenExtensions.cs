// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CancellationTokenExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base;

using System;
using System.Threading;

/// <summary>
/// Cancellation token extensions.
/// </summary>
public static class CancellationTokenExtensions
{
    /// <summary>
    /// Create a cancellation with the specified <see cref="CancellationToken"/> and timeout.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>The cancellation.</returns>
    public static Cancellation ToCancellationWithTimeout(this CancellationToken cancellationToken, TimeSpan timeout)
    {
        return new Cancellation(timeout, cancellationToken);
    }

    /// <summary>
    /// Create a cancellation with the specified <see cref="CancellationTokenSource"/> and timeout.
    /// </summary>
    /// <param name="cancellationTokenSource">The cancellation token source.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>The cancellation.</returns>
    public static Cancellation ToCancellationWithTimeout(this CancellationTokenSource cancellationTokenSource, TimeSpan timeout)
    {
        return new Cancellation(timeout, cancellationTokenSource.Token);
    }
}