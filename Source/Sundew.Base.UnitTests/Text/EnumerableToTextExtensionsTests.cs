// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableToTextExtensionsTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Text
{
    using System;
    using System.Globalization;
    using System.Text;
    using FluentAssertions;
    using Sundew.Base.Text;
    using Xunit;

    public class EnumerableToTextExtensionsTests
    {
        private const string ExpecteCharSeparatedResult = "2.3|2.5|6.5";
        private const string ExpectedStringSeparatedResult = "2.3| 2.5| 6.5";
        private const string StringSeparator = "| ";
        private const char CharSeparator = '|';
        private static readonly double[] Values = { 2.3, 2.5, 6.5 };

        [Fact]
        public void JoinToString_When_PassingFormatProvider_Then_ResultShouldBeExpectedResult()
        {
            var result = Values.JoinToString(CharSeparator, CultureInfo.InvariantCulture);

            result.Should().Be(ExpecteCharSeparatedResult);
        }

        [Fact]
        public void JoinToString_Then_ResultShouldBeExpectedResult()
        {
            var result = Values.JoinToString((builder, d) => builder.Append(d, CultureInfo.InvariantCulture), CharSeparator);

            result.Should().Be(ExpecteCharSeparatedResult);
        }

        [Fact]
        public void JoinToString_When_PassingFormatProviderAndStringSeparator_Then_ResultShouldBeExpectedResult()
        {
            var result = Values.JoinToString(StringSeparator, CultureInfo.InvariantCulture);

            result.Should().Be(ExpectedStringSeparatedResult);
        }

        [Fact]
        public void JoinToString_When_UsingStringSeparator_Then_ResultShouldBeExpectedResult()
        {
            var result = Values.JoinToString((builder, d) => builder.Append(d, CultureInfo.InvariantCulture), StringSeparator);

            result.Should().Be(ExpectedStringSeparatedResult);
        }

        [Fact]
        public void AppendItems_Then_ResultShouldBeExpectedResult()
        {
            var result = new StringBuilder().AppendItems(Values, CharSeparator, CultureInfo.InvariantCulture).ToString();

            result.Should().Be(ExpecteCharSeparatedResult);
        }

        [Fact]
        public void AppendItems_When_UsingStringSeparator_Then_ResultShouldBeExpectedResult()
        {
            var result = new StringBuilder().AppendItems(Values, StringSeparator, CultureInfo.InvariantCulture).ToString();

            result.Should().Be(ExpectedStringSeparatedResult);
        }

        [Fact]
        public void AppendItems_When_ArrayIsEmptyAndUsingStringSeparator_Then_ResultShouldBeEmpty()
        {
            var result = new StringBuilder().AppendItems(Array.Empty<string>(), StringSeparator, CultureInfo.InvariantCulture).ToString();

            result.Should().Be(string.Empty);
        }

        [Theory]
        [InlineData(true, "Hi|there|!")]
        [InlineData(false, "Hi|there||!")]
        public void AppendItems_When_UsingNullableClassOverloadAndCharSeparator_Then_ResultShouldBeExpectedResult(bool skipNullValues, string expectedResult)
        {
            var result = new StringBuilder().AppendItems(new[] { "Hi", "there", null, "!" }, CharSeparator, CultureInfo.InvariantCulture, skipNullValues).ToString();

            result.Should().Be(expectedResult);
        }

        [Fact]
        public void AppendItems_When_UsingNotNullOverloadAndCharSeparator_Then_ResultShouldBeExpectedResult()
        {
            var result = new StringBuilder().AppendItems(new[] { "Hi", "there", "!" }, CharSeparator, CultureInfo.InvariantCulture).ToString();

            result.Should().Be("Hi|there|!");
        }
    }
}