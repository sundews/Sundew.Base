// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensionsOnlyOneTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Collections;

using System.Linq;
using AwesomeAssertions;
using Sundew.Base.Collections.Linq;

public class EnumerableExtensionsOnlyOneTests
{
    [Test]
    [Arguments(new int[] { 1 }, 1)]
    [Arguments(null, 0)]
    [Arguments(new int[0], 0)]
    [Arguments(new[] { 1, 2 }, 0)]
    public void OnlyOneOrDefault_When_ItemTypeIsStruct_Then_ResultShouldBeExpectedResult(int[]? items, int? expectedResult)
    {
        var result = items.OnlyOneOrDefaultValue();

        result.Should().Be(expectedResult);
    }

    [Test]
    [Arguments(null, null)]
    [Arguments(new[] { "1" }, 1)]
    [Arguments(new[] { "1", "2" }, null)]
    [Arguments(new string?[] { null }, null)]
    [Arguments(new string?[] { null, null }, null)]
    [Arguments(new string?[0], null)]
    public void OnlyOneOrDefault_When_ItemTypeIsNullableStruct_Then_ResultShouldBeExpectedResult(string?[]? items, int? expectedResult)
    {
        var result = (items?.Select<string?, int?>(x => x != null ? int.Parse(x) : null) ?? null).OnlyOneOrDefault();

        result.Should().Be(expectedResult);
    }

    [Test]
    [Arguments(new[] { "1" }, "1")]
    [Arguments(null, null)]
    [Arguments(new string?[] { null }, null)]
    [Arguments(new[] { "1", "2" }, null)]
    public void OnlyOneOrDefault_When_ItemTypeIsClass_Then_ResultShouldBeExpectedResult(string?[]? items, string? expectedResult)
    {
        var result = items.OnlyOneOrDefault();

        result.Should().Be(expectedResult);
    }

    [Test]
    [Arguments(new int[] { 1 }, true, 1)]
    [Arguments(null, false, 0)]
    [Arguments(new int[0], false, 0)]
    [Arguments(new[] { 1, 2 }, false, 0)]
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

    [Test]
    [Arguments(new[] { "1" }, true, 1)]
    [Arguments(null, false, 0)]
    [Arguments(new[] { "1", "2" }, false, 0)]
    [Arguments(new string?[] { null }, false, 0)]
    [Arguments(new string?[] { null, null }, false, 0)]
    [Arguments(new string?[0], false, 0)]
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

    [Test]
    [Arguments(new[] { "1" }, true, "1")]
    [Arguments(null, false, null)]
    [Arguments(new string?[] { null }, false, null)]
    [Arguments(new[] { "1", "2" }, false, null)]
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