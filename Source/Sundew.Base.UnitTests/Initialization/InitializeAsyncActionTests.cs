// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializeAsyncActionTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Initialization
{
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Sundew.Base.Initialization;
    using Xunit;

    public class InitializeAsyncActionTests
    {
        [Fact]
        public async Task InitializeAsync_When_Awaiting_Then_ManualResetEventShouldBeSet()
        {
            var manualResetEvent = new ManualResetEventSlim(false);
            var testee = new InitializeAsyncAction(
                () =>
                {
                    Thread.Sleep(10);
                    manualResetEvent.Set();
                    return default;
                });

            await testee.InitializeAsync();

            manualResetEvent.IsSet.Should().BeTrue();
        }

        [Fact]
        public void InitializeAsync_When_NotAwaitingAndUsingYield_Then_ManualResetEventShouldNotYetHaveBeenSet()
        {
            var manualResetEvent = new ManualResetEventSlim(false);
            var testee = new InitializeAsyncAction(
                async () =>
                {
                    await Task.Delay(10);
                    manualResetEvent.Set();
                });

            testee.InitializeAsync();

            manualResetEvent.IsSet.Should().BeFalse();
        }
    }
}