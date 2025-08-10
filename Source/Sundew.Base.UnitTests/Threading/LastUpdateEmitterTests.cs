// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LastUpdateEmitterTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Threading;

using System;
using System.Threading.Tasks;
using AwesomeAssertions;
using Sundew.Base.Collections.Concurrent;
using Sundew.Base.Threading;
using Xunit;

public class LastUpdateEmitterTests
{
    [Fact]
    public async Task Update_When_UsingValueType_Then_ShouldContainOnlyProcessedValues()
    {
        const int finalExpectedValue = 5;
        var expectedResult = new ConcurrentList<int>();
        var manualResetEvent = new ManualResetEventAsync(false);
        var testee = new LastUpdateEmitter<int>(async x =>
        {
            await Task.Delay(50);
            expectedResult.Add(x);
            if (x == finalExpectedValue)
            {
                manualResetEvent.Set();
            }
        });

        testee.Update(1);
        testee.Update(2);
        testee.Update(3);
        testee.Update(4);

        testee.Update(finalExpectedValue);

        var successfullyWaited = await manualResetEvent.WaitAsync(TimeSpan.FromMilliseconds(700));

        Assert.Multiple(
            () => expectedResult.Should().Equal([1, finalExpectedValue]),
            () => successfullyWaited.Should().BeTrue());
    }

    [Fact]
    public async Task Update_When_UsingReferenceType_Then_ShouldContainOnlyProcessedValues()
    {
        const string finalExpectedValue = "5";
        var expectedResult = new ConcurrentList<string?>();
        var manualResetEvent = new ManualResetEventAsync(false);
        var testee = new LastUpdateEmitter<string>(async x =>
        {
            await Task.Delay(50);
            expectedResult.Add(x);
            if (x == finalExpectedValue)
            {
                manualResetEvent.Set();
            }
        });

        testee.Update("1");
        testee.Update("2");
        testee.Update("3");
        testee.Update("4");

        testee.Update(finalExpectedValue);

        var successfullyWaited = await manualResetEvent.WaitAsync(TimeSpan.FromMilliseconds(700));

        Assert.Multiple(
            () => expectedResult.Should().Equal(["1", finalExpectedValue]),
            () => successfullyWaited.Should().BeTrue());
    }

    [Fact]
    public async Task Update_When_UsingOptionalReferenceType_Then_ShouldContainOnlyProcessedValues()
    {
        const string? finalExpectedValue = null;
        var expectedResult = new ConcurrentList<string?>();
        var manualResetEvent = new ManualResetEventAsync(false);
        var testee = new LastUpdateEmitter<string?>(async x =>
        {
            await Task.Delay(50);
            expectedResult.Add(x);
            if (x == finalExpectedValue)
            {
                manualResetEvent.Set();
            }
        });

        testee.Update("1");
        testee.Update("2");
        testee.Update("3");
        testee.Update("4");

        testee.Update(finalExpectedValue);

        var successfullyWaited = await manualResetEvent.WaitAsync(TimeSpan.FromMilliseconds(700));
        Assert.Multiple(
            () => expectedResult.Should().Equal(["1", finalExpectedValue]),
            () => successfullyWaited.Should().BeTrue());
    }
}