// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OExtensionsCombineTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Primitives;

using System;
using System.Globalization;
using AwesomeAssertions;
using Xunit;

public class OExtensionsCombineTests
{
    [Theory]
    [InlineData(5, 10.5, 15.5)]
    [InlineData(5, null, null)]
    [InlineData(null, 10.5, null)]
    public void Combine_When_OperandsAreNullableStructsAndResultIsNullableStruct_Then_ResultShouldBeExpectedResult(int? lhsOption, double? rhsOption, double? expectedResult)
    {
        var result = lhsOption.Combine(rhsOption, (lhs, rhs) => lhs + rhs);

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(5, 10.5, "510.5")]
    [InlineData(5, null, null)]
    [InlineData(null, 10.5, null)]
    public void Combine_When_OperandsAreNullableStructsAndResultIsNullableClass_Then_ResultShouldBeExpectedResult(int? lhsOption, double? rhsOption, string? expectedResult)
    {
        var result = lhsOption.Combine(rhsOption, (lhs, rhs) => FormattableString.Invariant($"{lhs}{rhs}"));

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("5", "10.5", 15.5)]
    [InlineData("5", null, null)]
    [InlineData(null, "10.5", null)]
    public void Combine_When_OperandsAreNullableClassesAndResultIsNullableStruct_Then_ResultShouldBeExpectedResult(string? lhsOption, string? rhsOption, double? expectedResult)
    {
        var result = lhsOption.Combine(rhsOption, (lhs, rhs) => int.Parse(lhs) + double.Parse(rhs, CultureInfo.InvariantCulture));

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("5", "10.5", "510.5")]
    [InlineData("5", null, null)]
    [InlineData(null, "10.5", null)]
    public void Combine_When_OperandsAreNullableClassesAndResultIsNullableClass_Then_ResultShouldBeExpectedResult(string? lhsOption, string? rhsOption, string? expectedResult)
    {
        var result = lhsOption.Combine(rhsOption, (lhs, rhs) => $"{lhs}{rhs}");

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("5", 10.5, "510.5")]
    [InlineData("5", null, null)]
    [InlineData(null, 10.5, null)]
    public void Combine_When_LhsIsNullableClassAndRhsIsNullableStructAndResultIsNullableClass_Then_ResultShouldBeExpectedResult(string? lhsOption, double? rhsOption, string? expectedResult)
    {
        var result = lhsOption.Combine(rhsOption, (lhs, rhs) => lhs + rhs.ToString(CultureInfo.InvariantCulture));

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("5", 10.5, 15.5)]
    [InlineData("5", null, null)]
    [InlineData(null, 10.5, null)]
    public void Combine_When_LhsIsNullableClassAndRhsIsNullableStructAndResultIsNullableStruct_Then_ResultShouldBeExpectedResult(string? lhsOption, double? rhsOption, double? expectedResult)
    {
        var result = lhsOption.Combine(rhsOption, (lhs, rhs) => int.Parse(lhs) + rhs);

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(10.5, "5", "10.55")]
    [InlineData(null, "5", null)]
    [InlineData(10.5, null, null)]
    public void Combine_When_LhsIsNullableStructAndRhsIsNullableClassAndResultIsNullableClass_Then_ResultShouldBeExpectedResult(double? lhsOption, string? rhsOption, string? expectedResult)
    {
        var result = lhsOption.Combine(rhsOption, (lhs, rhs) => lhs.ToString(CultureInfo.InvariantCulture) + rhs);

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(10.5, "5", 15.5)]
    [InlineData(null, "5", null)]
    [InlineData(10.5, null, null)]
    public void Combine_When_LhsIsNullableStructAndRhsIsNullableClassAndResultIsNullableStruct_Then_ResultShouldBeExpectedResult(double? lhsOption, string? rhsOption, double? expectedResult)
    {
        var result = lhsOption.Combine(rhsOption, (lhs, rhs) => lhs + int.Parse(rhs));

        result.Should().Be(expectedResult);
    }
}