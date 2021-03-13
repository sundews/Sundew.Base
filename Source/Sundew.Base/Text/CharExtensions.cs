// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CharExtensions.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Text;

    /// <summary>
    /// Easy to use methods for <see cref="char"/>s.
    /// </summary>
    public static class CharExtensions
    {
        /// <summary>
        /// Repeats the specified count.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="count">The count.</param>
        /// <returns>The string with repeated characters.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Repeat(this char character, int count)
        {
            if (count <= 0)
            {
                return string.Empty;
            }

            return new string(character, count);
        }

        /// <summary>
        /// Splits the specified input with the <see cref="SplitFunc" />.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="splitFunc">The split function.</param>
        /// <param name="stringSplitOptions">The string split options.</param>
        /// <returns>
        /// The splitted strings as an <see cref="IEnumerable{T}" />.
        /// </returns>
        public static IEnumerable<string> Split(
            this ReadOnlyMemory<char> input,
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
        /// The splitted strings as an <see cref="IEnumerable{T}" />.
        /// </returns>
        public static IEnumerable<string> Split(
            this ReadOnlyMemory<char> input,
            SplitFuncWithoutStringBuilder splitFunc,
            StringSplitOptions stringSplitOptions = StringSplitOptions.None)
        {
            return
                input.Split(
                    (character, index, _) => splitFunc(character, index),
                    stringSplitOptions);
        }

        /// <summary>
        /// Splits the specified split function.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="splitFunc">The split function.</param>
        /// <param name="stringSplitOptions">The string split options.</param>
        /// <returns>
        /// The splitted memory.
        /// </returns>
        public static IEnumerable<string> Split(this ReadOnlyMemory<char> input, SplitFunc splitFunc, StringSplitOptions stringSplitOptions = StringSplitOptions.None)
        {
            if (input.IsEmpty)
            {
                yield break;
            }

            var stringBuilder = new StringBuilder();
            for (var index = 0; index < input.Length; index++)
            {
                var character = input.Span[index];
                var splitResult = splitFunc(character, index, stringBuilder);
                if (splitResult.HasFlag(SplitAction.Include))
                {
                    stringBuilder.Append(character);
                }

                if (splitResult.HasFlag(SplitAction.Split) && ShouldOutputString(stringSplitOptions, stringBuilder))
                {
                    yield return stringBuilder.ToString();
                    stringBuilder.Clear();
                }

                if (splitResult == SplitAction.SplitAndSplitCurrent)
                {
                    yield return new string(character, 1);
                }

                if (splitResult == SplitAction.SplitAndInclude)
                {
                    stringBuilder.Append(character);
                }
            }

            if (ShouldOutputString(stringSplitOptions, stringBuilder))
            {
                yield return stringBuilder.ToString();
            }
        }

        /// <summary>
        /// Splits the specified input with the <see cref="SplitFunc" />.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="splitFunc">The split function.</param>
        /// <param name="stringSplitOptions">The string split options.</param>
        /// <returns>
        /// The splitted strings as an <see cref="IEnumerable{T}" />.
        /// </returns>
        public static IEnumerable<ReadOnlyMemory<char>> SplitMemory(
            this ReadOnlyMemory<char> input,
            SplitMemoryFuncWithoutIndex splitFunc,
            StringSplitOptions stringSplitOptions = StringSplitOptions.None)
        {
            return input.SplitMemory((character, _) => splitFunc(character), stringSplitOptions);
        }

        /// <summary>
        /// Splits the specified split function.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="splitFunc">The split function.</param>
        /// <param name="stringSplitOptions">The string split options.</param>
        /// <returns>
        /// The splitted memory.
        /// </returns>
        public static IEnumerable<ReadOnlyMemory<char>> SplitMemory(this ReadOnlyMemory<char> input, SplitMemoryFunc splitFunc, StringSplitOptions stringSplitOptions = StringSplitOptions.None)
        {
            if (input.IsEmpty)
            {
                yield break;
            }

            var startIndex = 0;
            var length = 0;
            for (var index = 0; index < input.Length; index++)
            {
                var character = input.Span[index];
                var splitResult = splitFunc(character, index);
                if (splitResult == SplitAction.Ignore && startIndex == index)
                {
                    startIndex++;
                }

                if (splitResult.HasFlag(SplitAction.Include))
                {
                    if (length < 0 && startIndex + length <= index)
                    {
                        throw new InvalidOperationException($"Cannot use SplitAction.Ignored at index: {index} within a section in a SplitMemory call. Use Split alternative.");
                    }

                    length++;
                }

                if (splitResult.HasFlag(SplitAction.Split) && ShouldOutputString(stringSplitOptions, length))
                {
                    var oldLength = length;
                    var oldStartIndex = startIndex;
                    startIndex = index + 1;
                    length = 0;
                    yield return input.Slice(oldStartIndex, oldLength);
                }

                if (splitResult == SplitAction.SplitAndSplitCurrent)
                {
                    startIndex = index + 1;
                    length = 0;
                    yield return input.Slice(index, 1);
                }

                if (splitResult == SplitAction.SplitAndInclude)
                {
                    length++;
                }
            }

            if (ShouldOutputString(stringSplitOptions, length))
            {
                yield return input.Slice(startIndex, length);
            }
        }

        private static bool ShouldOutputString(StringSplitOptions stringSplitOptions, StringBuilder stringBuilder)
        {
            return stringBuilder.Length > 0 || stringSplitOptions == StringSplitOptions.None;
        }

        private static bool ShouldOutputString(StringSplitOptions stringSplitOptions, int length)
        {
            return length > 0 || stringSplitOptions == StringSplitOptions.None;
        }
    }
}