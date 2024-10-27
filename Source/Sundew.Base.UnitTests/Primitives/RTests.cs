// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Primitives;

using System;
using FluentAssertions;
using Xunit;

public class RTests
{
    [Fact]
    public void ImplicitCast_When_CastingToResult_Then_ValueShouldBeExpectedValue()
    {
        const int expectedValue = 34;
        const int expectedError = 0;

        R<int, double> r = R.Success(expectedValue);

        r.IsSuccess.Should().BeTrue();
        r.Value.Should().Be(expectedValue);
        r.Error.Should().Be(expectedError);
    }

    [Fact]
    public void ImplicitCast_WhenCastingToIfError_Then_ErrorShouldBeExpectedError()
    {
        const string expectedError = "Failed";

        RoE<string> r = R.Error(expectedError);

        r.IsSuccess.Should().BeFalse();
        r.Error.Should().Be(expectedError);
    }

    [Fact]
    public void ImplicitCast_WhenCastingToResult_Then_ErrorShouldBeExpectedError()
    {
        const int expectedValue = 0;
        const string expectedError = "Failed";

        R<int, string> r = R.Error(expectedError);

        r.IsSuccess.Should().BeFalse();
        r.Value.Should().Be(expectedValue);
        r.Error.Should().Be(expectedError);
    }

    [Theory]
    [InlineData(true, 0)]
    [InlineData(false, 34)]
    public void Map_Then_ResultErrorShouldBeExpectedError(
        bool expectedResult,
        int expectedError)
    {
        var testee = R.FromError(expectedResult, expectedError);

        var result = testee.Map(Convert.ToDouble);

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
        var testee = R.FromError(expectedResult, () => expectedError);

        var result = testee.Map(expectedValue);

        result.IsSuccess.Should().Be(expectedResult);
        result.Value.Should().Be(expectedValue);
        result.Error.Should().Be(expectedError);
    }

    [Fact]
    public void Deconstruction_When_DeconstructingAllParameters_Then_DeconstructedValuesShouldBedExpectedResult()
    {
        const bool expectedIsSuccess = true;
        const double expectedValue = 65d;
        const int expectedError = 45;

        var (isSuccess, value, error) = R.From(expectedIsSuccess, 65d, expectedError);

        isSuccess.Should().Be(expectedIsSuccess);
        value.Should().Be(expectedValue);
        error.Should().Be(expectedError);
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

    [Fact]
    public void ToOption_When_SuccessResultAndTargetIsReferenceType_Then_ResultShouldBeSuccessAndValueShouldBeExpectedResult()
    {
        const string expectedResult = "Value";
        var testee = R.Success(expectedResult);

        R<string, string> r = testee;

        var result = r.ToOption();

        result.HasValue.Should().BeTrue();
        result.GetValueOrDefault().Value.Should().Be(expectedResult);
    }

    [Fact]
    public void ToResultOption_When_SuccessResultOptionAndTargetIsOptionalClass_Then_ResultShouldBeSuccessAndValueShouldBeExpectedResult()
    {
        var testee = R.SuccessOption(default(string));

        R<string?, string> r = testee;

        var result = r.ToResultOption();

        result.HasValue.Should().BeFalse();
        result.GetValueOrDefault().Value.Should().BeNull();
    }

    [Fact]
    public void ToResultOption_When_NullAndTargetIsOptionalReferenceType_Then_ResultShouldBeSuccessAndValueShouldBeExpectedResult()
    {
        var expectedResult = "Value";
        var testee = R.SuccessOption(expectedResult);

        R<string?, string> r = testee;

        var result = r.ToResultOption();

        result.HasValue.Should().BeTrue();
        result.GetValueOrDefault().Value.Should().Be(expectedResult);
    }

    [Fact]
    public void ToOptionResult_When_NullAndTargetIsReferenceType_Then_ResultShouldBeSuccessAndValueShouldBeExpectedResult()
    {
        var expectedResult = "Value";
        var testee = R.Success(expectedResult);

        R<string, string> r = testee;

        var result = r.ToOptionResult();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedResult);
    }

    [Fact]
    public void ToValueOptionResult_When_NullAndTargetIsValueType_Then_ResultShouldBeSuccessAndValueShouldBeExpectedResult()
    {
        var expectedResult = 4;
        var testee = R.Success(expectedResult);

        R<int, string> r = testee;

        var result = r.ToValueOptionResult();

        result.IsSuccess.Should().BeTrue();
        result.Value.HasValue.Should().BeTrue();
        result.Value.Should().Be(expectedResult);
    }

    [Fact]
    public void Evaluate_When_Success_Then_ResultShouldBeExpectedResult()
    {
        var expectedResult = "value";
        var testee = R.Success(expectedResult);

        R<string> r = testee;

        var result = r.Evaluate(default(string));

        result.Should().Be(expectedResult);
    }
}