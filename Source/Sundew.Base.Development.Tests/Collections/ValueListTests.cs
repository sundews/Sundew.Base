// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueListTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Collections;

using AwesomeAssertions;
using Sundew.Base.Collections.Immutable;
using Xunit;

public class ValueListTests
{
    [Fact]
    public void Equals_When_UsedWithInt_Then_LhsAndRhsShouldBeEqual()
    {
        ValueList<int> lhs = System.Collections.Immutable.ImmutableList.Create(4, 5, 6);
        ValueList<int> rhs = System.Collections.Immutable.ImmutableList.Create(4, 5, 6);

        ((object)lhs).Should().Be(rhs);
    }
}