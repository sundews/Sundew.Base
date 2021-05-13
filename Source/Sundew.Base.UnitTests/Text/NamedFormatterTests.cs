// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamedFormatterTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Text
{
    using FluentAssertions;
    using Sundew.Base.Text;
    using Xunit;

    public class NamedFormatterTests
    {
        [Theory]
        [InlineData("Hello {Name}.", new string[] { "Name" }, new object[] { "World" }, "Hello World.")]
        public void T(string format, string[] names, object[] input, string expectedResult)
        {
            var testee = new NamedFormatter(names, System.StringComparison.OrdinalIgnoreCase);

            var result = testee.Format(format, input);

            result.Should().Be(expectedResult);
        }
    }
}
