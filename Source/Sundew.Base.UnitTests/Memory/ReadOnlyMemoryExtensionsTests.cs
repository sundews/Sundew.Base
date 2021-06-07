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

            var result = testee.Split(
                (character, index, _) =>
                {
                    return character switch
                    {
                        ';' => SplitAction.Split,
                        ':' => testee.Span[index + 1] == ';' ? SplitAction.Ignore : SplitAction.Include,
                        _ => SplitAction.Include,
                    };
                },
                SplitOptions.RemoveEmptyEntries);

            result.Select(x => x.ToString()).Should().Equal(expectedResults);
        }

        [Theory]
        [InlineData(@"-a -b ""1 _ ewr _ 23"" -c -d 32 -e 34", new[] { "-a", "-b", "1 _ ewr _ 23", "-c", "-d", "32", "-e", "34" })]
        [InlineData(@"-a -b ""1 \"" ewr \"" 23"" -c -d 32 -e 34", new[] { "-a", "-b", @"1 "" ewr "" 23", "-c", "-d", "32", "-e", "34" })]
        [InlineData("a warm up", new[] { "a", "warm", "up" })]
        public void Split_When_LexingCommandLine_Then_ResultShouldBeExpectedResult(string input, string[] expectedResult)
        {
            var result = SplitBasedCommandLineLexer(input.AsMemory());

            result.Select(x => x.ToString()).Should().Equal(expectedResult);
        }

        [Fact]
        public void Split_When_InputIsNull_ThenResultShouldBeEmpty()
        {
            const string? input = null;

            var result = input.AsMemory().Split((_, _, _) => SplitAction.Include);

            result.Should().BeEmpty();
        }

        [Fact]
        public void Split_When_SplittingOnLastCharacterAndRemoveEmptyEntriesIsSet_Then_LastEntryShouldBeRemoved()
        {
            const string Input = "m:s:t:";

            var result = Input.AsMemory().Split(
                (character, _, _) =>
                {
                    return character switch
                    {
                        ':' => SplitAction.Split,
                        _ => SplitAction.Include,
                    };
                },
                SplitOptions.RemoveEmptyEntries);

            result.Select(x => x.ToString()).Should().Equal("m", "s", "t");
        }

        [Theory]
        [InlineData("Name", new[] { "Name" })]
        [InlineData("Person.Name.Length", new[] { "Person", ".", "Name", ".", "Length" })]
        [InlineData("(DockPanel.Dock)", new[] { "(", "DockPanel", ".", "Dock", ")" })]
        [InlineData("(DockPanel.Dock).Length", new[] { "(", "DockPanel", ".", "Dock", ")", ".", "Length" })]
        [InlineData("Child.(DockPanel.Dock)", new[] { "Child", ".", "(", "DockPanel", ".", "Dock", ")" })]
        [InlineData("Persons[(sys:Int32)6].Length", new[] { "Persons", "[", "(", "sys", ":", "Int32", ")", "6", "]", ".", "Length" })]
        [InlineData("[Name,Age]", new[] { "[", "Name", ",", "Age", "]" })]
        [InlineData(".", new[] { "." })]
        [InlineData("", new string[0])]
        public void Split_Then_LaResultShouldBeExpectedResult(string input, string[] expectedResult)
        {
            var result = input.AsMemory().Split(
                (character, _, _) =>
                {
                    return character switch
                    {
                        ':' => SplitAction.SplitAndSplitCurrent,
                        '.' => SplitAction.SplitAndSplitCurrent,
                        ',' => SplitAction.SplitAndSplitCurrent,
                        '[' => SplitAction.SplitAndSplitCurrent,
                        ']' => SplitAction.SplitAndSplitCurrent,
                        '(' => SplitAction.SplitAndSplitCurrent,
                        ')' => SplitAction.SplitAndSplitCurrent,
                        _ => SplitAction.Include,
                    };
                },
                SplitOptions.RemoveEmptyEntries);

            result.Select(x => x.ToString()).Should().Equal(expectedResult);
        }

        [Theory]
        [InlineData("123   1234", 3, new[] { "   ", "1234" })]
        public void Splity_When_TrimmingWhitespaceAndTheStart_Then_ResultShouldBeExpectedResult(string input, int startIndex, string[] expectedResult)
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
        public void SplitMemory_When_UsingIncludeAndSplit_Then_ResultShouldBeExpectedResult(string input, string[] expectedResult)
        {
            var result = input.AsMemory().Split(
                (character, index, _) =>
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
        [InlineData(
            @"Sundew ""Text with space"" ""Multiple space after this""     http:\\url.com\ \""Sundew\""With\""Quotes\""\",
            new[] { "Sundew", "Text with space", "Multiple space after this", @"http:\\url.com\", @"""Sundew""With""Quotes""\" })]
        [InlineData(@"-fl \""\""", new[] { "-fl", @"""""" })]
        public void Split_Then_ResultShouldBeExpectedResult(string input, string[] expectedResult)
        {
            var result = SplitBasedCommandLineLexer(input.AsMemory());

            result.Select(x => x.ToString()).Should().Equal(expectedResult);
        }

        private static IEnumerable<ReadOnlyMemory<char>> SplitBasedCommandLineLexer(ReadOnlyMemory<char> input)
        {
            const char doubleQuote = '\"';
            const char slash = '\\';
            const char space = ' ';
            var isInQuote = false;
            var isInEscape = false;
            var previousWasSpace = false;
            return input.Split(
                (character, index, splitContext) =>
                {
                    var actualIsInEscape = isInEscape;
                    var actualPreviousWasSpace = previousWasSpace;
                    isInEscape = false;
                    previousWasSpace = false;
                    switch (character)
                    {
                        case slash:
                            if (splitContext.GetNextOrDefault(index) == doubleQuote)
                            {
                                isInEscape = true;
                                return SplitAction.Ignore;
                            }

                            return SplitAction.Include;
                        case doubleQuote:
                            if (!actualIsInEscape)
                            {
                                isInQuote = !isInQuote;
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
                SplitOptions.None);
        }
    }
}