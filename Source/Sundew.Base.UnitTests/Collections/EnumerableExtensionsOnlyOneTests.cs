// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensionsOnlyOneTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Collections;

using System.Linq;
using AwesomeAssertions;
using Sundew.Base.Collections.Linq;
using Xunit;

public class EnumerableExtensionsOnlyOneTests
{
    [Theory]
    [InlineData(new int[] { 1 }, 1)]
    [InlineData(null, 0)]
    [InlineData(new int[0], 0)]
    [InlineData(new[] { 1, 2 }, 0)]
    public void OnlyOneOrDefault_When_ItemTypeIsStruct_Then_ResultShouldBeExpectedResult(int[]? items, int? expectedResult)
    {
        var result = items.OnlyOneOrDefaultValue();

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(new[] { "1" }, 1)]
    [InlineData(new[] { "1", "2" }, null)]
    [InlineData(new string?[] { null }, null)]
    [InlineData(new string?[] { null, null }, null)]
    [InlineData(new string?[0], null)]
    public void OnlyOneOrDefault_When_ItemTypeIsNullableStruct_Then_ResultShouldBeExpectedResult(string?[]? items, int? expectedResult)
    {
        var result = (items?.Select<string?, int?>(x => x != null ? int.Parse(x) : null) ?? null).OnlyOneOrDefault();

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(new[] { "1" }, "1")]
    [InlineData(null, null)]
    [InlineData(new string?[] { null }, null)]
    [InlineData(new[] { "1", "2" }, null)]
    public void OnlyOneOrDefault_When_ItemTypeIsClass_Then_ResultShouldBeExpectedResult(string?[]? items, string? expectedResult)
    {
        var result = items.OnlyOneOrDefault();

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(new int[] { 1 }, true, 1)]
    [InlineData(null, false, 0)]
    [InlineData(new int[0], false, 0)]
    [InlineData(new[] { 1, 2 }, false, 0)]
    public void TryGetOnlyOneOrDefault_When_ItemTypeIsStruct_Then_ResultShouldBeExpectedResult(int[]? items, bool expectedResult, int expectedItem)
    {
        bool result = false;
        if (items.TryGetOnlyOneValue(out var item))
        {
            item.ToString().Should().Be(expectedItem!.ToString());
            result = true;
        }

        result.Should().Be(expectedResult);
        item.Should().Be(expectedItem);
    }

    [Theory]
    [InlineData(new[] { "1" }, true, 1)]
    [InlineData(null, false, 0)]
    [InlineData(new[] { "1", "2" }, false, 0)]
    [InlineData(new string?[] { null }, false, 0)]
    [InlineData(new string?[] { null, null }, false, 0)]
    [InlineData(new string?[0], false, 0)]
    public void TryGetOnlyOneOrDefault_When_ItemTypeIsNullableStruct_Then_ResultShouldBeExpectedResult(string?[]? items, bool expectedResult, int? expectedItem)
    {
        var result = (items?.Select<string?, int?>(x => x != null ? int.Parse(x) : null) ?? null).TryGetOnlyOne(out var item);

        result.Should().Be(expectedResult);
        item.Should().Be(expectedItem);
        if (result)
        {
            item.ToString().Should().Be(expectedItem.ToString());
        }
    }

    [Theory]
    [InlineData(new[] { "1" }, true, "1")]
    [InlineData(null, false, null)]
    [InlineData(new string?[] { null }, false, null)]
    [InlineData(new[] { "1", "2" }, false, null)]
    public void TryGetOnlyOneOrDefault_When_ItemTypeIsClass_Then_ResultShouldBeExpectedResult(string?[]? items, bool expectedResult, string? expectedItem)
    {
        bool result = false;
        if (items.TryGetOnlyOne(out var item))
        {
            item.ToString().Should().Be(expectedItem!.ToString());
            result = true;
        }

        result.Should().Be(expectedResult);
        item.Should().Be(expectedItem);
    }
}