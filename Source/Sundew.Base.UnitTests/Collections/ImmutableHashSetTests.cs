// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImmutableHashSetTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Collections;

using System.Collections.Immutable;
using AwesomeAssertions;
using Sundew.Base.Collections.Immutable;
using Xunit;

public class ImmutableHashSetTests
{
    [Fact]
    public void TryAdd_Then_IsNewShouldBeTrue()
    {
        var immutableSet = ImmutableHashSet.Create(4);

        var (set, wasAdded) = immutableSet.TryAdd(5);

        wasAdded.Should().BeTrue();
        set.Should().Equal(new[] { 4, 5 });
    }

    [Fact]
    public void TryAdd_When_ItemExists_Then_IsNewShouldBeFalse()
    {
        var immutableSet = ImmutableHashSet.Create(4);

        var (set, wasAdded) = immutableSet.TryAdd(4);

        wasAdded.Should().BeFalse();
        set.Should().Equal(new[] { 4 });
    }
}