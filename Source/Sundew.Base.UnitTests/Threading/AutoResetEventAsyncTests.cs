// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoResetEventAsyncTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Threading
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Sundew.Base.Threading;
    using Xunit;

    public class AutoResetEventAsyncTests
    {
        private readonly AutoResetEventAsync testee;

        public AutoResetEventAsyncTests()
        {
            this.testee = new AutoResetEventAsync();
        }

        [Fact]
        public async Task WaitAsync_When_Set_Then_ResultShouldBeTrue()
        {
            this.testee.Set();

            var result = await this.testee.WaitAsync();

            result.Should().BeTrue();
            this.testee.IsSet.Should().BeFalse();
        }

        [Fact]
        public async Task Set_When_Waiting_Then_ResultShouldBeTrue()
        {
            var waitTask = Task.Run(async () =>
            {
                await Task.Delay(5);
                return await this.testee.WaitAsync();
            });

            this.testee.Set();

            var result = await waitTask;
            result.Should().BeTrue();
            this.testee.IsSet.Should().BeFalse();
        }

        [Fact]
        public async Task WaitAsync_When_Cancelled_Then_ResultShouldBeFalse()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var waitTask = Task.Run(async () => await this.testee.WaitAsync(cancellationTokenSource.Token));
            cancellationTokenSource.Cancel();

            var result = await waitTask;
            result.Should().BeFalse();
            this.testee.IsSet.Should().BeFalse();
        }

        [Fact]
        public async Task WaitAsync_When_Timedout_Then_ResultShouldBeFalse()
        {
            var waitTask = Task.Run(async () => await this.testee.WaitAsync(TimeSpan.FromMilliseconds(1)));
            await Task.Delay(10);

            var result = await waitTask;
            result.Should().BeFalse();
            this.testee.IsSet.Should().BeFalse();
        }
    }
}