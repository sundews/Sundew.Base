// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeoutCancellationToken.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base;

using System;
using System.Threading;

/// <summary>
/// A timeout cancellation token used to pass timeout into a cancellation token.
/// </summary>
public readonly struct TimeoutCancellationToken
{
    private readonly CancellationTokenSource cancellationTokenSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeoutCancellationToken"/> struct.
    /// </summary>
    /// <param name="timeoutMilliseconds">The timeoutMilliseconds.</param>
    public TimeoutCancellationToken(int timeoutMilliseconds)
        : this(TimeSpan.FromMilliseconds(timeoutMilliseconds))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeoutCancellationToken"/> struct.
    /// </summary>
    /// <param name="timeout">The timeout.</param>
    public TimeoutCancellationToken(TimeSpan timeout)
    {
        this.Timeout = timeout;
        this.cancellationTokenSource = new CancellationTokenSource(timeout);
        var token = this;
        this.cancellationTokenSource.Token.Register(_ => token.Dispose(), __._);
    }

    private TimeoutCancellationToken(CancellationToken cancellationToken)
    {
        this.Timeout = System.Threading.Timeout.InfiniteTimeSpan;
        this.cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var token = this;
        this.cancellationTokenSource.Token.Register(_ => token.Dispose(), __._);
    }

    /// <summary>
    /// Gets a token that never cancels.
    /// </summary>
    public static TimeoutCancellationToken None => default;

    /// <summary>
    /// Gets the timeout.
    /// </summary>
    public TimeSpan Timeout { get; }

    /// <summary>
    /// Gets the token.
    /// </summary>
    public CancellationToken Token => this.cancellationTokenSource?.Token ?? CancellationToken.None;

    /// <summary>
    /// Converts the <see cref="TimeoutCancellationToken"/> into a regular <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="timeoutCancellationToken">The timeout cancellation token.</param>
    public static implicit operator CancellationToken(TimeoutCancellationToken timeoutCancellationToken)
    {
        return timeoutCancellationToken.Token;
    }

    /// <summary>
    /// Converts the <see cref="TimeSpan"/> into a <see cref="TimeoutCancellationToken"/>.
    /// </summary>
    /// <param name="timeout">The timeout.</param>
    public static implicit operator TimeoutCancellationToken(TimeSpan timeout)
    {
        return new TimeoutCancellationToken(timeout);
    }

    /// <summary>
    /// Converts the regular <see cref="CancellationToken"/> into a <see cref="TimeoutCancellationToken"/>.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static implicit operator TimeoutCancellationToken(CancellationToken cancellationToken)
    {
        return cancellationToken == CancellationToken.None ? default : new TimeoutCancellationToken(cancellationToken);
    }

    private void Dispose()
    {
        this.cancellationTokenSource?.Dispose();
    }
}