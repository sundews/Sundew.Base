// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposeAsyncActionTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Disposal;

using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Sundew.Base.Disposal;
using Xunit;

public class DisposeAsyncActionTests
{
    [Fact]
    public async Task DisposeAsync_When_UsedWithAsyncAwait_Then_DisposeShouldBeCompleted()
    {
        var manualResetEvent = new ManualResetEventSlim(false);

        var testee = new DisposeAsyncAction(async () =>
        {
            await Task.Delay(200);
            manualResetEvent.Set();
        });

        await testee.DisposeAsync();

        manualResetEvent.IsSet.Should().BeTrue();
    }

    [Fact]
    public void DisposeAsync_When_UsedWithAsyncAwait_Then_DisposeShouldNotBeCompleted()
    {
        var manualResetEvent = new ManualResetEventSlim(false);

        var testee = new DisposeAsyncAction(async () =>
        {
            await Task.Delay(200);
            manualResetEvent.Set();
        });

        _ = testee.DisposeAsync();

        manualResetEvent.IsSet.Should().BeFalse();
    }
}