// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyListExtensionsTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Collections;

using System;
using System.Collections.Generic;
using AwesomeAssertions;
using Sundew.Base.Collections;
using Xunit;

public class ReadOnlyListExtensionsTests
{
    [Fact]
    public void ConvertAll_Then_SourceIsArray_Then_ResultShouldBeExpectedResult()
    {
        var array = new[] { 0, 1, 2, 3, 4 };
        var expectedResult = new long[] { 0, 1, 2, 3, 4 };

        var result = array.ConvertAll(x => (long)x);

        result.Should().Equal(expectedResult);
    }

    [Fact]
    public void ConvertAll_Then_SourceIsList_Then_ResultShouldBeExpectedResult()
    {
        var list = new List<int> { 0, 1, 2, 3, 4 };
        var expectedResult = new long[] { 0, 1, 2, 3, 4 };

        var result = list.ConvertAll(x => (long)x);

        result.Should().Equal(expectedResult);
    }

    [Fact]
    public void HasAny_When_SourceIsListAndHasItems_Then_ResultShouldBeTrue()
    {
        var list = new List<int> { 0, 1, 2, 3, 4 };

        var result = list.HasAny;

        result.Should().BeTrue();
    }

    [Fact]
    public void HasAny_When_SourceIsArrayAndHasItems_Then_ResultShouldBeTrue()
    {
        var list = new int[] { 0, 1, 2, 3, 4 };

        var result = list.HasAny;

        result.Should().BeTrue();
    }

    [Fact]
    public void IsEmpty_When_SourceIsListAndHasItems_Then_ResultShouldBeTrue()
    {
        var list = new List<int>();

        var result = list.IsEmpty;

        result.Should().BeTrue();
    }

    [Fact]
    public void IsEmpty_When_SourceIsArrayAndHasItems_Then_ResultShouldBeTrue()
    {
        var list = Array.Empty<int>();

        var result = list.IsEmpty;

        result.Should().BeTrue();
    }
}