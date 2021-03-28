// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensionsTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Text
{
    using System.Globalization;
    using System.Text;
    using FluentAssertions;
    using Sundew.Base.Text;
    using Xunit;

    public class EnumerableExtensionsTests
    {
        private const string ExpecteCharSeparatedResult = "2.3|2.5|6.5";
        private const string ExpectedStringSeparatedResult = "2.3| 2.5| 6.5";
        private const string StringSeparator = "| ";
        private const char CharSeparator = '|';
        private static readonly double[] Values = { 2.3, 2.5, 6.5 };

        [Fact]
        public void AggregateToString_When_PassingFormatProvider_Then_ResultShouldBeExpectedResult()
        {
            var result = Values.AggregateToString(CharSeparator, CultureInfo.InvariantCulture);

            result.Should().Be(ExpecteCharSeparatedResult);
        }

        [Fact]
        public void AggregateToString_Then_ResultShouldBeExpectedResult()
        {
            var result = Values.AggregateToString((builder, d) => builder.Append(d, CultureInfo.InvariantCulture), CharSeparator);

            result.Should().Be(ExpecteCharSeparatedResult);
        }

        [Fact]
        public void AggregateToString_When_PassingFormatProviderAndStringSeparator_Then_ResultShouldBeExpectedResult()
        {
            var result = Values.AggregateToString(StringSeparator, CultureInfo.InvariantCulture);

            result.Should().Be(ExpectedStringSeparatedResult);
        }

        [Fact]
        public void AggregateToString_When_AndStringSeparator_Then_ResultShouldBeExpectedResult()
        {
            var result = Values.AggregateToString((builder, d) => builder.Append(d, CultureInfo.InvariantCulture), StringSeparator);

            result.Should().Be(ExpectedStringSeparatedResult);
        }

        [Fact]
        public void AggregateToStringBuilder_Then_ResultShouldBeExpectedResult()
        {
            var result = Values.AggregateToStringBuilder(new StringBuilder(), CharSeparator, CultureInfo.InvariantCulture).ToString();

            result.Should().Be(ExpecteCharSeparatedResult);
        }

        [Fact]
        public void AggregateToStringBuilder_When_AndStringSeparator_Then_ResultShouldBeExpectedResult()
        {
            var result = Values.AggregateToStringBuilder(new StringBuilder(), StringSeparator, CultureInfo.InvariantCulture).ToString();

            result.Should().Be(ExpectedStringSeparatedResult);
        }
    }
}