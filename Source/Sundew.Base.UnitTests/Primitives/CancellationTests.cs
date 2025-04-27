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

    [Theory]
    [InlineData(300, true)]
    [InlineData(10, false)]
    public async Task ImplicitOperator_When_PassingRegularCancellationToken_Then_ResultIsExpectedResult(int waitForCancellationTimeout, bool expectedResult)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var expectedTimeout = TimeSpan.FromMilliseconds(100);

        var cancellationWithTimeout = cancellationTokenSource.ToCancellationWithTimeout(expectedTimeout);
        var result = await CancellableCall2(cancellationWithTimeout, waitForCancellationTimeout);

        result.Should().Be(expectedResult);
        cancellationWithTimeout.Timeout.Should().Be(expectedTimeout);
    }

    private static Task<bool> CancellableCall(CancellationToken cancellationToken, int waitTimeout)
    {
        var resetEvent = new ManualResetEventAsync();
        cancellationToken.Register(_ => { resetEvent.Set(); }, __._);
        return resetEvent.WaitAsync(TimeSpan.FromMilliseconds(waitTimeout), CancellationToken.None);
    }

    private static async Task<bool> CancellableCall2(Cancellation cancellation, int waitTimeout)
    {
        using (cancellation.EnableCancellation())
        {
            var resetEvent = new ManualResetEventAsync();
            cancellation.Token.Register(_ => { resetEvent.Set(); }, __._);
            return await resetEvent.WaitAsync(TimeSpan.FromMilliseconds(waitTimeout), CancellationToken.None);
        }
    }
}