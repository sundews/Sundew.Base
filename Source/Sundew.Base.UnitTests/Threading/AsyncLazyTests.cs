// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncLazyTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Threading;

using System;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Sundew.Base.Threading;
using Xunit;

public class AsyncLazyTests
{
    private const string ExpectedResult = "Computed";

    [Fact]
    public async Task Await_Then_ResultShouldBeExpectedResult()
    {
        var expectedResult = 3;
        var asyncLazy = new AsyncLazy<int>(() => Task.FromResult(expectedResult));

        var result = await asyncLazy;

        result.Should().Be(expectedResult);
    }

    [Fact]
    public async Task GetValueAsync_When_CancellationTokenIsCancelled_Then_TaskCancelledExceptionIsThrown()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var testee = new AsyncLazy<string>(async cancellationToken =>
        {
            await Task.Delay(500, cancellationToken);
            return ExpectedResult;
        });

        var cancelTask = Task.Run(async () =>
        {
            await Task.Delay(10, CancellationToken.None);
            await cancellationTokenSource.CancelAsync();
        });

        await Assert.ThrowsAsync<TaskCanceledException>(
            async () => await testee.GetValueAsync(cancellationTokenSource.Token));
        await cancelTask;
    }

    [Fact]
    public async Task GetValueAsync_When_FirstAttemptIsCancelledAndASecondAttemptIsMade_Then_ResultShouldBeExpectedResult()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var testee = new AsyncLazy<string>(async cancellationToken =>
        {
            await Task.Delay(500, cancellationToken);
            return ExpectedResult;
        });

        var cancelTask = Task.Run(async () =>
        {
            await Task.Delay(10, CancellationToken.None);
            await cancellationTokenSource.CancelAsync();
        });

        await Assert.ThrowsAsync<TaskCanceledException>(
            async () => await testee.GetValueAsync(cancellationTokenSource.Token));
        await cancelTask;

        var cancellationTokenSource2 = new CancellationTokenSource();
        string? result = null;
        var action = async () => result = await testee.GetValueAsync(cancellationTokenSource2.Token);
        await action.Should().NotThrowAsync();
        result.Should().Be(ExpectedResult);
    }

    [Fact]
    public async Task GetValueAsync_When_CalledTwice_Then_ResultShouldBeExpectedResult()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var hasRun = false;
        var testee = new AsyncLazy<string>(async cancellationToken =>
        {
            if (hasRun)
            {
                throw new NotSupportedException("Was only allowed to run once");
            }

            await Task.Delay(10, cancellationToken);
            hasRun = true;
            return ExpectedResult;
        });

        string? result = null;
        var action = async () => result = await testee.GetValueAsync(cancellationTokenSource.Token);
        await action.Should().NotThrowAsync();
        string? result2 = null;
        var action2 = async () => result2 = await testee.GetValueAsync(cancellationTokenSource.Token);
        await action2.Should().NotThrowAsync();
        result.Should().Be(ExpectedResult);
        result2.Should().Be(ExpectedResult);
    }
}