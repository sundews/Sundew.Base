// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CancellableJobTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Threading.Jobs;

using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Sundew.Base.Threading.Jobs;

public class CancellableJobTests
{
    [Test]
    public async Task Start_Then_IsRunningShouldBeTrueUntilStopped()
    {
        using var testee = new CancellableJob(async token => await Task.Delay(Timeout.InfiniteTimeSpan, token));

        var result = await testee.StartAsync();
        var isRunningAfterStart = testee.IsRunning;
        await testee.StopAsync();
        var isRunningAfterStop = testee.IsRunning;

        using (new AssertionScope())
        {
            result.Status.Should().Be(JobStartStatus.Started);
            isRunningAfterStart.Should().BeTrue();
            isRunningAfterStop.Should().BeFalse();
        }
    }

    [Test]
    public void Start_When_AlreadyRunning_Then_ResultShouldBeWasAlreadyRunning()
    {
        using var testee = new CancellableJob(async token => await Task.Delay(Timeout.InfiniteTimeSpan, token));

        testee.Start();
        var result = testee.Start();

        result.Status.Should().Be(JobStartStatus.WasAlreadyRunning);
    }

    [Test]
    public async Task StopAsync_When_RacingWithJobCompletion_Then_NoExceptionShouldBeThrown()
    {
        for (var i = 0; i < 200; i++)
        {
            using var testee = new CancellableJob(_ => Task.CompletedTask);

            await testee.StartAsync();
            var result = await testee.StopAsync();

            result.IsSuccess.Should().BeTrue("stopping a job that completes concurrently must not fail (iteration {0})", i);
        }
    }
}
