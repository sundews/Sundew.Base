// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CurrentThreadTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Threading;

using System.Diagnostics;
using System.Threading;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Sundew.Base.Threading;

[Explicit]
public class CurrentThreadTests
{
    [Test]
    [Arguments(20, 1000)]
    [Arguments(0, 200)]
    [Arguments(1000, 2000)]
    public void Sleep_When_Cancelled_Then_ElapsedTimeShouldBeWithInRange(int cancelAfter, int sleep)
    {
        var testee = new CurrentThread();
        var stopwatch = Stopwatch.StartNew();
        using var cancellationTokenSource = new CancellationTokenSource(cancelAfter);

        var result = testee.Sleep(sleep, cancellationTokenSource.Token);

        stopwatch.Stop();

        using (new AssertionScope())
        {
            result.Should().BeFalse();
            stopwatch.ElapsedMilliseconds.Should().BeInRange(cancelAfter, (sleep + 10) * 3);
        }
    }

    [Test]
    public void Sleep_When_NotCancelled_Then_ElapsedTimeShouldBeWithInRange()
    {
        var testee = new CurrentThread();
        var stopwatch = Stopwatch.StartNew();

        var result = testee.Sleep(10, CancellationToken.None);

        stopwatch.Stop();

        using (new AssertionScope())
        {
            result.Should().BeTrue();
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(80);
        }
    }

    [Test]
    [Arguments(20)]
    [Arguments(0)]
    [Arguments(200)]
    [Arguments(1000)]
    public void Sleep_Then_ElapsedTimeShouldBeWithInRange(int sleep)
    {
        var testee = new CurrentThread();
        var stopwatch = Stopwatch.StartNew();
        testee.Sleep(sleep);

        stopwatch.Stop();
        ((double)stopwatch.ElapsedMilliseconds).Should().BeInRange(sleep, (sleep + 10) * 3);
    }
}