// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyMemoryExtensionsTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Memory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Sundew.Base.Memory;
    using Xunit;

    public class ReadOnlyMemoryExtensionsTests
    {
        [Theory]
        [InlineData("1|2|3,4|5|6", new[] { "123", "456" })]
        public void Split_When_IgnoringInSection_Then_ResultShouldBeExpectedResult(string input, string[] expectedResult)
        {
            var result = input.AsMemory().Split(
                (character, index, _) =>
                {
                    if (character == '|')
                    {
                        return SplitAction.Ignore;
                    }

                    if (character == ',')
                    {
                        return SplitAction.Split;
                    }

                    return SplitAction.Include;
                });

            result.Select(x => x.ToString()).Should().Equal(expectedResult);
        }

        [Theory]
        [InlineData("123   1234", 3, new[] { "   ", "1234" })]
        public void Split_When_TrimmingWhitespaceAndTheStart_Then_ResultShouldBeExpectedResult(string input, int startIndex, string[] expectedResult)
        {
            var result = input.AsMemory().Split(
                (character, index, _) =>
                {
                    if (index < startIndex)
                    {
                        return SplitAction.Ignore;
                    }

                    if (char.IsWhiteSpace(character))
                    {
                        return SplitAction.Include;
                    }

                    return SplitAction.SplitAndIncludeRest;
                });

            result.Select(x => x.ToString()).Should().Equal(expectedResult);
        }

        [Theory]
        [InlineData("ValuE1234", new[] { "ValuE", "1234" })]
        public void Split_When_UsingIncludeAndSplit_Then_ResultShouldBeExpectedResult(string input, string[] expectedResult)
        {
            var result = input.AsMemory().Split(
                (character, _, _) =>
                {
                    if (character == 'E')
                    {
                        return SplitAction.IncludeAndSplit;
                    }

                    return SplitAction.Include;
                });

            result.Select(x => x.ToString()).Should().Equal(expectedResult);
        }

        [Theory]
        [InlineData("warm:;up", new[] { "warm", "up" })]
        [InlineData(":;ok:;correct:;", new[] { "ok", "correct" })]
        [InlineData(":;ok:;correct", new[] { "ok", "correct" })]
        [InlineData("ok:;correct:;", new[] { "ok", "correct" })]
        [InlineData("ok:;cor:rect:;", new[] { "ok", "cor:rect" })]
        public void Split_Then_ResultShouldBeExpectedResults(string input, string[] expectedResults)
        {
            var testee = input.AsMemory();

            var result = testee.Split((character, index, _) =>
            {
                return character switch
                {
                    ';' => SplitAction.Split,
                    ':' => testee.Span[index + 1] == ';' ? SplitAction.Ignore : SplitAction.Include,
                    _ => SplitAction.Include,
                };
            });

            result.Select(x => x.ToString()).Should().Equal(expectedResults);
        }

        [Theory]
        [InlineData(@"-a -b ""1 _ ewr _ 23"" -c -d 32 -e 34", new[] { "-a", "-b", "1 _ ewr _ 23", "-c", "-d", "32", "-e", "34" })]
        [InlineData(@"-a -b ""1 """" ewr """" 23"" -c -d 32 -e 34", new[] { "-a", "-b", @"1 "" ewr "" 23", "-c", "-d", "32", "-e", "34" })]
        [InlineData("a warm up", new[] { "a", "warm", "up" })]
        public void Split_When_LexingCommandLine_Then_ResultShouldBeExpectedResult(string input, string[] expectedResult)
        {
            var result = SplitBasedCommandLineLexer(input);

            result.Select(x => x.ToString()).Should().Equal(expectedResult);
        }

        private static IEnumerable<ReadOnlyMemory<char>> SplitBasedCommandLineLexer(string input)
        {
            const char space = ' ';
            const char doubleQuote = '\"';
            var isInQuote = false;
            var isInEscape = false;
            return input.AsMemory().Split(
                (character, _, _) =>
                {
                    var actualIsInEscape = isInEscape;
                    isInEscape = false;
                    switch (character)
                    {
                        case doubleQuote:
                            if (!actualIsInEscape)
                            {
                                isInEscape = true;
                                isInQuote = true;
                            }

                            return actualIsInEscape ? SplitAction.Include : SplitAction.Ignore;
                        case space:
                            if (actualIsInEscape)
                            {
                                isInQuote = false;
                                return SplitAction.Split;
                            }

                            return isInQuote ? SplitAction.Include : SplitAction.Split;
                        default:
                            return SplitAction.Include;
                    }
                });
        }
    }
}