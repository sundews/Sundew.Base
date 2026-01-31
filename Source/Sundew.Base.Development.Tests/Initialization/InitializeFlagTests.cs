// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializeFlagTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Initialization;

using System;
using System.Threading.Tasks;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Sundew.Base.Initialization;

public class InitializeFlagTests
{
    [Test]
    public void Initialize_Then_ResultShouldBeTrue()
    {
        var testee = new InitializeFlag();

        var result = testee.Initialize();

        result.Should().BeTrue();
    }

    [Test]
    public void IsInitialize_Then_ResultShouldBeFalse()
    {
        var testee = new InitializeFlag();

        var result = testee.IsInitialized;

        result.Should().BeFalse();
    }

    [Test]
    public void IsInitialize_When_Initialize_Then_ResultShouldBeTrue()
    {
        var testee = new InitializeFlag();
        testee.Initialize();

        var result = testee.IsInitialized;

        result.Should().BeTrue();
    }

    [Test]
    public void Initialize_When_Initialize_Then_ResultShouldBeFalse()
    {
        var testee = new InitializeFlag();
        testee.Initialize();

        var result = testee.Initialize();

        result.Should().BeFalse();
    }

    [Test]
    public async Task WhenInitialized_Then_IsInitializedShouldBeTrue()
    {
        var testee = new InitializeFlag();
        _ = Task.Run(async () =>
        {
            await Task.Delay(100).ConfigureAwait(false);
            testee.Initialize();
        });

        var result = await testee.WhenInitialized(new Cancellation(TimeSpan.FromMilliseconds(500)));

        using (new AssertionScope())
        {
            result.Should().BeTrue();
            testee.IsInitialized.Should().BeTrue();
        }
    }

    [Test]
    public async Task WhenInitialized_When_Timedout_Then_IsInitializedShouldBeFalse()
    {
        var testee = new InitializeFlag();
        _ = Task.Run(async () =>
        {
            await Task.Delay(500).ConfigureAwait(false);
            testee.Initialize();
        });

        var result = await testee.WhenInitialized(new Cancellation(TimeSpan.FromMilliseconds(50)));

        using (new AssertionScope())
        {
            result.Should().BeFalse();
            testee.IsInitialized.Should().BeFalse();
        }
    }
}