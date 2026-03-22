// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WithTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Primitives;

using AwesomeAssertions;
using AwesomeAssertions.Execution;

public class WithTests
{
    [Test]
    public void ImplicitConversionAndGetValueOrDefault_Then_ResultShouldBeExpectedResult()
    {
        const string expectedLastName = "LastName";
        const int expectedAge = 25;
        var expected = new Pass(new With<string?>(null, true), new With<string?>(expectedLastName, true), new With<int?>(expectedAge, true), new With<string?>(null, false));
        const string? expectedTitle = "Default";

        var pass = new Pass { Name = With.Default, LastName = expectedLastName, Age = expectedAge };

        using (new AssertionScope())
        {
            pass.Should().Be(expected);
            pass.Name.GetValueOrDefault(string.Empty).Should().BeNull();
            pass.LastName.GetValueOrDefault(string.Empty).Should().Be(expectedLastName);
            pass.Age.GetValueOrDefault(-1).Should().Be(expectedAge);
            pass.Title.GetValueOrDefault(expectedTitle).Should().Be(expectedTitle);
        }
    }

    public record Pass(With<string?> Name, With<string?> LastName, With<int?> Age, With<string?> Title)
    {
        public Pass()
            : this(default, default, default, default)
        {
        }
    }
}