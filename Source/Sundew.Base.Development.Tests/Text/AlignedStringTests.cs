// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AlignedStringTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Text;

using AwesomeAssertions;
using Sundew.Base.Text;

public class AlignedStringTests
{
    [Test]
    [Arguments(1.23, "{0:12,-=><}", @"/----1.23----\")]
    [Arguments(1.20, "{0:12,-=><}", @"/-----1.2----\")]
    [Arguments(1.20, "{0:12,-=<}", @"/----1.2-----\")]
    [Arguments(1.23, "{0:12,-=<<}", @"/----1.23----\")]
    [Arguments(1.20, "{0:12,-=<<}", @"/----1.2-----\")]
    [Arguments(1.20, "{0:12,-<<}", @"/1.2---------\")]
    [Arguments(1.23, "{0:12,-<<}", @"/1.23--------\")]
    [Arguments(1.20, "{0:12,-><}", @"/---------1.2\")]
    [Arguments(1.23, "{0:12,-><}", @"/--------1.23\")]
    [Arguments(1.23, "{0:12, =><}", @"/    1.23    \")]
    [Arguments(1.20, "{0:12, =><}", @"/     1.2    \")]
    [Arguments(1.20, "{0:12, =<}", @"/    1.2     \")]
    [Arguments(1.23, "{0:12, =<<}", @"/    1.23    \")]
    [Arguments(1.20, "{0:12, =<<}", @"/    1.2     \")]
    [Arguments(1.20, "{0:12, <<}", @"/1.2         \")]
    [Arguments(1.23, "{0:12, <<}", @"/1.23        \")]
    [Arguments(1.20, "{0:12, ><}", @"/         1.2\")]
    [Arguments(1.23, "{0:12, ><}", @"/        1.23\")]
    [Arguments(1.2345678910111213, "{0:12,-=<>:N}", @"/----1.23----\")]
    [Arguments(1.2345678910111213, "{0:12,-=<>:N3}", @"/---1.235----\")]
    [Arguments(1.2345678910111213, "{0:12,-=<>}", @"/1.2345678910\")]
    [Arguments(1.2345678910111213, "{0:12,-=<<}", @"/678910111213\")]
    [Arguments(1.2345678910111213, "{0:12,-=<<:N}", @"/----1.23----\")]
    [Arguments(1.2345678910111213, "{0:12,-=<<:N3}", @"/---1.235----\")]
    [Arguments(1.2345678910111213, "{0:12,-=<>E:N14}", @"/1.234567891…\")]
    [Arguments(null, @"{0:12,-=<<}", @"/------------\")]
    [Arguments(Strings.Empty, @"{0:12,-=<<}", @"/------------\")]
    [Arguments("A long text that should be limited", "{0:12,-<>E}", @"/A long text…\")]
    [Arguments("A long text that should be limited", "{0:12,-<>D}", @"/A long te...\")]
    [Arguments("A long text that should be limited", "{0:12,-<<E}", @"/ …be limited\")]
    [Arguments("A long text that should be limited", "{0:12,-<<D}", @"/...e limited\")]
    [Arguments("A long text that should be limited", "{0:11,-<<D}", @"/ ...limited\")]
    [Arguments("A long text that should be limited", "{0:13,-<>E}", @"/A long text… \")]
    [Arguments("A long text that should be limited", "{0:10,-<>D}", @"/A long... \")]
    [Arguments("A long text             with space", "{0:18,-<>D}", @"/A long text...    \")]
    [Arguments("A long text             with space", "{0:18,-<>E}", @"/A long text…      \")]
    public void Format_Then_ResultShouldBeExpectedResult(object? value, string format, string expectedResult)
    {
        var result = AlignedString.FormatInvariant($@"/{format}\", value);

        result.Should().Be(expectedResult);
    }
}