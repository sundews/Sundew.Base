// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializeActionTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
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

    public class InitializeActionTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task InitializeAsync_When_Awaiting_Then_ManualResetEventShouldBeSet(bool useYield)
        {
            var manualResetEvent = new ManualResetEventSlim(false);
            var testee = new InitializeAction(
                () =>
                {
                    Thread.Sleep(10);
                    manualResetEvent.Set();
                }, useYield);

            await testee.InitializeAsync();

            manualResetEvent.IsSet.Should().BeTrue();
        }

        [Fact]
        public void InitializeAsync_When_NotAwaitingAndUsingYield_Then_ManualResetEventShouldNotYetHaveBeenSet()
        {
            var manualResetEvent = new ManualResetEventSlim(false);
            var testee = new InitializeAction(
                () =>
                {
                    Thread.Sleep(10);
                    manualResetEvent.Set();
                }, true);

            testee.InitializeAsync();

            manualResetEvent.IsSet.Should().BeFalse();
        }
    }
}