// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensionsTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Text
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FluentAssertions;
    using Sundew.Base.Text;
    using Xunit;

    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("sundew", "Sundew")]
        [InlineData("Sundew", "Sundew")]
        [InlineData("S", "S")]
        [InlineData("s", "S")]
        [InlineData("", "")]
        [InlineData(null, null)]
        public void Capitalize_Then_FirstLetterInResultShouldBeUpperCase(string input, string expectedResult)
        {
            var result = input.Capitalize();

            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("sundew", "sundew")]
        [InlineData("Sundew", "sundew")]
        [InlineData("S", "s")]
        [InlineData("s", "s")]
        [InlineData("", "")]
        [InlineData(null, null)]
        public void Uncapitalize_Then_FirstLetterInResultShouldBeUpperCase(string input, string expectedResult)
        {
            var result = input.Uncapitalize();

            result.Should().Be(expectedResult);
        }

        [Fact]
        public void RemoveWhitespace_Then_ResultShouldNotContainAnyWhiteSpaces()
        {
            const string AnyString = "s u n d e w";

            var result = AnyString.RemoveWhitespace();

            result.Should().Be("sundew");
        }

        [Theory]
        [InlineData("0123456789", 5, "ABC", "01234ABC89")]
        [InlineData("0123456789", 0, "ABC", "ABC3456789")]
        [InlineData("0123456789", 7, "ABC", "0123456ABC")]
        public void ReplaceAt_Then_ResultShouldBeExpectedResult(string input, int startIndex, string replacement, string expectedResult)
        {
            var result = input.ReplaceAt(startIndex, replacement);

            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(5)]
        public void ReplaceAt_When_StartIndexOutOfRange_Then_ArgumentOutOfRangeExceptionShouldBeThrown(int startIndex)
        {
            const string input = "0123";

            Action action = () => input.ReplaceAt(startIndex, "12");

            action.Should().Throw<ArgumentOutOfRangeException>().And.ParamName.Should().Be("startIndex");
        }

        [Theory]
        [InlineData(1, -1)]
        [InlineData(1, 4)]
        [InlineData(3, 4)]
        public void ReplaceAt_When_LengthOutOfRange_Then_ArgumentOutOfRangeExceptionShouldBeThrown(int startIndex, int length)
        {
            const string input = "0123";

            Action action = () => input.ReplaceAt(startIndex, "567", length);

            action.Should().Throw<ArgumentOutOfRangeException>().And.ParamName.Should().Be("length");
        }

        [Fact]
        public void Split_When_InputIsNull_ThenResultShouldBeEmpty()
        {
            const string? input = null;

            var result = input.Split((_, _, _) => SplitAction.Include);

            result.Should().BeEmpty();
        }

        [Fact]
        public void Split_When_SplittingOnLastCharacterAndRemoveEmptyEntriesIsSet_Then_LastEntryShouldBeRemoved()
        {
            const string Input = "m:s:t:";

            var result = Input.Split(
                (character, _, _) =>
                {
                    return character switch
                    {
                        ':' => SplitAction.Split,
                        _ => SplitAction.Include,
                    };
                },
                StringSplitOptions.RemoveEmptyEntries);

            result.Should().Equal("m", "s", "t");
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
            var result = input.Split(
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
                StringSplitOptions.RemoveEmptyEntries);

            result.Should().Equal(expectedResult);
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
        public void SplitMemory_Then_ResultShouldBeExpectedResult(string input, string[] expectedResult)
        {
            var result = input.SplitMemory(
                (character, _) =>
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
                StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToString());

            result.Should().Equal(expectedResult);
        }

        [Theory]
        [InlineData("123   1234", 3, new[] { "   ", "1234" })]
        public void SplitMemory_When_TrimmingWhitespaceAndTheStart_Then_ResultShouldBeExpectedResult(string input, int startIndex, string[] expectedResult)
        {
            var result = input.SplitMemory(
                (character, index) =>
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
        [InlineData("123   1234", 3, new[] { "   ", "1234" })]
        public void Splity_When_TrimmingWhitespaceAndTheStart_Then_ResultShouldBeExpectedResult(string input, int startIndex, string[] expectedResult)
        {
            var result = input.Split(
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
            var result = input.SplitMemory(
                (character, index) =>
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
        [InlineData("ValuE1234", new[] { "ValuE", "1234" })]
        public void Split_When_UsingIncludeAndSplit_Then_ResultShouldBeExpectedResult(string input, string[] expectedResult)
        {
            var result = input.Split(
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
        [InlineData("-a- warm up", new[] { "-a-", "warm", "up" })]
        [InlineData(@"-a -b ""1 _ ewr _ 23"" -c -d 32 -e 34", new[] { "-a", "-b", "1 _ ewr _ 23", "-c", "-d", "32", "-e", "34" })]
        [InlineData(@"-a -b ""1 """" ewr """" 23"" -c -d 32 -e 34", new[] { "-a", "-b", @"1 "" ewr "" 23", "-c", "-d", "32", "-e", "34" })]
        public void Split_When_LexingCommandLine_Then_ResultShouldBeExpectedResult(string input, string[] expectedResult)
        {
            var result = SplitBasedCommandLineParser(input.AsMemory());

            result.Should().Equal(expectedResult);
        }

        [Theory]
        [InlineData(
            @"Sundew ""Text with space"" ""Multiple space after this""     http:\\url.com\ """"Sundew""""With""""Quotes""""\",
            new[] { "Sundew", "Text with space", "Multiple space after this", @"http:\\url.com\", @"""Sundew""With""Quotes""\" })]
        [InlineData(@"-fl """"""""", new[] { "-fl", @"""""" })]
        public void Split_Then_ResultShouldBeExpectedResult(string input, string[] expectedResult)
        {
            var result = SplitBasedCommandLineParser(input.AsMemory());

            result.Should().Equal(expectedResult);
        }

        [Theory]
        [InlineData("0123456789ABCDEFGHIJK", Alignment.Right, false, "0123456789")]
        [InlineData("0123456789ABCDEFGHIJ", Alignment.Right, true, "ABCDEFGHIJ")]
        [InlineData("0123456789", Alignment.Right, false, "0123456789")]
        [InlineData("s", Alignment.CenterLeft, false, "    s     ")]
        [InlineData("s", Alignment.CenterRight, false, "     s    ")]
        [InlineData("s", Alignment.Right, false, "         s")]
        [InlineData("s", Alignment.Left, false, "s         ")]
        [InlineData("", Alignment.Right, false, "          ")]
        [InlineData(null, Alignment.Right, false, "          ")]
        public void LimitAndPad_Then_StringLengthShouldBe10(string input, Alignment alignment, bool isLimitLeft, string expectedResult)
        {
            var result = input.AlignAndLimit(10, ' ', alignment, Limit.With(isLimitLeft));

            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(null, new string[0])]
        [InlineData("", new string[0])]
        [InlineData(";", new string[0])]
        [InlineData(";;", new[] { ";" })]
        [InlineData("T;", new[] { "T" })]
        [InlineData("T;;", new[] { "T;", })]
        [InlineData("##vso[task.setvariable variable=package_{0}]{3}", new[] { "##vso[task.setvariable variable=package_{0}]{3}" })]
        [InlineData("##vso[task.setvariable variable=package_{0}]{3};##vso[task.setvariable variable=source_{0}]{2}", new[] { "##vso[task.setvariable variable=package_{0}]{3}", "##vso[task.setvariable variable=source_{0}]{2}" })]
        [InlineData("MessageWithSemiColon;;ShouldNotSplitWhenEscaped", new[] { "MessageWithSemiColon;ShouldNotSplitWhenEscaped" })]
        [InlineData("1;2;3", new[] { "1", "2", "3" })]
        public void Split_When_CharacterSplitParserIsUsed_Then_ResultShouldBeExpectedResult(string input, string[] expectedResult)
        {
            var result = CharacterSplitParser(input);

            result.Should().Equal(expectedResult);
        }

        [Theory]
        [InlineData(null, new string[0])]
        [InlineData("", new string[0])]
        [InlineData(";", new string[0])]
        [InlineData(";;", new string[0])]
        [InlineData("T;", new[] { "T" })]
        [InlineData("T;;", new[] { "T", })]
        [InlineData("##vso[task.setvariable variable=package_{0}]{3}", new[] { "##vso[task.setvariable variable=package_{0}]{3}" })]
        [InlineData("##vso[task.setvariable variable=package_{0}]{3};##vso[task.setvariable variable=source_{0}]{2}", new[] { "##vso[task.setvariable variable=package_{0}]{3}", "##vso[task.setvariable variable=source_{0}]{2}" })]
        [InlineData("1;2;3", new[] { "1", "2", "3" })]
        public void SplitMemory_When_CharacterSplitMemoryParserIsUsed_Then_ResultShouldBeExpectedResult(string input, string[] expectedResult)
        {
            var result = CharacterSplitMemoryParser(input);

            result.Select(x => x.ToString()).Should().Equal(expectedResult);
        }

        private static IEnumerable<string> CharacterSplitParser(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                const char semiColon = ';';
                var lastWasSemiColon = false;
                return input.Split(
                    (character, _, _) =>
                    {
                        var wasSemiColon = lastWasSemiColon;
                        if (wasSemiColon)
                        {
                            lastWasSemiColon = false;
                        }

                        switch (character)
                        {
                            case semiColon:
                                if (wasSemiColon)
                                {
                                    return SplitAction.Include;
                                }

                                lastWasSemiColon = true;
                                return SplitAction.Ignore;
                            default:
                                if (wasSemiColon)
                                {
                                    return SplitAction.SplitAndInclude;
                                }

                                return SplitAction.Include;
                        }
                    },
                    StringSplitOptions.RemoveEmptyEntries);
            }

            return Array.Empty<string>();
        }

        private static IEnumerable<ReadOnlyMemory<char>> CharacterSplitMemoryParser(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                const char semiColon = ';';
                return input.SplitMemory(
                    (character, _) =>
                    {
                        switch (character)
                        {
                            case semiColon:
                                return SplitAction.Split;
                            default:
                                return SplitAction.Include;
                        }
                    },
                    StringSplitOptions.RemoveEmptyEntries);
            }

            return Array.Empty<ReadOnlyMemory<char>>();
        }

        private static IEnumerable<string> SplitBasedCommandLineParser(ReadOnlyMemory<char> input)
        {
            const char doubleQuote = '\"';
            const char space = ' ';
            var isInQuote = false;
            var isInEscape = false;
            return input.Split(
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
                },
                StringSplitOptions.RemoveEmptyEntries);
        }
    }
}