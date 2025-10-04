// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Primitives;

using System;
using AwesomeAssertions;
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
    [InlineData(true, true, true, true, false)]
    [InlineData(true, false, false, false, true)]
    [InlineData(false, false, true, false, false)]
    [InlineData(false, true, true, false, false)]
    public void IsSuccess_Then_ResultShouldHaveExpectedValues(
        bool option,
        bool result,
        bool expectedResult,
        bool expectedSuccessHasValue,
        bool expectedErrorHasValue)
    {
        R<int, int>? testee = option ? R.From(result, 1, 2) : null;

        var actualResult = testee.IsSuccess(out var optionalValue, out var failedResult);

        actualResult.Should().Be(expectedResult);
        optionalValue.HasValue.Should().Be(expectedSuccessHasValue);
        failedResult.HasValue.Should().Be(expectedErrorHasValue);
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
    public void FromValue_Then_ResultShouldBeExpectedResult(bool option, int optionalValue, int? expectedResult)
    {
        var result = O.FromValue(option, () => optionalValue);

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(42)]
    [InlineData(null)]
    public void MapValue_When_NullableValueTupleAndResultIsStruct_Then_ResultShouldBeExpectedResult(int? expectedResult)
    {
        var tupleOption = this.GetValueTupleOption("Any", expectedResult);

        var result = tupleOption.MapValue(x => x.Value);

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(42)]
    [InlineData(null)]
    public void MapValue_When_NullableValueTupleAndResultIsNullableStruct_Then_ResultShouldBeExpectedResult(int? expectedResult)
    {
        var tupleOption = this.GetValueTupleOption("Any", expectedResult);

        var result = tupleOption.MapValue(x => (int?)x.Value);

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("T", 1)]
    [InlineData(null, null)]
    public void MapValue_When_Option_Then_ResultShouldBeExpectedResult(string? input, int? expectedResult)
    {
        var result = input.MapValue(x => x.Length);

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("T", 1)]
    [InlineData(null, null)]
    public void MapValue_When_OptionAndResultIsNullable_Then_ResultShouldBeExpectedResult(string? input, int? expectedResult)
    {
        var result = input.MapValue(x => (int?)x.Length);

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("T")]
    [InlineData(null)]
    public void Map_When_NullableValueTupleAndResultIsClass_Then_ResultShouldBeExpectedResult(string? expectedResult)
    {
        var tupleOption = this.GetValueTupleOption(expectedResult, 42);

        var result = tupleOption.Map(x => x.Name);

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("T")]
    [InlineData(null)]
    public void Map_When_OptionAndResultIsClass_Then_ResultShouldBeExpectedResult(string? expectedResult)
    {
        var result = expectedResult.Map(x => x);

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("T")]
    [InlineData(null)]
    public void Map_When_NullableValueTupleAndResultIsNullableClass_Then_ResultShouldBeExpectedResult(string? expectedResult)
    {
        var tupleOption = this.GetValueTupleOption(expectedResult, 42);

        var result = tupleOption.Map(x => (string?)x.Name);

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("T")]
    [InlineData(null)]
    public void Map_When_OptionAndResultIsNullableClass_Then_ResultShouldBeExpectedResult(string? expectedResult)
    {
        var result = expectedResult.Map(x => x.ToString());

        result.Should().Be(expectedResult);
    }

    private (string Name, int Value)? GetValueTupleOption(string? name, int? value)
    {
        return value.HasValue && name != null ? (name, value.Value) : null;
    }
}