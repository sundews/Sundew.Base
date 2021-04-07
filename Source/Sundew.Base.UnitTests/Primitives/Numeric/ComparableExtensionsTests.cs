// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComparableExtensionsTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Primitives.Numeric
{
    using FluentAssertions;
    using Sundew.Base.Primitives.Numeric;
    using Xunit;

    public class ComparableExtensionsTests
    {
        private const double AValue = 4;
        private const double AHigherComparisionValue = 5;
        private const double ALowerComparisionValue = 3;
        private const double AEqualComparisionValue = 4;

        [Fact]
        public void IsGreaterThanOrEqualTo_When_ValueIsLower_Then_ResultShouldBeTrue()
        {
            var result = AValue.IsGreaterThanOrEqualTo(ALowerComparisionValue);

            result.Should().BeTrue();
        }

        [Fact]
        public void IsGreaterThanOrEqualTo_When_ValueIsGreater_Then_ResultShouldBeFalse()
        {
            var result = AValue.IsGreaterThanOrEqualTo(AHigherComparisionValue);

            result.Should().BeFalse();
        }

        [Fact]
        public void IsGreaterThanOrEqualTo_When_ValuesAreEqual_Then_ResultShouldBeTrue()
        {
            var result = AValue.IsGreaterThanOrEqualTo(AEqualComparisionValue);

            result.Should().BeTrue();
        }

        [Fact]
        public void IsLessThanOrEqualTo_When_ValueIsLower_Then_ResultShouldBeFalse()
        {
            var result = AValue.IsLessThanOrEqualTo(ALowerComparisionValue);

            result.Should().BeFalse();
        }

        [Fact]
        public void IsLessThanOrEqualTo_When_ValueIsGreater_Then_ResultShouldBeTrue()
        {
            var result = AValue.IsLessThanOrEqualTo(AHigherComparisionValue);

            result.Should().BeTrue();
        }

        [Fact]
        public void IsLessThanOrEqualTo_When_ValuesAreEqual_Then_ResultShouldBeTrue()
        {
            var result = AValue.IsLessThanOrEqualTo(AEqualComparisionValue);

            result.Should().BeTrue();
        }

        [Fact]
        public void IsLessThan_When_ValueIsLower_Then_ResultShouldBeFalse()
        {
            var result = AValue.IsLessThan(ALowerComparisionValue);

            result.Should().BeFalse();
        }

        [Fact]
        public void IsLessThan_When_ValueIsGreater_Then_ResultShouldBeTrue()
        {
            var result = AValue.IsLessThan(AHigherComparisionValue);

            result.Should().BeTrue();
        }

        [Fact]
        public void IsLessThan_When_ValuesAreEqual_Then_ResultShouldBeFalse()
        {
            var result = AValue.IsLessThan(AEqualComparisionValue);

            result.Should().BeFalse();
        }

        [Fact]
        public void IsGreaterThan_When_ValueIsLower_Then_ResultShouldBeTrue()
        {
            var result = AValue.IsGreaterThan(ALowerComparisionValue);

            result.Should().BeTrue();
        }

        [Fact]
        public void IsGreaterThan_When_ValueIsGreater_Then_ResultShouldBeFalse()
        {
            var result = AValue.IsGreaterThan(AHigherComparisionValue);

            result.Should().BeFalse();
        }

        [Fact]
        public void IsGreaterThan_When_ValuesAreEqual_Then_ResultShouldBeFalse()
        {
            var result = AValue.IsGreaterThan(AEqualComparisionValue);

            result.Should().BeFalse();
        }

        [Fact]
        public void IsEqualTo_When_ValuesAreEqual_Then_ResultShouldBeTrue()
        {
            var result = AValue.IsEqualTo(AEqualComparisionValue);

            result.Should().BeTrue();
        }

        [Fact]
        public void IsEqualTo_When_ValuesAreNotEqual_Then_ResultShouldBeFalse()
        {
            var result = AValue.IsEqualTo(ALowerComparisionValue);

            result.Should().BeFalse();
        }

        [Fact]
        public void IsNotEqualTo_When_ValuesAreNotEqual_Then_ResultShouldBeTrue()
        {
            var result = AValue.IsNotEqualTo(ALowerComparisionValue);

            result.Should().BeTrue();
        }

        [Fact]
        public void IsNotEqualTo_When_ValuesAreEqual_Then_ResultShouldBeFalse()
        {
            var result = AValue.IsNotEqualTo(AEqualComparisionValue);

            result.Should().BeFalse();
        }
    }
}