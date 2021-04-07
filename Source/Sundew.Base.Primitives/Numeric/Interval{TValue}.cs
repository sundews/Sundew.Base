// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Interval{TValue}.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives.Numeric
{
    using System;
    using Sundew.Base.Equality;

    /// <summary>
    /// Represents a interval as a min and a max value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public readonly struct Interval<TValue> : IEquatable<Interval<TValue>>
        where TValue : struct, IComparable<TValue>
    {
        /// <summary>Initializes a new instance of the <see cref="Interval{TValue}"/> struct.</summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <exception cref="RangeException{TValue}">Thrown if min is greater than max.</exception>
        public Interval(TValue min, TValue max)
        {
            if (min.IsGreaterThan(max))
            {
                throw new RangeException<TValue>(min, max);
            }

            this.Min = min;
            this.Max = max;
        }

        /// <summary>
        /// Gets the minimum.
        /// </summary>
        /// <value>
        /// The minimum.
        /// </value>
        public TValue Min { get; }

        /// <summary>
        /// Gets the maximum.
        /// </summary>
        /// <value>
        /// The maximum.
        /// </value>
        public TValue Max { get; }

        /// <summary>Implements the operator ==.</summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Interval<TValue> left, Interval<TValue> right)
        {
            return left.Equals(right);
        }

        /// <summary>Implements the operator !=.</summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Interval<TValue> left, Interval<TValue> right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Limits the specified value to the min and max value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The limited value.</returns>
        public TValue Limit(TValue value)
        {
            if (value.IsLessThan(this.Min))
            {
                return this.Min;
            }

            if (value.IsGreaterThan(this.Max))
            {
                return this.Max;
            }

            return value;
        }

        /// <summary>
        /// Checks if the specified value is within the interval of the min and max inclusive.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c>, if the value is within min and max, otherwise <c>false</c>.
        /// </returns>
        public bool IsWithin(TValue value)
        {
            return this.IsWithin(value, IntervalMode.Inclusive);
        }

        /// <summary>
        /// Checks if the specified value is within the interval of the min and max.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="intervalMode">The interval mode.</param>
        /// <returns>
        ///   <c>true</c>, if the value is within min and max, otherwise <c>false</c>.
        /// </returns>
        public bool IsWithin(TValue value, IntervalMode intervalMode)
        {
            return ComparableExtensions.InternalIsWithinInterval(value, this.Min, this.Max, intervalMode);
        }

        /// <summary>
        /// Determines whether the specified interval is within.
        /// </summary>
        /// <param name="interval">The interval.</param>
        /// <param name="intervalMode">The interval mode.</param>
        /// <returns>
        ///   <c>true</c> if the specified interval is within; otherwise, <c>false</c>.
        /// </returns>
        public bool IsWithin(in Interval<TValue> interval, IntervalMode intervalMode)
        {
            return ComparableExtensions.InternalIsWithinInterval(interval.Min, this.Min, this.Max, intervalMode) &&
                   ComparableExtensions.InternalIsWithinInterval(interval.Max, this.Min, this.Max, intervalMode);
        }

        /// <summary>
        /// Overlaps the specified interval.
        /// </summary>
        /// <param name="interval">The interval.</param>
        /// <returns>
        ///   <c>true</c> if the specified interval overlaps; otherwise, <c>false</c>.
        /// </returns>
        public bool Overlaps(in Interval<TValue> interval)
        {
            return Comparison.Min(this.Max, interval.Max).CompareTo(Comparison.Max(this.Min, interval.Min)) > 0;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"Interval: min: {this.Min}, max: {this.Max}";
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///   <span class="keyword">
        ///     <span class="languageSpecificText">
        ///       <span class="cs">true</span>
        ///       <span class="vb">True</span>
        ///       <span class="cpp">true</span>
        ///     </span>
        ///   </span>
        ///   <span class="nu">
        ///     <span class="keyword">true</span> (<span class="keyword">True</span> in Visual Basic)</span> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <span class="keyword"><span class="languageSpecificText"><span class="cs">false</span><span class="vb">False</span><span class="cpp">false</span></span></span><span class="nu"><span class="keyword">false</span> (<span class="keyword">False</span> in Visual Basic)</span>.
        /// </returns>
        public bool Equals(Interval<TValue> other)
        {
            return this.Min.Equals(other.Min) && this.Max.Equals(other.Max);
        }

        /// <summary>Determines whether the specified <see cref="object"/>, is equal to this instance.</summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return EqualityHelper.Equals(this, obj);
        }

        /// <summary>Returns a hash code for this instance.</summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return EqualityHelper.GetHashCode(this.Min.GetHashCode(), this.Max.GetHashCode());
        }
    }
}