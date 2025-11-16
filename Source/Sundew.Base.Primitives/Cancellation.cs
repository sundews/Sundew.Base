// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Cancellation.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

/// <summary>
/// A timeout cancellation token used to pass timeout into a cancellation token.
/// </summary>
public struct Cancellation
{
    private readonly CancellationToken externalCancellationToken;
    private readonly CancellationTokenSource? cancellationTokenSource;
    private readonly Flag? consumedFlag = new Flag();

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
        this.externalCancellationToken = cancellationToken;
        this.Timeout = timeout;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Cancellation"/> struct.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="timeout">The timeout.</param>
    /// <param name="cancellationTokenSource">The cancellation token source.</param>
    private Cancellation(CancellationToken cancellationToken, TimeSpan timeout, CancellationTokenSource cancellationTokenSource)
    {
        this.externalCancellationToken = cancellationToken;
        this.Timeout = timeout;
        this.cancellationTokenSource = cancellationTokenSource;
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
    public CancellationToken Token => this.cancellationTokenSource?.Token ?? this.externalCancellationToken;

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
    /// Create creates a <see cref="Cancellation.Enabler"/> and starts the timeout.
    /// </summary>
    /// <returns>The linked cancellation token source.</returns>
    public Enabler EnableCancellation()
    {
        return this.EnableCancellation(true);
    }

    /// <summary>
    /// Create creates a <see cref="Cancellation.Enabler"/> and starts the timeout if specified.
    /// </summary>
    /// <param name="startTimeout">The start timeout.</param>
    /// <returns>The linked cancellation token source.</returns>
    public Enabler EnableCancellation(bool startTimeout)
    {
        if (!this.cancellationTokenSource.HasValue)
        {
            this = new Cancellation(
                this.externalCancellationToken,
                this.consumedFlag.HasValue ? this.Timeout : System.Threading.Timeout.InfiniteTimeSpan,
                this.externalCancellationToken != CancellationToken.None ? CancellationTokenSource.CreateLinkedTokenSource(this.externalCancellationToken) : new CancellationTokenSource());
        }

        if (startTimeout && this.Timeout != System.Threading.Timeout.InfiniteTimeSpan)
        {
            this.cancellationTokenSource?.CancelAfter(this.Timeout);
        }

        return new Enabler(this, this.Token);
    }

    /// <summary>
    /// Represents a running cancellation.
    /// </summary>
    public sealed class Enabler : IDisposable
    {
        private const int InternalCancelReason = (int)Base.CancelReason.Internal;
        private const int NoCancelReason = -1;
        private readonly Cancellation cancellation;
        private readonly CancellationToken cancellationToken;
        private int cancelReason = NoCancelReason;

        /// <summary>
        /// Initializes a new instance of the <see cref="Enabler"/> class.
        /// </summary>
        /// <param name="cancellation">The cancellation.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Enabler(Cancellation cancellation, CancellationToken cancellationToken)
        {
            this.cancellation = cancellation;
            this.cancellationToken = cancellationToken;
        }

        /// <summary>
        /// Gets the token.
        /// </summary>
        public CancellationToken Token => this.cancellationToken;

        /// <summary>
        /// Gets a value indicating whether cancellation is requested.
        /// </summary>
        [MemberNotNullWhen(true, nameof(CancelReason))]
        public bool IsCancellationRequested => this.Token.IsCancellationRequested;

        /// <summary>
        /// Gets the cancel reason if cancellation is requested.
        /// </summary>
        public CancelReason? CancelReason => this.IsCancellationRequested ? this.GetCancelReason(this.cancellation.externalCancellationToken) : null;

        /// <summary>
        /// Gets a value indicating whether cancellation is supported.
        /// </summary>
        public bool CanBeCanceled => this.Token.CanBeCanceled;

        /// <summary>
        /// Converts the <see cref="Cancellation"/> into a regular <see cref="CancellationToken"/>.
        /// </summary>
        /// <param name="enabler">The timeout cancellation token.</param>
        public static implicit operator CancellationToken(Cancellation.Enabler enabler)
        {
            return enabler.Token;
        }

        /// <summary>
        /// Throws an <see cref="T:System.OperationCanceledException" /> if cancellation has been requested for this token.
        /// </summary>
        /// <returns><c>true</c>.</returns>
        public bool ContinueOrThrowIfCancellationRequested()
        {
            this.Token.ThrowIfCancellationRequested();
            return true;
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

        /// <summary>
        /// Registers a delegate that will be called when this <see cref="T:System.Threading.CancellationToken" /> is canceled.
        /// </summary>
        /// <param name="callback">The delegate to be executed when the <see cref="T:System.Threading.CancellationToken" /> is canceled.</param>
        /// <returns>The <see cref="T:System.Threading.CancellationTokenRegistration" /> instance that can be used to unregister the callback.</returns>
        public CancellationTokenRegistration Register(Action<CancelReason> callback)
        {
            var token = this.cancellation.externalCancellationToken;
            return this.Register(_ => callback(this.GetCancelReason(token)), false);
        }

        /// <summary>
        /// Registers a delegate that will be called when this <see cref="T:System.Threading.CancellationToken" /> is canceled.
        /// </summary>
        /// <param name="callback">The delegate to be executed when the <see cref="T:System.Threading.CancellationToken" /> is canceled.</param>
        /// <param name="useSynchronizationContext">A value that indicates whether to capture the current <see cref="T:System.Threading.SynchronizationContext" /> and use it when invoking the <paramref name="callback" />.</param>
        /// <returns>The <see cref="T:System.Threading.CancellationTokenRegistration" /> instance that can be used to unregister the callback.</returns>
        public CancellationTokenRegistration Register(Action<CancelReason> callback, bool useSynchronizationContext)
        {
            var token = this.cancellation.externalCancellationToken;
            return this.Token.Register(_ => callback(this.GetCancelReason(token)), useSynchronizationContext);
        }

        /// <summary>
        /// Registers a delegate that will be called when this <see cref="T:System.Threading.CancellationToken" /> is canceled.
        /// </summary>
        /// <param name="callback">The delegate to be executed when the <see cref="T:System.Threading.CancellationToken" /> is canceled.</param>
        /// <param name="state">The state to pass to the <paramref name="callback" /> when the delegate is invoked. This may be null.</param>
        /// <returns>The <see cref="T:System.Threading.CancellationTokenRegistration" /> instance that can be used to unregister the callback.</returns>
        public CancellationTokenRegistration Register(Action<CancelReason, object?> callback, object? state)
        {
            var token = this.cancellation.externalCancellationToken;
            return this.Token.Register(x => callback(this.GetCancelReason(token), x), state);
        }

        /// <summary>Registers a delegate that will be called when this <see cref="T:System.Threading.CancellationToken">CancellationToken</see> is canceled.</summary>
        /// <param name="callback">The delegate to be executed when the <see cref="T:System.Threading.CancellationToken">CancellationToken</see> is canceled.</param>
        /// <param name="state">The state to pass to the <paramref name="callback" /> when the delegate is invoked.  This may be <see langword="null" />.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="callback" /> is <see langword="null" />.</exception>
        /// <returns>The <see cref="T:System.Threading.CancellationTokenRegistration" /> instance that can be used to unregister the callback.</returns>
        public CancellationTokenRegistration Register(Action<CancelReason, object?, CancellationToken> callback, object? state)
        {
            var token = this.cancellation.externalCancellationToken;
            return this.Token.Register(x => callback(this.GetCancelReason(token), x, token), state, false);
        }

        /// <summary>Registers a delegate that will be called when this <see cref="T:System.Threading.CancellationToken" /> is canceled.</summary>
        /// <param name="callback">The delegate to be executed when the <see cref="T:System.Threading.CancellationToken" /> is canceled.</param>
        /// <param name="state">The state to pass to the <paramref name="callback" /> when the delegate is invoked. This may be null.</param>
        /// <param name="useSynchronizationContext">A Boolean value that indicates whether to capture the current <see cref="T:System.Threading.SynchronizationContext" /> and use it when invoking the <paramref name="callback" />.</param>
        /// <exception cref="T:System.ObjectDisposedException">The associated <see cref="T:System.Threading.CancellationTokenSource" /> has been disposed.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="callback" /> is null.</exception>
        /// <returns>The <see cref="T:System.Threading.CancellationTokenRegistration" /> instance that can be used to unregister the callback.</returns>
        public CancellationTokenRegistration Register(Action<CancelReason, object?> callback, object? state, bool useSynchronizationContext)
        {
            var token = this.cancellation.externalCancellationToken;
            return this.Token.Register(actualState => callback(this.GetCancelReason(token), actualState), state, useSynchronizationContext);
        }

        /// <summary>
        /// Requests cancellation.
        /// </summary>
        /// <returns><c>true</c>, if cancellation was requested, otherwise <c>false</c>.</returns>
        public bool Cancel()
        {
            var result = Interlocked.CompareExchange(ref this.cancelReason, InternalCancelReason, NoCancelReason);
            if (result == NoCancelReason && this.cancellation.cancellationTokenSource.HasValue)
            {
                this.cancellation.cancellationTokenSource.Cancel();
                return true;
            }

            return false;
        }

#if NET7_0_OR_GREATER
        /// <summary>
        /// Requests cancellation.
        /// </summary>
        /// <returns><c>true</c>, if cancellation was requested, otherwise <c>false</c>.</returns>
        public async System.Threading.Tasks.Task<bool> CancelAsync()
        {
            var result = Interlocked.CompareExchange(ref this.cancelReason, InternalCancelReason, NoCancelReason);
            if (result == NoCancelReason && this.cancellation.cancellationTokenSource.HasValue)
            {
                await this.cancellation.cancellationTokenSource.CancelAsync().ConfigureAwait(false);
                return true;
            }

            return false;
        }

#endif

        /// <summary>
        /// Requests cancellation after the specified time span.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <returns><c>true</c>, if cancellation was requested, otherwise <c>false</c>.</returns>
        public bool CancelAfter(TimeSpan timeSpan)
        {
            if (this.cancellation.cancellationTokenSource.HasValue)
            {
                this.cancellation.cancellationTokenSource.CancelAfter(timeSpan);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Dispose the underlying <see cref="CancellationTokenSource"/>.
        /// </summary>
        public void Dispose()
        {
            if (this.cancellation.consumedFlag?.Set() ?? false)
            {
                this.cancellation.cancellationTokenSource?.Dispose();
            }
        }

        private CancelReason GetCancelReason(in CancellationToken externalCancellationToken)
        {
            return externalCancellationToken.IsCancellationRequested ? Base.CancelReason.External : this.cancelReason == NoCancelReason ? Base.CancelReason.Timeout : (CancelReason)this.cancelReason;
        }
    }
}