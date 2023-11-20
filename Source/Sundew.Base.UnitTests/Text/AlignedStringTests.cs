// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AlignedStringTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Text
{
    using FluentAssertions;
    using Sundew.Base.Text;
    using Xunit;

    public class AlignedStringTests
    {
        [Theory]
        [InlineData(1.23, "{0:12,-=><}", @"/----1.23----\")]
        [InlineData(1.20, "{0:12,-=><}", @"/-----1.2----\")]
        [InlineData(1.20, "{0:12,-=<}",  @"/----1.2-----\")]
        [InlineData(1.23, "{0:12,-=<<}", @"/----1.23----\")]
        [InlineData(1.20, "{0:12,-=<<}", @"/----1.2-----\")]
        [InlineData(1.20, "{0:12,-<<}",  @"/1.2---------\")]
        [InlineData(1.23, "{0:12,-<<}",  @"/1.23--------\")]
        [InlineData(1.20, "{0:12,-><}",  @"/---------1.2\")]
        [InlineData(1.23, "{0:12,-><}",  @"/--------1.23\")]
        [InlineData(1.23, "{0:12, =><}", @"/    1.23    \")]
        [InlineData(1.20, "{0:12, =><}", @"/     1.2    \")]
        [InlineData(1.20, "{0:12, =<}",  @"/    1.2     \")]
        [InlineData(1.23, "{0:12, =<<}", @"/    1.23    \")]
        [InlineData(1.20, "{0:12, =<<}", @"/    1.2     \")]
        [InlineData(1.20, "{0:12, <<}",  @"/1.2         \")]
        [InlineData(1.23, "{0:12, <<}",  @"/1.23        \")]
        [InlineData(1.20, "{0:12, ><}",  @"/         1.2\")]
        [InlineData(1.23, "{0:12, ><}",  @"/        1.23\")]
        [InlineData(1.2345678910111213, "{0:12,-=<>:N}", @"/----1.23----\")]
        [InlineData(1.2345678910111213, "{0:12,-=<>:N3}", @"/---1.235----\")]
        [InlineData(1.2345678910111213, "{0:12,-=<>}", @"/1.2345678910\")]
        [InlineData(1.2345678910111213, "{0:12,-=<<}", @"/678910111213\")]
        [InlineData(1.2345678910111213, "{0:12,-=<<:N}", @"/----1.23----\")]
        [InlineData(1.2345678910111213, "{0:12,-=<<:N3}", @"/---1.235----\")]
        [InlineData(1.2345678910111213, "{0:12,-=<>E:N14}", @"/1.234567891…\")]
        [InlineData(null, @"{0:12,-=<<}", @"/------------\")]
        [InlineData(Strings.Empty, @"{0:12,-=<<}", @"/------------\")]
        [InlineData("A long text that should be limited", "{0:12,-<>E}", @"/A long text…\")]
        [InlineData("A long text that should be limited", "{0:12,-<>D}", @"/A long te...\")]
        [InlineData("A long text that should be limited", "{0:12,-<<E}", @"/ …be limited\")]
        [InlineData("A long text that should be limited", "{0:12,-<<D}", @"/...e limited\")]
        [InlineData("A long text that should be limited", "{0:11,-<<D}", @"/ ...limited\")]
        [InlineData("A long text that should be limited", "{0:13,-<>E}", @"/A long text… \")]
        [InlineData("A long text that should be limited", "{0:10,-<>D}", @"/A long... \")]
        [InlineData("A long text             with space", "{0:18,-<>D}", @"/A long text...    \")]
        [InlineData("A long text             with space", "{0:18,-<>E}", @"/A long text…      \")]
        public void Format_Then_ResultShouldBeExpectedResult(object? value, string format, string expectedResult)
        {
            var result = AlignedString.FormatInvariant($@"/{format}\", value);

            result.Should().Be(expectedResult);
        }
    }
}