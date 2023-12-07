﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImmutableListTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Collections;

using System.Collections.Immutable;
using System.Globalization;
using FluentAssertions;
using Sundew.Base.Collections;
using Sundew.Base.Collections.Immutable;
using Xunit;

public class ImmutableListTests
{
    [Fact]
    public void TryAddSuccesses_Then_ResultShouldBeExpectedResult()
    {
        var expectedResult = new[] { 1, 4 };
        var immutableArray = ImmutableList.Create<int>();

        var allPositiveResult = expectedResult.AllOrFailed(x => Item.From(x > 0, x));

        var result = immutableArray.TryAddSuccesses(allPositiveResult);

        result.Should().Equal(expectedResult);
    }

    [Fact]
    public void TryAddError_When_InputIsStruct_Then_ResultShouldBeExpectedResult()
    {
        var expectedResult = new[] { -1 };
        var input = new[] { -1, 4 };
        var immutableList = ImmutableList.Create<int>();

        var allPositiveResult = input.AllOrFailed(x => Item.From(x > 0, x));

        var result = immutableList.TryAddErrors(allPositiveResult);

        result.Should().Equal(expectedResult);
    }

    [Fact]
    public void TryAddError_When_InputIsClass_Then_ResultShouldBeExpectedResult()
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

            return Item.Fail(x)._<int>();
        });

        var result = immutableList.TryAddErrors(allPositiveResult);

        result.Should().Equal(expectedResult);
    }

    [Fact]
    public void TryAddAnyError_When_InputIsClass_Then_ResultShouldBeExpectedResult()
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

        var result = immutableArray.TryAddAnyErrors(allPositiveResult);

        allPositiveResult.IsSuccess.Should().BeTrue();
        result.Should().Equal(expectedResult);
    }
}