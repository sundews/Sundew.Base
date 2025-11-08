// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueListTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Collections;

using System.Collections.Immutable;
using AwesomeAssertions;
using Sundew.Base.Collections.Immutable;
using Xunit;

public class ValueListTests
{
    [Fact]
    public void Equals_When_UsedWithInt_Then_LhsAndRhsShouldBeEqual()
    {
        ValueList<int> lhs = System.Collections.Immutable.ImmutableList.Create(4, 5, 6);
        ValueList<int> rhs = System.Collections.Immutable.ImmutableList.Create(4, 5, 6);

        ((object)lhs).Should().Be(rhs);
    }

    [Fact]
    public void GetHashCode_When_UsedWithInt_Then_LhsAndRhsShouldBeEqual()
    {
        ValueList<int> lhs = System.Collections.Immutable.ImmutableList.Create(4, 5, 6);
        ValueList<int> rhs = System.Collections.Immutable.ImmutableList.Create(4, 5, 6);

        lhs.GetHashCode().Should().Be(rhs.GetHashCode());
    }

    [Fact]
    public void Equals_When_UsedWithString_Then_LhsAndRhsShouldBeEqual()
    {
        ValueList<string> lhs = System.Collections.Immutable.ImmutableList.Create("4", "5", "6");
        ValueList<string> rhs = System.Collections.Immutable.ImmutableList.Create("4", "5", "6");

        ((object)lhs).Should().Be(rhs);
    }

    [Fact]
    public void GetHashCode_When_UsedWithString_Then_LhsAndRhsShouldBeEqual()
    {
        ValueList<string> lhs = System.Collections.Immutable.ImmutableList.Create("4", "5", "6");
        ValueList<string> rhs = System.Collections.Immutable.ImmutableList.Create("4", "5", "6");

        lhs.GetHashCode().Should().Be(rhs.GetHashCode());
    }

    [Fact]
    public void Equals_When_UsedWithAnotherArray_Then_LhsAndRhsShouldBeEqual()
    {
        ValueList<ValueList<string>> lhs = ImmutableArray.Create<ValueList<string>>(["1", "2", "3"], ["4", "5", "6"]);
        ValueList<ValueList<string>> rhs = ImmutableArray.Create<ValueList<string>>(["1", "2", "3"], ["4", "5", "6"]);

        ((object)lhs).Should().Be(rhs);
    }

    [Fact]
    public void GetHashCode_When_UsedWithAnotherArray_Then_LhsAndRhsShouldBeEqual()
    {
        ValueList<ValueList<string>> lhs = ImmutableArray.Create<ValueList<string>>(["1", "2", "3"], ["4", "5", "6"]);
        ValueList<ValueList<string>> rhs = ImmutableArray.Create<ValueList<string>>(["1", "2", "3"], ["4", "5", "6"]);

        lhs.GetHashCode().Should().Be(rhs.GetHashCode());
    }
}