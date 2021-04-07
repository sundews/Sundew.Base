// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposeActionTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Disposal
{
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Sundew.Base.Disposal;
    using Xunit;

    public class DisposeActionTests
    {
        [Fact]
        public void Dispose_When_UsedWithAsyncAwait_Then_DisposeShouldNotReturnUntilCompleted()
        {
            var manualResetEvent = new ManualResetEventSlim(false);

            var testee = new DisposeAction(async () =>
            {
                await Task.Delay(20);
                manualResetEvent.Set();
            });

            testee.Dispose();

            manualResetEvent.IsSet.Should().BeTrue();
        }

        [Fact]
        public async Task DisposeAsync_When_UsedWithAsyncAwait_Then_DisposeShouldNotReturnUntilCompleted()
        {
            var manualResetEvent = new ManualResetEventSlim(false);

            var testee = new DisposeAction(async () =>
            {
                await Task.Delay(20);
                manualResetEvent.Set();
            });

            await testee.DisposeAsync();

            manualResetEvent.IsSet.Should().BeTrue();
        }
    }
}