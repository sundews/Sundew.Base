// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueArrayTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Collections;

using FluentAssertions;
using Sundew.Base.Collections;
using Xunit;

public class ValueArrayTests
{
    [Fact]
    public void Equals_When_UsedWithInt_Then_LhsAndRhsShouldBeEqual()
    {
        ValueArray<int> lhs = System.Collections.Immutable.ImmutableArray.Create(4, 5, 6);
        ValueArray<int> rhs = System.Collections.Immutable.ImmutableArray.Create(4, 5, 6);

        ((object)lhs).Should().Be(rhs);
    }
}