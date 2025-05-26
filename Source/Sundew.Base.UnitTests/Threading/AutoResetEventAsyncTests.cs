// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoResetEventAsyncTests.cs" company="Sundews">
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

public class AutoResetEventAsyncTests
{
    private readonly AutoResetEventAsync testee;

    public AutoResetEventAsyncTests()
    {
        this.testee = new AutoResetEventAsync();
    }

    [Fact]
    public async Task WaitAsync_When_Set_Then_ResultShouldBeTrue()
    {
        this.testee.Set();

        var result = await this.testee.WaitAsync();

        result.Should().BeTrue();
        this.testee.IsSet.Should().BeFalse();
    }

    [Fact]
    public async Task Set_When_Waiting_Then_ResultShouldBeTrue()
    {
        var waitTask = Task.Run(async () =>
        {
            await Task.Delay(5);
            return await this.testee.WaitAsync();
        });

        this.testee.Set();

        var result = await waitTask;

        Assert.Multiple(
            () => result.Should().BeTrue(),
            () => this.testee.IsSet.Should().BeFalse());
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
        await Task.Delay(10);

        var result = await waitTask;

        Assert.Multiple(
            () => result.Should().BeFalse(),
            () => this.testee.IsSet.Should().BeFalse());
    }

    [Fact]
    public async Task WaitAsync_When_SetWithInterval_Then_WaitersShouldBeNotifiedOneAtATime()
    {
        var waitTask1 = Task.Run(async () => await this.testee.WaitAsync(TimeSpan.FromMilliseconds(100)));
        var waitTask2 = Task.Run(async () =>
        {
            await Task.Delay(50);
            return await this.testee.WaitAsync(TimeSpan.FromMilliseconds(100));
        });
        await Task.Delay(50);
        this.testee.Set();

        var task1Result = await waitTask1;

        Assert.Multiple(
            () => task1Result.Should().BeTrue(),
            () => this.testee.IsSet.Should().BeFalse(),
            () => waitTask2.IsCompleted.Should().BeFalse());

        await Task.Delay(10);
        this.testee.Set();

        var task2Result = await waitTask2;

        Assert.Multiple(
            () => task2Result.Should().BeTrue(),
            () => this.testee.IsSet.Should().BeFalse());
    }

    [Fact]
    public async Task WaitAsync_When_SetWithInterval_Then_WaitersShouldBeNotifiedOneAtATimeInAnyOrder()
    {
        var waitTask1 = Task.Run(async () => await this.testee.WaitAsync(TimeSpan.FromMilliseconds(100)));
        var waitTask2 = Task.Run(async () =>
        {
            await Task.Delay(50);
            return await this.testee.WaitAsync(TimeSpan.FromMilliseconds(100));
        });
        await Task.Delay(50);
        this.testee.Set();

        var task = await Task.WhenAny(waitTask1, waitTask2);

        var firstTask = task == waitTask1 ? waitTask1 : waitTask2;
        var otherTask = task == waitTask1 ? waitTask2 : waitTask1;

        var resultFirstTask = await firstTask;

        Assert.Multiple(
            () => resultFirstTask.Should().BeTrue(),
            () => this.testee.IsSet.Should().BeFalse(),
            () => otherTask.IsCompleted.Should().BeFalse());

        await Task.Delay(10);
        this.testee.Set();

        var otherTaskResult = await otherTask;

        Assert.Multiple(
            () => otherTaskResult.Should().BeTrue(),
            () => this.testee.IsSet.Should().BeFalse());
    }

    [Fact]
    public async Task WaitAsync_When_AlreadySetAndLaterAgain_Then_WaitersShouldBeNotifiedOneAtATime()
    {
        this.testee.Set();
        var waitTask1 = Task.Run(async () => await this.testee.WaitAsync(TimeSpan.FromMilliseconds(100)));
        var waitTask2 = Task.Run(async () =>
        {
            await Task.Delay(50);
            return await this.testee.WaitAsync(TimeSpan.FromMilliseconds(100));
        });

        var task1Result = await waitTask1;

        Assert.Multiple(
        () => task1Result.Should().BeTrue(),
        () => this.testee.IsSet.Should().BeFalse(),
        () => waitTask2.IsCompleted.Should().BeFalse());

        await Task.Delay(10);
        this.testee.Set();

        var task2Result = await waitTask2;

        Assert.Multiple(
            () => task2Result.Should().BeTrue(),
            () => this.testee.IsSet.Should().BeFalse());
    }

    [Fact]
    public async Task WaitAsync_When_AlreadySetAndLaterAgain_Then_WaitersShouldBeNotifiedOneAtATimeInAnyOrder()
    {
        this.testee.Set();
        var waitTask1 = Task.Run(async () => await this.testee.WaitAsync(TimeSpan.FromMilliseconds(100)));
        var waitTask2 = Task.Run(async () => await this.testee.WaitAsync(TimeSpan.FromMilliseconds(100)));

        var task = await Task.WhenAny(waitTask1, waitTask2);

        var firstTask = task == waitTask1 ? waitTask1 : waitTask2;
        var otherTask = task == waitTask1 ? waitTask2 : waitTask1;

        var resultFirstTask = await firstTask;
        Assert.Multiple(
            () => resultFirstTask.Should().BeTrue(),
            () => this.testee.IsSet.Should().BeFalse(),
            () => otherTask.IsCompleted.Should().BeFalse());

        await Task.Delay(10);
        this.testee.Set();

        var otherTaskResult = await otherTask;

        Assert.Multiple(
            () => otherTaskResult.Should().BeTrue(),
            () => this.testee.IsSet.Should().BeFalse());
    }
}