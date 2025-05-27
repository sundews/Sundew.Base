// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManualResetEventAsyncTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Threading;

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Sundew.Base.Threading;
using Xunit;

public class ManualResetEventAsyncTests
{
    private readonly ManualResetEventAsync testee;

    public ManualResetEventAsyncTests()
    {
        this.testee = new ManualResetEventAsync();
    }

    [Fact]
    public async Task WaitAsync_When_Set_Then_ResultShouldBeTrue()
    {
        this.testee.Set();

        var result = await this.testee.WaitAsync();

        Assert.Multiple(
            () => result.Should().BeTrue(),
            () => this.testee.IsSet.Should().BeTrue());
    }

    [Fact]
    public async Task Set_When_Waiting_Then_ResultShouldBeTrue()
    {
        var waitTask = Task.Run(async () =>
        {
            await Task.Delay(500);
            return await this.testee.WaitAsync();
        });

        this.testee.Set();

        var result = await waitTask;

        Assert.Multiple(
            () => result.Should().BeTrue(),
            () => this.testee.IsSet.Should().BeTrue());
    }

    [Fact]
    public async Task WaitAsync_When_Cancelled_Then_ResultShouldBeFalse()
    {
        var cancellationTokenSource = new CancellationTokenSource();

        var waitTask = Task.Run(async () => await this.testee.WaitAsync(cancellationTokenSource.Token));
        await cancellationTokenSource.CancelAsync();

        var result = await waitTask;

        Assert.Multiple(
            () => result.Should().BeFalse(),
            () => this.testee.IsSet.Should().BeFalse());
    }

    [Fact]
    public async Task WaitAsync_When_Timedout_Then_ResultShouldBeFalse()
    {
        var waitTask = Task.Run(async () => await this.testee.WaitAsync(TimeSpan.FromMilliseconds(1)));

        var result = await waitTask;

        Assert.Multiple(
            () => result.Should().BeFalse(),
            () => this.testee.IsSet.Should().BeFalse());
    }

    [Fact]
    public async Task WaitAsync_When_ResetAndSet_Then_ResultShouldBeFalse()
    {
        var waitTask = Task.Run(async () => await this.testee.WaitAsync(TimeSpan.FromMilliseconds(1000)));
        await Task.Delay(10);
        this.testee.Reset();
        await Task.Delay(10);
        this.testee.Set();

        var result = await waitTask;

        Assert.Multiple(
            () => result.Should().BeTrue(),
            () => this.testee.IsSet.Should().BeTrue());
    }

    [Fact]
    public async Task WaitAsync_When_SetAndResetBeforeAndSetAfter_Then_ResultShouldBeTrue()
    {
        this.testee.Set();
        this.testee.Reset();
        var waitTask = Task.Run(async () => await this.testee.WaitAsync(TimeSpan.FromMilliseconds(1000)));
        await Task.Delay(500);
        this.testee.Set();

        var result = await waitTask;

        Assert.Multiple(
            () => result.Should().BeTrue(),
            () => this.testee.IsSet.Should().BeTrue());
    }

    [Fact]
    public async Task WaitAsync_When_PreviouslyAwaitedAndSetAndResetBeforeAndSetAfter_Then_ResultShouldBeTrue()
    {
        var waitTask1 = Task.Run(async () => await this.testee.WaitAsync(TimeSpan.FromMilliseconds(1000)));
        this.testee.Set();
        var waitTask1Result = await waitTask1;

        Assert.Multiple(
            () => waitTask1Result.Should().BeTrue(),
            () => this.testee.IsSet.Should().BeTrue());

        this.testee.Reset();
        this.testee.IsSet.Should().BeFalse();

        var waitTask2 = Task.Run(async () => await this.testee.WaitAsync(TimeSpan.FromMilliseconds(1000)));
        await Task.Delay(200);
        this.testee.Set();

        var result = await waitTask2;

        Assert.Multiple(
            () => result.Should().BeTrue(),
            () => this.testee.IsSet.Should().BeTrue());
    }

