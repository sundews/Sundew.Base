﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringBuilderExtensionsTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Text;

using System.Text;
using AwesomeAssertions;
using Sundew.Base.Text;
using Xunit;

public class StringBuilderExtensionsTests
{
    [Fact]
    public void ToString_When_PassingRange_Then_ResultShouldBeExpectedResult()
    {
        const string expectedResult = "1,2,3,4";
        var values = new[] { 1, 2, 3, 4 };
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendItems(values, (builder, i) => builder.Append(i).Append(','));

        var result = stringBuilder.ToString(..^1);

        result.Should().Be(expectedResult);
    }

    [Fact]
    public void ToStringWithoutLast_Then_ResultShouldBeExpectedResult()
    {
        const string expectedResult = "1,2,3,4";
        var values = new[] { 1, 2, 3, 4 };
        var stringBuilder = new StringBuilder().AppendItems(values, (builder, i) => builder.Append(i).Append(','));

        var result = stringBuilder.ToStringWithoutLast(0);

        result.Should().Be(expectedResult);
    }

    [Fact]
    public void ToString_When_UserEndOffset_Then_ResultShouldBeExpectedResult()
    {
        const string expectedResult = "1, 2, 3, 4";
        var values = new[] { 1, 2, 3, 4 };
        const string separator = ", ";
        var stringBuilder = new StringBuilder().AppendItems(values, (builder, i) => builder.Append(i).Append(separator));

        var result = stringBuilder.ToString(0, separator);

        result.Should().Be(expectedResult);
    }

    [Fact]
    public void Remove_When_UserEndOffset_Then_ResultShouldBeExpectedResult()
    {
        const string expectedResult = "1, 2, 3, 4";
        var values = new[] { 1, 2, 3, 4 };
        const string separator = ", ";
        var stringBuilder = new StringBuilder().AppendItems(values, (builder, i) => builder.Append(i).Append(separator));

        var result = stringBuilder.Remove(separator);

        result.ToString().Should().Be(expectedResult);
    }
}