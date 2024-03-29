﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Primitives
{
    using System;
    using FluentAssertions;
    using Sundew.Base.Primitives;
    using Xunit;

    public class OTests
    {
        [Theory]
        [InlineData(true, 34, 0)]
        [InlineData(false, 0, 23)]
        public void ToResult_Then_ResultShouldHaveExpectedValues(
            bool expectedResult,
            int expectedValue,
            int expectedError)
        {
            int? testee = expectedResult ? expectedValue : null;

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
            R<int, int>? testee = option ? R.From(result, 1, 2) : null;

            var actualResult = testee.IsSuccess(out var optionalValue, out var failedResult);

            actualResult.Should().Be(expectedResult);
            optionalValue.HasValue.Should().Be(expectedHasValue);
            failedResult.IsSuccess.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(true, 5, 5)]
        [InlineData(false, 5, null)]
        public void ToOption_When_OptionalValueIsStruct_Then_ResultShouldBeExpectedResult(bool option, int optionalValue, int? expectedResult)
        {
            var result = option.ToOption(optionalValue);

            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(true, "5", "5")]
        [InlineData(false, "5", null)]
        public void ToOption_When_OptionalValueIsClass_Then_ResultShouldBeExpectedResult(bool option, string optionalValue, string? expectedResult)
        {
            var result = option.ToOption(optionalValue);

            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(true, "5", "5")]
        [InlineData(false, "5", null)]
        public void From_Then_ResultShouldBeExpectedResult(bool option, string optionalValue, string? expectedResult)
        {
            var result = O.From(option, () => optionalValue);

            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(true, 5, 5)]
        [InlineData(false, 5, default)]
        public void FromValue_Then_ResultShouldBeExpectedResult2(bool option, int optionalValue, int? expectedResult)
        {
            var result = O.FromValue(option, () => optionalValue);

            result.Should().Be(expectedResult);
        }
    }
}