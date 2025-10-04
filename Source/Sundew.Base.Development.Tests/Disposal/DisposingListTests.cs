// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposingListTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Disposal;

using System.Collections.Generic;
using AwesomeAssertions;
using Sundew.Base.Disposal;
using Xunit;

public class DisposingListTests
{
    [Fact]
    public void Dispose_Then_ItemsShouldBeDisposedInExpectedOrder()
    {
        var expectedOrder = new[] { 1, 2 };
        var testee = new DisposingList();
        var disposeOrder = new List<int>();
        testee.Add(new DisposeAction(() => disposeOrder.Add(1)));
        testee.Add(new DisposeAction(() => disposeOrder.Add(2)));

        testee.Dispose();

        testee.GetDisposers().Should().BeEmpty();
        disposeOrder.Should().Equal(expectedOrder);
    }

    [Fact]
    public void Dispose_When_AddedThroughAddRange_Then_ItemsShouldBeDisposedInExpectedOrder()
    {
        var expectedOrder = new[] { 1, 2 };
        var testee = new DisposingList<DisposeAction>();
        var disposeOrder = new List<int>();
        testee.AddRange([new DisposeAction(() => disposeOrder.Add(1)), new DisposeAction(() => disposeOrder.Add(2))]);

        testee.Dispose();

        testee.GetDisposers().Should().BeEmpty();
        disposeOrder.Should().Equal(expectedOrder);
    }
}