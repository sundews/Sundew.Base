// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttempterTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Primitives.Computation
{
    using System.Collections.Generic;
    using FluentAssertions;
    using Sundew.Base.Primitives.Computation;
    using Xunit;

    public class AttempterTests
    {
        [Fact]
        public void Attempt_Then_ResultsShouldEqualExpectedResults()
        {
            var expectedResults = new[] { true, true, false };
            var testee = new Attempter(2);

            var results = new List<bool>();
            foreach (var ignored in expectedResults)
            {
                results.Add(testee.Attempt());
            }

            results.Should().Equal(expectedResults);
        }
    }
}