// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Text.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.PerformanceTests.Split
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Sundew.Base.Memory;
    using Sundew.Base.Text;

    public delegate SplitAction SplitTextFunc(char character, int index, StringBuilder stringBuilder);

    public static class Text
    {
        public static IEnumerable<string> SplitBasedCommandLineParser(string input)
        {
            const char doubleQuote = '\"';
            const char slash = '\\';
            const char space = ' ';
            var isInQuote = false;
            var isInEscape = false;
            var previousWasSpace = false;
            var memory = input.AsMemory();
            return memory.Split(
                (character, index, _) =>
                {
                    var actualIsInEscape = isInEscape;
                    var actualPreviousWasSpace = previousWasSpace;
                    isInEscape = false;
                    previousWasSpace = false;
                    switch (character)
                    {
                        case slash:
                            var nextIndex = index + 1;
                            if (isInQuote && input.Length > nextIndex && memory.Span[nextIndex] == doubleQuote)
                            {
                                isInEscape = true;
                                return SplitAction.Ignore;
                            }

                            return SplitAction.Include;
                        case doubleQuote:
                            if (!actualIsInEscape)
                            {
                                isInEscape = true;
                                isInQuote = true;
                            }

                            return actualIsInEscape ? SplitAction.Include : SplitAction.Ignore;
                        case space:
                            previousWasSpace = true;
                            if (actualIsInEscape)
                            {
                                isInQuote = false;
                                return SplitAction.Split;
                            }

                            if (actualPreviousWasSpace)
                            {
                                return SplitAction.Ignore;
                            }

                            return isInQuote ? SplitAction.Include : SplitAction.Split;
                        default:
                            return SplitAction.Include;
                    }
                },
                StringSplitOptions.None);
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
        public static IEnumerable<string> Split(this ReadOnlyMemory<char> input, SplitTextFunc splitFunc, StringSplitOptions stringSplitOptions = StringSplitOptions.None)
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

                if (splitResult.HasFlag(SplitAction.Split) && ShouldOutputString(stringSplitOptions, stringBuilder.Length))
                {
                    yield return stringBuilder.ToString();
                    stringBuilder.Clear();
                }

                switch (splitResult)
                {
                    case SplitAction.SplitAndSplitCurrent:
                        yield return new string(character, 1);
                        break;
                    case SplitAction.SplitAndInclude:
                        stringBuilder.Append(character);
                        break;
                }

                if (splitResult == SplitAction.SplitAndIncludeRest)
                {
#if NET48
                    var charSpan = input.Span.Slice(index, input.Length - index);
                    for (int i = 0; i < charSpan.Length; i++)
                    {
                        stringBuilder.Append(charSpan[i]);
                    }
#else
                    stringBuilder.Append(input.Span.Slice(index, input.Length - index));
#endif
                    yield return stringBuilder.ToString();
                    yield break;
                }
            }

            if (ShouldOutputString(stringSplitOptions, stringBuilder.Length))
            {
                yield return stringBuilder.ToString();
            }
        }

        private static bool ShouldOutputString(StringSplitOptions stringSplitOptions, int length)
        {
            return length > 0 || stringSplitOptions == StringSplitOptions.None;
        }
    }
}
