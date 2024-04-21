// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionExtensionsTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Collections;

using System.Collections.Generic;
using FluentAssertions;
using Sundew.Base.Collections;
using Xunit;

public class CollectionExtensionsTests
{
    [Fact]
    public void AddAllThatHasValue_Then_ResultShouldBeEqualToExpectedResult()
    {
        var resultList = new List<int>();
        var values = new int?[] { null, 0, 1, 2, null, 4, null };
        var expectedResult = new[] { 0, 1, 2, 4 };

        var result = resultList.AddAllThatHasValue(values);

        result.Should().BeTrue();
        resultList.Should().Equal(expectedResult);
    }

    [Fact]
    public void AddAllThatHasValue_WhenInputIsReferenceType_Then_ResultShouldBeEqualToExpectedResult()
    {
        var resultList = new List<string>();
        var values = new string?[] { null, "0", "1", "2", null, "4", null };
        var expectedResult = new[] { "0", "1", "2", "4" };

        var result = resultList.AddAllThatHasValue(values);

        result.Should().BeTrue();
        resultList.Should().Equal(expectedResult);
    }
}