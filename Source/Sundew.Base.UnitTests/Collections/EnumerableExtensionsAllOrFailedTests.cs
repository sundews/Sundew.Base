// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensionsAllOrFailedTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Collections;

using System.Linq;
using FluentAssertions;
using Sundew.Base.Collections;
using Xunit;

public class EnumerableExtensionsAllOrFailedTests
{
    [Fact]
    public void AllOrFailedOfInt32_When_SelectorSucceeds_Then_ResultShouldBeAllItemWithExpectedItems()
    {
        var expectedItems = new int?[] { 1, 2, 3, 4 };
        var result = expectedItems.AllOrFailed(x =>
        {
            if (x.HasValue)
            {
                return Item.Pass(x.Value);
            }

            return Item.Fail();
        });

        result.Should().BeOfType<All<int?, int>>().Which
            .Items.Should().Equal(expectedItems.Cast<int>());
    }

    [Fact]
    public void AllOrFailedOfInt32_When_SelectorHasOneError_Then_ResultShouldBeErrorResult()
    {
        var result = new int?[] { 1, 2, null, 4, null }.AllOrFailed(x =>
        {
            if (x.HasValue)
            {
                return Item.Pass(x.Value);
            }

            return Item.Fail();
        });

        result.Should().BeOfType<Failed<int?, int>>().Which
            .Items.Should().Equal(new[] { new FailedItem<int?>(2, null), new FailedItem<int?>(4, null) });
    }

    [Fact]
    public void AllOrFailedIfNotNullInt32_When_SelectorSucceeds_Then_ResultShouldBeAllItemWithExpectedItems()
    {
        var expectedItems = new int?[] { 1, 2, 3, 4 };

        var result = expectedItems.AllOrFailed(Item.PassIfNotNull);

        result.Should().BeOfType<All<int?, int>>().Which
            .Items.Should().Equal(expectedItems.Cast<int>());
    }

    [Fact]
    public void AllOrFailedInt32_When_SelectorHasOneError_Then_ResultShouldBeNone()
    {
        var result = new int?[] { 1, 2, null, 4, null }.AllOrFailed();

        result.Should().BeOfType<Failed<int?, int>>().Which
            .Items.Should().Equal(new[] { new FailedItem<int?>(2, null), new FailedItem<int?>(4, null) });
    }

    [Fact]
    public void AllOrFailedOfString_When_SelectorSucceeds_Then_ResultShouldBeAllItemWithExpectedItems()
    {
        var expectedItems = new string?[] { "1", "2", "3", "4" };
        var result = expectedItems.AllOrFailed(x =>
        {
            if (x != null)
            {
                return Item.Pass(x);
            }

            return Item.Fail();
        });

        result.Should().BeOfType<All<string?, string>>().Which
            .Items.Should().Equal(expectedItems.Cast<string>());
    }

    [Fact]
    public void AllOrFailedOfString_When_SelectorHasOneError_Then_ResultShouldBeErrorResult2()
    {
        var result = new string?[] { "1", "2", null, "4", null }.AllOrFailed(x =>
        {
            if (x != null)
            {
                return Item.Pass(x);
            }

            return Item.Fail();
        });

        result.Should().BeOfType<Failed<string?, string>>().Which
            .Items.Should().Equal(new[] { new FailedItem<string?>(2, null), new FailedItem<string?>(4, null) });
    }

    [Fact]
    public void AllOrFailedOfString_When_SelectorHasOneError_Then_ResultShouldBeErrorResult()
    {
        var result = new string?[] { "1", "2", null, "4", null }.AllOrFailed(x =>
        {
            if (x != null)
            {
                return Item.Pass(x);
            }

            return Item.Fail();
        });

        result.Should().BeOfType<Failed<string?, string>>().Which
            .Items.Should().Equal(new[] { new FailedItem<string?>(2, null), new FailedItem<string?>(4, null) });
    }

    [Fact]
    public void AllOrFailedIfNotNullString_When_SelectorSucceeds_Then_ResultShouldBeAllItemWithExpectedItems()
    {
        var expectedItems = new string?[] { "1", "2", "3", "4" };

        var result = expectedItems.AllOrFailed(Item.PassIfNotNull);

        result.Should().BeOfType<All<string?, string>>().Which
            .Items.Should().Equal(expectedItems.Cast<string>());
    }

    [Fact]
    public void AllOrFailed_When_SelectorHasOneError_Then_ResultShouldBeNothing()
    {
        var result = new string?[] { "1", "2", null, "4", null }.AllOrFailed();

        result.Should().BeOfType<Failed<string?, string>>().Which
            .Items.Should().Equal(new[] { new FailedItem<string?>(2, null), new FailedItem<string?>(4, null) });
    }
}