// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComparableExtensions.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives.Numeric
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Extensions methods for IComparable.
    /// </summary>
    public static class ComparableExtensions
    {
        /// <summary>
        /// Determines whether LHS is greater than the specified RHS.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="lhs">The LHS value.</param>
        /// <param name="rhs">The RHS value.</param>
        /// <returns>
        ///   <c>true</c> if the specified LHS is greater than RHS otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGreaterThan<TValue>(this TValue lhs, TValue rhs)
            where TValue : struct, IComparable<TValue>
        {
            return lhs.CompareTo(rhs) > 0;
        }

        /// <summary>
        /// Determines whether LHS is is greater than or equal to the RHS.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="lhs">The LHS value.</param>
        /// <param name="rhs">The RHS value.</param>
        /// <returns>
        ///   <c>true</c> if the specified LHS is greater than or equal to RHS otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGreaterThanOrEqualTo<TValue>(this TValue lhs, TValue rhs)
            where TValue : struct, IComparable<TValue>
        {
            return lhs.CompareTo(rhs) >= 0;
        }

        /// <summary>
        /// Determines whether LHS is less than the specified RHS.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="lhs">The LHS value.</param>
        /// <param name="rhs">The RHS value.</param>
        /// <returns>
        ///   <c>true</c> if the specified LHS is less than RHS otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLessThan<TValue>(this TValue lhs, TValue rhs)
            where TValue : struct, IComparable<TValue>
        {
            return lhs.CompareTo(rhs) < 0;
        }

        /// <summary>
        /// Determines whether LHS is less than or equal to the specified RHS.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="lhs">The LHS value.</param>
        /// <param name="rhs">The RHS value.</param>
        /// <returns>
        ///   <c>true</c> if the specified LHS is less than or equal to RHS otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLessThanOrEqualTo<TValue>(this TValue lhs, TValue rhs)
            where TValue : struct, IComparable<TValue>
        {
            return lhs.CompareTo(rhs) <= 0;
        }

        /// <summary>
        /// Determines whether LHS is equal to the specified RHS.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="lhs">The LHS value.</param>
        /// <param name="rhs">The RHS value.</param>
        /// <returns>
        ///   <c>true</c> if the specified LHS is less than or equal to RHS otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEqualTo<TValue>(this TValue lhs, TValue rhs)
            where TValue : struct, IComparable<TValue>
        {
            return lhs.CompareTo(rhs) == 0;
        }

        /// <summary>
        /// Determines whether the LHS is not equal to the specified RHS.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="lhs">The LHS value.</param>
        /// <param name="rhs">The RHS value.</param>
        /// <returns>
        ///   <c>true</c> if the specified LHS is not equal to RHS otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotEqualTo<TValue>(this TValue lhs, TValue rhs)
            where TValue : struct, IComparable<TValue>
        {
            return lhs.CompareTo(rhs) != 0;
        }

        /// <summary>
        /// Determines whether the specified value with within the interval.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <param name="intervalMode">The interval mode.</param>
        /// <returns>
        ///   <c>true</c> if the specified quantity is within the interval, otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="RangeException{TValue}">Thrown if min is greater than max.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWithinInterval<TValue>(this TValue value, TValue min, TValue max, IntervalMode intervalMode = IntervalMode.Inclusive)
            where TValue : struct, IComparable<TValue>
        {
            if (min.IsGreaterThan(max))
            {
                throw new RangeException<TValue>(min, max);
            }

            return InternalIsWithinInterval(value, min, max, intervalMode);
        }

        /// <summary>
        /// Determines whether [is within interval] [the specified value].
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <param name="intervalMode">The interval mode.</param>
        /// <returns>
        ///   <c>true</c> if [is within interval] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        internal static bool InternalIsWithinInterval<TValue>(TValue value, TValue min, TValue max, IntervalMode intervalMode = IntervalMode.Inclusive)
            where TValue : struct, IComparable<TValue>
        {
            switch (intervalMode)
            {
                case IntervalMode.Exclusive:
                    return min.IsLessThan(value) && max.IsGreaterThan(value);
                case IntervalMode.MinExclusive:
                    return min.IsLessThan(value) && max.IsGreaterThanOrEqualTo(value);
                case IntervalMode.MaxExclusive:
                    return min.IsLessThanOrEqualTo(value) && max.IsGreaterThan(value);
                default:
                    return min.IsLessThanOrEqualTo(value) && max.IsGreaterThanOrEqualTo(value);
            }
        }
    }
}