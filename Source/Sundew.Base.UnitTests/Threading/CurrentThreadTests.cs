// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CurrentThreadTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Threading;

using System.Diagnostics;
using System.Threading;
using AwesomeAssertions;
using Sundew.Base.Threading;
using Xunit;

public class CurrentThreadTests
{
    [Theory]
    [InlineData(20, 500)]
    [InlineData(0, 200)]
    [InlineData(1000, 1500)]
    public void Sleep_When_Cancelled_Then_ElapsedTimeShouldBeWithInRange(int cancelAfter, int sleep)
    {
        var testee = new CurrentThread();
        using var cancellationTokenSource = new CancellationTokenSource(cancelAfter);
        var stopwatch = Stopwatch.StartNew();

        var result = testee.Sleep(sleep, cancellationTokenSource.Token);

        stopwatch.Stop();
        result.Should().BeFalse();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(sleep);
    }

    [Fact]
    public void Sleep_When_NotCancelled_Then_ElapsedTimeShouldBeWithInRange()
    {
        var testee = new CurrentThread();
        var stopwatch = Stopwatch.StartNew();

        var result = testee.Sleep(10, CancellationToken.None);

        stopwatch.Stop();
        stopwatch.ElapsedMilliseconds.Should().BeInRange(10, 80);
    }

    [Theory]
    [InlineData(20)]
    [InlineData(0)]
    [InlineData(200)]
    [InlineData(1000)]
    public void Sleep_Then_ElapsedTimeShouldBeWithInRange(int sleep)
    {
        var testee = new CurrentThread();
        var stopwatch = Stopwatch.StartNew();
        testee.Sleep(sleep);

        stopwatch.Stop();
        ((double)stopwatch.ElapsedMilliseconds).Should().BeInRange(sleep, (sleep + 10) * 1.8);
    }
}