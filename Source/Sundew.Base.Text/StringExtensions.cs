// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Extends the string with extension methods.
    /// </summary>
    public static class StringExtensions
    {
        private static readonly Regex WhitespaceRegex = new (@"\s+");

        /// <summary>
        /// Toes the URI.
        /// </summary>
        /// <param name="path">The URI string.</param>
        /// <param name="uriKind">Kind of the URI.</param>
        /// <returns>The new Uri.</returns>
        public static Uri ToUri(this string path, UriKind uriKind = UriKind.RelativeOrAbsolute)
        {
            return new Uri(path, uriKind);
        }

        /// <summary>
        /// Removes the whitespace.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>A new containing no whitespace.</returns>
        public static string RemoveWhitespace(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return WhitespaceRegex.Replace(input, string.Empty);
        }

        /// <summary>
        /// Splits the specified input with the <see cref="SplitFunc" />.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="splitFunc">The split function.</param>
        /// <param name="stringSplitOptions">The string split options.</param>
        /// <returns>
        /// The split strings as an <see cref="IEnumerable{T}" />.
        /// </returns>
        public static IEnumerable<string> Split(
            this string input,
            SplitFuncWithoutIndex splitFunc,
            StringSplitOptions stringSplitOptions = StringSplitOptions.None)
        {
            return
                input.Split((character, _, builder) => splitFunc(character, builder), stringSplitOptions);
        }

        /// <summary>
        /// Splits the specified input with the <see cref="SplitFunc" />.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="splitFunc">The split function.</param>
        /// <param name="stringSplitOptions">The string split options.</param>
        /// <returns>
        /// The split strings as an <see cref="IEnumerable{T}" />.
        /// </returns>
        public static IEnumerable<string> Split(
            this string? input,
            SplitFuncWithoutStringBuilder splitFunc,
            StringSplitOptions stringSplitOptions = StringSplitOptions.None)
        {
            return
                input.Split(
                    (character, index, _) => splitFunc(character, index),
                    stringSplitOptions);
        }

        /// <summary>
        /// Splits the specified input with the <see cref="SplitFunc" />.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="splitFunc">The split function.</param>
        /// <param name="stringSplitOptions">The string split options.</param>
        /// <returns>
        /// The split strings as an <see cref="IEnumerable{T}" />.
        /// </returns>
        public static IEnumerable<string> Split(this string? input, SplitFunc splitFunc, StringSplitOptions stringSplitOptions = StringSplitOptions.None)
        {
            return input?.AsMemory().Split(splitFunc, stringSplitOptions) ?? Enumerable.Empty<string>();
        }

        /// <summary>
        /// Splits the specified input with the <see cref="SplitFunc" />.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="splitFunc">The split function.</param>
        /// <param name="stringSplitOptions">The string split options.</param>
        /// <returns>
        /// The split strings as an <see cref="IEnumerable{T}" />.
        /// </returns>
        public static IEnumerable<ReadOnlyMemory<char>> SplitMemory(
            this string? input,
            SplitMemoryFuncWithoutIndex splitFunc,
            StringSplitOptions stringSplitOptions = StringSplitOptions.None)
        {
            return input.SplitMemory((character, _) => splitFunc(character), stringSplitOptions);
        }

        /// <summary>
        /// Splits the specified input with the <see cref="SplitFunc" />.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="splitFunc">The split function.</param>
        /// <param name="stringSplitOptions">The string split options.</param>
        /// <returns>
        /// The split strings as an <see cref="IEnumerable{T}" />.
        /// </returns>
        public static IEnumerable<ReadOnlyMemory<char>> SplitMemory(this string? input, SplitMemoryFunc splitFunc, StringSplitOptions stringSplitOptions = StringSplitOptions.None)
        {
            return input?.AsMemory().SplitMemory(splitFunc, stringSplitOptions) ?? Enumerable.Empty<ReadOnlyMemory<char>>();
        }

        /// <summary>
        /// Replaces text at the given index.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="replacement">The replacement.</param>
        /// <returns>The new string.</returns>
        public static string ReplaceAt(this string input, int startIndex, string replacement)
        {
            VerifyBounds(input, startIndex, replacement.Length);

            return ReplaceAtUnsafe(input, startIndex, replacement, replacement.Length);
        }

        /// <summary>
        /// Replaces text at the given index by the given length.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="replacement">The replacement.</param>
        /// <param name="length">The length.</param>
        /// <returns>
        /// The new string.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">length.</exception>
        public static string ReplaceAt(this string input, int startIndex, string replacement, int length)
        {
            if (length < 0 || replacement.Length < length)
            {
                throw new ArgumentOutOfRangeException(nameof(length), length, $"The length: {length} is not between 0 and the replacement length: {replacement.Length}.");
            }

            VerifyBounds(input, startIndex, length);

            return ReplaceAtUnsafe(input, startIndex, replacement, length);
        }

        /// <summary>Gets a string where the first letter is lowercase.</summary>
        /// <param name="input">The input.</param>
        /// <returns>The uncapitalized string.</returns>
        public static string Uncapitalize(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return input.ToLower(0, 1, CultureInfo.CurrentCulture);
        }

        /// <summary>Gets a string where the first letter is lowercase.</summary>
        /// <param name="input">The input.</param>
        /// <param name="cultureInfo">The culture info.</param>
        /// <returns>The capitalized string.</returns>
        public static string Uncapitalize(this string input, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return input.ToLower(0, 1, cultureInfo);
        }

        /// <summary>Gets a string where the letters from the start index are lowercase.</summary>
        /// <param name="input">The input.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="cultureInfo">The culture info.</param>
        /// <returns>The capitalized string.</returns>
        public static string ToLower(this string input, int startIndex, CultureInfo? cultureInfo = default)
        {
            return input.ToLower(startIndex, input.Length - startIndex, cultureInfo);
        }

        /// <summary>Gets a string where the letters within the range are lowercase.</summary>
        /// <param name="input">The input.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The length.</param>
        /// <param name="cultureInfo">The culture info.</param>
        /// <returns>The new string.</returns>
        public static string ToLower(this string input, int startIndex, int length, CultureInfo? cultureInfo = default)
        {
            VerifyBounds(input, startIndex, length);
            if (length == 0)
            {
                return input;
            }

            for (int i = 0; i < length; i++)
            {
                if (!char.IsLower(input[i]))
                {
                    return PrivateToLower(input, startIndex, length, cultureInfo ?? CultureInfo.CurrentCulture);
                }
            }

            return input;
        }

        /// <summary>Capitalizes the specified culture information.</summary>
        /// <param name="input">The input.</param>
        /// <returns>The capitalized string.</returns>
        public static string Capitalize(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return input.ToUpper(0, 1, CultureInfo.CurrentCulture);
        }

        /// <summary>Capitalizes the specified culture information.</summary>
        /// <param name="input">The input.</param>
        /// <param name="cultureInfo">The culture information.</param>
        /// <returns>The capitalized string.</returns>
        public static string Capitalize(this string input, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return input.ToUpper(0, 1, cultureInfo);
        }

        /// <summary>Gets a string where the letters from the start index are uppercase.</summary>
        /// <param name="input">The input.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="cultureInfo">The culture info.</param>
        /// <returns>The capitalized string.</returns>
        public static string ToUpper(this string input, int startIndex, CultureInfo? cultureInfo = null)
        {
            return input.ToUpper(startIndex, input.Length - startIndex, cultureInfo);
        }

        /// <summary>Gets a string where the letters within the range are uppercase.</summary>
        /// <param name="input">The input.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The length.</param>
        /// <param name="cultureInfo">The culture info.</param>
        /// <returns>The capitalized string.</returns>
        public static string ToUpper(this string input, int startIndex, int length, CultureInfo? cultureInfo = default)
        {
            VerifyBounds(input, startIndex, length);
            if (length == 0)
            {
                return input;
            }

            for (int i = startIndex; i < length; i++)
            {
                if (!char.IsUpper(input[i]))
                {
                    return PrivateToUpper(input, startIndex, length, cultureInfo ?? CultureInfo.CurrentCulture);
                }
            }

            return input;
        }

        /// <summary>
        /// Fixes the length left.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="length">The length.</param>
        /// <param name="paddingCharacter">The padding character.</param>
        /// <param name="limitSide">The limit side.</param>
        /// <returns>The string with the fixed length.</returns>
        public static string LimitAndPadLeft(this string value, int length, char paddingCharacter, LimitSide limitSide = LimitSide.Right)
        {
            return value.LimitAndPad(length, paddingCharacter, PadSide.Left, limitSide);
        }

        /// <summary>
        /// Fixes the length right.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="length">The length.</param>
        /// <param name="paddingCharacter">The padding character.</param>
        /// <param name="limitSide">The limit side.</param>
        /// <returns>The string with the fixed length.</returns>
        public static string LimitAndPadRight(this string value, int length, char paddingCharacter, LimitSide limitSide = LimitSide.Right)
        {
            return value.LimitAndPad(length, paddingCharacter, PadSide.Right, limitSide);
        }

        /// <summary>
        /// Fixes the length.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="length">The length.</param>
        /// <param name="paddingCharacter">The padding character.</param>
        /// <param name="padSide">The pad side.</param>
        /// <param name="limitSide">The limit side.</param>
        /// <returns>The string with the fixed length.</returns>
        public static string LimitAndPad(this string value, int length, char paddingCharacter, PadSide padSide, LimitSide limitSide)
        {
            if (string.IsNullOrEmpty(value))
            {
                return paddingCharacter.Repeat(length);
            }

            var remainingCharacters = length - value.Length;
            if (remainingCharacters == 0)
            {
                return value;
            }

            if (remainingCharacters < 0)
            {
                switch (limitSide)
                {
                    case LimitSide.Left:
                        var excessCharacters = remainingCharacters * -1;
                        return value.Substring(excessCharacters, length);
                    default:
                        return value.Substring(0, length);
                }
            }

            switch (padSide)
            {
                case PadSide.BothRight:
                    {
                        var remainingLeft = remainingCharacters / 2;
                        return paddingCharacter.Repeat(remainingLeft) +
                               value +
                               paddingCharacter.Repeat(remainingLeft + (remainingCharacters % 2));
                    }

                case PadSide.BothLeft:
                    {
                        var remainingRight = remainingCharacters / 2;
                        return paddingCharacter.Repeat(remainingRight + (remainingCharacters % 2)) +
                               value +
                               paddingCharacter.Repeat(remainingRight);
                    }

                case PadSide.Left:
                    {
                        return paddingCharacter.Repeat(remainingCharacters) + value;
                    }

                default:
                    {
                        return value + paddingCharacter.Repeat(remainingCharacters);
                    }
            }
        }

        private static void VerifyBounds(string input, int startIndex, int length)
        {
            if (startIndex < 0 || input.Length < startIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, $"The startIndex: {startIndex} is not within 0 and the length of the input: {input.Length}.");
            }

            if (input.Length < startIndex + length)
            {
                throw new ArgumentOutOfRangeException(nameof(length), length, $"The length: {length} at the startIndex: {startIndex} exceeds the input length: {input.Length}.");
            }
        }

        private static string ReplaceAtUnsafe(this string input, int startIndex, string replacement, int length)
        {
            unsafe
            {
                fixed (char* inputPointer = input)
                {
                    var newString = new string(inputPointer, 0, input.Length);
                    fixed (char* newStringPointer = newString)
                    {
                        fixed (char* replacementPointer = replacement)
                        {
                            var startPointer = newStringPointer + startIndex;
                            Unsafe.CopyBlock(startPointer, replacementPointer, (uint)length << 1);
                            return newString;
                        }
                    }
                }
            }
        }

        private static string PrivateToUpper(string input, in int startIndex, in int length, CultureInfo cultureInfo)
        {
            unsafe
            {
                fixed (char* inputPointer = input)
                {
                    var value = new string(inputPointer, 0, input.Length);
                    fixed (char* fixedPointer = value)
                    {
                        var pointer = fixedPointer + startIndex;
                        for (int i = 0; i < length; i++)
                        {
                            *pointer = cultureInfo.TextInfo.ToUpper(*pointer);
                            pointer++;
                        }

                        return value;
                    }
                }
            }
        }

        private static string PrivateToLower(string input, in int startIndex, int length, CultureInfo cultureInfo)
        {
            unsafe
            {
                fixed (char* inputPointer = input)
                {
                    var value = new string(inputPointer, 0, input.Length);
                    fixed (char* fixedPointer = value)
                    {
                        var pointer = fixedPointer + startIndex;
                        for (int i = 0; i < length; i++)
                        {
                            *pointer = cultureInfo.TextInfo.ToLower(*pointer);
                            pointer++;
                        }

                        return value;
                    }
                }
            }
        }
    }
}
