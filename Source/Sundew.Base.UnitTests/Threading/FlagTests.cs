// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlagTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Threading
{
    using FluentAssertions;
    using Sundew.Base.Threading;
    using Xunit;

    public class FlagTests
    {
        private readonly Flag testee = new Flag();

        [Fact]
        public void IsSet_When_Set_Then_ResultShouldBeTrue()
        {
            this.testee.Set();

            this.testee.IsSet.Should().BeTrue();
        }

        [Fact]
        public void IsSet_Then_ResultShouldBeFalse()
        {
            this.testee.IsSet.Should().BeFalse();
        }

        [Fact]
        public void Clear_When_Set_Then_ResultShouldBeTrue()
        {
            this.testee.Set();

            var result = this.testee.Clear();

            result.Should().BeTrue();
            this.testee.IsSet.Should().BeFalse();
        }

        [Fact]
        public void Clear_Then_ResultShouldBeFalse()
        {
            var result = this.testee.Clear();

            result.Should().BeFalse();
            this.testee.IsSet.Should().BeFalse();
        }
    }
}