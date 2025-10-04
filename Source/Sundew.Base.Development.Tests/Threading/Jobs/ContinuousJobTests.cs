// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContinuousJobTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Threading.Jobs;

using System;
using System.Threading;
using AwesomeAssertions;
using Sundew.Base.Threading.Jobs;
using Xunit;

public class ContinuousJobTests
{
    [Fact]
    public void Start_When_AlreadyStarted_Then_ResultShouldBeAlreadyStarted()
    {
        using var testee = new ContinuousJob(_ => { });
        var result = testee.Start();
        result = testee.Start();

        result.Status.Should().Be(JobStartStatus.WasAlreadyRunning);
    }

    [Fact]
    public void Stop_When_NotStarted_Then_ResultShouldBeSuccess()
    {
        using var testee = new ContinuousJob(_ => { });
        var result = testee.Stop();

        ((bool)result).Should().BeTrue();
    }

    [Fact]
    public void Start_When_ExceptionIsThrownAndHandled4Times_Then_TesteExceptionShouldContainThrownException()
    {
        using var resetEvent = new ManualResetEventSlim();
        var errorCounter = 0;
        using var testee = new ContinuousJob(
            _ => throw new InvalidOperationException(),
            (Exception _, ref bool handled) => handled = errorCounter++ < 4);

        var startResult = testee.Start();
        testee.Wait();

        Thread.Sleep(1);

        startResult.Status.Should().Be(JobStartStatus.Started);
        testee.Exception!.InnerException.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public void Stop_When_ExceptionIsThrown_Then_ResultShouldContainThrownException()
    {
        using var resetEvent = new ManualResetEventSlim();
        using var testee = new ContinuousJob(_ =>
        {
            resetEvent.Set();
            throw new InvalidOperationException();
        });

        var startResult = testee.Start();
        resetEvent.Wait();
        Thread.Sleep(1);

        var result = testee.Stop();

        startResult.Status.Should().Be(JobStartStatus.Started);
        result.Error!.InnerException.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public void Stop_Then_ResultShouldBeSuccess()
    {
        using var resetEvent = new ManualResetEventSlim();
        using var testee = new ContinuousJob(_ => resetEvent.Set());
        var startResult = testee.Start();
        resetEvent.Wait();
        Thread.Sleep(1);

        var result = testee.Stop();

        startResult.Status.Should().Be(JobStartStatus.Started);
        ((bool)result).Should().BeTrue();
    }
}