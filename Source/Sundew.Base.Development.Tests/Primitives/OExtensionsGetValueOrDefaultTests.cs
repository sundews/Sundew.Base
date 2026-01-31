// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OExtensionsGetValueOrDefaultTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Primitives;

using AwesomeAssertions;

public class OExtensionsGetValueOrDefaultTests
{
    [Test]
    [Arguments(5, 10)]
    [Arguments(null, -1)]
    public void GetValueOrDefault_When_OperandIsNullableStructAndAlternativeIsStruct_Then_ResultShouldBeExpectedResult(int? lhsOption, int expectedResult)
    {
        var result = lhsOption.GetValueOrDefault(lhs => lhs + 5, () => -1);

        result.Should().Be(expectedResult);
    }

    [Test]
    [Arguments("5", "55")]
    [Arguments(null, "-1")]
    public void GetValueOrDefault_When_OperandIsNullableStructAndAlternativeIsClass_Then_ResultShouldBeExpectedResult(string? lhsOption, string expectedResult)
    {
        var result = lhsOption.GetValueOrDefault(lhs => lhs + 5, () => "-1");

        result.Should().Be(expectedResult);
    }
}