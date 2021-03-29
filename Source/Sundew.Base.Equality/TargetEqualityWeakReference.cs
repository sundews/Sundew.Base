// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TargetEqualityWeakReference.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Equality
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A weak reference that compares equality by reference on the target.
    /// </summary>
    /// <typeparam name="TTarget">The type of the target.</typeparam>
    public class TargetEqualityWeakReference<TTarget> : IEquatable<TargetEqualityWeakReference<TTarget>>
            where TTarget : class
    {
        private readonly IEqualityComparer<TTarget> equalityComparer;
        private readonly WeakReference<TTarget> weakReference;
        private readonly int hashCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetEqualityWeakReference{TTarget}"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        public TargetEqualityWeakReference([AllowNull] TTarget target)
            : this(target, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetEqualityWeakReference{TTarget}" /> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="trackResurrection">if set to <c>true</c> [track resurrection].</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        public TargetEqualityWeakReference([AllowNull] TTarget target, bool trackResurrection, IEqualityComparer<TTarget>? equalityComparer = null)
        {
            this.equalityComparer = equalityComparer ?? ReferenceEqualityComparer<TTarget>.Instance;
#pragma warning disable CS8604 // Possible null reference argument.
            this.weakReference = new WeakReference<TTarget>(target, trackResurrection);
#pragma warning restore CS8604 // Possible null reference argument.
            this.hashCode = target == null ? 0 : this.equalityComparer.GetHashCode(target);
        }

        /// <summary>
        /// Tries to get the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>A value indicating whether the target is available.</returns>
        public bool TryGetTarget(out TTarget target)
        {
            return this.weakReference.TryGetTarget(out target);
        }

        /// <summary>
        /// Determines whether the other is equal to this instance..
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if equal, otherwise <c>false</c>.</returns>
        public bool Equals(TargetEqualityWeakReference<TTarget>? other)
        {
            return EqualityHelper.Equals(this, other, otherWeakReference =>
            {
                var thisResult = this.weakReference.TryGetTarget(out var target);
                var otherResult = otherWeakReference.TryGetTarget(out var otherTarget);
                return thisResult == otherResult && (!thisResult || this.equalityComparer.Equals(target, otherTarget));
            });
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object? obj)
        {
            return EqualityHelper.Equals(this, obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return this.hashCode;
        }
    }
}