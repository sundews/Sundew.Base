// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueArrayTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Collections;

using System.Collections.Immutable;
using AwesomeAssertions;
using Sundew.Base.Collections.Immutable;
using Xunit;

public class ValueArrayTests
{
    [Fact]
    public void Equals_When_IsDefault_Then_LhsAndRhsShouldBeEqual()
    {
        ValueArray<int> lhs = default;
        ValueArray<int> rhs = default;

        ((object)lhs).Should().Be(rhs);
    }

    [Fact]
    public void Equals_When_UsedWithInt_Then_LhsAndRhsShouldBeEqual()
    {
        ValueArray<int> lhs = System.Collections.Immutable.ImmutableArray.Create(4, 5, 6);
        ValueArray<int> rhs = System.Collections.Immutable.ImmutableArray.Create(4, 5, 6);

        ((object)lhs).Should().Be(rhs);
    }

    [Fact]
    public void GetHashCode_When_UsedWithInt_Then_LhsAndRhsShouldBeEqual()
    {
        ValueArray<int> lhs = System.Collections.Immutable.ImmutableArray.Create(4, 5, 6);
        ValueArray<int> rhs = System.Collections.Immutable.ImmutableArray.Create(4, 5, 6);

        lhs.GetHashCode().Should().Be(rhs.GetHashCode());
    }

    [Fact]
    public void Equals_When_UsedWithString_Then_LhsAndRhsShouldBeEqual()
    {
        ValueArray<string> lhs = System.Collections.Immutable.ImmutableArray.Create("4", "5", "6");
        ValueArray<string> rhs = System.Collections.Immutable.ImmutableArray.Create("4", "5", "6");

        ((object)lhs).Should().Be(rhs);
    }

    [Fact]
    public void GetHashCode_When_UsedWithString_Then_LhsAndRhsShouldBeEqual()
    {
        ValueArray<string> lhs = System.Collections.Immutable.ImmutableArray.Create("4", "5", "6");
        ValueArray<string> rhs = System.Collections.Immutable.ImmutableArray.Create("4", "5", "6");

        lhs.GetHashCode().Should().Be(rhs.GetHashCode());
    }

    [Fact]
    public void Equals_When_UsedWithAnotherArray_Then_LhsAndRhsShouldBeEqual()
    {
        ValueArray<ValueArray<string>> lhs = ImmutableArray.Create<ValueArray<string>>(["1", "2", "3"], ["4", "5", "6"]);
        ValueArray<ValueArray<string>> rhs = ImmutableArray.Create<ValueArray<string>>(["1", "2", "3"], ["4", "5", "6"]);

        ((object)lhs).Should().Be(rhs);
    }

    [Fact]
    public void GetHashCode_When_UsedWithAnotherArray_Then_LhsAndRhsShouldBeEqual()
    {
        ValueArray<ValueArray<string>> lhs = ImmutableArray.Create<ValueArray<string>>(["1", "2", "3"], ["4", "5", "6"]);
        ValueArray<ValueArray<string>> rhs = ImmutableArray.Create<ValueArray<string>>(["1", "2", "3"], ["4", "5", "6"]);

        lhs.GetHashCode().Should().Be(rhs.GetHashCode());
    }
}