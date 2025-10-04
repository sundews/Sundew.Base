// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImmutableListTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Collections;

using System.Collections.Immutable;
using System.Globalization;
using AwesomeAssertions;
using Sundew.Base.Collections.Linq;
using Xunit;

public class ImmutableListTests
{
    [Fact]
    public void AddAllIfSuccess_Then_ResultShouldBeExpectedResult()
    {
        var expectedResult = new[] { 1, 4 };
        var immutableArray = ImmutableList.Create<int>();

        var allPositiveResult = expectedResult.AllOrFailed(x => Item.From(x > 0, x));

        var result = immutableArray.AddAllIfSuccess(allPositiveResult);

        result.Should().Equal(expectedResult);
    }

    [Fact]
    public void AddFailedIfError_When_InputIsStruct_Then_ResultShouldBeExpectedResult()
    {
        var expectedResult = new[] { -1 };
        var input = new[] { -1, 4 };
        var immutableList = ImmutableList.Create<int>();

        var allPositiveResult = input.AllOrFailed(x => Item.From(x > 0, x));

        var result = immutableList.AddFailedIfError(allPositiveResult);

        result.Should().Equal(expectedResult);
    }

    [Fact]
    public void AddFailedIfError_When_InputIsClass_Then_ResultShouldBeExpectedResult()
    {
        var expectedResult = new[] { "-1" };
        var input = new[] { "-1", "4" };
        var immutableList = ImmutableList.Create<string>();

        var allPositiveResult = input.AllOrFailed(x =>
        {
            if (int.TryParse(x, CultureInfo.InvariantCulture, out var result) && result > 0)
            {
                return Item.Pass(result);
            }

            return Item.Fail(x).Omits<int>();
        });

        var result = immutableList.AddFailedIfError(allPositiveResult);

        result.Should().Equal(expectedResult);
    }

    [Fact]
    public void AddFailedIfAnyError_When_InputIsClass_Then_ResultShouldBeExpectedResult()
    {
        var expectedResult = new[] { "0" };
        var input = new[] { "0", "4" };
        var immutableArray = ImmutableList.Create<string>();

        var allPositiveResult = input.AllOrFailed(x =>
        {
            if (int.TryParse(x, CultureInfo.InvariantCulture, out var result) && result > 0)
            {
                return Item.Pass(result);
            }

            return Item.From(true, result, x);
        });

        var result = immutableArray.AddFailedIfAnyError(allPositiveResult);

        allPositiveResult.IsSuccess.Should().BeTrue();
        result.Should().Equal(expectedResult);
    }
}