// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PercentageTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Numeric;

using System.Globalization;
using FluentAssertions;
using Sundew.Base.Numeric;
using Xunit;

public class PercentageTests
{
    [Theory]
    [InlineData("-3.5%", "en-US", -0.035)]
    [InlineData("1.5%", "en-US", 0.015)]
    [InlineData("-3,5 %", "da-DK", -0.035)]
    [InlineData("1,5 %", "da-DK", 0.015)]
    public void Parse_When_CultureInfoIsSpecified_Then_ResultShouldBeExpectedResult(string input, string culture, double expectedResult)
    {
        var result = Percentage.Parse(input, new CultureInfo(culture));

        result.Value.Should().Be(expectedResult);
    }
}