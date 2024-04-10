// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrayExtensionsTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Collections;

using FluentAssertions;
using Sundew.Base.Collections;
using Xunit;

public class ArrayExtensionsTests
{
    [Fact]
    public void GetSegment_Then_ResultShouldBeEqualToExpectedResult()
    {
        var values = new[] { 0, 1, 2, 3, 4, 5 };
        var expectedResult = new[] { 3, 4, 5 };

        var result = values.GetSegment(3);

        result.Should().Equal(expectedResult);
    }
}