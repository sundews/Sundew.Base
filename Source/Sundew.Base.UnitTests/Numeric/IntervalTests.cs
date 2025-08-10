// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntervalTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Numeric;

using AwesomeAssertions;
using Sundew.Base.Numeric;
using Xunit;

public class IntervalTests
{
    [Theory]
    [InlineData(2, 6, 3, 5, true)]
    [InlineData(2, 4, 3, 4, true)]
    [InlineData(2, 4, 1, 3, true)]
    [InlineData(2, 2, 1, 4, true)]
    [InlineData(1, 2, 4, 2, false)]
    [InlineData(4, 2, 1, 2, false)]
    [InlineData(1, 2, 3, 2, false)]
    [InlineData(3, 2, 1, 2, false)]
    [InlineData(7, 5, 1, 6, false)]
    public void Overlaps_Then_ResultShouldBeExpectedResult(int min1, int length1, int min2, int length2, bool expectedResult)
    {
        var interval1 = Interval.FromMinAndLength(min1, length1);
        var interval2 = Interval.FromMinAndLength(min2, length2);

        var result = interval1.Overlaps(interval2);

        result.Should().Be(expectedResult);
    }
}