// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Primitives.Computation
{
    using System;
    using FluentAssertions;
    using Sundew.Base.Primitives.Computation;
    using Xunit;

    public class OTests
    {
        [Fact]
        public void Some_Then_ValueShouldBeExpectedValue()
        {
            const int ExpectedValue = 34;

            O<int> result = O.Some(ExpectedValue);

            result.HasValue.Should().BeTrue();
            result.Value.Should().Be(ExpectedValue);
        }

        [Fact]
        public void ImplicitCast_When_OptionIsNone_Then_ValueShouldBeExpectedValue()
        {
            O<int> result = O.None;

            result.HasValue.Should().BeFalse();
        }

        [Theory]
        [InlineData(true, 34, 0)]
        [InlineData(false, 0, 23)]
        public void ToResult_Then_ResultShouldHaveExpectedValues(
            bool expectedResult,
            int expectedValue,
            int expectedError)
        {
            var testee = O.From(expectedResult, expectedValue);

            var result = testee.ToResult(Convert.ToDouble, expectedError);

            result.IsSuccess.Should().Be(expectedResult);
            result.Value.Should().Be(expectedValue);
            result.Error.Should().Be((byte)expectedError);
        }

        [Theory]
        [InlineData(true, true, true, true)]
        [InlineData(true, false, false, false)]
        [InlineData(false, false, true, false)]
        [InlineData(false, true, true, false)]
        public void IsSuccess_Then_ResultShouldHaveExpectedValues(
            bool option,
            bool result,
            bool expectedResult,
            bool expectedHasValue)
        {
            var testee = O.From(option, R.From(result, 1, 2));

            var actualResult = testee.IsSuccess(out var optionalValue, out var failedResult);

            actualResult.Should().Be(expectedResult);
            optionalValue.HasValue.Should().Be(expectedHasValue);
            failedResult.IsSuccess.Should().Be(expectedResult);
        }
    }
}