// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NaturalTextComparer.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <summary>
/// Comparer that considers natural strings.
/// </summary>
public class NaturalTextComparer : IComparer<string>
{
    private readonly StringComparison stringComparison;

    /// <summary>
    /// Initializes a new instance of the <see cref="NaturalTextComparer"/> class.
    /// </summary>
    /// <param name="stringComparison">The current culture.</param>
    public NaturalTextComparer(StringComparison stringComparison)
    {
        this.stringComparison = stringComparison;
    }

    /// <summary>
    /// Compares the two sides.
    /// </summary>
    /// <param name="lhs">The lhs.</param>
    /// <param name="rhs">The rhs.</param>
    /// <returns>A value indicating which side is considered the higher value.</returns>
    public int Compare(string? lhs, string? rhs)
    {
        if (lhs == null)
        {
            return rhs == null ? 0 : -1;
        }

        if (rhs == null)
        {
            return 1;
        }

        // The algorithm works by considering a string as sections of characters (anything by digits) and digits.
        // ABC123DEF consists of the sections ABC, 123, DEF
        // Characters sections are compared using the specified StringComparison.
        // Digits are compared ordinally. If one operand is shorted than the other it is filled with leading zeros "0".
        // E.g. comparing 12 with 123, would do an ordinal comparison of 012 and 123.
        // We need to iterate the longest string because "0004" is numerically higher than "1"
        var length = Math.Max(lhs.Length, rhs.Length);
        var lhsSpan = lhs.AsSpan();
        var rhsSpan = rhs.AsSpan();
        var lhsStart = 0;
        var rhsStart = 0;
        var lhsLength = 0;
        var rhsLength = 0;

        //// Check if first character is a digit.
        var lastLhsWasDigit = false;
        if (lhs.Length > 0)
        {
            lastLhsWasDigit = char.IsDigit(lhsSpan[0]);
            lhsLength = 1;
        }

        var lastRhsWasDigit = false;
        if (rhs.Length > 0)
        {
            lastRhsWasDigit = char.IsDigit(rhsSpan[0]);
            rhsLength = 1;
        }

        //// Iterate from the second character
        for (int i = 1; i < length; i++)
        {
            // Figure out if we have a digit. If there are no more characters we take the last state
            var lhsIsDigit = lhsStart + lhsLength < lhs.Length ? char.IsDigit(lhsSpan[i]) : lastLhsWasDigit;
            var rhsIsDigit = rhsStart + rhsLength < rhs.Length ? char.IsDigit(rhsSpan[i]) : lastRhsWasDigit;

            // Check if there was a character/digit switch in either direction
            var lhsSwitched = lhsIsDigit ^ lastLhsWasDigit;
            var rhsSwitched = rhsIsDigit ^ lastRhsWasDigit;

            // When characters switched synchroniously we should compare that section and start a new section
            if (lhsSwitched && rhsSwitched)
            {
                // The sections consisted of digits, do a numerical comparison.
                if (lastLhsWasDigit && lastRhsWasDigit)
                {
                    var result = CompareNumerically(lhsSpan, lhsStart, lhsLength, rhsSpan, rhsStart, rhsLength);
                    //// If sections differed we are done.
                    if (result != 0)
                    {
                        return result;
                    }
                }
                //// Do an alphanumerical comparison.
                else
                {
                    var result = lhsSpan.Slice(lhsStart, lhsLength).CompareTo(rhsSpan.Slice(rhsStart, rhsLength), this.stringComparison);
                    //// If sections differed we are done.
                    if (result != 0)
                    {
                        return result;
                    }
                }

                //// Both strings switched, so we reset section.
                lhsStart = i;
                rhsStart = i;
                lhsLength = 1;
                rhsLength = 1;
            }
            //// If one side switched, we are close to the end
            else if (lhsSwitched ^ rhsSwitched)
            {
                //// If both side consisted of digits, we should gather the following digits.
                if (lastLhsWasDigit && lastRhsWasDigit)
                {
                    for (int j = lhsStart + lhsLength; j < lhsSpan.Length; j++)
                    {
                        if (char.IsDigit(lhsSpan[j]))
                        {
                            lhsLength++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    for (int j = rhsStart + rhsLength; j < rhsSpan.Length; j++)
                    {
                        if (char.IsDigit(rhsSpan[j]))
                        {
                            rhsLength++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    // Since one side switched so we know we will end
                    return CompareNumerically(lhsSpan, lhsStart, lhsLength, rhsSpan, rhsStart, rhsLength);
                }
                //// Compare alphanumerically
                else
                {
                    var result = lhsSpan.Slice(lhsStart, lhsLength).CompareTo(rhsSpan.Slice(rhsStart, rhsLength), this.stringComparison);
                    //// If sections differed we are done.
                    if (result != 0)
                    {
                        return result;
                    }
                }

                //// We know we are about to end, but we start a new section and store digit state, before we continue.
                lhsStart = i;
                rhsStart = i;
                lhsLength = 1;
                rhsLength = 1;
                lastLhsWasDigit = lhsIsDigit;
                lastRhsWasDigit = rhsIsDigit;
                continue;
            }
            else
            {
                //// Increment lengths as long as there are characters, no switch or there are digits
                lhsLength = lhsStart + lhsLength < lhs.Length && (!lhsSwitched || (lastLhsWasDigit && lhsIsDigit)) ? lhsLength + 1 : lhsLength;
                rhsLength = rhsStart + rhsLength < rhs.Length && (!rhsSwitched || (lastRhsWasDigit && rhsIsDigit)) ? rhsLength + 1 : rhsLength;
            }

            //// The two side differ, so we are about to end
            if (lhsIsDigit ^ rhsIsDigit)
            {
                //// If lhs has digits
                if (lastLhsWasDigit && lhsIsDigit)
                {
                    //// Gather the remaining digits
                    for (int j = lhsStart + lhsLength; j < lhsSpan.Length; j++)
                    {
                        if (char.IsDigit(lhsSpan[j]))
                        {
                            lhsLength++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                //// If rhs has digits
                if (lastRhsWasDigit && rhsIsDigit)
                {
                    // Gather the remaining digits
                    for (int j = rhsStart + rhsLength; j < rhsSpan.Length; j++)
                    {
                        if (char.IsDigit(rhsSpan[j]))
                        {
                            rhsLength++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                //// Compare alphanumerically since one side will not be digits
                return lhsSpan.Slice(lhsStart, lhsLength).CompareTo(rhsSpan.Slice(rhsStart, rhsLength), this.stringComparison);
            }

            //// Store the last digit state.
            lastLhsWasDigit = lhsIsDigit;
            lastRhsWasDigit = rhsIsDigit;
        }

        //// If the last sections had digits.
        if (lastLhsWasDigit && lastRhsWasDigit)
        {
            //// Compare numerically
            return CompareNumerically(lhsSpan, lhsStart, lhsLength, rhsSpan, rhsStart, rhsLength);
        }

        //// Else compare alphanumerically
        return lhsSpan.Slice(lhsStart, lhsLength).CompareTo(rhsSpan.Slice(rhsStart, rhsLength), this.stringComparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CompareNumerically(ReadOnlySpan<char> lhsSpan, int lhsStart, int lhsLength, ReadOnlySpan<char> rhsSpan, int rhsStart, int rhsLength)
    {
        ReadOnlySpan<char> maxSpan = lhsSpan.Slice(lhsStart, lhsLength);
        ReadOnlySpan<char> minSpan = rhsSpan.Slice(rhsStart, rhsLength);
        var swapped = false;
        if (lhsLength < rhsLength)
        {
            var temp = minSpan;
            minSpan = maxSpan;
            maxSpan = temp;
            swapped = true;
        }

        unsafe
        {
            Span<char> minSpanWithPadding = stackalloc char[maxSpan.Length];
            var paddedSpan = minSpanWithPadding.Slice(0, maxSpan.Length - minSpan.Length);
            paddedSpan.Fill('0');
            minSpan.CopyTo(minSpanWithPadding.Slice(paddedSpan.Length, minSpan.Length));
            return swapped ? ((ReadOnlySpan<char>)minSpanWithPadding).CompareTo(maxSpan, StringComparison.Ordinal) : maxSpan.CompareTo(minSpanWithPadding, StringComparison.Ordinal);
        }
    }
}