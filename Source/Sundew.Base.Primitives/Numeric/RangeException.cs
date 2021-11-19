// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RangeException.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives.Numeric;

using System;
using System.Globalization;

/// <summary>
/// Exception for indicating an invalid range.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
/// <seealso cref="System.Exception" />
public class RangeException<TValue> : Exception
    where TValue : struct, IComparable<TValue>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RangeException{TValue}" /> class.
    /// </summary>
    /// <param name="min">The minimum.</param>
    /// <param name="max">The maximum.</param>
    /// <param name="messageFormat">The message.</param>
    public RangeException(TValue min, TValue max, string? messageFormat = null)
        : base(GetMessage(min, max, messageFormat))
    {
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

    private static string GetMessage(TValue min, TValue max, string? message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return $"Min: {min} is greater than max: {max}";
        }

        return string.Format(CultureInfo.CurrentCulture, message, min, max);
    }
}