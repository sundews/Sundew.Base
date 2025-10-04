// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializeActionTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Initialization;

using System.Threading;
using AwesomeAssertions;
using Sundew.Base.Initialization;
using Xunit;

public class InitializeActionTests
{
    [Fact]
    public void Initialize_When_Awaiting_Then_ManualResetEventShouldBeSet()
    {
        var manualResetEvent = new ManualResetEventSlim(false);
        var testee = new InitializeAction(
            () =>
            {
                Thread.Sleep(10);
                manualResetEvent.Set();
            });

        testee.Initialize();

        manualResetEvent.IsSet.Should().BeTrue();
    }
}