// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Initializer.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Initialization;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using global::Initialization.Interfaces;

/// <summary>
/// Discriminated union for initializable.
/// </summary>
public abstract class Initializer : IEquatable<Initializer>
{
    /// <summary>
    /// Checks this instance for equality against the other.
    /// </summary>
    /// <param name="left">The left hand side.</param>
    /// <param name="right">The right hand side.</param>
    /// <returns><c>true</c>, if the two instances are equal, otherwise <c>false</c>.</returns>
    public static bool operator ==(Initializer? left, Initializer? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// Checks this instance for inequality against the other.
    /// </summary>
    /// <param name="left">The left hand side.</param>
    /// <param name="right">The right hand side.</param>
    /// <returns><c>true</c>, if the two instances are different, otherwise <c>false</c>.</returns>
    public static bool operator !=(Initializer? left, Initializer? right)
    {
        return !Equals(left, right);
    }

    /// <summary>
    /// Tries to get a Disposer from the candidate.
    /// </summary>
    /// <param name="candidate">The candidate.</param>
    /// <returns>An <see cref="Initializer"/> or null.</returns>
    public static Initializer? TryGet(object candidate)
    {
        var initializable = candidate as IInitializable;
        var asyncInitializable = candidate as IAsyncInitializable;
        if (asyncInitializable != null && initializable != null)
        {
            return new Deferred(initializable, asyncInitializable);
        }

        if (asyncInitializable != null)
        {
            return new Asynchronous(asyncInitializable);
        }

        if (initializable != null)
        {
            return new Synchronous(initializable);
        }

        return null;
    }

    /// <summary>
    /// Checks this instance for equality against the other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c>, if the two instances are equal, otherwise <c>false</c>.</returns>
    public abstract bool Equals(Initializer? other);

    /// <summary>
    /// Checks this instance for equality against the other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c>, if the two instances are equal, otherwise <c>false</c>.</returns>
    public override bool Equals(object? other)
    {
        return ReferenceEquals(this, other) || (other is Initializer otherDisposer && this.Equals(otherDisposer));
    }

    /// <summary>
    /// Gets the hashcode.
    /// </summary>
    /// <returns>The hashcode.</returns>
    public override int GetHashCode()
    {
        return RuntimeHelpers.GetHashCode(this);
    }

    /// <summary>
    /// Initializes the initializable.
    /// </summary>
    /// <param name="initializationReporter">The initialization reporter.</param>
    public abstract void Initialize(IInitializationReporter? initializationReporter);

    /// <summary>
    /// Initializes the initializable async.
    /// </summary>
    /// <param name="initializableReporter">The initialization reporter.</param>
    /// <returns>A value task.</returns>
    public abstract ValueTask InitializeAsync(IInitializationReporter? initializableReporter);

    /// <summary>
    /// Represents an async initializable.
    /// </summary>
    public sealed class Asynchronous : Initializer
    {
        private readonly IAsyncInitializable asyncInitializable;

        /// <summary>
        /// Initializes a new instance of the <see cref="Initialization.Initializer.Asynchronous"/> class.
        /// </summary>
        /// <param name="asyncInitializable">The async initializable.</param>
        public Asynchronous(IAsyncInitializable asyncInitializable)
        {
            this.asyncInitializable = asyncInitializable;
        }

        /// <summary>
        /// Initializes the initializable.
        /// </summary>
        /// <param name="initializationReporter">The initialization reporter.</param>
        public override void Initialize(IInitializationReporter? initializationReporter)
        {
            var valueTask = this.asyncInitializable.InitializeAsync();
            if (valueTask.IsCompleted)
            {
                initializationReporter?.Initialized(this.GetType(), this.asyncInitializable);
                return;
            }

            valueTask.AsTask().Wait();
            initializationReporter?.Initialized(this.GetType(), this.asyncInitializable);
        }

        /// <summary>
        /// Initializes the initializable async.
        /// </summary>
        /// <param name="initializableReporter">The initialization reporter.</param>
        /// <returns>A value task.</returns>
        public override async ValueTask InitializeAsync(IInitializationReporter? initializableReporter)
        {
            await this.asyncInitializable.InitializeAsync().ConfigureAwait(false);
            initializableReporter?.Initialized(this.GetType(), this.asyncInitializable);
        }

        /// <summary>
        /// Check this instance for equality against the other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c>, if the two instances are equal, otherwise <c>false</c>.</returns>
        public override bool Equals(Initializer? other)
        {
            return other is Asynchronous asynchronous && this.asyncInitializable.Equals(asynchronous.asyncInitializable);
        }

        /// <summary>
        /// Gets the hashcode.
        /// </summary>
        /// <returns>The hashcode.</returns>
        public override int GetHashCode()
        {
            return this.asyncInitializable.GetHashCode();
        }
    }

    /// <summary>
    /// Represents an async initializable.
    /// </summary>
    public sealed class Synchronous : Initializer
    {
        private readonly IInitializable initializable;

        /// <summary>
        /// Initializes a new instance of the <see cref="Initialization.Initializer.Synchronous"/> class.
        /// </summary>
        /// <param name="initializable">The initializable.</param>
        public Synchronous(IInitializable initializable)
        {
            this.initializable = initializable;
        }

        /// <summary>
        /// Initializes the initializable.
        /// </summary>
        /// <param name="initializationReporter">The initialization reporter.</param>
        public override void Initialize(IInitializationReporter? initializationReporter)
        {
            this.initializable.Initialize();
            initializationReporter?.Initialized(this.GetType(), this.initializable);
        }

        /// <summary>
        /// Initializes the initializable async.
        /// </summary>
        /// <returns>A value task.</returns>
        /// <param name="initializableReporter">The initialization reporter.</param>
        public override ValueTask InitializeAsync(IInitializationReporter? initializableReporter)
        {
            this.initializable.Initialize();
            initializableReporter?.Initialized(this.GetType(), this.initializable);
            return default;
        }

        /// <summary>
        /// Check this instance for equality against the other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c>, if the two instances are equal, otherwise <c>false</c>.</returns>
        public override bool Equals(Initializer? other)
        {
            return other is Synchronous synchronous && this.initializable.Equals(synchronous.initializable);
        }

        /// <summary>
        /// Gets the hashcode.
        /// </summary>
        /// <returns>The hashcode.</returns>
        public override int GetHashCode()
        {
            return this.initializable.GetHashCode();
        }
    }

    /// <summary>
    /// Represents a list of initializers.
    /// </summary>
    public sealed class Initializers : Initializer
    {
        private readonly IReadOnlyCollection<Initializer> initializers;

        /// <summary>
        /// Initializes a new instance of the <see cref="Initialization.Initializer.Initializers"/> class.
        /// </summary>
        /// <param name="initializers">The initializers.</param>
        public Initializers(IEnumerable<Initializer> initializers)
            : this(initializers.ToReadOnly())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Initialization.Initializer.Initializers"/> class.
        /// </summary>
        /// <param name="initializers">The initializers.</param>
        public Initializers(IReadOnlyCollection<Initializer> initializers)
        {
            this.initializers = initializers;
        }

        /// <summary>
        /// Initializes the initializers.
        /// </summary>
        /// <param name="initializationReporter">The initialization reporter.</param>
        public override void Initialize(IInitializationReporter? initializationReporter)
        {
            foreach (var initializer in this.initializers)
            {
                initializer.Initialize(initializationReporter);
            }
        }

        /// <summary>
        /// Initializes the initializers async.
        /// </summary>
        /// <param name="initializableReporter">The initialization reporter.</param>
        /// <returns>A value task.</returns>
        public override async ValueTask InitializeAsync(IInitializationReporter? initializableReporter)
        {
            foreach (var initializer in this.initializers)
            {
                await initializer.InitializeAsync(initializableReporter).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Check this instance for equality against the other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c>, if the two instances are equal, otherwise <c>false</c>.</returns>
        public override bool Equals(Initializer? other)
        {
            return other is Initializers otherInitializers && this.initializers.Equals(otherInitializers.initializers);
        }

        /// <summary>
        /// Gets the hashcode.
        /// </summary>
        /// <returns>The hashcode.</returns>
        public override int GetHashCode()
        {
            return this.initializers.GetHashCode();
        }
    }

    /// <summary>
    /// Represents a list of initializers.
    /// </summary>
    public sealed class SynchronousInitializables : Initializer
    {
        private readonly IReadOnlyCollection<IInitializable> initializables;

        /// <summary>
        /// Initializes a new instance of the <see cref="Initialization.Initializer.SynchronousInitializables"/> class.
        /// </summary>
        /// <param name="initializables">The asyncInitializables.</param>
        public SynchronousInitializables(IEnumerable<IInitializable> initializables)
            : this(initializables.ToReadOnly())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Initialization.Initializer.SynchronousInitializables"/> class.
        /// </summary>
        /// <param name="initializables">The asyncInitializables.</param>
        public SynchronousInitializables(IReadOnlyCollection<IInitializable> initializables)
        {
            this.initializables = initializables;
        }

        /// <summary>
        /// Initializes the initializers.
        /// </summary>
        /// <param name="initializationReporter">The initialization reporter.</param>
        public override void Initialize(IInitializationReporter? initializationReporter)
        {
            foreach (var initializable in this.initializables)
            {
                initializable.Initialize();
                initializationReporter?.Initialized(this.GetType(), initializable);
            }
        }

        /// <summary>
        /// Initializes the initializers async.
        /// </summary>
        /// <param name="initializableReporter">The initialization reporter.</param>
        /// <returns>A value task.</returns>
        public override ValueTask InitializeAsync(IInitializationReporter? initializableReporter)
        {
            foreach (var initializable in this.initializables)
            {
                initializable.Initialize();
                initializableReporter?.Initialized(this.GetType(), initializable);
            }

            return default;
        }

        /// <summary>
        /// Check this instance for equality against the other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c>, if the two instances are equal, otherwise <c>false</c>.</returns>
        public override bool Equals(Initializer? other)
        {
            return other is SynchronousInitializables synchronousInitializables && this.initializables.Equals(synchronousInitializables.initializables);
        }

        /// <summary>
        /// Gets the hashcode.
        /// </summary>
        /// <returns>The hashcode.</returns>
        public override int GetHashCode()
        {
            return this.initializables.GetHashCode();
        }
    }

    /// <summary>
    /// Represents a list of initializers.
    /// </summary>
    public sealed class AsynchronousInitializables : Initializer
    {
        private readonly IReadOnlyCollection<IAsyncInitializable> asyncInitializables;

        /// <summary>
        /// Initializes a new instance of the <see cref="Initialization.Initializer.AsynchronousInitializables"/> class.
        /// </summary>
        /// <param name="initializables">The asyncInitializables.</param>
        public AsynchronousInitializables(IEnumerable<IAsyncInitializable> initializables)
            : this(initializables.ToReadOnly())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Initialization.Initializer.AsynchronousInitializables"/> class.
        /// </summary>
        /// <param name="asyncInitializables">The asyncInitializables.</param>
        public AsynchronousInitializables(IReadOnlyCollection<IAsyncInitializable> asyncInitializables)
        {
            this.asyncInitializables = asyncInitializables;
        }

        /// <summary>
        /// Initializes the initializers.
        /// </summary>
        /// <param name="initializationReporter">The initialization reporter.</param>
        public override void Initialize(IInitializationReporter? initializationReporter)
        {
            foreach (var asyncInitializable in this.asyncInitializables)
            {
                var valueTask = asyncInitializable.InitializeAsync();
                if (valueTask.IsCompleted)
                {
                    initializationReporter?.Initialized(this.GetType(), asyncInitializable);
                    return;
                }

                valueTask.AsTask().Wait();
                initializationReporter?.Initialized(this.GetType(), asyncInitializable);
            }
        }

        /// <summary>
        /// Initializes the initializers async.
        /// </summary>
        /// <param name="initializableReporter">The initialization reporter.</param>
        /// <returns>A value task.</returns>
        public override async ValueTask InitializeAsync(IInitializationReporter? initializableReporter)
        {
            foreach (var asyncInitializable in this.asyncInitializables)
            {
                await asyncInitializable.InitializeAsync().ConfigureAwait(false);
                initializableReporter?.Initialized(this.GetType(), asyncInitializable);
            }
        }

        /// <summary>
        /// Check this instance for equality against the other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c>, if the two instances are equal, otherwise <c>false</c>.</returns>
        public override bool Equals(Initializer? other)
        {
            return other is AsynchronousInitializables asynchronousInitializables && this.asyncInitializables.Equals(asynchronousInitializables.asyncInitializables);
        }

        /// <summary>
        /// Gets the hashcode.
        /// </summary>
        /// <returns>The hashcode.</returns>
        public override int GetHashCode()
        {
            return this.asyncInitializables.GetHashCode();
        }
    }

    private sealed class Deferred : Initializer
    {
        private readonly IInitializable initializable;
        private readonly IAsyncInitializable asyncInitializable;

        internal Deferred(IInitializable initializable, IAsyncInitializable asyncInitializable)
        {
            this.initializable = initializable;
            this.asyncInitializable = asyncInitializable;
        }

        public override void Initialize(IInitializationReporter? initializationReporter)
        {
            this.initializable.Initialize();
            initializationReporter?.Initialized(this.GetType(), this.initializable);
        }

        public override async ValueTask InitializeAsync(IInitializationReporter? initializableReporter)
        {
            await this.asyncInitializable.InitializeAsync().ConfigureAwait(false);
            initializableReporter?.Initialized(this.GetType(), this.initializable);
        }

        /// <summary>
        /// Check this instance for equality against the other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c>, if the two instances are equal, otherwise <c>false</c>.</returns>
        public override bool Equals(Initializer? other)
        {
            return other is Deferred deferred && this.initializable.Equals(deferred.initializable) && this.asyncInitializable.Equals(deferred.asyncInitializable);
        }

        /// <summary>
        /// Gets the hashcode.
        /// </summary>
        /// <returns>The hashcode.</returns>
        public override int GetHashCode()
        {
            return this.initializable.GetHashCode();
        }
    }
}