// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamedFormatStringTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Text;

using System;
using System.Collections.Generic;
using AwesomeAssertions;
using Sundew.Base.Text;
using Xunit;

public class NamedFormatStringTests
{
    [Theory]
    [InlineData("Hello {Name}.", new string[] { "Name" }, new object[] { "World" }, "Hello World.")]
    [InlineData("{Greeting} how are you?", new string[] { "Greeting" }, new object[] { "Hello" }, "Hello how are you?")]
    [InlineData("What do you mean{Punctuation}", new string[] { "Punctuation" }, new object[] { "?" }, "What do you mean?")]
    [InlineData("Don't insert here: {{Escaped}}, but here: {Insertion}", new string[] { "Insertion" }, new object[] { "Inserted" }, "Don't insert here: {Escaped}, but here: Inserted")]
    [InlineData("Multiple insertions: {One}, {Two}", new string[] { "One", "Two" }, new object[] { "first", "second" }, "Multiple insertions: first, second")]
    [InlineData("Formatted number insertions: {One:N2}, {Two:N4}", new string[] { "One", "Two" }, new object[] { 1.23456, 9.87654321 }, "Formatted number insertions: 1.23, 9.8765")]
    [InlineData("Accept normal indices: {0:N2}, {1:N4}, {One:N3}, {Two:N2}", new string[] { "One", "Two" }, new object[] { 1.23456, 9.87654321 }, "Accept normal indices: 1.23, 9.8765, 1.235, 9.88")]
    [InlineData("Padding: {0,-10:N2}, {1,10:N4}, {One,-10:N3}, {Two,10:N2}", new string[] { "One", "Two" }, new object[] { 1.23456, 9.87654321 }, "Padding: 1.23      ,     9.8765, 1.235     ,       9.88")]
    public void FormatInvariant_Then_ResultShouldBeExpectedResult(string format, string[] names, object[] input, string expectedResult)
    {
        var testee = NamedFormatString.CreateInvariant(format, names);

        var result = testee.Format(input);

        result.Should().Be(expectedResult);
    }

    [Fact]
    public void ImplicitFormatOperator_Then_ResultShouldBeExpectedResult()
    {
        var testee = NamedFormatString.CreateInvariant("{One}, {Two}, {0}", ["One", "Two"]);

        string result = testee;

        result.Should().Be("{0}, {1}, {0}");
        testee.FormatNames.Should().Equal(new[] { ("One", 0), ("Two", 1) });
    }

    [Fact]
    public void GetNullArguments_Then_NullNamesShouldBeExpectedResult()
    {
        var expectedResult = new List<(string Name, int Index)>() { ("Two", 1) };
        var testee = NamedFormatString.CreateInvariant("{One}, {Two}, {0}", ["One", "Two"]);

        var result = testee.GetNullArguments(1, null);

        result.Should().Equal(expectedResult);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void FormatInvariant_When_PassingNullArray_Then_ArgumentNullExceptionShouldBeThrown()
    {
        var testee = NamedFormatString.CreateInvariant("{One}", ["One"]);
        object[]? array = null;

#pragma warning disable CS8604 // Possible null reference argument.
        Action act = () => testee.Format(array);
#pragma warning restore CS8604 // Possible null reference argument.

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void FormatInvariant_Then_ResultShouldBeStringFormattedWithExpectedResult()
    {
        const string expectedResult = "$, \", 4";

        var result = NamedFormatString.FormatInvariant("{Dollar}, {DQ}, {0}", NamedValues.Create(("Dollar", "$"), ("DQ", "\"")), 4);
        result.TryGet(out var content, out var _);

        content!.Result.Should().Be(expectedResult);
    }

    [Fact]
    public void FormatInvariant_Then_ContentShouldBeExpectedResult()
    {
        const string expectedResult = "$, \", 4";

        var result = NamedFormatString.FormatInvariant("{Dollar}, {DQ}, {0}", NamedValues.Create(("Dollar", "$"), ("DQ", "\"")), 4);

        result.TryGet(out var content, out var _);

        content!.Result.Should().Be(expectedResult);
    }

    [Fact]
    public void FormatInvariant_When_PassingStringArray_Then_ContentShouldBeExpectedResult()
    {
        const string expectedResult = "$, \", 4";
        var arguments = new[] { "4" };

        var result = NamedFormatString.FormatInvariant("{Dollar}, {DQ}, {0}", NamedValues.Create(("Dollar", "$"), ("DQ", "\"")), arguments);

        result.TryGet(out var content, out var _);

        content!.Result.Should().Be(expectedResult);
    }

    [Fact]
    public void FormatInvariant_When_AllNamesAreFound_Then_ContentShouldBeExpectedResult()
    {
        const string expectedResult = "$, \", 4";
        var arguments = new[] { "4" };

        var result = NamedFormatString.FormatInvariant("{Dollar}, {DQ}, {0}", (name, _) => name switch { "Dollar" => R.SuccessOption("$"), "DQ" => R.SuccessOption("\""), _ => R.SuccessOption(default(string?)), }, arguments);

        var content = result.Value!;

        content.Result.Should().Be(expectedResult);
        content.HasNulls.Should().BeFalse();
    }

    [Fact]
    public void FormatInvariant_When_SomeNameMapToNull_Then_ContentShouldBeExpectedResult()
    {
        const string expectedResult = "$, , 4";
        var arguments = new[] { "4" };

        var result = NamedFormatString.FormatInvariant("{Dollar}, {DQ}, {0}", (name, _) => name switch { "Dollar" => R.SuccessOption("$"), _ => R.SuccessOption(default(string?)), }, arguments);

        var content = result.Value!;

        content.Result.Should().Be(expectedResult);
        content.HasNulls.Should().BeTrue();
    }

    [Fact]
    public void FormatInvariant_When_OneNameCannotBeFound_Then_ContentShouldBeExpectedResult()
    {
        var arguments = new[] { "4" };

        var result = NamedFormatString.FormatInvariant("{Dollar}, {DQ}, {0}", (name, _) => name switch { "Dollar" => R.SuccessOption("$"), _ => R.Error(), }, arguments);

        var content = result.Error!;

        content.Names.Should().Equal(["DQ"]);
    }
}