    [Fact]
    public async Task WaitAsync_When_SetAndResetBeforeAndTimeoutAfter_Then_ResultShouldBeFalse()
    {
        var waitTask1 = Task.Run(async () => await this.testee.WaitAsync(TimeSpan.FromMilliseconds(200)));
        await Task.Delay(50);
        this.testee.Set();
        var waitTask1Result = await waitTask1;

        Assert.Multiple(
            () => waitTask1Result.Should().BeTrue(),
            () => this.testee.IsSet.Should().BeTrue());

        this.testee.Reset();
        this.testee.IsSet.Should().BeFalse();

        var waitTask2 = Task.Run(async () => await this.testee.WaitAsync(TimeSpan.FromMilliseconds(200)));

        var result = await waitTask2;

        Assert.Multiple(
            () => result.Should().BeFalse(),
            () => this.testee.IsSet.Should().BeFalse());
    }

    [Fact]
    public async Task WaitAsync_When_PreviouslyAwaitedAndDelayedSetAndResetBeforeAndSetAfter_Then_ResultShouldBeTrue()
    {
        var waitTask1 = Task.Run(async () => await this.testee.WaitAsync(TimeSpan.FromMilliseconds(200)));
        await Task.Delay(50);
        this.testee.Set();
        var waitTask1Result = await waitTask1;

        Assert.Multiple(
            () => waitTask1Result.Should().BeTrue(),
            () => this.testee.IsSet.Should().BeTrue());

        this.testee.Reset();
        this.testee.IsSet.Should().BeFalse();

        var waitTask2 = Task.Run(async () => await this.testee.WaitAsync(TimeSpan.FromMilliseconds(200)));
        await Task.Delay(50);
        this.testee.Set();

        var result = await waitTask2;

        Assert.Multiple(
            () => result.Should().BeTrue(),
            () => this.testee.IsSet.Should().BeTrue());
    }

    [Fact]
    public async Task WaitAsync_When_SetAndResetAndTimedout_Then_ResultShouldBeFalse()
    {
        this.testee.Set();
        this.testee.Reset();
        var waitTask = Task.Run(async () => await this.testee.WaitAsync(TimeSpan.FromMilliseconds(200)));

        var result = await waitTask;

        Assert.Multiple(
            () => result.Should().BeFalse(),
            () => this.testee.IsSet.Should().BeFalse());
    }

    [Fact]
    public async Task WaitAsync_When_Set_Then_AllWaitersShouldBeNotified()
    {
        var waitTask1 = Task.Run(async () => await this.testee.WaitAsync(TimeSpan.FromMilliseconds(500)));
        var waitTask2 = Task.Run(async () => await this.testee.WaitAsync(TimeSpan.FromMilliseconds(500)));
        await Task.Delay(10);
        this.testee.Set();

        var result = await Tasks.WhenAll(waitTask1, waitTask2);

        Assert.Multiple(
            () => result.Should().Be((true, true)),
            () => this.testee.IsSet.Should().BeTrue());
    }

    [Fact]
    public async Task WaitAsync_When_AwaitTwoTaskAndTimesOut_Then_ResultShouldBeFalse()
    {
        var waitTask1 = Task.Run(async () => await this.testee.WaitAsync(TimeSpan.FromMilliseconds(1)));
        var waitTask2 = Task.Run(async () => await this.testee.WaitAsync(TimeSpan.FromMilliseconds(1)));
        await Task.Delay(10);

        var result = await Tasks.WhenAll(waitTask1, waitTask2);

        Assert.Multiple(
            () => result.Should().Be((false, false)),
            () => this.testee.IsSet.Should().BeFalse());
    }

    [Fact]
    public async Task Reset_When_TimesOut_Then_IsSetShouldBeFalse()
    {
        var waitTask1 = Task.Run(async () => await this.testee.WaitAsync(TimeSpan.FromMilliseconds(100)));
        var waitTask2 = Task.Run(async () => await this.testee.WaitAsync(TimeSpan.FromMilliseconds(100)));
        await Task.Delay(50);

        this.testee.Reset();

        var result = await Tasks.WhenAll(waitTask1, waitTask2);

        Assert.Multiple(
            () => result.Should().Be((false, false)),
            () => this.testee.IsSet.Should().BeFalse());
    }

    [Fact]
    public void Reset_When_Set_Then_IsSetShouldBeFalse()
    {
        this.testee.Set();

        this.testee.Reset();

        this.testee.IsSet.Should().BeFalse();
    }

    [Fact]
    public void Set_Then_IsSetShouldBeTrue()
    {
        this.testee.Set();

        this.testee.IsSet.Should().BeTrue();
    }
}