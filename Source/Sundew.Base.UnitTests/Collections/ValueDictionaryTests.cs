﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueDictionaryTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Collections;

using System.Collections.Immutable;
using AwesomeAssertions;
using Sundew.Base.Collections.Immutable;
using Xunit;

public class ValueDictionaryTests
{
    [Fact]
    public void Equals_Then_LhsAndRhsShouldBeEqual()
    {
        ValueDictionary<int, int> lhs = ImmutableDictionary.Create<int, int>().Add(1, 2).Add(3, 4);
        ValueDictionary<int, int> rhs = ImmutableDictionary.Create<int, int>().Add(1, 2).Add(3, 4);

        ((object)lhs).Should().Be(rhs);
    }
}