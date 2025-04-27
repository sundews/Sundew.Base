// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CancellationTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Primitives;

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Sundew.Base.Threading;
using Xunit;

public class CancellationTests
{
    [Theory]
    [InlineData(300, true)]
    [InlineData(10, false)]
    public async Task ImplicitOperator_Then_ResultIsExpectedResult(int waitForCancellationTimeout, bool expectedResult)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellingTask = Task.Run(async () =>
        {
            await Task.Delay(100, CancellationToken.None).ConfigureAwait(false);
            await cancellationTokenSource.CancelAsync();
            cancellationTokenSource.Dispose();
        });

        var result = await CancellableCall(cancellationTokenSource.ToCancellationWithTimeout(TimeSpan.FromMilliseconds(100)), waitForCancellationTimeout);
        await cancellingTask;

        result.Should().Be(expectedResult);
    }

    [Fact]
    public async Task ImplicitOperator_When_PassingDefaultCancellation_Then_ResultIsExpectedResult()
    {
        var result = await CancellableCall(default(Cancellation), 100);

        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(300, true)]
    [InlineData(10, false)]
    public async Task ImplicitOperator_When_PassingRegularCancellationToken_Then_ResultIsExpectedResult(int waitForCancellationTimeout, bool expectedResult)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellingTask = Task.Run(async () =>
        {
            await Task.Delay(100, CancellationToken.None).ConfigureAwait(false);
            await cancellationTokenSource.CancelAsync();
            cancellationTokenSource.Dispose();
        });

        var result = await CancellableCall2(cancellationTokenSource.Token, waitForCancellationTimeout);
        await cancellingTask;

        result.Should().Be(expectedResult);
    }

    private static Task<bool> CancellableCall(CancellationToken cancellationToken, int waitTimeout)
    {
        var resetEvent = new ManualResetEventAsync();
        cancellationToken.Register(_ => { resetEvent.Set(); }, __._);
        return resetEvent.WaitAsync(TimeSpan.FromMilliseconds(waitTimeout), CancellationToken.None);
    }

    private static Task<bool> CancellableCall2(Cancellation cancellationToken, int waitTimeout)
    {
        var resetEvent = new ManualResetEventAsync();
        cancellationToken.Token.Register(_ => { resetEvent.Set(); }, __._);
        return resetEvent.WaitAsync(TimeSpan.FromMilliseconds(waitTimeout), CancellationToken.None);
    }
}