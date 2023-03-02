// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RAsyncTests.cs" company="Hukano">
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

    public class RAsyncTests
    {
        [Fact]
        public async Task ImplicitCast_When_CastingToResult_Then_ValueShouldBeExpectedValue()
        {
            const int ExpectedValue = 34;
            const int ExpectedError = 0;

            R<int, double> r = await ComputeAsync(() => R.SuccessAsync(ExpectedValue)).ConfigureAwait(false);

            r.IsSuccess.Should().BeTrue();
            r.Value.Should().Be(ExpectedValue);
            r.Error.Should().Be(ExpectedError);
        }

        [Fact]
        public async Task ImplicitCast_WhenCastingToIfError_Then_ErrorShouldBeExpectedError()
        {
            const string ExpectedError = "Failed";

            R<string> r = await ComputeAsync(() => R.ErrorAsync(ExpectedError)).ConfigureAwait(false);

            r.IsSuccess.Should().BeFalse();
            r.Error.Should().Be(ExpectedError);
        }

        [Fact]
        public async Task ImplicitCast_WhenCastingToResult_Then_ErrorShouldBeExpectedError()
        {
            const int ExpectedValue = default;
            const string ExpectedError = "Failed";

            R<int, string> r = await ComputeAsync(() => R.ErrorAsync(ExpectedError)).ConfigureAwait(false);

            r.IsSuccess.Should().BeFalse();
            r.Value.Should().Be(ExpectedValue);
            r.Error.Should().Be(ExpectedError);
        }

        [Theory]
        [InlineData(true, 0)]
        [InlineData(false, 34)]
        public async Task With_Then_ResultErrorShouldBeExpectedError(
            bool expectedResult,
            int expectedError)
        {
            var testee = await ComputeAsync(() => R.FromErrorAsync(expectedResult, expectedError)).ConfigureAwait(false);

            var result = testee.With(Convert.ToDouble);

            result.IsSuccess.Should().Be(expectedResult);
            result.Error.Should().Be(expectedError);
            result.Error.Should().BeOfType(typeof(double));
        }

        [Theory]
        [InlineData(true, 34, 0)]
        [InlineData(false, 0, 23)]
        public async Task To_Then_ResultValueShouldBeExpectedValue(
            bool expectedResult,
            int expectedValue,
            int expectedError)
        {
            var testee = await ComputeAsync(() => R.FromErrorAsync(expectedResult, expectedError)).ConfigureAwait(false);

            var result = testee.To(expectedValue);

            result.IsSuccess.Should().Be(expectedResult);
            result.Value.Should().Be(expectedValue);
            result.Error.Should().Be(expectedError);
        }

        [Fact]
        public async Task Deconstruction_When_DeconstructingAllParameters_Then_DeconstructedValuesShouldBedExpectedResult()
        {
            const bool ExpectedIsSuccess = true;
            const double ExpectedValue = 65d;

            var (isSuccess, value, error) = await ComputeAsync(() => R.FromAsync(ExpectedIsSuccess, ExpectedValue, 45)).ConfigureAwait(false);

            isSuccess.Should().Be(ExpectedIsSuccess);
            value.Should().Be(ExpectedValue);
            error.Should().Be(default);
        }

        [Fact]
        public async Task ErrorAsync_When_PassingResultIntoValueTaskAsyncMethod_Then_ResultShouldBeValueTaskOfResult()
        {
            const double ExpectedError = 65d;

            R<double> r = await ComputeAsync(() => R.ErrorAsync(65d)).ConfigureAwait(false);

            r.IsSuccess.Should().BeFalse();
            r.Error.Should().Be(ExpectedError);
        }

        [Fact]
        public async Task SuccessAsync_When_PassingResultIntoValueTaskAsyncMethodAndReturningToResult_Then_ResultShouldBeValueTaskOfResult()
        {
            const double ExpectedValue = 65d;

            R<double, double> r = await ComputeAsync(() => R.SuccessAsync(65d)).ConfigureAwait(false);

            r.IsSuccess.Should().BeTrue();
            r.Value.Should().Be(ExpectedValue);
            r.Error.Should().Be(default);
        }

        [Fact]
        public async Task ErrorAsync_When_PassingResultIntoValueTaskAsyncMethodAndReturningToResult_Then_ResultShouldBeValueTaskOfResult()
        {
            const double ExpectedError = 65d;

            R<double, double> r = await ComputeAsync(() => R.ErrorAsync(65d)).ConfigureAwait(false);

            r.IsSuccess.Should().BeFalse();
            r.Value.Should().Be(default);
            r.Error.Should().Be(ExpectedError);
        }

        private static async ValueTask<TResult> ComputeAsync<TResult>(Func<ValueTask<TResult>> func)
        {
            return await func().ConfigureAwait(false);
        }
    }
}