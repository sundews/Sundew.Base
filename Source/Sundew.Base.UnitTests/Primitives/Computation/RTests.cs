// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Primitives.Computation
{
    using System;
    using FluentAssertions;
    using Sundew.Base.Primitives.Computation;
    using Xunit;

    public class RTests
    {
        [Fact]
        public void ImplicitCast_When_CastingToResult_Then_ValueShouldBeExpectedValue()
        {
            const int ExpectedValue = 34;
            const int ExpectedError = 0;

            R<int, double> r = R.Success(ExpectedValue);

            r.IsSuccess.Should().BeTrue();
            r.Value.Should().Be(ExpectedValue);
            r.Error.Should().Be(ExpectedError);
        }

        [Fact]
        public void ImplicitCast_WhenCastingToIfError_Then_ErrorShouldBeExpectedError()
        {
            const string ExpectedError = "Failed";

            R<string> r = R.Error(ExpectedError);

            r.IsSuccess.Should().BeFalse();
            r.Error.Should().Be(ExpectedError);
        }

        [Fact]
        public void ImplicitCast_WhenCastingToResult_Then_ErrorShouldBeExpectedError()
        {
            const int ExpectedValue = 0;
            const string ExpectedError = "Failed";

            R<int, string> r = R.Error(ExpectedError);

            r.IsSuccess.Should().BeFalse();
            r.Value.Should().Be(ExpectedValue);
            r.Error.Should().Be(ExpectedError);
        }

        [Theory]
        [InlineData(true, 0)]
        [InlineData(false, 34)]
        public void With_Then_ResultErrorShouldBeExpectedError(
            bool expectedResult,
            int expectedError)
        {
            var testee = R.From(expectedResult, expectedError);

            var result = testee.With(Convert.ToDouble);

            result.IsSuccess.Should().Be(expectedResult);
            result.Error.Should().Be(expectedError);
            result.Error.Should().BeOfType(typeof(double));
        }

        [Theory]
        [InlineData(true, 34, 0)]
        [InlineData(false, 0, 23)]
        public void To_Then_ResultValueShouldBeExpectedValue(
            bool expectedResult,
            int expectedValue,
            int expectedError)
        {
            var testee = R.From(expectedResult, () => expectedError);

            var result = testee.To(expectedValue);

            result.IsSuccess.Should().Be(expectedResult);
            result.Value.Should().Be(expectedValue);
            result.Error.Should().Be(expectedError);
        }

        [Fact]
        public void Deconstruction_When_DeconstructingAllParameters_Then_DeconstructedValuesShouldBedExpectedResult()
        {
            const bool ExpectedIsSuccess = true;
            const double ExpectedValue = 65d;
            const int ExpectedError = 45;

            var (isSuccess, value, error) = R.From(ExpectedIsSuccess, 65d, ExpectedError);

            isSuccess.Should().Be(ExpectedIsSuccess);
            value.Should().Be(ExpectedValue);
            error.Should().Be(ExpectedError);
        }

        [Fact]
        public void ImplicitCast_When_SuccessResultAndTargetIsOptionalStruct_Then_ResultShouldBeSuccessAndValueShouldBeNull()
        {
            var testee = R.Success();

            R<int?, string> result = testee;

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeNull();
        }

        [Fact]
        public void ImplicitCast_When_SuccessResultAndTargetIsOptionalClass_Then_ResultShouldBeSuccessAndValueShouldBeNull()
        {
            var testee = R.Success();

            R<string?, string> result = testee;

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeNull();
        }

        [Theory]
        [InlineData("Value")]
        [InlineData(null)]
        public void ImplicitCast_When_SuccessResultOptionAndTargetIsOptionalClass_Then_ResultShouldBeSuccessAndValueShouldBeExpectedResult(string? expectedResult)
        {
            var testee = R.SuccessOption(expectedResult);

            R<string?, string> result = testee;

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(expectedResult);
        }
    }
}