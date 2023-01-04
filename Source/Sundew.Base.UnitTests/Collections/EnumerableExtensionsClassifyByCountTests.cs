// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensionsClassifyByCountTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Collections;

using FluentAssertions;
using Sundew.Base.Collections;
using Xunit;

public class EnumerableExtensionsClassifyByCountTests
{
    [Fact]
    public void T()
    {
        var testee = new[] { 1 };

        var result = testee.ClassifyByCount();

        result.Should().BeOfType<Single<int>>();
    }
}