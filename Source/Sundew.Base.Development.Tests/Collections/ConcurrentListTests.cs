// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConcurrentListTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Collections;

using AwesomeAssertions;
using Sundew.Base.Collections.Concurrent;
using Xunit;

public class ConcurrentListTests
{
    [Fact]
    public void Add_Then_ListShouldBeExpectedList()
    {
        var testee = new ConcurrentList<int>();

        testee.Add(1);

        testee.Should().Equal([1]);
    }

    [Fact]
    public void Insert_When_IndexIsHigherThanCount_Then_ListShouldGrowAndItemBeAdded()
    {
        var testee = new ConcurrentList<int>();
        testee.Add(1);

        testee.Insert(2, 2);

        testee.Should().Equal([1, 0, 2]);
    }

    [Fact]
    public void Insert_Then_ListShouldGrowAndItemBeAdded()
    {
        var testee = new ConcurrentList<int>();
        testee.Add(1);
        testee.Add(3);
        testee.Add(4);

        testee.Insert(1, 2);

        testee.Should().Equal([1, 2, 3, 4]);
    }

    [Fact]
    public void Indexer_When_IndexIsHigherThanCount_Then_ListShouldGrowAndItemBeAdded()
    {
        var testee = new ConcurrentList<int>();
        testee.Add(1);

        testee[2] = 2;

        testee.Should().Equal([1, 0, 2]);
    }
}