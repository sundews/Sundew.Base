// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncLazyOfTTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Threading;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Sundew.Base.Threading;

[Repeat(100)]
public class AsyncLazyOfTTests
{
    private const string ExpectedResult = "Computed";

    [Test]
    public async Task Await_Then_TaskResultShouldBeExpectedResult()
    {
        var expectedResult = new List<int> { 3 };
        var asyncLazy = new AsyncLazy<IList<int>, List<int>>(() => Task.FromResult(expectedResult));
        IAsyncLazy<IList<int>> asyncLazyBase = asyncLazy;

        var result = await asyncLazyBase;

        result.Should().BeSameAs(expectedResult);
    }

    [Test]
    public async Task GetValueAsync_When_CancellationTokenIsCancelled_Then_TaskCancelledExceptionIsThrown()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var testee = new AsyncLazy<string, string>(async () =>
        {
            await Task.Delay(500);
            return ExpectedResult;
        });

        var cancelTask = Task.Run(async () =>
        {
            await Task.Delay(10, CancellationToken.None);
            await cancellationTokenSource.CancelAsync();
        });

        await cancelTask;
        await Assert.ThrowsAsync<OperationCanceledException>(
            async () => await testee.GetValueAsync(cancellationTokenSource.Token));
    }

    [Test]
    public async Task GetValueAsync_When_FirstAttemptIsCancelledAndASecondAttemptIsMade_Then_ResultShouldBeExpectedResult()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var testee = new AsyncLazy<string, string>(async () =>
        {
            await Task.Delay(500);
            return ExpectedResult;
        });

        var cancelTask = Task.Run(async () =>
        {
            await Task.Delay(10, CancellationToken.None);
            await cancellationTokenSource.CancelAsync();
        });

        await cancelTask;
        await Assert.ThrowsAsync<OperationCanceledException>(
            async () => await testee.GetValueAsync(cancellationTokenSource.Token));

        var cancellationTokenSource2 = new CancellationTokenSource();
        string? result = null;
        var action = async () => result = await testee.GetValueAsync(cancellationTokenSource2.Token);
        await action.Should().NotThrowAsync();
        result.Should().Be(ExpectedResult);
    }

    [Test]
    public async Task GetValueAsync_When_CalledTwice_Then_ResultShouldBeExpectedResult()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var hasRun = false;
        var testee = new AsyncLazy<string, string>(async () =>
        {
            if (hasRun)
            {
                throw new NotSupportedException("Was only allowed to run once");
            }

            await Task.Delay(25, cancellationTokenSource.Token);
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