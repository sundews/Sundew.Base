// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OExtensionsToOptionTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Primitives;

using FluentAssertions;
using Sundew.Base.Primitives;
using Xunit;

public class OExtensionsToOptionTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetValueOrDefault_When_OperandIsNullableStructAndAlternativeIsStruct_Then_ResultShouldBeExpectedResult(bool isSuccess)
    {
        var option = isSuccess.ToOption(3);
        option.HasValue.Should().Be(isSuccess);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetValueOrDefault_When_OperandIsNullableStructAndAlternativeIsStruct_Then_ResultShouldBeExpectedResult2(bool isSuccess)
    {
        var option = isSuccess.ToOption("text");
        option.HasValue().Should().Be(isSuccess);
    }
}