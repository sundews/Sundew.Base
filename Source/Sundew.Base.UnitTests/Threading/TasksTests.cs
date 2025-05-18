// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TasksTests.cs" company="Sundews">
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

public class TasksTests
{
    [Fact]
    public async Task WhenAll_When_ATaskIsCancelled_Then_OuterTaskIsCancelled()
    {
        async Task<(int, int, int)> Test()
        {
            var cancellationTokenSource = new System.Threading.CancellationTokenSource();
            cancellationTokenSource.CancelAfter(10);
            var task1 = Task.FromResult(1);
            var task2 = Task.Run(
                async () =>
                {
                    await Task.Delay(100, CancellationToken.None).ConfigureAwait(false);
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    return 2;
                },
                cancellationTokenSource.Token);
            var task3 = Task.FromResult(3);
            return await Tasks.WhenAll(task1, task2, task3);
        }

        await Assert.ThrowsAsync<OperationCanceledException>(Test);
    }

    [Fact]
    public async Task WhenAll_When_ATaskThrow_Then_ExceptionIsRethrown()
    {
        async Task<(int, int, int)> Test()
        {
            var task1 = Task.FromResult(1);
            var task2 = Task.Run(async () =>
            {
                await Task.Delay(10).ConfigureAwait(false);
                return new Random(1).Next(0, 1) > -1 ? throw new InvalidOperationException() : 2;
            });
            var task3 = Task.FromResult(3);
            return await Tasks.WhenAll(task1, task2, task3);
        }

        await Assert.ThrowsAsync<InvalidOperationException>(Test);
    }

    [Fact]
    public async Task WhenAll_Then_ResultAreExpectedResults()
    {
        var task1 = Task.FromResult(1);
        var task2 = Task.FromResult(2);
        var task3 = Task.FromResult(3);
        var result = await Tasks.WhenAll(task1, task2, task3);
        result.Item1.Should().Be(1);
        result.Item2.Should().Be(2);
        result.Item3.Should().Be(3);
    }
}