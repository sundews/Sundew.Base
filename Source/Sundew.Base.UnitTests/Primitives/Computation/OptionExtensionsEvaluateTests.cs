// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionExtensionsEvaluateTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Primitives.Computation
{
    using FluentAssertions;
    using Sundew.Base.Primitives.Computation;
    using Xunit;

    public class OptionExtensionsEvaluateTests
    {
        [Theory]
        [InlineData(5, 10)]
        [InlineData(null, -1)]
        public void Evaluate_When_OperandIsNullableStructAndAlternativeIsStruct_Then_ResultShouldBeExpectedResult(int? lhsOption, int expectedResult)
        {
            var result = lhsOption.Evaluate(lhs => lhs + 5, () => -1);

            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("5", "55")]
        [InlineData(null, "-1")]
        public void Evaluate_When_OperandIsNullableStructAndAlternativeIsClass_Then_ResultShouldBeExpectedResult(string? lhsOption, string expectedResult)
        {
            var result = lhsOption.Evaluate(lhs => lhs + 5, () => "-1");

            result.Should().Be(expectedResult);
        }
    }
}