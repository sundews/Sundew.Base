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
/// Represents a cancellation intent combining an external <see cref="CancellationToken"/> with timeout support.
/// All copies of a constructed <see cref="Cancellation"/> share the same underlying core, so enabling cancellation
/// anywhere in a call chain links all participants: cancelling through any <see cref="Enabler"/> cancels them all.
/// The default value represents no cancellation and does not allocate.
/// </summary>
public readonly struct Cancellation
{
    private readonly CancellationIdentity? core;

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
        this.core = new CancellationIdentity(cancellationToken, timeout);
    }

    /// <summary>
    /// Gets a cancellation that never cancels.
    /// </summary>
    public static Cancellation None => default;

    /// <summary>
    /// Gets the timeout. <see cref="System.Threading.Timeout.InfiniteTimeSpan"/> indicates that there is no timeout.
    /// </summary>
    public TimeSpan Timeout => this.core?.Timeout ?? System.Threading.Timeout.InfiniteTimeSpan;

    /// <summary>
    /// Gets the token. Once cancellation has been enabled, this returns the linked token shared by all copies.
    /// </summary>
    public CancellationToken Token => this.core?.GetCurrentToken() ?? CancellationToken.None;

    /// <summary>
    /// Gets a value indicating whether cancellation is requested.
    /// </summary>
    public bool IsCancellationRequested => this.Token.IsCancellationRequested;

    /// <summary>
    /// Gets a value indicating whether cancellation is supported.
    /// </summary>
    public bool CanBeCanceled => this.Token.CanBeCanceled;

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
    /// The timeout is only started by the first enabler; additional enablers share the running deadline.
    /// </summary>
    /// <returns>The enabler.</returns>
    public Enabler EnableCancellation()
    {
        return this.EnableCancellation(true);
    }

    /// <summary>
    /// Create creates a <see cref="Cancellation.Enabler"/> and starts the timeout if specified.
    /// The timeout is only started once; additional enablers share the running deadline.
    /// </summary>
    /// <param name="startTimeout">The start timeout.</param>
    /// <returns>The enabler.</returns>
    public Enabler EnableCancellation(bool startTimeout)
    {
        var actualCore = this.core ?? new CancellationIdentity(CancellationToken.None, System.Threading.Timeout.InfiniteTimeSpan);
        return actualCore.CreateEnabler(startTimeout);
    }

    /// <summary>
    /// Represents a running cancellation. All enablers created from copies of the same <see cref="Cancellation"/>
    /// share one cancellation source: cancelling any enabler cancels them all, and the source is only disposed
    /// when the last enabler has been disposed.
    /// </summary>
    public sealed class Enabler : IDisposable
    {
        private readonly CancellationIdentity cancellationIdentity;
        private readonly CancellationToken cancellationToken;
        private int isDisposed;

        internal Enabler(CancellationIdentity cancellationIdentity, CancellationToken cancellationToken)
        {
            this.cancellationIdentity = cancellationIdentity;
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
        public CancelReason? CancelReason => this.IsCancellationRequested ? this.cancellationIdentity.GetCancelReason() : null;

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
            return this.Register(_ => callback(this.cancellationIdentity.GetCancelReason()), false);
        }

        /// <summary>
        /// Registers a delegate that will be called when this <see cref="T:System.Threading.CancellationToken" /> is canceled.
        /// </summary>
        /// <param name="callback">The delegate to be executed when the <see cref="T:System.Threading.CancellationToken" /> is canceled.</param>
        /// <param name="useSynchronizationContext">A value that indicates whether to capture the current <see cref="T:System.Threading.SynchronizationContext" /> and use it when invoking the <paramref name="callback" />.</param>
        /// <returns>The <see cref="T:System.Threading.CancellationTokenRegistration" /> instance that can be used to unregister the callback.</returns>
        public CancellationTokenRegistration Register(Action<CancelReason> callback, bool useSynchronizationContext)
        {
            return this.Token.Register(_ => callback(this.cancellationIdentity.GetCancelReason()), useSynchronizationContext);
        }

        /// <summary>
        /// Registers a delegate that will be called when this <see cref="T:System.Threading.CancellationToken" /> is canceled.
        /// </summary>
        /// <param name="callback">The delegate to be executed when the <see cref="T:System.Threading.CancellationToken" /> is canceled.</param>
        /// <param name="state">The state to pass to the <paramref name="callback" /> when the delegate is invoked. This may be null.</param>
        /// <returns>The <see cref="T:System.Threading.CancellationTokenRegistration" /> instance that can be used to unregister the callback.</returns>
        public CancellationTokenRegistration Register(Action<CancelReason, object?> callback, object? state)
        {
            return this.Token.Register(x => callback(this.cancellationIdentity.GetCancelReason(), x), state);
        }

        /// <summary>Registers a delegate that will be called when this <see cref="T:System.Threading.CancellationToken">CancellationToken</see> is canceled.</summary>
        /// <param name="callback">The delegate to be executed when the <see cref="T:System.Threading.CancellationToken">CancellationToken</see> is canceled.</param>
        /// <param name="state">The state to pass to the <paramref name="callback" /> when the delegate is invoked.  This may be <see langword="null" />.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="callback" /> is <see langword="null" />.</exception>
        /// <returns>The <see cref="T:System.Threading.CancellationTokenRegistration" /> instance that can be used to unregister the callback.</returns>
        public CancellationTokenRegistration Register(Action<CancelReason, object?, CancellationToken> callback, object? state)
        {
            var externalToken = this.cancellationIdentity.ExternalToken;
            return this.Token.Register(x => callback(this.cancellationIdentity.GetCancelReason(), x, externalToken), state, false);
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
            return this.Token.Register(actualState => callback(this.cancellationIdentity.GetCancelReason(), actualState), state, useSynchronizationContext);
        }

        /// <summary>
        /// Requests cancellation. All enablers sharing the same <see cref="Cancellation"/> observe the cancellation.
        /// </summary>
        /// <returns><c>true</c>, if cancellation was requested, otherwise <c>false</c>.</returns>
        public bool Cancel()
        {
            return this.cancellationIdentity.Cancel();
        }

#if NET7_0_OR_GREATER
        /// <summary>
        /// Requests cancellation. All enablers sharing the same <see cref="Cancellation"/> observe the cancellation.
        /// </summary>
        /// <returns><c>true</c>, if cancellation was requested, otherwise <c>false</c>.</returns>
        public System.Threading.Tasks.Task<bool> CancelAsync()
        {
            return this.cancellationIdentity.CancelAsync();
        }

#endif

        /// <summary>
        /// Requests cancellation after the specified time span.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <returns><c>true</c>, if cancellation was requested, otherwise <c>false</c>.</returns>
        public bool CancelAfter(TimeSpan timeSpan)
        {
            return this.cancellationIdentity.CancelAfter(timeSpan);
        }

        /// <summary>
        /// Releases this enabler. The shared <see cref="CancellationTokenSource"/> is disposed when the last enabler is released.
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.Exchange(ref this.isDisposed, 1) == 0)
            {
                this.cancellationIdentity.Release();
            }
        }
    }

    internal sealed class CancellationIdentity
    {
        private const int InternalCancelReason = (int)Base.CancelReason.Internal;
        private const int NoCancelReason = -1;

        private readonly object lockObject = new();
        private readonly CancellationToken externalCancellationToken;

        private CancellationTokenSource? cancellationTokenSource;
        private int enablerCount;
        private bool isTimeoutStarted;
        private int cancelReason = NoCancelReason;

        public CancellationIdentity(CancellationToken externalCancellationToken, TimeSpan timeout)
        {
            this.externalCancellationToken = externalCancellationToken;
            this.Timeout = timeout;
        }

        public TimeSpan Timeout { get; }

        public CancellationToken ExternalToken => this.externalCancellationToken;

        public CancellationToken GetCurrentToken()
        {
            lock (this.lockObject)
            {
                return this.cancellationTokenSource?.Token ?? this.externalCancellationToken;
            }
        }

        public Enabler CreateEnabler(bool startTimeout)
        {
            CancellationToken token;
            lock (this.lockObject)
            {
                this.enablerCount++;
                if (this.cancellationTokenSource == null)
                {
                    this.cancellationTokenSource = this.externalCancellationToken.CanBeCanceled
                        ? CancellationTokenSource.CreateLinkedTokenSource(this.externalCancellationToken)
                        : new CancellationTokenSource();
                    this.cancelReason = NoCancelReason;
                }

                if (startTimeout && !this.isTimeoutStarted && this.Timeout != System.Threading.Timeout.InfiniteTimeSpan)
                {
                    this.cancellationTokenSource.CancelAfter(this.Timeout);
                    this.isTimeoutStarted = true;
                }

                token = this.cancellationTokenSource.Token;
            }

            return new Enabler(this, token);
        }

        public bool Cancel()
        {
            var cancellationTokenSource = this.PrepareCancel();
            if (!cancellationTokenSource.HasValue)
            {
                return false;
            }

            try
            {
                cancellationTokenSource.Cancel();
                return true;
            }
            catch (ObjectDisposedException)
            {
                // The last enabler was disposed concurrently, meaning the operation the source guarded has already completed.
                return false;
            }
        }

#if NET7_0_OR_GREATER
        public async System.Threading.Tasks.Task<bool> CancelAsync()
        {
            var cancellationTokenSource = this.PrepareCancel();
            if (!cancellationTokenSource.HasValue)
            {
                return false;
            }

            try
            {
                await cancellationTokenSource.CancelAsync().ConfigureAwait(false);
                return true;
            }
            catch (ObjectDisposedException)
            {
                // The last enabler was disposed concurrently, meaning the operation the source guarded has already completed.
                return false;
            }
        }
#endif

        public bool CancelAfter(TimeSpan timeSpan)
        {
            CancellationTokenSource? cancellationTokenSource;
            lock (this.lockObject)
            {
                cancellationTokenSource = this.cancellationTokenSource;
            }

            if (!cancellationTokenSource.HasValue)
            {
                return false;
            }

            try
            {
                cancellationTokenSource.CancelAfter(timeSpan);
                return true;
            }
            catch (ObjectDisposedException)
            {
                // The last enabler was disposed concurrently, meaning the operation the source guarded has already completed.
                return false;
            }
        }

        public void Release()
        {
            CancellationTokenSource? cancellationTokenSourceToDispose = null;
            lock (this.lockObject)
            {
                this.enablerCount--;
                if (this.enablerCount == 0)
                {
                    cancellationTokenSourceToDispose = this.cancellationTokenSource;
                    this.cancellationTokenSource = null;
                    this.isTimeoutStarted = false;
                }
            }

            cancellationTokenSourceToDispose?.Dispose();
        }

        public CancelReason GetCancelReason()
        {
            if (this.externalCancellationToken.IsCancellationRequested)
            {
                return Base.CancelReason.External;
            }

            var currentCancelReason = Volatile.Read(ref this.cancelReason);
            return currentCancelReason == NoCancelReason ? Base.CancelReason.Timeout : (CancelReason)currentCancelReason;
        }

        private CancellationTokenSource? PrepareCancel()
        {
            if (Interlocked.CompareExchange(ref this.cancelReason, InternalCancelReason, NoCancelReason) != NoCancelReason)
            {
                return null;
            }

            lock (this.lockObject)
            {
                return this.cancellationTokenSource;
            }
        }
    }
}
