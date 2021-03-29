// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultAsyncTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Primitives.Computation
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Sundew.Base.Primitives.Computation;
    using Xunit;

    public class ResultAsyncTests
    {
        [Fact]
        public async Task ImplicitCast_WhenCastingToIfSuccess_Then_ValueShouldBeExpectedValue()
        {
            const int ExpectedValue = 34;

            Result.IfSuccess<int> result = await ComputeAsync(() => Result.SuccessAsync(ExpectedValue)).ConfigureAwait(false);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(ExpectedValue);
        }

        [Fact]
        public async Task ImplicitCast_When_CastingToResult_Then_ValueShouldBeExpectedValue()
        {
            const int ExpectedValue = 34;
            const int ExpectedError = 0;

            Result<int, double> result = await ComputeAsync(() => Result.SuccessAsync(ExpectedValue)).ConfigureAwait(false);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(ExpectedValue);
            result.Error.Should().Be(ExpectedError);
        }

        [Fact]
        public async Task ImplicitCast_WhenCastingToIfError_Then_ErrorShouldBeExpectedError()
        {
            const string ExpectedError = "Failed";

            Result.IfError<string> result = await ComputeAsync(() => Result.ErrorAsync(ExpectedError)).ConfigureAwait(false);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(ExpectedError);
        }

        [Fact]
        public async Task ImplicitCast_WhenCastingToResult_Then_ErrorShouldBeExpectedError()
        {
            const int ExpectedValue = 0;
            const string ExpectedError = "Failed";

            Result<int, string> result = await ComputeAsync(() => Result.ErrorAsync(ExpectedError)).ConfigureAwait(false);

            result.IsSuccess.Should().BeFalse();
            result.Value.Should().Be(ExpectedValue);
            result.Error.Should().Be(ExpectedError);
        }

        [Theory]
        [InlineData(true, 0)]
        [InlineData(false, 34)]
        public async Task ConvertError_Then_ResultErrorShouldBeExpectedError(
            bool expectedResult,
            int expectedError)
        {
            var testee = await ComputeAsync(() => Result.FromErrorAsync(expectedResult, expectedError)).ConfigureAwait(false);

            var result = testee.ConvertError(Convert.ToDouble);

            result.IsSuccess.Should().Be(expectedResult);
            result.Error.Should().Be(expectedError);
            result.Error.Should().BeOfType(typeof(double));
        }

        [Theory]
        [InlineData(true, 34, 0)]
        [InlineData(false, 0, 23)]
        public async Task WithValue_Then_ResultValueShouldBeExpectedValue(
            bool expectedResult,
            int expectedValue,
            int expectedError)
        {
            var testee = await ComputeAsync(() => Result.FromErrorAsync(expectedResult, expectedError)).ConfigureAwait(false);

            var result = testee.WithValue(expectedValue);

            result.IsSuccess.Should().Be(expectedResult);
            result.Value.Should().Be(expectedValue);
            result.Error.Should().Be(expectedError);
        }

        [Theory]
        [InlineData(true, 34, 0)]
        [InlineData(false, 0, 23)]
        public async Task Convert_Then_ResultShouldHaveExpectedValues(
            bool expectedResult,
            int expectedValue,
            int expectedError)
        {
            var testee = await ComputeAsync(() => Result.FromValueAsync(expectedResult, expectedValue)).ConfigureAwait(false);

            var result = testee.Convert(Convert.ToDouble, expectedError);

            result.IsSuccess.Should().Be(expectedResult);
            result.Value.Should().Be(expectedValue);
            result.Error.Should().Be((byte)expectedError);
        }

        [Fact]
        public async Task Deconstruction_When_DeconstructingAllParameters_Then_DeconstructedValuesShouldBedExpectedResult()
        {
            const bool ExpectedIsSuccess = true;
            const double ExpectedValue = 65d;
            const int ExpectedError = 45;

            var (isSuccess, value, error) = await ComputeAsync(() => Result.FromAsync(ExpectedIsSuccess, 65d, 45)).ConfigureAwait(false);

            isSuccess.Should().Be(ExpectedIsSuccess);
            value.Should().Be(ExpectedValue);
            error.Should().Be(ExpectedError);
        }

        [Fact]
        public async Task SuccessAsync_When_PassingResultIntoValueTaskAsyncMethod_Then_ResultShouldBeValueTaskOfResult()
        {
            const double ExpectedValue = 65d;

            Result.IfSuccess<double> result = await ComputeAsync(() => Result.SuccessAsync(65d)).ConfigureAwait(false);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(ExpectedValue);
        }

        [Fact]
        public async Task ErrorAsync_When_PassingResultIntoValueTaskAsyncMethod_Then_ResultShouldBeValueTaskOfResult()
        {
            const double ExpectedError = 65d;

            Result.IfError<double> result = await ComputeAsync(() => Result.ErrorAsync(65d)).ConfigureAwait(false);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(ExpectedError);
        }

        [Fact]
        public async Task SuccessAsync_When_PassingResultIntoValueTaskAsyncMethodAndReturningToResult_Then_ResultShouldBeValueTaskOfResult()
        {
            const double ExpectedValue = 65d;

            Result<double, double> result = await ComputeAsync(() => Result.SuccessAsync(65d)).ConfigureAwait(false);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(ExpectedValue);
        }

        [Fact]
        public async Task ErrorAsync_When_PassingResultIntoValueTaskAsyncMethodAndReturningToResult_Then_ResultShouldBeValueTaskOfResult()
        {
            const double ExpectedError = 65d;

            Result<double, double> result = await ComputeAsync(() => Result.ErrorAsync(65d)).ConfigureAwait(false);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(ExpectedError);
        }

        private static async ValueTask<TResult> ComputeAsync<TResult>(Func<ValueTask<TResult>> func)
        {
            return await func().ConfigureAwait(false);
        }
    }
}