// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringBuilderExtensions.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text;

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// Extends the string with extension methods.
/// </summary>
public static class StringBuilderExtensions
{
    private const string Format = "{0}";
    private static readonly Regex StartWhitespaceRegex = new(@"^\s+", RegexOptions.Compiled);
    private static readonly Regex EndWhitespaceRegex = new(@"\s+$", RegexOptions.Compiled);

    /// <summary>
    /// Appends the specified value.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="value">The value.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns>
    /// The <see cref="StringBuilder" />.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder Append(this StringBuilder stringBuilder, object value, IFormatProvider formatProvider)
    {
        return stringBuilder.AppendFormat(formatProvider, Format, value);
    }

    /// <summary>
    /// Gets the <see cref="StringBuilder"/> string, without the last character.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <returns>
    /// A string.
    /// </returns>
    public static string ToStringWithoutLast(this StringBuilder stringBuilder)
    {
        return stringBuilder.ToStringWithoutLast(0);
    }

    /// <summary>
    /// Gets the <see cref="StringBuilder"/> string, starting at the specified index, without the last character.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="index">The index.</param>
    /// <returns>
    /// A string.
    /// </returns>
    public static string ToStringWithoutLast(this StringBuilder stringBuilder, int index)
    {
        return stringBuilder.ToString(index, stringBuilder.Length - index - 1);
    }

    /// <summary>
    /// Gets the string, from start to the specified from end.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="fromEnd">The end offset.</param>
    /// <returns>
    /// A string.
    /// </returns>
    public static string ToString(this StringBuilder stringBuilder, FromEnd fromEnd)
    {
        return stringBuilder.ToString(0, stringBuilder.Length - fromEnd.Value);
    }

    /// <summary>
    /// Gets the string, from the specified start index to the specified from end.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="index">The index.</param>
    /// <param name="fromEnd">The from end length.</param>
    /// <returns>A string.</returns>
    public static string ToString(this StringBuilder stringBuilder, int index, FromEnd fromEnd)
    {
        return stringBuilder.ToString(index, stringBuilder.Length - index - fromEnd.Value);
    }

#if NETSTANDARD2_1
    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="range">The range.</param>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public static string ToString(this StringBuilder stringBuilder, Range range)
    {
        var (offset, length) = range.GetOffsetAndLength(stringBuilder.Length);
        return stringBuilder.ToString(offset, length);
    }
#endif

    /// <summary>
    /// Fixes the length.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="value">The value.</param>
    /// <param name="length">The length.</param>
    /// <param name="paddingCharacter">The padding character.</param>
    /// <param name="alignment">The alignment.</param>
    /// <param name="limit">The limit.</param>
    /// <param name="cultureInfo">The culture information.</param>
    /// <returns>
    /// The string with the fixed length.
    /// </returns>
    public static StringBuilder Append(this StringBuilder stringBuilder, string value, int length, char paddingCharacter, Alignment alignment, Limit limit, CultureInfo cultureInfo)
    {
        return stringBuilder.Append(value, length, paddingCharacter, alignment, limit, cultureInfo.TextInfo.IsRightToLeft);
    }

    /// <summary>
    /// Removes text from the end.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="fromEnd">The from end.</param>
    /// <returns>
    /// The <see cref="StringBuilder" />.
    /// </returns>
    public static StringBuilder Remove(this StringBuilder stringBuilder, FromEnd fromEnd)
    {
        var length = fromEnd.Value;
        return stringBuilder.Remove(stringBuilder.Length - length, length);
    }

