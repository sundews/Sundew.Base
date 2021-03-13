// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Interval.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Numeric
{
    using System;

    /// <summary>
    /// Methods for creating intervals.
    /// </summary>
    public static class Interval
    {
        /// <summary>
        /// Creates an interval with the specified minimum and a length.
        /// </summary>
        /// <param name="minimum">The minimum.</param>
        /// <param name="length">The length.</param>
        /// <returns>An interval.</returns>
        public static Interval<int> FromMinAndLength(int minimum, int length)
        {
            return new Interval<int>(minimum, minimum + length);
        }

        /// <summary>
        /// Creates an interval with the specified minimum and a length.
        /// </summary>
        /// <param name="minimum">The minimum.</param>
        /// <param name="length">The length.</param>
        /// <returns>An interval.</returns>
        public static Interval<double> FromMinAndLength(double minimum, double length)
        {
            return new Interval<double>(minimum, minimum + length);
        }

        /// <summary>
        /// Creates an interval with the specified minimum and maximum.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>An interval.</returns>
        public static Interval<TValue> From<TValue>(TValue min, TValue max)
            where TValue : struct, IComparable<TValue>
        {
            return new Interval<TValue>(min, max);
        }
    }
}