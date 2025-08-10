// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensionsByCardinalityTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Collections;

using System;
using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions;
using Sundew.Base.Collections.Linq;
using Xunit;

public class EnumerableExtensionsByCardinalityTests
{
    [Fact]
    public void ByCardinality_When_ListIsEmpty_Then_ResultShouldBeEmpty()
    {
        var testee = Array.Empty<int>();

        var result = testee.ByCardinality();

        result.Should().BeOfType<Empty<int>>();
    }

    [Fact]
    public void ByCardinality_When_ListHasOneItem_Then_ResultShouldBeSingle()
    {
        var testee = new[] { 1 };

        var result = testee.ByCardinality();

        result.Should().BeOfType<Single<int>>().Which.Item.Should().Be(1);
    }

    [Fact]
    public void ByCardinality_When_ListHasMultipleItems_Then_ResultShouldBeMultiple()
    {
        var testee = new[] { 1, 2, 3 };

        var result = testee.ByCardinality();

        result.Should().BeOfType<Multiple<int>>().Which.Items.Should().Equal(testee);
    }

    [Fact]
    public void ByCardinality_When_EnumerableYieldsNoItems_Then_ResultShouldBeEmpty()
    {
        var testee = new[] { 1, 2, 3 }.Where(x => x < 1);

        var result = testee.ByCardinality();

        result.Should().BeOfType<Empty<int>>();
    }

    [Fact]
    public void ByCardinality_When_EnumerableHasSingleItem_Then_ResultShouldBeSingle()
    {
        var testee = new[] { 1, 2, 3 }.Where(x => x < 2);

        var result = testee.ByCardinality();

        result.Should().BeOfType<Single<int>>().Which.Item.Should().Be(1);
    }

    [Fact]
    public void ByCardinality_When_EnumerableHasMultipleItems_Then_ResultShouldBeMultiple()
    {
        var testee = new[] { 1, 2, 3 }.Where(x => x < 3);

        var result = testee.ByCardinality();

        result.Should().BeOfType<Multiple<int>>().Which.Items.Should().Equal(new[] { 1, 2 });
    }

    [Fact]
    public void ByCardinality_When_EnumerableIsNull_Then_ResultShouldBeEmpty()
    {
        IEnumerable<int>? testee = null;

        var result = testee.ByCardinality();

        result.Should().BeOfType<Empty<int>>();
    }
}