    /// <summary>
    /// Fixes the length.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="value">The value.</param>
    /// <param name="length">The length.</param>
    /// <param name="paddingCharacter">The padding character.</param>
    /// <param name="alignment">The alignment.</param>
    /// <param name="limit">The limit.</param>
    /// <param name="isRightToLeft">if set to <c>true</c> [is right to left].</param>
    /// <returns>
    /// The <see cref="StringBuilder" />.
    /// </returns>
    public static StringBuilder Append(this StringBuilder stringBuilder, string value, int length, char paddingCharacter, Alignment alignment, Limit limit = default, bool isRightToLeft = false)
    {
        if (string.IsNullOrEmpty(value))
        {
            return stringBuilder.Append(paddingCharacter, length);
        }

        var remainingCharacters = length - value.Length;
        if (remainingCharacters == 0)
        {
            return stringBuilder.Append(value);
        }

        if (remainingCharacters < 0)
        {
            var isLeft = limit.IsLeft ^ isRightToLeft;
            var limitIndicator = limit.LimitIndicator ?? string.Empty;
            length -= limitIndicator.Length;
            if (isLeft)
            {
                var startIndex = Math.Abs(remainingCharacters) + limitIndicator.Length;
                var match = StartWhitespaceRegex.Match(value, startIndex, length);
                var whitespace = string.Empty;
                if (match.Success)
                {
                    whitespace = match.Value;
                    startIndex = match.Index;
                }

                return stringBuilder.Append(whitespace).Append(limitIndicator).Append(value, startIndex + match.Length, length - match.Length);
            }
            else
            {
                var startIndex = 0;
                var match = EndWhitespaceRegex.Match(value, startIndex, length);
                var whitespace = string.Empty;
                if (match.Success)
                {
                    whitespace = match.Value;
                }

                return stringBuilder.Append(value, startIndex, length - match.Length).Append(limitIndicator).Append(whitespace);
            }
        }

        return alignment switch
        {
            Alignment.CenterLeft => isRightToLeft
                ? AppendCenterRight(stringBuilder, value, paddingCharacter, remainingCharacters)
                : AppendCenterLeft(stringBuilder, value, paddingCharacter, remainingCharacters),
            Alignment.CenterRight => isRightToLeft
                ? AppendCenterLeft(stringBuilder, value, paddingCharacter, remainingCharacters)
                : AppendCenterRight(stringBuilder, value, paddingCharacter, remainingCharacters),
            Alignment.Left => isRightToLeft
                ? AppendRight(stringBuilder, value, paddingCharacter, remainingCharacters)
                : AppendLeft(stringBuilder, value, paddingCharacter, remainingCharacters),
            _ => isRightToLeft
                ? AppendLeft(stringBuilder, value, paddingCharacter, remainingCharacters)
                : AppendRight(stringBuilder, value, paddingCharacter, remainingCharacters),
        };
    }

#if !NETSTANDARD2_1
    internal static StringBuilder Append(this StringBuilder stringBuilder, ReadOnlySpan<char> span)
    {
        foreach (var character in span)
        {
            stringBuilder.Append(character);
        }

        return stringBuilder;
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static StringBuilder AppendRight(StringBuilder stringBuilder, string value, char paddingCharacter, int remainingCharacters)
    {
        return stringBuilder.Append(paddingCharacter, remainingCharacters).Append(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static StringBuilder AppendLeft(StringBuilder stringBuilder, string value, char paddingCharacter, int remainingCharacters)
    {
        return stringBuilder.Append(value).Append(paddingCharacter, remainingCharacters);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static StringBuilder AppendCenterLeft(StringBuilder stringBuilder, string value, char paddingCharacter, int remainingCharacters)
    {
        var remainingLeft = remainingCharacters / 2;
        return stringBuilder.Append(paddingCharacter, remainingLeft).Append(value).Append(paddingCharacter, remainingLeft + (remainingCharacters % 2));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static StringBuilder AppendCenterRight(StringBuilder stringBuilder, string value, char paddingCharacter, int remainingCharacters)
    {
        var remainingRight = remainingCharacters / 2;
        return stringBuilder.Append(paddingCharacter, remainingRight + (remainingCharacters % 2)).Append(value).Append(paddingCharacter, remainingRight);
    }
}