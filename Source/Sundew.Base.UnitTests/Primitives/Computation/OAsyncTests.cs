// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OAsyncTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
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

    public class OAsyncTests
    {
        [Fact]
        public async Task SomeAsync_Then_ValueShouldBeExpectedValue()
        {
            const int ExpectedValue = 34;

            O<int> result = await ComputeAsync(() => O.SomeAsync(ExpectedValue)).ConfigureAwait(false);

            result.HasValue.Should().BeTrue();
            result.Value.Should().Be(ExpectedValue);
        }

        [Fact]
        public async Task ToResult_When_CastingToResult_Then_ValueShouldBeExpectedValue()
        {
            const int ExpectedValue = 34;
            const int ExpectedError = 0;

            var option = await ComputeAsync(() => O.SomeAsync(ExpectedValue)).ConfigureAwait(false);

            var result = option.ToResult(87);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(ExpectedValue);
            result.Error.Should().Be(ExpectedError);
        }

        [Fact]
        public async Task ToResult_When_OptionIsNoneOption_Then_ErrorShouldBeExpectedError()
        {
            const string ExpectedError = "Failed";
            var option = await ComputeAsync(O.NoneAsync).ConfigureAwait(false);

            var result = option.ToResult(ExpectedError);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(ExpectedError);
        }

        [Theory]
        [InlineData(true, 34)]
        [InlineData(false, 0)]
        public async Task With_Then_ResultValueShouldBeExpectedValue(
            bool expectedResult,
            int expectedValue)
        {
            var testee = await ComputeAsync(() => O.FromAsync(expectedResult, 45)).ConfigureAwait(false);

            var result = testee.With(expectedValue);

            result.HasValue.Should().Be(expectedResult);
            result.Value.Should().Be(expectedValue);
        }

        [Theory]
        [InlineData(true, 34, 0)]
        [InlineData(false, 0, 23)]
        public async Task ToResult_Then_ResultShouldHaveExpectedValues(
            bool expectedResult,
            int expectedValue,
            int expectedError)
        {
            var testee = await ComputeAsync(() => O.FromAsync(expectedResult, expectedValue)).ConfigureAwait(false);

            var result = testee.ToResult(Convert.ToDouble, expectedError);

            result.IsSuccess.Should().Be(expectedResult);
            result.Value.Should().Be(expectedValue);
            result.Error.Should().Be((byte)expectedError);
        }

        [Theory]
        [InlineData(true, 34, 34)]
        [InlineData(false, 45, 0)]
        public async Task Deconstruction_When_DeconstructingAllParameters_Then_DeconstructedValuesShouldBedExpectedResult(
            bool expectedResult,
            int inputValue,
            int expectedValue)
        {
            var (isSuccess, value) = await ComputeAsync(() => O.FromAsync(expectedResult, inputValue)).ConfigureAwait(false);

            isSuccess.Should().Be(expectedResult);
            value.Should().Be(expectedValue);
        }

        private static async ValueTask<TResult> ComputeAsync<TResult>(Func<ValueTask<TResult>> func)
        {
            return await func().ConfigureAwait(false);
        }
    }
}