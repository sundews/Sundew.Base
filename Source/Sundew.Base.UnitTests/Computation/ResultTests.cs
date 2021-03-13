// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Computation
{
    using System;
    using FluentAssertions;
    using Sundew.Base.Computation;
    using Xunit;

    public class ResultTests
    {
        [Fact]
        public void ImplicitCast_WhenCastingToIfSuccess_Then_ValueShouldBeExpectedValue()
        {
            const int ExpectedValue = 34;

            Result.IfSuccess<int> result = Result.Success(ExpectedValue);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(ExpectedValue);
        }

        [Fact]
        public void ImplicitCast_When_CastingToResult_Then_ValueShouldBeExpectedValue()
        {
            const int ExpectedValue = 34;
            const int ExpectedError = 0;

            Result<int, double> result = Result.Success(ExpectedValue);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(ExpectedValue);
            result.Error.Should().Be(ExpectedError);
        }

        [Fact]
        public void ImplicitCast_WhenCastingToIfError_Then_ErrorShouldBeExpectedError()
        {
            const string ExpectedError = "Failed";

            Result.IfError<string> result = Result.Error(ExpectedError);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(ExpectedError);
        }

        [Fact]
        public void ImplicitCast_WhenCastingToResult_Then_ErrorShouldBeExpectedError()
        {
            const int ExpectedValue = 0;
            const string ExpectedError = "Failed";

            Result<int, string> result = Result.Error(ExpectedError);

            result.IsSuccess.Should().BeFalse();
            result.Value.Should().Be(ExpectedValue);
            result.Error.Should().Be(ExpectedError);
        }

        [Theory]
        [InlineData(true, 0)]
        [InlineData(false, 34)]
        public void ConvertError_Then_ResultErrorShouldBeExpectedError(
            bool expectedResult,
            int expectedError)
        {
            var testee = Result.FromError(expectedResult, expectedError);

            var result = testee.ConvertError(Convert.ToDouble);

            result.IsSuccess.Should().Be(expectedResult);
            result.Error.Should().Be(expectedError);
            result.Error.Should().BeOfType(typeof(double));
        }

        [Theory]
        [InlineData(true, 34, 0)]
        [InlineData(false, 0, 23)]
        public void WithValue_Then_ResultValueShouldBeExpectedValue(
            bool expectedResult,
            int expectedValue,
            int expectedError)
        {
            var testee = Result.FromError(expectedResult, expectedError);

            var result = testee.WithValue(expectedValue);

            result.IsSuccess.Should().Be(expectedResult);
            result.Value.Should().Be(expectedValue);
            result.Error.Should().Be(expectedError);
        }

        [Theory]
        [InlineData(true, 34, 0)]
        [InlineData(false, 0, 23)]
        public void Convert_Then_ResultShouldHaveExpectedValues(
            bool expectedResult,
            int expectedValue,
            int expectedError)
        {
            var testee = Result.FromValue(expectedResult, expectedValue);

            var result = testee.Convert(Convert.ToDouble, expectedError);

            result.IsSuccess.Should().Be(expectedResult);
            result.Value.Should().Be(expectedValue);
            result.Error.Should().Be((byte)expectedError);
        }

        [Fact]
        public void Deconstruction_When_DeconstructingAllParameters_Then_DeconstructedValuesShouldBedExpectedResult()
        {
            const bool ExpectedIsSuccess = true;
            const double ExpectedValue = 65d;
            const int ExpectedError = 45;

            var (isSuccess, value, error) = Result.From(ExpectedIsSuccess, 65d, 45);

            isSuccess.Should().Be(ExpectedIsSuccess);
            value.Should().Be(ExpectedValue);
            error.Should().Be(ExpectedError);
        }
    }
}