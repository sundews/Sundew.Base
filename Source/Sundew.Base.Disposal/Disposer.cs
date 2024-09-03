// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Disposer.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Disposal;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using global::Disposal.Interfaces;
using Sundew.Base.Collections;

/// <summary>
/// Abstract base class for wrapping <see cref="IDisposable"/>.
/// </summary>
public abstract class Disposer : IEquatable<Disposer>
{
    /// <summary>
    /// Checks this instance for equality against the other.
    /// </summary>
    /// <param name="left">The left hand side.</param>
    /// <param name="right">The right hand side.</param>
    /// <returns><c>true</c>, if the two instances are equal, otherwise <c>false</c>.</returns>
    public static bool operator ==(Disposer? left, Disposer? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// Checks this instance for inequality against the other.
    /// </summary>
    /// <param name="left">The left hand side.</param>
    /// <param name="right">The right hand side.</param>
    /// <returns><c>true</c>, if the two instances are different, otherwise <c>false</c>.</returns>
    public static bool operator !=(Disposer? left, Disposer? right)
    {
        return !Equals(left, right);
    }

    /// <summary>
    /// Tries to get a Disposer from the candidate.
    /// </summary>
    /// <param name="candidate">The candidate.</param>
    /// <returns>An <see cref="Disposer"/> or null.</returns>
    public static Disposer? TryGet(object candidate)
    {
        var disposable = candidate as IDisposable;
#if !NETSTANDARD1_3
        var asyncDisposable = candidate as IAsyncDisposable;
        if (asyncDisposable != null && disposable != null)
        {
            return new Deferred(disposable, asyncDisposable);
        }

        if (asyncDisposable != null)
        {
            return new Asynchronous(asyncDisposable);
        }
#endif

        if (disposable != null)
        {
            return new Synchronous(disposable);
        }

        return null;
    }

    /// <summary>
    /// Disposes the disposer.
    /// </summary>
    /// <param name="disposalReporter">The disposal reporter.</param>
    public abstract void Dispose(IDisposalReporter? disposalReporter);

    /// <summary>
    /// Disposes the disposer async.
    /// </summary>
    /// <param name="disposalReporter">The disposal reporter.</param>
    /// <returns>A value task.</returns>
    public abstract ValueTask DisposeAsync(IDisposalReporter? disposalReporter);

    /// <summary>
    /// Checks this instance for equality against the other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c>, if the two instances are equal, otherwise <c>false</c>.</returns>
    public abstract bool Equals(Disposer? other);

    /// <summary>
    /// Checks this instance for equality against the other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c>, if the two instances are equal, otherwise <c>false</c>.</returns>
    public override bool Equals(object? other)
    {
        return ReferenceEquals(this, other) || (other is Disposer otherDisposer && this.Equals(otherDisposer));
    }

    /// <summary>
    /// Gets the hashcode.
    /// </summary>
    /// <returns>The hashcode.</returns>
    public override int GetHashCode()
    {
        return RuntimeHelpers.GetHashCode(this);
    }

#if !NETSTANDARD1_3
    /// <summary>
    /// Represents an async disposable.
    /// </summary>
    public sealed class Asynchronous : Disposer
    {
        private readonly IAsyncDisposable asyncDisposable;

        /// <summary>
        /// Initializes a new instance of the <see cref="Disposal.Disposer.Asynchronous"/> class.
        /// </summary>
        /// <param name="asyncDisposable">The disposable.</param>
        public Asynchronous(IAsyncDisposable asyncDisposable)
        {
            this.asyncDisposable = asyncDisposable;
        }

        /// <summary>
        /// Disposes the disposable.
        /// </summary>
        /// <param name="disposalReporter">The disposal reporter.</param>
        public override void Dispose(IDisposalReporter? disposalReporter)
        {
            var valueTask = this.asyncDisposable.DisposeAsync();
            if (valueTask.IsCompleted)
            {
                disposalReporter?.Disposed(this.GetType(), this.asyncDisposable);
                return;
            }

            valueTask.AsTask().Wait();
            disposalReporter?.Disposed(this.GetType(), this.asyncDisposable);
        }

        /// <summary>
        /// Disposes the disposable async.
        /// </summary>
        /// <param name="disposalReporter">The disposal reporter.</param>
        /// <returns>A value task.</returns>
        public override async ValueTask DisposeAsync(IDisposalReporter? disposalReporter)
        {
            await this.asyncDisposable.DisposeAsync().ConfigureAwait(false);
            disposalReporter?.Disposed(this.GetType(), this.asyncDisposable);
        }

        /// <summary>
        /// Check this instance for equality against the other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c>, if the two instances are equal, otherwise <c>false</c>.</returns>
        public override bool Equals(Disposer? other)
        {
            return other is Asynchronous asynchronous && this.asyncDisposable.Equals(asynchronous.asyncDisposable);
        }

        /// <summary>
        /// Gets the hashcode.
        /// </summary>
        /// <returns>The hashcode.</returns>
        public override int GetHashCode()
        {
            return this.asyncDisposable.GetHashCode();
        }
    }
#endif

    /// <summary>
    /// Represents a synchronous disposable.
    /// </summary>
    public sealed class Synchronous : Disposer
    {
        private readonly IDisposable disposable;

        /// <summary>
        /// Initializes a new instance of the <see cref="Disposal.Disposer.Synchronous"/> class.
        /// </summary>
        /// <param name="disposable">The disposable.</param>
        public Synchronous(IDisposable disposable)
        {
            this.disposable = disposable;
        }

        /// <summary>
        /// Disposes the disposable.
        /// </summary>
        /// <param name="disposalReporter">The disposal reporter.</param>
        public override void Dispose(IDisposalReporter? disposalReporter)
        {
            this.disposable.Dispose();
            disposalReporter?.Disposed(this.GetType(), this.disposable);
        }

        /// <summary>
        /// Disposes the disposable async.
        /// </summary>
        /// <param name="disposalReporter">The disposal reporter.</param>
        /// <returns>A value task.</returns>
        public override ValueTask DisposeAsync(IDisposalReporter? disposalReporter)
        {
            this.disposable.Dispose();
            disposalReporter?.Disposed(this.GetType(), this.disposable);
            return default;
        }

        /// <summary>
        /// Check this instance for equality against the other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c>, if the two instances are equal, otherwise <c>false</c>.</returns>
        public override bool Equals(Disposer? other)
        {
            return other is Synchronous synchronous && this.disposable.Equals(synchronous.disposable);
        }

        /// <summary>
        /// Gets the hashcode.
        /// </summary>
        /// <returns>The hashcode.</returns>
        public override int GetHashCode()
        {
            return this.disposable.GetHashCode();
        }
    }

    /// <summary>
    /// Represents a list of disposers.
    /// </summary>
    public sealed class Disposers : Disposer
    {
        private readonly IReadOnlyCollection<Disposer> disposers;

        /// <summary>
        /// Initializes a new instance of the <see cref="Disposer.Disposers"/> class.
        /// </summary>
        /// <param name="disposables">The asyncDisposables.</param>
        public Disposers(params Disposer[] disposables)
            : this(disposables.ToReadOnlyCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Disposal.Disposer.Disposers"/> class.
        /// </summary>
        /// <param name="disposers">The disposers.</param>
        public Disposers(IEnumerable<Disposer> disposers)
            : this(disposers.ToReadOnlyCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Disposal.Disposer.Disposers"/> class.
        /// </summary>
        /// <param name="disposers">The disposers.</param>
        public Disposers(IReadOnlyCollection<Disposer> disposers)
        {
            this.disposers = disposers;
        }

        /// <summary>
        /// Disposes the asyncDisposables.
        /// </summary>
        /// <param name="disposalReporter">The disposal reporter.</param>
        public override void Dispose(IDisposalReporter? disposalReporter)
        {
            foreach (var disposer in this.disposers)
            {
                disposer.Dispose(disposalReporter);
            }
        }

        /// <summary>
        /// Disposes the disposers async.
        /// </summary>
        /// <param name="disposalReporter">The disposal reporter.</param>
        /// <returns>A value task.</returns>
        public override async ValueTask DisposeAsync(IDisposalReporter? disposalReporter)
        {
            foreach (var disposer in this.disposers)
            {
                await disposer.DisposeAsync(disposalReporter).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Check this instance for equality against the other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c>, if the two instances are equal, otherwise <c>false</c>.</returns>
        public override bool Equals(Disposer? other)
        {
            return other is Disposers otherDisposers && this.disposers.Equals(otherDisposers.disposers);
        }

        /// <summary>
        /// Gets the hashcode.
        /// </summary>
        /// <returns>The hashcode.</returns>
        public override int GetHashCode()
        {
            return this.disposers.GetHashCode();
        }
    }

    /// <summary>
    /// Represents a list of disposers.
    /// </summary>
    public sealed class SynchronousDisposables : Disposer
    {
        private readonly IReadOnlyCollection<IDisposable> disposables;

        /// <summary>
        /// Initializes a new instance of the <see cref="Disposer.SynchronousDisposables"/> class.
        /// </summary>
        /// <param name="disposables">The asyncDisposables.</param>
        public SynchronousDisposables(params IDisposable[] disposables)
            : this(disposables.ToReadOnlyCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Disposer.SynchronousDisposables"/> class.
        /// </summary>
        /// <param name="disposables">The asyncDisposables.</param>
        public SynchronousDisposables(IEnumerable<IDisposable> disposables)
            : this(disposables.ToReadOnlyCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Disposer.SynchronousDisposables"/> class.
        /// </summary>
        /// <param name="disposables">The asyncDisposables.</param>
        public SynchronousDisposables(IReadOnlyCollection<IDisposable> disposables)
        {
            this.disposables = disposables;
        }

        /// <summary>
        /// Disposes the disposers.
        /// </summary>
        /// <param name="disposalReporter">The disposable reporter.</param>
        public override void Dispose(IDisposalReporter? disposalReporter)
        {
            foreach (var disposable in this.disposables)
            {
                disposable.Dispose();
                disposalReporter?.Disposed(this.GetType(), disposable);
            }
        }

        /// <summary>
        /// Disposes the disposers async.
        /// </summary>
        /// <param name="disposalReporter">The disposal reporter.</param>
        /// <returns>A value task.</returns>
        public override ValueTask DisposeAsync(IDisposalReporter? disposalReporter)
        {
            foreach (var disposable in this.disposables)
            {
                disposable.Dispose();
                disposalReporter?.Disposed(this.GetType(), disposable);
            }

            return default;
        }

        /// <summary>
        /// Check this instance for equality against the other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c>, if the two instances are equal, otherwise <c>false</c>.</returns>
        public override bool Equals(Disposer? other)
        {
            return other is SynchronousDisposables synchronousDisposables && this.disposables.Equals(synchronousDisposables.disposables);
        }

        /// <summary>
        /// Gets the hashcode.
        /// </summary>
        /// <returns>The hashcode.</returns>
        public override int GetHashCode()
        {
            return this.disposables.GetHashCode();
        }
    }

#if !NETSTANDARD1_3
    /// <summary>
    /// Represents a list of disposers.
    /// </summary>
    public sealed class AsynchronousDisposables : Disposer
    {
        private readonly IEnumerable<IAsyncDisposable> asyncDisposables;

        /// <summary>
        /// Initializes a new instance of the <see cref="Disposer.AsynchronousDisposables"/> class.
        /// </summary>
        /// <param name="asyncDisposables">The asyncDisposables.</param>
        public AsynchronousDisposables(params IAsyncDisposable[] asyncDisposables)
            : this((IEnumerable<IAsyncDisposable>)asyncDisposables)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Disposer.AsynchronousDisposables"/> class.
        /// </summary>
        /// <param name="asyncDisposables">The asyncDisposables.</param>
        public AsynchronousDisposables(IEnumerable<IAsyncDisposable> asyncDisposables)
        {
            this.asyncDisposables = asyncDisposables;
        }

        /// <summary>
        /// Disposes the disposers.
        /// </summary>
        /// <param name="disposalReporter">The disposal reporter.</param>
        public override void Dispose(IDisposalReporter? disposalReporter)
        {
            foreach (var asyncDisposable in this.asyncDisposables)
            {
                var valueTask = asyncDisposable.DisposeAsync();
                if (valueTask.IsCompleted)
                {
                    disposalReporter?.Disposed(this.GetType(), asyncDisposable);
                    return;
                }

                valueTask.AsTask().Wait();
                disposalReporter?.Disposed(this.GetType(), asyncDisposable);
            }
        }

        /// <summary>
        /// Disposes the disposers async.
        /// </summary>
        /// <param name="disposalReporter">The disposal reporter.</param>
        /// <returns>A value task.</returns>
        public override async ValueTask DisposeAsync(IDisposalReporter? disposalReporter)
        {
            foreach (var asyncDisposable in this.asyncDisposables)
            {
                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                disposalReporter?.Disposed(this.GetType(), asyncDisposable);
            }
        }

        /// <summary>
        /// Check this instance for equality against the other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c>, if the two instances are equal, otherwise <c>false</c>.</returns>
        public override bool Equals(Disposer? other)
        {
            return other is AsynchronousDisposables asynchronousDisposables && this.asyncDisposables.Equals(asynchronousDisposables.asyncDisposables);
        }

        /// <summary>
        /// Gets the hashcode.
        /// </summary>
        /// <returns>The hashcode.</returns>
        public override int GetHashCode()
        {
            return this.asyncDisposables.GetHashCode();
        }
    }

    private sealed class Deferred : Disposer
    {
        private readonly IDisposable disposable;
        private readonly IAsyncDisposable asyncDisposable;

        internal Deferred(IDisposable disposable, IAsyncDisposable asyncDisposable)
        {
            this.disposable = disposable;
            this.asyncDisposable = asyncDisposable;
            if (!ReferenceEquals(this.disposable, this.asyncDisposable))
            {
                throw new ArgumentException($"The two arguments: {disposable} and {asyncDisposable} must be the same instance.", nameof(asyncDisposable));
            }
        }

        /// <summary>
        /// Disposes the disposers.
        /// </summary>
        /// <param name="disposalReporter">The disposable reporter.</param>
        public override void Dispose(IDisposalReporter? disposalReporter)
        {
            this.disposable.Dispose();
            disposalReporter?.Disposed(this.GetType(), this.disposable);
        }

        /// <summary>
        /// Disposes the disposers async.
        /// </summary>
        /// <param name="disposalReporter">The disposable reporter.</param>
        /// <returns>A value task.</returns>
        public override async ValueTask DisposeAsync(IDisposalReporter? disposalReporter)
        {
            await this.asyncDisposable.DisposeAsync().ConfigureAwait(false);
            disposalReporter?.Disposed(this.GetType(), this.disposable);
        }

        /// <summary>
        /// Check this instance for equality against the other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c>, if the two instances are equal, otherwise <c>false</c>.</returns>
        public override bool Equals(Disposer? other)
        {
            return other is Deferred deferred && this.asyncDisposable.Equals(deferred.asyncDisposable) && this.disposable.Equals(deferred.disposable);
        }

        /// <summary>
        /// Gets the hashcode.
        /// </summary>
        /// <returns>The hashcode.</returns>
        public override int GetHashCode()
        {
            return this.disposable.GetHashCode();
        }
    }
#endif
}