// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LastUpdateEmitterTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Threading;

using System;
using System.Threading.Tasks;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Sundew.Base.Collections.Concurrent;
using Sundew.Base.Threading;

public class LastUpdateEmitterTests
{
    [Test]
    public async Task Update_When_UsingValueType_Then_ShouldContainOnlyProcessedValues()
    {
        const int finalExpectedValue = 5;
        var expectedResult = new ConcurrentList<int>();
        var manualResetEvent = new ManualResetEventAsync();
        var testee = new LastUpdateEmitter<int>(async x =>
        {
            await Task.Delay(50);
            expectedResult.Add(x);
            if (x == finalExpectedValue)
            {
                manualResetEvent.Set();
            }
        });

        _ = testee.Update(1);
        _ = testee.Update(2);
        _ = testee.Update(3);
        _ = testee.Update(4);

        _ = testee.Update(finalExpectedValue);

        var successfullyWaited = await manualResetEvent.WaitAsync();

        using (new AssertionScope())
        {
            expectedResult.Should().Equal([1, finalExpectedValue]);
            successfullyWaited.Should().BeTrue();
        }
    }

    [Test]
    public async Task Update_When_UsingReferenceType_Then_ShouldContainOnlyProcessedValues()
    {
        const string finalExpectedValue = "5";
        var expectedResult = new ConcurrentList<string?>();
        var manualResetEvent = new ManualResetEventAsync();
        var testee = new LastUpdateEmitter<string>(async x =>
        {
            await Task.Delay(50);
            expectedResult.Add(x);
            if (x == finalExpectedValue)
            {
                manualResetEvent.Set();
            }
        });

        _ = testee.Update("1");
        _ = testee.Update("2");
        _ = testee.Update("3");
        _ = testee.Update("4");

        _ = testee.Update(finalExpectedValue);

        var successfullyWaited = await manualResetEvent.WaitAsync();

        using (new AssertionScope())
        {
            expectedResult.Should().Equal(["1", finalExpectedValue]);
            successfullyWaited.Should().BeTrue();
        }
    }

    [Test]
    public async Task Update_When_UsingOptionalReferenceType_Then_ShouldContainOnlyProcessedValues()
    {
        const string? finalExpectedValue = null;
        var expectedResult = new ConcurrentList<string?>();
        var manualResetEvent = new ManualResetEventAsync();
        var testee = new LastUpdateEmitter<string?>(async x =>
        {
            await Task.Delay(50);
            expectedResult.Add(x);
            if (x == finalExpectedValue)
            {
                manualResetEvent.Set();
            }
        });

        _ = testee.Update("1");
        _ = testee.Update("2");
        _ = testee.Update("3");
        _ = testee.Update("4");

        _ = testee.Update(finalExpectedValue);

        await manualResetEvent.WaitAsync();
        using (new AssertionScope())
        {
            expectedResult.Should().Equal("1", finalExpectedValue);
        }
    }
}