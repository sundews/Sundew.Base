// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensionsAllOrFailedTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Collections;

using System.Linq;
using FluentAssertions;
using Sundew.Base.Collections.Linq;
using Xunit;

public class EnumerableExtensionsAllOrFailedTests
{
    [Fact]
    public void AllOrFailed_When_Int32AndSelectorSucceeds_Then_ResultShouldBeAllItemWithExpectedItems()
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

        result.Value.Items.Should().Equal(expectedItems.Cast<int>());
    }

    [Fact]
    public void AllOrFailed_When_Int32AndSelectorHasOneError_Then_ResultShouldBeErrorResult()
    {
        var result = new int?[] { 1, 2, null, 4, null }.AllOrFailed(x =>
        {
            if (x.HasValue)
            {
                return Item.Pass(x.Value);
            }

            return Item.Fail();
        });

        result.Error.Should().BeOfType<Failed<int?>>().Which
            .Items.Should().Equal(new[] { new FailedItem<int?>(2, null), new FailedItem<int?>(4, null) });
    }

    [Fact]
    public void AllOrFailed_When_NonNullInt32AndSelectorSucceeds_Then_ResultShouldBeAllItemWithExpectedItems()
    {
        var expectedItems = new int?[] { 1, 2, 3, 4 };

        var result = expectedItems.AllOrFailed(Item.PassIfHasValue);

        result.Value.Should().BeOfType<All<int>>().Which
            .Items.Should().Equal(expectedItems.Cast<int>());
    }

    [Fact]
    public void AllOrFailed_When_Int32AndSelectorHasOneError_Then_ResultShouldBeNone()
    {
        var result = new int?[] { 1, 2, null, 4, null }.AllOrFailed();

        result.Error.Should().BeOfType<Failed<int?>>().Which
            .Items.Should().Equal(new[] { new FailedItem<int?>(2, null), new FailedItem<int?>(4, null) });
    }

    [Fact]
    public void AllOrFailed_When_StringAndSelectorSucceeds_Then_ResultShouldBeAllItemWithExpectedItems()
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

        result.Value.Should().BeOfType<All<string>>().Which
            .Items.Should().Equal(expectedItems.Cast<string>());
    }

    [Fact]
    public void AllOrFailed_When_StringAndSelectorHasTwoErrors_Then_ResultShouldBeErrorResult()
    {
        var result = new string?[] { "1", "2", null, "4", null }.AllOrFailed(x =>
        {
            if (x != null)
            {
                return Item.Pass(x);
            }

            return Item.Fail();
        });

        result.Error.Items.Should().Equal(new[] { new FailedItem<string?>(2, null), new FailedItem<string?>(4, null) });
    }

    [Fact]
    public void AllOrFailed_When_NonNullStringAndSelectorSucceeds_Then_ResultShouldBeAllItemWithExpectedItems()
    {
        var expectedItems = new string?[] { "1", "2", "3", "4" };

        var result = expectedItems.AllOrFailed(Item.PassIfHasValue);

        result.Value.Items.Should().Equal(expectedItems.Cast<string>());
    }

    [Fact]
    public void AllOrFailed_When_SelectorHasOneError_Then_ResultShouldBeNothing()
    {
        var result = new string?[] { "1", "2", null, "4", null }.AllOrFailed();

        result.Error.Items.Should().Equal(new[] { new FailedItem<string?>(2, null), new FailedItem<string?>(4, null) });
    }

    [Fact]
    public void AllOrFailed_When_NonNullStringSelectorSucceeds_Then_ResultShouldBeAllItemWithExpectedItems()
    {
        var expectedItems = new string?[] { "1", "2", "3", "4" };

        var result = expectedItems.AllOrFailed(x =>
        {
            if (x != null)
            {
                return Item.Pass<double, string>(double.Parse(x));
            }

            return Item.Fail("Failed");
        });

        result.Value.Items.Should().Equal(expectedItems.Select(x => double.Parse(x!)));
    }

    [Fact]
    public void AllOrFailed_When_NonNullStringSelectorReturnsError_Then_ResultShouldBeFailedWithFailedItems()
    {
        var expectedItems = new string?[] { "1", "2", null, "4", null };

        var result = expectedItems.AllOrFailed(x =>
        {
            if (x != null)
            {
                return Item.Pass<double, string>(double.Parse(x));
            }

            return Item.Fail("Failed");
        });

        result.Error.Items.Should().Equal(new[] { new FailedItem<string?, string>(2, null, "Failed"), new FailedItem<string?, string>(4, null, "Failed") });
    }
}