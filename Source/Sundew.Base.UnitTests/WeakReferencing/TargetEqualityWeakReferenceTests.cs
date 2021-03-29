// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TargetEqualityWeakReferenceTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.WeakReferencing
{
    using FluentAssertions;
    using Microsoft.VisualBasic;
    using Sundew.Base.Equality;
    using Xunit;

    public class TargetEqualityWeakReferenceTests
    {
        [Fact]
        public void Equals_When_BothTargetsAreNull_Then_ResultShouldBeTrue()
        {
            var testee = new TargetEqualityWeakReference<Target>(null!);
            var testee2 = new TargetEqualityWeakReference<Target>(null!);

            var result = testee.Equals(testee2);

            result.Should().BeTrue();
        }

        [Fact]
        public void Equals_When_TargetsAreTheSame_Then_ResultShouldBeTrue()
        {
            var target = new Target();
            var testee = new TargetEqualityWeakReference<Target>(target);
            var testee2 = new TargetEqualityWeakReference<Target>(target);

            var result = testee.Equals(testee2);

            result.Should().BeTrue();
        }

        [Fact]
        public void Equals_When_TargetsDiffer_Then_ResultShouldBeFalse()
        {
            var target1 = new Target();
            var target2 = new Target();
            var testee = new TargetEqualityWeakReference<Target>(target1);
            var testee2 = new TargetEqualityWeakReference<Target>(target2);

            var result = testee.Equals(testee2);

            result.Should().BeFalse();
        }

        [Fact]
        public void Equals_When_Target2IsNull_Then_ResultShouldBeFalse()
        {
            var target1 = new Target();
            var testee = new TargetEqualityWeakReference<Target>(target1);
            var testee2 = new TargetEqualityWeakReference<Target>(null!);

            var result = testee.Equals(testee2);

            result.Should().BeFalse();
        }

        [Fact]
        public void Equals_When_Target1IsNull_Then_ResultShouldBeFalse()
        {
            var target2 = new Target();
            var testee = new TargetEqualityWeakReference<Target>(null!);
            var testee2 = new TargetEqualityWeakReference<Target>(target2);

            var result = testee.Equals(testee2);

            result.Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_When_TargetisNull_Then_ResultShouldBe0()
        {
            var testee = new TargetEqualityWeakReference<Target>(null!);

            var result = testee.GetHashCode();

            result.Should().Be(0);
        }

        [Fact]
        public void GetHashCode_When_TargetisNull_Then_ResultShouldNotBe0()
        {
            var testee = new TargetEqualityWeakReference<Target>(new Target());

            var result = testee.GetHashCode();

            result.Should().NotBe(0);
        }

        private class Target
        {
        }
    }
}