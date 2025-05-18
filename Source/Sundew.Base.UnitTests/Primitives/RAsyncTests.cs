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

        R<int, double> r = await ComputeAsync(() => R.SuccessCompleted(expectedValue));

        r.IsSuccess.Should().BeTrue();
        r.Value.Should().Be(expectedValue);
        r.Error.Should().Be(expectedError);
    }

    [Fact]
    public async Task ImplicitCast_WhenCastingToIfError_Then_ErrorShouldBeExpectedError()
    {
        const string expectedError = "Failed";

        RoE<string> r = await ComputeAsync(() => R.ErrorCompleted(expectedError));

        r.IsSuccess.Should().BeFalse();
        r.Error.Should().Be(expectedError);
    }

    [Fact]
    public async Task ImplicitCast_WhenCastingToResult_Then_ErrorShouldBeExpectedError()
    {
        const int expectedValue = default;
        const string expectedError = "Failed";

        R<int, string> r = await ComputeAsync(() => R.ErrorCompleted(expectedError));

        r.IsSuccess.Should().BeFalse();
        r.Value.Should().Be(expectedValue);
        r.Error.Should().Be(expectedError);
    }

    [Theory]
    [InlineData(true, 0)]
    [InlineData(false, 34)]
    public async Task Map_Then_ResultErrorShouldBeExpectedError(
        bool expectedResult,
        int expectedError)
    {
        var testee = await ComputeAsync(() => R.FromErrorCompleted(expectedResult, expectedError));

        var result = testee.Map(Convert.ToDouble);

        result.IsSuccess.Should().Be(expectedResult);
        result.Error.Should().Be(expectedError);
        result.Error.Should().BeOfType(typeof(double));
    }

    [Theory]
    [InlineData(true, 34, 0)]
    [InlineData(false, 0, 23)]
    public async Task Map_Then_ResultValueShouldBeExpectedValue(
        bool expectedResult,
        int expectedValue,
        int expectedError)
    {
        var testee = await ComputeAsync(() => R.FromErrorCompleted(expectedResult, expectedError));

        var result = testee.Map(expectedValue);

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

        var (isSuccess, value, error) = await ComputeAsync(() => R.FromCompleted(expectedIsSuccess, expectedValue, expectedError));

        isSuccess.Should().Be(expectedIsSuccess);
        value.Should().Be(expectedValue);
        error.Should().Be(expectedError);
    }

    [Fact]
    public async Task ErrorAsync_When_PassingResultIntoValueTaskAsyncMethod_Then_ResultShouldBeValueTaskOfResult()
    {
        const double expectedError = 65d;

        RoE<double> r = await ComputeAsync(() => R.ErrorCompleted(65d));

        r.IsSuccess.Should().BeFalse();
        r.Error.Should().Be(expectedError);
    }

    [Fact]
    public async Task SuccessAsync_When_PassingResultIntoValueTaskAsyncMethodAndReturningToResult_Then_ResultShouldBeValueTaskOfResult()
    {
        const double expectedValue = 65d;

        R<double, double> r = await ComputeAsync(() => R.SuccessCompleted(65d));

        r.IsSuccess.Should().BeTrue();
        r.Value.Should().Be(expectedValue);
        r.Error.Should().Be(default);
    }

    [Fact]
    public async Task ErrorAsync_When_PassingResultIntoValueTaskAsyncMethodAndReturningToResult_Then_ResultShouldBeValueTaskOfResult()
    {
        const double expectedError = 65d;

        R<double, double> r = await ComputeAsync(() => R.ErrorCompleted(65d));

        r.IsSuccess.Should().BeFalse();
        r.Value.Should().Be(default);
        r.Error.Should().Be(expectedError);
    }

    [Theory]
    [InlineData("string")]
    [InlineData(null)]
    public async Task ImplicitCast_When_UsingOptionalReferenceTypeInGenericMethodAndCastingToResult_Then_ResultShouldBeSuccessAndValueShouldBeExpectedResult(string? expectedResult)
    {
        static Task<R<TValue>> GenericAsync<TValue>(TValue value)
        {
            return R.SuccessOption(value).Map();
        }

        var result = await GenericAsync(expectedResult);

        result.IsSuccess.Should().Be(expectedResult.HasValue());
        result.Value.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("string")]
    [InlineData(null)]
    public async Task ImplicitCast_When_UsingOptionalReferenceTypeInGenericMethodAndCastingToResultOption_Then_ResultShouldBeSuccessAndValueShouldBeExpectedResult(string? expectedResult)
    {
        static Task<R<TValue?>> GenericAsync<TValue>(TValue value)
        {
            return R.SuccessOption(value);
        }

        var result = await GenericAsync(expectedResult);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(42)]
    [InlineData(null)]
    public async Task ImplicitCast_When_UsingOptionalValueTypeInGenericMethodAndCastingToResult_Then_ResultShouldBeSuccessAndValueShouldBeExpectedResult(int? expectedResult)
    {
        static Task<R<TValue>> GenericAsync<TValue>(TValue value)
        {
            return R.SuccessOption(value).Map();
        }

        var result = await GenericAsync(expectedResult);

        result.IsSuccess.Should().Be(expectedResult.HasValue);
        result.Value.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(42)]
    [InlineData(null)]
    public async Task ImplicitCast_When_UsingOptionalValueTypeInGenericMethodAndCastingToResultOption_Then_ResultShouldBeSuccessAndValueShouldBeExpectedResult(int? expectedResult)
    {
        static Task<R<TValue?>> GenericAsync<TValue>(TValue value)
        {
            return R.SuccessOption(value);
        }

        var result = await GenericAsync(expectedResult);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedResult);
    }

    [Fact]
    public async Task SuccessOptionMap_When_StringComingFromGenericMethod_Then_ResultShouldBeSuccessAndValueShouldBeExpectedResult()
    {
        static Task<R<TValue>> GenericAsync<TValue>(TValue value)
        {
            return R.SuccessOption(value).Map();
        }

        const string expectedResult = "string";
        var result = await GenericAsync(expectedResult);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedResult);
    }

    [Fact]
    public async Task SuccessOptionMap_When_StringComingFromGenericMethod_Then_ResultShouldBeSuccessAndValueShouldBeExpectedResult2()
    {
        static Task<R<TValue, TError>> GenericAsync<TValue, TError>(TValue value, Func<TError> errorFunc)
        {
            return R.SuccessOption(value).Map(errorFunc);
        }

        const string expectedResult = "string";
        var result = await GenericAsync(expectedResult, () => 3);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedResult);
    }

    private static async ValueTask<TResult> ComputeAsync<TResult>(Func<ValueTask<TResult>> func)
    {
        return await func();
    }
}