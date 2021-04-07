// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumExtensionsTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Primitives
{
    using System;
    using FluentAssertions;
    using Sundew.Base.Primitives;
    using Xunit;

    public class EnumExtensionsTests
    {
        [Flags]
        public enum NumbersSbyte : sbyte
        {
#pragma warning disable SA1136 // Enum values should be on separate lines
            One, Two, Three, Four, Max = sbyte.MaxValue,
        }

        [Flags]
        public enum NumbersInt : int
        {
            One, Two, Three, Four, Max = int.MaxValue,
        }

        [Flags]
        public enum NumbersUlong : ulong
        {
            One, Two, Three, Four, Max = 0xFFFFFFFFFFFFFFFF,
        }

        public enum Number
        {
            One, Two, Three,
#pragma warning restore SA1136 // Enum values should be on separate lines
        }

        [Theory]
        [InlineData("Max", NumbersUlong.Max)]
        [InlineData("One, Two, Four,Max", NumbersUlong.One | NumbersUlong.Four | NumbersUlong.Max)]
        [InlineData("Two, Four,Max", NumbersUlong.Two | NumbersUlong.Four | NumbersUlong.Max)]
        public void ParseFlagsEnum_When_EnumTypeIsUlong_Then_ResultShouldBeExpectedResult(string value, NumbersUlong expectedResult)
        {
            var result = value.ParseFlagsEnum<NumbersUlong>();

            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("Max", NumbersInt.Max)]
        [InlineData("One, Two, Four,Max", NumbersInt.One | NumbersInt.Four | NumbersInt.Max)]
        [InlineData("Two, Four,Max", NumbersInt.Two | NumbersInt.Four | NumbersInt.Max)]
        public void ParseFlagsEnum_When_EnumTypeIsInt_Then_ResultShouldBeExpectedResult(string value, NumbersInt expectedResult)
        {
            var result = value.ParseFlagsEnum<NumbersInt>();

            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("Max", NumbersSbyte.Max)]
        [InlineData("One, Two, Four,Max", NumbersSbyte.One | NumbersSbyte.Four | NumbersSbyte.Max)]
        [InlineData("Two, Four,Max", NumbersSbyte.Two | NumbersSbyte.Four | NumbersSbyte.Max)]
        public void ParseFlagsEnum_When_EnumTypeIsSbyte_Then_ResultShouldBeExpectedResult(string value, NumbersSbyte expectedResult)
        {
            var result = value.ParseFlagsEnum<NumbersSbyte>();

            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("Max", true, NumbersSbyte.Max)]
        [InlineData("One, Two, Four,Max", true, NumbersSbyte.One | NumbersSbyte.Four | NumbersSbyte.Max)]
        [InlineData("Two, Four,Max", true, NumbersSbyte.Two | NumbersSbyte.Four | NumbersSbyte.Max)]
        [InlineData(null, false, NumbersSbyte.One)]
        public void TryParseFlagsEnum_When_EnumTypeIsSbyte_Then_ResultShouldBeExpectedResult(string value, bool expectedResult, NumbersSbyte expectedNumbers)
        {
            var result = value.TryParseFlagsEnum(out NumbersSbyte numbers);

            result.Should().Be(expectedResult);
            numbers.Should().Be(expectedNumbers);
        }

        [Theory]
        [InlineData("One", true, Number.One)]
        [InlineData("Two", true, Number.Two)]
        [InlineData(null, false, Number.One)]
        public void TryParseEnum_Then_ResultShouldBeExpectedResult(string input, bool expectedResult, Number expectedNumber)
        {
            var result = input.TryParseEnum(out Number number);

            result.Should().Be(expectedResult);
            number.Should().Be(expectedNumber);
        }

        [Theory]
        [InlineData(null, Number.One)]
        [InlineData(Number.Two, Number.Two)]
        public void ToEnumOrDefault_Then_ResultShouldBeExpected(object? value, Number expectedNumber)
        {
            var result = value.ToEnumOrDefault<Number>();

            result.Should().Be(expectedNumber);
        }

        [Theory]
        [InlineData(null, Number.One)]
        [InlineData(Number.Two, Number.Two)]
        public void ToEnumOrDefault_When_PassingDefaultValue_Then_ResultShouldBeExpected(object? value, Number expectedNumber)
        {
            var result = value.ToEnumOrDefault(expectedNumber);

            result.Should().Be(expectedNumber);
        }
    }
}