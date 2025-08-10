// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlagTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Threading;

using AwesomeAssertions;
using Xunit;

public class FlagTests
{
    private readonly Flag testee = new();

    [Fact]
    public void IsSet_When_Set_Then_ResultShouldBeTrue()
    {
        this.testee.Set();

        this.testee.IsSet.Should().BeTrue();
    }

    [Fact]
    public void IsSet_Then_ResultShouldBeFalse()
    {
        this.testee.IsSet.Should().BeFalse();
    }

    [Fact]
    public void Clear_When_Set_Then_ResultShouldBeTrue()
    {
        this.testee.Set();

        var result = this.testee.Clear();

        result.Should().BeTrue();
        this.testee.IsSet.Should().BeFalse();
    }

    [Fact]
    public void Clear_Then_ResultShouldBeFalse()
    {
        var result = this.testee.Clear();

        result.Should().BeFalse();
        this.testee.IsSet.Should().BeFalse();
    }
}