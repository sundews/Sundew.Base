// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Interval.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Numeric;

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
    public static R<Interval<int>> TryGetFromMinAndLength(int minimum, int length)
    {
        var max = minimum + length;
        return TryGetFrom(minimum, max);
    }

    /// <summary>
    /// Creates an interval with the specified minimum and a length.
    /// </summary>
    /// <param name="minimum">The minimum.</param>
    /// <param name="length">The length.</param>
    /// <returns>An interval.</returns>
    public static R<Interval<double>> TryGetFromMinAndLength(double minimum, double length)
    {
        var max = minimum + length;
        return TryGetFrom(minimum, max);
    }

    /// <summary>
    /// Creates an interval with the specified minimum and maximum.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="min">The minimum.</param>
    /// <param name="max">The maximum.</param>
    /// <returns>An interval.</returns>
    public static R<Interval<TValue>> TryGetFrom<TValue>(TValue min, TValue max)
        where TValue : IComparable<TValue>
    {
        return R.From(min.IsLessThan(max), new Interval<TValue>(min, max));
    }

    /// <summary>
    /// Creates an interval with the specified minimum and a length.
    /// </summary>
    /// <param name="minimum">The minimum.</param>
    /// <param name="length">The length.</param>
    /// <returns>An interval.</returns>
    public static Interval<int> FromMinAndLength(int minimum, int length)
    {
        var max = minimum + length;
        return From(minimum, max);
    }

    /// <summary>
    /// Creates an interval with the specified minimum and a length.
    /// </summary>
    /// <param name="minimum">The minimum.</param>
    /// <param name="length">The length.</param>
    /// <returns>An interval.</returns>
    public static Interval<double> FromMinAndLength(double minimum, double length)
    {
        var max = minimum + length;
        return From(minimum, max);
    }

    /// <summary>
    /// Creates an interval with the specified minimum and maximum.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="min">The minimum.</param>
    /// <param name="max">The maximum.</param>
    /// <returns>An interval.</returns>
    public static Interval<TValue> From<TValue>(TValue min, TValue max)
        where TValue : IComparable<TValue>
    {
        if (min.IsLessThanOrEqualTo(max))
        {
            return new Interval<TValue>(min, max);
        }

        throw new RangeException<TValue>(min, max);
    }
}