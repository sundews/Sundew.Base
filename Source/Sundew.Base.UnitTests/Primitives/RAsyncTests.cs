// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RAsyncTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Primitives;

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

public class RAsyncTests
{
    [Fact]
    public async Task ImplicitCast_When_CastingToResult_Then_ValueShouldBeExpectedValue()
    {
        const int expectedValue = 34;
        const int expectedError = 0;

        R<int, double> r = await ComputeAsync(() => R.SuccessAsync(expectedValue));

        r.IsSuccess.Should().BeTrue();
        r.Value.Should().Be(expectedValue);
        r.Error.Should().Be(expectedError);
    }

    [Fact]
    public async Task ImplicitCast_WhenCastingToIfError_Then_ErrorShouldBeExpectedError()
    {
        const string expectedError = "Failed";

        RoE<string> r = await ComputeAsync(() => R.ErrorAsync(expectedError));

        r.IsSuccess.Should().BeFalse();
        r.Error.Should().Be(expectedError);
    }

    [Fact]
    public async Task ImplicitCast_WhenCastingToResult_Then_ErrorShouldBeExpectedError()
    {
        const int expectedValue = default;
        const string expectedError = "Failed";

        R<int, string> r = await ComputeAsync(() => R.ErrorAsync(expectedError));

        r.IsSuccess.Should().BeFalse();
        r.Value.Should().Be(expectedValue);
        r.Error.Should().Be(expectedError);
    }

    [Theory]
    [InlineData(true, 0)]
    [InlineData(false, 34)]
    public async Task With_Then_ResultErrorShouldBeExpectedError(
        bool expectedResult,
        int expectedError)
    {
        var testee = await ComputeAsync(() => R.FromErrorAsync(expectedResult, expectedError));

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
        var testee = await ComputeAsync(() => R.FromErrorAsync(expectedResult, expectedError));

        var result = testee.With(expectedValue);

        result.IsSuccess.Should().Be(expectedResult);
        result.Value.Should().Be(expectedValue);
        result.Error.Should().Be(expectedError);
    }

    [Fact]
    public async Task Deconstruction_When_DeconstructingAllParameters_Then_DeconstructedValuesShouldBedExpectedResult()
    {
        const bool expectedIsSuccess = true;
        const double expectedValue = 65d;
        const double expectedError = 45d;

        var (isSuccess, value, error) = await ComputeAsync(() => R.FromAsync(expectedIsSuccess, expectedValue, expectedError));

        isSuccess.Should().Be(expectedIsSuccess);
        value.Should().Be(expectedValue);
        error.Should().Be(expectedError);
    }

    [Fact]
    public async Task ErrorAsync_When_PassingResultIntoValueTaskAsyncMethod_Then_ResultShouldBeValueTaskOfResult()
    {
        const double expectedError = 65d;

        RoE<double> r = await ComputeAsync(() => R.ErrorAsync(65d));

        r.IsSuccess.Should().BeFalse();
        r.Error.Should().Be(expectedError);
    }

    [Fact]
    public async Task SuccessAsync_When_PassingResultIntoValueTaskAsyncMethodAndReturningToResult_Then_ResultShouldBeValueTaskOfResult()
    {
        const double expectedValue = 65d;

        R<double, double> r = await ComputeAsync(() => R.SuccessAsync(65d));

        r.IsSuccess.Should().BeTrue();
        r.Value.Should().Be(expectedValue);
        r.Error.Should().Be(default);
    }

    [Fact]
    public async Task ErrorAsync_When_PassingResultIntoValueTaskAsyncMethodAndReturningToResult_Then_ResultShouldBeValueTaskOfResult()
    {
        const double expectedError = 65d;

        R<double, double> r = await ComputeAsync(() => R.ErrorAsync(65d));

        r.IsSuccess.Should().BeFalse();
        r.Value.Should().Be(default);
        r.Error.Should().Be(expectedError);
    }

    private static async ValueTask<TResult> ComputeAsync<TResult>(Func<ValueTask<TResult>> func)
    {
        return await func();
    }
}