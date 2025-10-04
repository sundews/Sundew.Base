// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OAsyncTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Primitives;

using System;
using System.Threading.Tasks;
using AwesomeAssertions;
using Xunit;

public class OAsyncTests
{
    [Fact]
    public async Task ToAsync_Then_ValueShouldBeExpectedValue()
    {
        const int expectedValue = 34;
        int? value = expectedValue;

        var result = await ComputeAsync(() => value.ToAsync());

        result.HasValue.Should().BeTrue();
        result.GetValueOrDefault().Should().Be(expectedValue);
    }

    [Fact]
    public async Task ToResult_When_CastingToResult_Then_ValueShouldBeExpectedValue()
    {
        const int expectedValue = 34;
        const int expectedError = 0;
        int? value = expectedValue;

        var option = await ComputeAsync(() => value.ToAsync());

        var result = option.ToResult(87);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedValue);
        result.Error.Should().Be(expectedError);
    }

    [Fact]
    public async Task ToResult_When_OptionIsNoneOption_Then_ErrorShouldBeExpectedError()
    {
        const string expectedError = "Failed";
        var option = await ComputeAsync(() => default(string).ToAsync());

        var result = option.ToResult(expectedError);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(expectedError);
    }

    [Theory]
    [InlineData(true, "341")]
    [InlineData(false, null)]
    public async Task Map_Then_ResultValueShouldBeExpectedValue(
        bool expectedResult,
        string? expectedValue)
    {
        string? option = expectedResult ? "34" : default;

        var testee = await ComputeAsync(() => option.ToAsync());

        var result = testee.Map(x => x + "1");

        result.HasValue.Should().Be(expectedResult);
        result.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData(true, 34, 0)]
    [InlineData(false, 0, 23)]
    public async Task ToResult_Then_ResultShouldHaveExpectedValues(
        bool expectedResult,
        int expectedValue,
        int expectedError)
    {
        int? option = expectedResult ? expectedValue : null;

        var testee = await ComputeAsync(() => option.ToAsync());

        var result = testee.ToResult(Convert.ToDouble, expectedError);

        result.IsSuccess.Should().Be(expectedResult);
        result.Value.Should().Be(expectedValue);
        result.Error.Should().Be((byte)expectedError);
    }

    private static async ValueTask<TResult> ComputeAsync<TResult>(Func<ValueTask<TResult>> func)
    {
        return await func().ConfigureAwait(false);
    }
}