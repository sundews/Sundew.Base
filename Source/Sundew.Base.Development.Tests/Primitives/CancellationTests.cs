// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CancellationTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Primitives;

using System;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Sundew.Base.Threading;
using Xunit;

public class CancellationTests
{
    [Theory]
    [InlineData(300, true)]
    [InlineData(10, false)]
    public async Task ImplicitOperator_When_CancellationOriginCancellationTokenSource_Then_ResultIsExpectedResult(int waitForCancellationTimeout, bool expectedResult)
    {
        var expectedTimeout = TimeSpan.FromMilliseconds(100);
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationWithTimeout = cancellationTokenSource.ToCancellationWithTimeout(expectedTimeout);
        using var cancellationEnabler = cancellationWithTimeout.EnableCancellation(false);

        cancellationTokenSource.CancelAfter(cancellationWithTimeout.Timeout);

        var result = await CancellableCall(cancellationWithTimeout, waitForCancellationTimeout);

        result.Should().Be(expectedResult);
        cancellationWithTimeout.Timeout.Should().Be(expectedTimeout);
    }

    [Fact]
    public async Task ImplicitOperator_When_PassingDefaultCancellation_Then_ResultIsExpectedResult()
    {
        var result = await CancellableCall(default(Cancellation), 100);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task EnableCancellation_When_Timeout_Then_CancelReasonShouldBeTimeout()
    {
        var cancellation = new Cancellation(TimeSpan.FromMilliseconds(50));
        using var enabler = cancellation.EnableCancellation();

        await Task.Delay(500);

        enabler.CancelReason.Should().Be(CancelReason.Timeout);
    }

    [Fact]
    public async Task EnableCancellation_When_CancelAsync_Then_CancelReasonShouldBeInternal()
    {
        var manualResetEventAsync = new ManualResetEventAsync();
        var cancellation = default(Cancellation);
        var enabler = cancellation.EnableCancellation();
        _ = Task.Run(async () =>
        {
            await Task.Delay(50);
            await enabler.CancelAsync();
            enabler.Dispose();
            manualResetEventAsync.Set();
        });

        await manualResetEventAsync.WaitAsync();

        enabler.CancelReason.Should().Be(CancelReason.Internal);
    }

    [Fact]
    public async Task EnablerRegister_When_Timeout_Then_CancelReasonShouldBeTimeout()
    {
        var manualResetEventAsync = new ManualResetEventAsync();
        var taskCompletionSource = new TaskCompletionSource<CancelReason>();
        var cancellation = new Cancellation(TimeSpan.FromMilliseconds(50));
        using var enabler = cancellation.EnableCancellation();
        using var register = enabler.Register(reason =>
        {
            taskCompletionSource.TrySetResult(reason);
            manualResetEventAsync.Set();
        });

        await manualResetEventAsync.WaitAsync();

        var result = await taskCompletionSource.Task;

        result.Should().Be(CancelReason.Timeout);
    }

    [Fact]
    public async Task EnableCancellation_When_ExternalCancelled_Then_CancelReasonShouldBeExternal()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellation = new Cancellation(cancellationTokenSource.Token);
        var enabler = cancellation.EnableCancellation();
        _ = Task.Run(async () =>
        {
            await Task.Delay(20, CancellationToken.None);
            await cancellationTokenSource.CancelAsync();
            enabler.Dispose();
            cancellationTokenSource.Dispose();
        });

        await Task.Delay(1000, CancellationToken.None);

        enabler.CancelReason.Should().Be(CancelReason.External);
    }

    [Fact]
    public void EnableCancellation_When_NotCancelled_Then_CancelReasonShouldBeNull()
    {
        var cancellation = default(Cancellation);
        using var enabler = cancellation.EnableCancellation();

        enabler.CancelReason.Should().BeNull();
    }

    [Theory]
    [InlineData(300, true)]
    [InlineData(10, false)]
    public async Task ImplicitOperator_When_PassingRegularCancellationToken_Then_ResultIsExpectedResult(int waitForCancellationTimeout, bool expectedResult)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var expectedTimeout = TimeSpan.FromMilliseconds(100);

        var cancellationWithTimeout = cancellationTokenSource.ToCancellationWithTimeout(expectedTimeout);
        var result = await CancellableCall(cancellationWithTimeout, waitForCancellationTimeout);

        result.Should().Be(expectedResult);
        cancellationWithTimeout.Timeout.Should().Be(expectedTimeout);
    }

    private static async Task<bool> CancellableCall(Cancellation cancellation, int waitTimeout)
    {
        using (cancellation.EnableCancellation())
        {
            var resetEvent = new ManualResetEventAsync();
            cancellation.Token.Register(_ => { resetEvent.Set(); }, __._);
            return await resetEvent.WaitAsync(new Cancellation(TimeSpan.FromMilliseconds(waitTimeout), CancellationToken.None));
        }
    }
}