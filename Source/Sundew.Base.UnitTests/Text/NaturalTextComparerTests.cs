// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NaturalTextComparerTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Text;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using FluentAssertions;
using Sundew.Base.Text;
using Sundew.Base.UnitTests.Infrastructure;
using Sundew.Test.Infrastructure.Text;
using Xunit;

public class NaturalTextComparerTests
{
    private readonly NaturalTextComparer testee;

    public NaturalTextComparerTests()
    {
        this.testee = new NaturalTextComparer(StringComparison.CurrentCulture);
    }

    public static IEnumerable<object[]> UnsortedCollections()
    {
        yield return new object[]
        {
            new[] { "foo", "foobar" },
            new[] { "foo", "foobar" },
        };
        yield return new object[]
        {
            new[] { "foobar", "foo" },
            new[] { "foo", "foobar" },
        };
        yield return new object[]
        {
            new[] { "10", "2" },
            new[] { "2", "10" },
        };
        yield return new object[]
        {
            new[] { "0010", "2" },
            new[] { "2", "0010" },
        };
        yield return new object[]
        {
            new[] { "1000", "2" },
            new[] { "2", "1000" },
        };
        yield return new object[]
        {
            new[] { "foo10", "foo2" },
            new[] { "foo2", "foo10" },
        };
        yield return new object[]
        {
            new[] { "10foo2", "2foo2" },
            new[] { "2foo2", "10foo2" },
        };
        yield return new object[]
        {
            new[] { "foo (10)", "foo (2)" },
            new[] { "foo (2)", "foo (10)" },
        };
        yield return new object[]
        {
            new[] { "foo 10 bar 10 x", "foo 10 bar 2 x" },
            new[] { "foo 10 bar 2 x", "foo 10 bar 10 x" },
        };
        yield return new object[]
        {
            new[] { "\"1\"", "\"JC\"", "1", "2", "21", "3" },
            new[] { "\"1\"", "\"JC\"", "1", "2", "3", "21" },
        };
    }

    [Theory]
    [InlineData(new string[] { null!, null! }, new string[] { null!, null!, })]
    [InlineData(new[] { null, "b" }, new[] { null, "b", })]
    [InlineData(new[] { "b", null }, new[] { null, "b", })]
    [InlineData(new[] { null, "" }, new[] { null, "", })]
    [InlineData(new[] { "", null }, new[] { null, "", })]
    [InlineData(new[] { "", "b" }, new[] { "", "b", })]
    [InlineData(new[] { "b", "" }, new[] { "", "b", })]
    [InlineData(new[] { "a", "b", "c" }, new[] { "a", "b", "c" })]
    [InlineData(new[] { "c", "b", "a" }, new[] { "a", "b", "c" })]
    [InlineData(new[] { "b", "a", "a" }, new[] { "a", "a", "b" })]
    [InlineData(new[] { "aa", "b", "a" }, new[] { "a", "b", "aa" })]
    [InlineData(new[] { "3", "12", "1" }, new[] { "1", "3", "12" })]
    [InlineData(new[] { "text3text", "text3atext", "text1" }, new[] { "text1", "text3atext", "text3text" })]
    [InlineData(new[] { "text3text", "text3atext" }, new[] { "text3atext", "text3text" })]
    [InlineData(new[] { "text4text", "text3atext" }, new[] { "text3atext", "text4text" })]
    [InlineData(new[] { "text3", "text123", "text1" }, new[] { "text1", "text3", "text123" })]
    [InlineData(new[] { "text3text", "text123text", "text1text" }, new[] { "text1text", "text3text", "text123text" })]
    [InlineData(new[] { "text3.4text", "text3.3text", "text3.2text" }, new[] { "text3.2text", "text3.3text", "text3.4text" })]
    [InlineData(new[] { "text3,4text", "text3,3text", "text3,2text" }, new[] { "text3,2text", "text3,3text", "text3,4text" })]
    [InlineData(new[] { "text3,4text", "text2,3text", "text1,2text" }, new[] { "text1,2text", "text2,3text", "text3,4text" })]
    [InlineData(new[] { "3,4text", "3,3text", "3,2text" }, new[] { "3,2text", "3,3text", "3,4text" })]
    [InlineData(new[] { "002", "0003", "01" }, new[] { "01", "002", "0003" })]
    [InlineData(new[] { "1text002", "1text0", "1text1" }, new[] { "1text0", "1text1", "1text002" })]
    [InlineData(new[] { "1a2b3c", "1a2b3c" }, new[] { "1a2b3c", "1a2b3c" })]
    [InlineData(new[] { "1", "a" }, new[] { "1", "a" })]
    [InlineData(new[] { "a", "1" }, new[] { "1", "a" })]
    [InlineData(new[] { "a1", "1a" }, new[] { "1a", "a1" })]
    [InlineData(new[] { "1a", "a1" }, new[] { "1a", "a1" })]
    [InlineData(new[] { "2", "1000" }, new[] { "2", "1000" })]
    [InlineData(new[] { "1000", "2" }, new[] { "2", "1000" })]
    [InlineData(new[] { "a12", "ab" }, new[] { "a12", "ab" })]
    [InlineData(new[] { "ab", "a12" }, new[] { "a12", "ab" })]
    [InlineData(new[] { "bb", "a12" }, new[] { "a12", "bb" })]
    [InlineData(new[] { "123", "ab" }, new[] { "123", "ab" })]
    [InlineData(new[] { "123a", "ab" }, new[] { "123a", "ab" })]
    [InlineData(new[] { "ab", "123" }, new[] { "123", "ab" })]
    [InlineData(new[] { "ab", "123a" }, new[] { "123a", "ab" })]
    [InlineData(new[] { "123", "123" }, new[] { "123", "123" })]
    [InlineData(new[] { "456", "123" }, new[] { "123", "456" })]
    [InlineData(new[] { "456", "1231" }, new[] { "456", "1231" })]
    [InlineData(new[] { "1231", "456" }, new[] { "456", "1231" })]
    [InlineData(new[] { "123ab456", "123ab123" }, new[] { "123ab123", "123ab456" })]
    [InlineData(new[] { "123ab1234", "123ab123" }, new[] { "123ab123", "123ab1234" })]
    [InlineData(new[] { "abaa", "aba" }, new[] { "aba", "abaa" })]
    [InlineData(new[] { "abcaa", "abca" }, new[] { "abca", "abcaa" })]
    [InlineData(new[] { "aba", "abaa" }, new[] { "aba", "abaa" })]
    [InlineData(new[] { "abca", "abcaa" }, new[] { "abca", "abcaa" })]

