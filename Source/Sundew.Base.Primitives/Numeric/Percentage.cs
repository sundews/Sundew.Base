// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Percentage.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives.Numeric;

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Sundew.Base.Equality;
using Sundew.Base.Primitives.Computation;

/// <summary>
/// Represents a percentage.
/// </summary>
/// <seealso cref="IEquatable{Percentage}" />
public readonly struct Percentage : IEquatable<Percentage>, IComparable<Percentage>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Percentage"/> struct.
    /// </summary>
    /// <param name="value">The percentage.</param>
    public Percentage(double value)
    {
        this.Value = value;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public double Value { get; }

    /// <summary>
    /// Implements the operator +.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static Percentage operator +(Percentage lhs)
    {
        return lhs;
    }

    /// <summary>
    /// Implements the operator -.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static Percentage operator -(Percentage lhs)
    {
        return new Percentage(-lhs.Value);
    }

    /// <summary>
    /// Implements the operator +.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static Percentage operator +(Percentage lhs, double rhs)
    {
        return new Percentage(lhs.Value + rhs);
    }

    /// <summary>
    /// Implements the operator -.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static Percentage operator -(Percentage lhs, double rhs)
    {
        return new Percentage(lhs.Value - rhs);
    }

    /// <summary>
    /// Implements the operator *.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static Percentage operator *(Percentage lhs, double rhs)
    {
        return new Percentage(lhs.Value * rhs);
    }

    /// <summary>
    /// Implements the operator /.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static Percentage operator /(Percentage lhs, double rhs)
    {
        return new Percentage(lhs.Value / rhs);
    }

    /// <summary>
    /// Implements the operator *.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static double operator *(double lhs, Percentage rhs)
    {
        return lhs * rhs.Value;
    }

    /// <summary>
    /// Implements the operator +.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static Percentage operator +(Percentage lhs, Percentage rhs)
    {
        return new Percentage(lhs.Value + rhs.Value);
    }

    /// <summary>
    /// Implements the operator -.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static Percentage operator -(Percentage lhs, Percentage rhs)
    {
        return new Percentage(lhs.Value - rhs.Value);
    }

    /// <summary>
    /// Implements the operator *.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static Percentage operator *(Percentage lhs, Percentage rhs)
    {
        return new Percentage(lhs.Value * rhs.Value);
    }

    /// <summary>
    /// Implements the operator /.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static Percentage operator /(Percentage lhs, Percentage rhs)
    {
        return new Percentage(lhs.Value / rhs.Value);
    }

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator ==(Percentage lhs, Percentage rhs)
    {
        return lhs.Value.Equals(rhs.Value);
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator !=(Percentage lhs, Percentage rhs)
    {
        return !lhs.Value.Equals(rhs.Value);
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator >(Percentage lhs, Percentage rhs)
    {
        return lhs.Value > rhs.Value;
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator <(Percentage lhs, Percentage rhs)
    {
        return lhs.Value < rhs.Value;
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator >=(Percentage lhs, Percentage rhs)
    {
        return lhs.Value >= rhs.Value;
    }

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="lhs">The LHS.</param>
    /// <param name="rhs">The RHS.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static bool operator <=(Percentage lhs, Percentage rhs)
    {
        return lhs.Value <= rhs.Value;
    }

    /// <summary>
    /// Tries to parse the input string.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="percentage">The percentage.</param>
    /// <returns>
    ///   <c>true</c> if parsing was successful otherwise <c>false</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(string input, out Percentage percentage)
    {
        return TryParse(input, CultureInfo.CurrentCulture, out percentage);
    }

    /// <summary>
    /// Tries to parse the input string.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="cultureInfo">The culture information.</param>
    /// <param name="percentage">The percentage.</param>
    /// <returns>
    ///   <c>true</c> if parsing was successful otherwise <c>false</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse(string input, CultureInfo cultureInfo, out Percentage percentage)
    {
        return TryParse(input, cultureInfo.NumberFormat, out percentage);
    }

    /// <summary>
    /// Tries to parse the input string.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="numberFormatInfo">The number format information.</param>
    /// <param name="percentage">The percentage.</param>
    /// <returns>
    ///     <c>true</c> if parsing was successful otherwise <c>false</c>.
    /// </returns>
    public static bool TryParse(string input, NumberFormatInfo numberFormatInfo, out Percentage percentage)
    {
        var result = TryParse(input, numberFormatInfo);
        if (result)
        {
            percentage = result.Value;
            return true;
        }

        percentage = default;
        return false;
    }

    /// <summary>
    /// Parses the specified input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>
    /// A percentage.
    /// </returns>
    /// <exception cref="ArgumentException">percentage.</exception>
    /// <exception cref="FormatException">The culture info number format was invalid.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Percentage Parse(string input)
    {
        return Parse(input, CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Parses the specified input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="cultureInfo">The culture information.</param>
    /// <returns>
    /// A percentage.
    /// </returns>
    /// <exception cref="ArgumentException">percentage.</exception>
    /// <exception cref="FormatException">The culture info number format was invalid.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Percentage Parse(string input, CultureInfo cultureInfo)
    {
        return Parse(input, cultureInfo.NumberFormat);
    }

    /// <summary>
    /// Parses the specified input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="numberFormatInfo">The number format information.</param>
    /// <returns>
    /// A percentage.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Percentage Parse(string input, NumberFormatInfo numberFormatInfo)
    {
        return TryParse(input, numberFormatInfo, true).Value;
    }

    /// <summary>
    /// Parses the specified input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>A percentage result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static O<Percentage> TryParse(string input)
    {
        return TryParse(input, CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Tries to parse the specified input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="cultureInfo">The culture information.</param>
    /// <returns>A percentage result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static O<Percentage> TryParse(string input, CultureInfo cultureInfo)
    {
        return TryParse(input, cultureInfo.NumberFormat);
    }

    /// <summary>
    /// Tries to parse the specified input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="numberFormatInfo">The number format information.</param>
    /// <returns>A percentage result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static O<Percentage> TryParse(string input, NumberFormatInfo numberFormatInfo)
    {
        return TryParse(input, numberFormatInfo, false);
    }

    /// <summary>
    /// Limits the specified minimum.
    /// </summary>
    /// <param name="min">The minimum.</param>
    /// <param name="max">The maximum.</param>
    /// <returns>The limited percentage.</returns>
    public Percentage Limit(double min, double max)
    {
        return new Percentage(Math.Min(Math.Max(min, this.Value), max));
    }

    /// <summary>
    /// Rounds the specified decimals.
    /// </summary>
    /// <returns>
    /// The rounded percentage.
    /// </returns>
    public Percentage Round()
    {
        return this.Round(0, MidpointRounding.ToEven);
    }

    /// <summary>
    /// Rounds the specified decimals.
    /// </summary>
    /// <param name="decimalDigits">The decimals.</param>
    /// <returns>The rounded percentage.</returns>
    public Percentage Round(int decimalDigits)
    {
        return this.Round(decimalDigits, MidpointRounding.ToEven);
    }

    /// <summary>
    /// Rounds the specified decimals.
    /// </summary>
    /// <param name="decimalDigits">The decimals.</param>
    /// <param name="midpointRounding">The midpoint rounding.</param>
    /// <returns>The rounded percentage.</returns>
    public Percentage Round(int decimalDigits, MidpointRounding midpointRounding)
    {
        return new Percentage(Math.Round(this.Value, decimalDigits, midpointRounding));
    }

    /// <summary>
    /// Returns the smallest integer percentage less than or equal to the specified percentage.
    /// </summary>
    /// <returns>The new percentage.</returns>
    public Percentage Ceiling()
    {
        return new Percentage(Math.Ceiling(this.Value));
    }

    /// <summary>
    /// Returns the largest integer percentage less than or equal to the specified percentage.
    /// </summary>
    /// <returns>The new percentage.</returns>
    public Percentage Floor()
    {
        return new Percentage(Math.Floor(this.Value));
    }

    /// <summary>
    /// Gets the absolute percentage.
    /// </summary>
    /// <returns>´The absolute percentage.</returns>
    public Percentage Absolute()
    {
        return new Percentage(Math.Abs(this.Value));
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.
    /// </returns>
    public bool Equals(Percentage other)
    {
        return this.Value.Equals(other.Value);
    }

    /// <summary>
    /// Compares to.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns>A value indicating whether the percentage is less than, equal to or greater than the specified value.</returns>
    public int CompareTo(Percentage other)
    {
        return this.Value.CompareTo(other.Value);
    }

    /// <summary>
    /// Determines whether the specified <see cref="object" />, is equal to this instance.
    /// </summary>
    /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object obj)
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
        return this.Value.GetHashCode();
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return this.ToString(CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <param name="cultureInfo">The culture information.</param>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public string ToString(CultureInfo cultureInfo)
    {
        const string percentageFormat = "P";
        return this.Value.ToString(percentageFormat, cultureInfo);
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <param name="percentageDecimalDigits">The percent decimal digits.</param>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public string ToString(int percentageDecimalDigits)
    {
        return this.ToString(CultureInfo.CurrentCulture, percentageDecimalDigits);
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <param name="cultureInfo">The culture information.</param>
    /// <param name="percentageDecimalDigits">The percent decimal digits.</param>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public string ToString(CultureInfo cultureInfo, int percentageDecimalDigits)
    {
        return this.Value.ToString($"P{percentageDecimalDigits}", cultureInfo);
    }

    /// <summary>
    /// Tries the parse.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="numberFormatInfo">The number format information.</param>
    /// <param name="shouldThrow">if set to <c>true</c> [should throw].</param>
    /// <returns>A percentage result.</returns>
    private static O<Percentage> TryParse(string input, NumberFormatInfo numberFormatInfo, bool shouldThrow)
    {
        if (string.IsNullOrEmpty(input))
        {
            if (shouldThrow)
            {
                throw new ArgumentException(
                    $"The string: {input} was not a valid percentage.",
                    nameof(input));
            }

            return O.None();
        }

#if NETSTANDARD2_1
        var isNegative = input.Contains('-', StringComparison.Ordinal);
#else
        const string dash = "-";
        var isNegative = input.Contains(dash);
#endif
        int start;
        int fromEnd;
        if (isNegative)
        {
            switch (numberFormatInfo.PercentNegativePattern)
            {
                case 0:
                    start = 1;
                    fromEnd = 2;
                    break;
                case 1:
                    start = 1;
                    fromEnd = 2;
                    break;
                case 2:
                case 3:
                    start = 2;
                    fromEnd = 0;
                    break;
                case 4:
                    start = 1;
                    fromEnd = 1;
                    break;
                case 5:
                case 6:
                    start = 0;
                    fromEnd = 2;
                    break;
                case 7:
                case 10:
                    start = 3;
                    fromEnd = 0;
                    break;
                case 8:
                case 11:
                    start = 0;
                    fromEnd = 3;
                    break;
                case 9:
                    start = 2;
                    fromEnd = 1;
                    break;
                default:
                    if (shouldThrow)
                    {
                        throw new FormatException(
                            $"The number format was invalid: {numberFormatInfo.PercentNegativePattern}");
                    }

                    return O.None();
            }

            return O.Some(new Percentage(-double.Parse(input.Substring(start, input.Length - fromEnd), numberFormatInfo) / 100));
        }

        switch (numberFormatInfo.PercentPositivePattern)
        {
            case 0:
                start = 0;
                fromEnd = 2;
                break;
            case 1:
                start = 0;
                fromEnd = 1;
                break;
            case 2:
                start = 1;
                fromEnd = 0;
                break;
            case 3:
                start = 2;
                fromEnd = 0;
                break;
            default:
                if (shouldThrow)
                {
                    throw new FormatException(
                        $"The number format was invalid: {numberFormatInfo.PercentPositivePattern}");
                }

                return O.None();
        }

        return O.Some(new Percentage(double.Parse(input.Substring(start, input.Length - fromEnd), numberFormatInfo) / 100));
    }
}