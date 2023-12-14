// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComparisonTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Numeric
{
    using FluentAssertions;
    using Sundew.Base.Numeric;
    using Xunit;

    public class ComparisonTests
    {
        [Fact]
        public void Min_Then_ResultShouldBeExpected()
        {
            var expectedPercentage = new Percentage(0.5);
            var percentageHigh = new Percentage(0.6);

            var result = Comparison.Min(expectedPercentage, percentageHigh);

            result.Should().Be(expectedPercentage);
        }

        [Fact]
        public void Max_Then_ResultShouldBeExpected()
        {
            var percentageLow = new Percentage(0.5);
            var expectedPercentage = new Percentage(0.6);

            var result = Comparison.Max(percentageLow, expectedPercentage);

            result.Should().Be(expectedPercentage);
        }

        [Fact]
        public void Min_When_BothAreEqual_Then_ResultShouldBeBoth()
        {
            var percentageLow = new Percentage(0.5);
            var percentageHigh = new Percentage(0.5);

            var result = Comparison.Min(percentageLow, percentageHigh);

            result.Should().Be(percentageLow);
            result.Should().Be(percentageHigh);
        }

        [Fact]
        public void Max_When_BothAreEqual_Then_ResultShouldBeBoth()
        {
            var percentageLow = new Percentage(0.6);
            var percentageHigh = new Percentage(0.6);

            var result = Comparison.Max(percentageLow, percentageHigh);

            result.Should().Be(percentageLow);
            result.Should().Be(percentageHigh);
        }
    }
}