    public void Sort_Then_InputShouldBeExpectedResult(string[] input, string[] expectedResult)
    {
        using (TemporarilySet.Create(new CultureInfo("da-DK"), ci => Thread.CurrentThread.CurrentCulture = ci, () => Thread.CurrentThread.CurrentCulture))
        {
            Array.Sort(input, this.testee);
        }

        input.Should().Equal(expectedResult);
    }

    [Theory(Skip = "Windows and DK culture only")]
    [InlineData(new string[] { null!, null! }, new string[] { null!, null!, })]
    [InlineData(new[] { null, "b" }, new[] { null, "b", })]
    [InlineData(new[] { "b", null }, new[] { null, "b", })]
    [InlineData(new[] { null, "" }, new[] { null, "", })]
    [InlineData(new[] { "", null }, new[] { null, "", })]
    [InlineData(new[] { "", "b" }, new[] { "", "b", })]
    [InlineData(new[] { "b", "" }, new[] { "", "b", })]
    [InlineData(new[] { "a", "b", "c" }, new[] { "a", "b", "c" })]
    [InlineData(new[] { "aa", "b", "a" }, new[] { "a", "b", "aa" })]
    [InlineData(new[] { "3", "12", "1" }, new[] { "1", "3", "12" })]
    [InlineData(new[] { "text3", "text123", "text1" }, new[] { "text1", "text3", "text123" })]
    [InlineData(new[] { "text3text", "text123text", "text1text" }, new[] { "text1text", "text3text", "text123text" })]
    [InlineData(new[] { "text3.4text", "text3.3text", "text3.2text" }, new[] { "text3.2text", "text3.3text", "text3.4text" })]
    [InlineData(new[] { "text3,4text", "text3,3text", "text3,2text" }, new[] { "text3,2text", "text3,3text", "text3,4text" })]
    [InlineData(new[] { "3,4text", "3,3text", "3,2text" }, new[] { "3,2text", "3,3text", "3,4text" })]
    [InlineData(new[] { "002", "0003", "01" }, new[] { "01", "002", "0003" })]
    [InlineData(new[] { "1text002", "1text0", "1text1" }, new[] { "1text0", "1text1", "1text002" })]
    [InlineData(new[] { "1a2b3c", "1a2b3c" }, new[] { "1a2b3c", "1a2b3c" })]
    [InlineData(new[] { "1", "a" }, new[] { "1", "a" })]
    [InlineData(new[] { "a", "1" }, new[] { "1", "a" })]
    [InlineData(new[] { "a1", "1a" }, new[] { "1a", "a1" })]
    [InlineData(new[] { "1a", "a1" }, new[] { "1a", "a1" })]
    [InlineData(new[] { "2", "1000" }, new[] { "2", "1000" })]
    [InlineData(new[] { "1000", "2" }, new[] { "2", "1000" })]
    [InlineData(new[] { "a12", "ab" }, new[] { "a12", "ab" })]
    [InlineData(new[] { "ab", "a12" }, new[] { "a12", "ab" })]
    [InlineData(new[] { "123", "ab" }, new[] { "123", "ab" })]
    [InlineData(new[] { "ab", "123" }, new[] { "123", "ab" })]
    [InlineData(new[] { "123", "123" }, new[] { "123", "123" })]
    [InlineData(new[] { "456", "123" }, new[] { "123", "456" })]
    [InlineData(new[] { "456", "1231" }, new[] { "456", "1231" })]
    [InlineData(new[] { "1231", "456" }, new[] { "456", "1231" })]
    [InlineData(new[] { "abaa", "aba" }, new[] { "aba", "abaa" })]
    [InlineData(new[] { "aba", "abaa" }, new[] { "aba", "abaa" })]

    public void Sort_Then_InputShouldBeExpectedResult2(string[] input, string[] expectedResult)
    {
        using (TemporarilySet.Create(new CultureInfo("da-DK"), ci => Thread.CurrentThread.CurrentCulture = ci, () => Thread.CurrentThread.CurrentCulture))
        {
            Array.Sort(input, new Win32NaturalTextComparer());
        }

        input.Should().Equal(expectedResult);
    }
}