// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CharExtensionsTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Text;

using AwesomeAssertions;
using Sundew.Base.Text;
using Xunit;

public class CharExtensionsTests
{
    [Fact]
    public void Repeat_Then_ResultShouldContainRepeatedCharacters()
    {
        var result = ' '.Repeat(5);

        result.Should().Be("     ");
    }
}