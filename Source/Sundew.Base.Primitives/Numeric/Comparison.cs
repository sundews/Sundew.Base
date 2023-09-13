// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Comparison.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives.Numeric;

using System;

/// <summary>
/// Contains numeric comparison methods.
/// </summary>
public static class Comparison
{
    /// <summary>
    /// Gets the smaller of the specified sides.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>The min value.</returns>
    public static TValue Min<TValue>(in TValue lhs, in TValue rhs)
        where TValue : struct, IComparable<TValue>
    {
        if (lhs.CompareTo(rhs) < 0)
        {
            return lhs;
        }

        return rhs;
    }

    /// <summary>
    /// Gets the larger of the specified sides.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>The max value.</returns>
    public static TValue Max<TValue>(in TValue lhs, in TValue rhs)
        where TValue : struct, IComparable<TValue>
    {
        if (lhs.CompareTo(rhs) > 0)
        {
            return lhs;
        }

        return rhs;
    }
}