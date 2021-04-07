﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializeFlagTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Initialization
{
    using FluentAssertions;
    using Sundew.Base.Initialization;
    using Xunit;

    public class InitializeFlagTests
    {
        [Fact]
        public void Initialize_Then_ResultShouldBeTrue()
        {
            var testee = new InitializeFlag();

            var result = testee.Initialize();

            result.Should().BeTrue();
        }

        [Fact]
        public void IsInitialize_Then_ResultShouldBeFalse()
        {
            var testee = new InitializeFlag();

            var result = testee.IsInitialized;

            result.Should().BeFalse();
        }

        [Fact]
        public void IsInitialize_When_Initialize_Then_ResultShouldBeTrue()
        {
            var testee = new InitializeFlag();
            testee.Initialize();

            var result = testee.IsInitialized;

            result.Should().BeTrue();
        }

        [Fact]
        public void Initialize_When_Initialize_Then_ResultShouldBeFalse()
        {
            var testee = new InitializeFlag();
            testee.Initialize();

            var result = testee.Initialize();

            result.Should().BeFalse();
        }
    }
}