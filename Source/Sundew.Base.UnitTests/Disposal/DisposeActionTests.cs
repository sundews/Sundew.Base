// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposeActionTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Disposal
{
    using System.Threading;
    using FluentAssertions;
    using Sundew.Base.Disposal;
    using Xunit;

    public class DisposeActionTests
    {
        [Fact]
        public void Dispose_When_Awaiting_Then_ManualResetEventShouldBeSet()
        {
            var manualResetEvent = new ManualResetEventSlim(false);
            var testee = new DisposeAction(
                () =>
                {
                    Thread.Sleep(10);
                    manualResetEvent.Set();
                });

            testee.Dispose();

            manualResetEvent.IsSet.Should().BeTrue();
        }
    }
}