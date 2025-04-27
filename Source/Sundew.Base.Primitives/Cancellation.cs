// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Cancellation.cs" company="Sundews">
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
public readonly struct Cancellation
{
    private readonly bool wasConstructed;

    /// <summary>
    /// Initializes a new instance of the <see cref="Cancellation"/> struct.
    /// </summary>
    /// <param name="timeoutMilliseconds">The timeoutMilliseconds.</param>
    public Cancellation(int timeoutMilliseconds)
        : this(TimeSpan.FromMilliseconds(timeoutMilliseconds), CancellationToken.None)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Cancellation"/> struct.
    /// </summary>
    /// <param name="timeout">The timeout.</param>
    public Cancellation(TimeSpan timeout)
        : this(timeout, CancellationToken.None)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Cancellation"/> struct.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Cancellation(CancellationToken cancellationToken)
        : this(System.Threading.Timeout.InfiniteTimeSpan, cancellationToken)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Cancellation"/> struct.
    /// </summary>
    /// <param name="timeout">The timeout.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Cancellation(TimeSpan timeout, CancellationToken cancellationToken)
        : this(cancellationToken, timeout)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Cancellation"/> struct.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="timeout">The timeout.</param>
    public Cancellation(CancellationToken cancellationToken, TimeSpan timeout)
    {
        this.Timeout = timeout;
        this.Token = cancellationToken;
        this.wasConstructed = true;
    }

    /// <summary>
    /// Gets a token that never cancels.
    /// </summary>
    public static Cancellation None => default;

    /// <summary>
    /// Gets the timeout.
    /// </summary>
    public TimeSpan Timeout { get; }

    /// <summary>
    /// Gets the token.
    /// </summary>
    public CancellationToken Token { get; }

    /// <summary>
    /// Gets a value indicating whether cancellation is requested.
    /// </summary>
    public bool IsCancellationRequested => this.Token.IsCancellationRequested;

    /// <summary>
    /// Gets a value indicating whether cancellation is supported.
    /// </summary>
    public bool CanBeCanceled => this.Token.CanBeCanceled;

    /// <summary>
    /// Converts the <see cref="Cancellation"/> into a regular <see cref="CancellationToken"/>.
    /// </summary>
    /// <param name="cancellation">The timeout cancellation token.</param>
    public static implicit operator CancellationToken(Cancellation cancellation)
    {
        return cancellation.Token;
    }

    /// <summary>
    /// Converts the <see cref="TimeSpan"/> into a <see cref="Cancellation"/>.
    /// </summary>
    /// <param name="timeout">The timeout.</param>
    public static implicit operator Cancellation(TimeSpan timeout)
    {
        return new Cancellation(timeout, CancellationToken.None);
    }

    /// <summary>
    /// Converts the <see cref="TimeSpan"/> into a <see cref="Cancellation"/>.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static implicit operator Cancellation(CancellationToken cancellationToken)
    {
        return new Cancellation(cancellationToken);
    }

    /// <summary>
    /// Create a linked token source and starts the timeout.
    /// </summary>
    /// <returns>The linked cancellation token source.</returns>
    public CancellationTokenSource CreateLinkedAndTryStartTimeout()
    {
        return this.CreateLinked(true);
    }

    /// <summary>
    /// Create a linked token source and starts the timeout.
    /// </summary>
    /// <param name="startTimeout">The start timeout.</param>
    /// <returns>The linked cancellation token source.</returns>
    public CancellationTokenSource CreateLinked(bool startTimeout)
    {
        var cancellationTokenSource = System.Threading.CancellationTokenSource.CreateLinkedTokenSource(this.Token);
        if (this.wasConstructed && startTimeout && this.Timeout != System.Threading.Timeout.InfiniteTimeSpan)
        {
            cancellationTokenSource.CancelAfter(this.Timeout);
        }

        return cancellationTokenSource;
    }

    /// <summary>
    /// Registers a delegate that will be called when this <see cref="T:System.Threading.CancellationToken" /> is canceled.
    /// </summary>
    /// <param name="callback">The delegate to be executed when the <see cref="T:System.Threading.CancellationToken" /> is canceled.</param>
    /// <returns>The <see cref="T:System.Threading.CancellationTokenRegistration" /> instance that can be used to unregister the callback.</returns>
    public CancellationTokenRegistration Register(Action callback)
    {
        return this.Register(callback, false);
    }

    /// <summary>
    /// Registers a delegate that will be called when this <see cref="T:System.Threading.CancellationToken" /> is canceled.
    /// </summary>
    /// <param name="callback">The delegate to be executed when the <see cref="T:System.Threading.CancellationToken" /> is canceled.</param>
    /// <param name="useSynchronizationContext">A value that indicates whether to capture the current <see cref="T:System.Threading.SynchronizationContext" /> and use it when invoking the <paramref name="callback" />.</param>
    /// <returns>The <see cref="T:System.Threading.CancellationTokenRegistration" /> instance that can be used to unregister the callback.</returns>
    public CancellationTokenRegistration Register(Action callback, bool useSynchronizationContext)
    {
        return this.Token.Register(callback, useSynchronizationContext);
    }

    /// <summary>
    /// Registers a delegate that will be called when this <see cref="T:System.Threading.CancellationToken" /> is canceled.
    /// </summary>
    /// <param name="callback">The delegate to be executed when the <see cref="T:System.Threading.CancellationToken" /> is canceled.</param>
    /// <param name="state">The state to pass to the <paramref name="callback" /> when the delegate is invoked. This may be null.</param>
    /// <returns>The <see cref="T:System.Threading.CancellationTokenRegistration" /> instance that can be used to unregister the callback.</returns>
    public CancellationTokenRegistration Register(Action<object?> callback, object? state)
    {
        return this.Token.Register(callback, state);
    }

    /// <summary>Registers a delegate that will be called when this <see cref="T:System.Threading.CancellationToken">CancellationToken</see> is canceled.</summary>
    /// <param name="callback">The delegate to be executed when the <see cref="T:System.Threading.CancellationToken">CancellationToken</see> is canceled.</param>
    /// <param name="state">The state to pass to the <paramref name="callback" /> when the delegate is invoked.  This may be <see langword="null" />.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="callback" /> is <see langword="null" />.</exception>
    /// <returns>The <see cref="T:System.Threading.CancellationTokenRegistration" /> instance that can be used to unregister the callback.</returns>
    public CancellationTokenRegistration Register(Action<object?, CancellationToken> callback, object? state)
    {
        var token = this;
        return this.Token.Register(x => callback(x, token.Token), state, false);
    }

    /// <summary>Registers a delegate that will be called when this <see cref="T:System.Threading.CancellationToken" /> is canceled.</summary>
    /// <param name="callback">The delegate to be executed when the <see cref="T:System.Threading.CancellationToken" /> is canceled.</param>
    /// <param name="state">The state to pass to the <paramref name="callback" /> when the delegate is invoked. This may be null.</param>
    /// <param name="useSynchronizationContext">A Boolean value that indicates whether to capture the current <see cref="T:System.Threading.SynchronizationContext" /> and use it when invoking the <paramref name="callback" />.</param>
    /// <exception cref="T:System.ObjectDisposedException">The associated <see cref="T:System.Threading.CancellationTokenSource" /> has been disposed.</exception>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="callback" /> is null.</exception>
    /// <returns>The <see cref="T:System.Threading.CancellationTokenRegistration" /> instance that can be used to unregister the callback.</returns>
    public CancellationTokenRegistration Register(Action<object?> callback, object? state, bool useSynchronizationContext)
    {
        return this.Token.Register(callback, state, useSynchronizationContext);
    }
}