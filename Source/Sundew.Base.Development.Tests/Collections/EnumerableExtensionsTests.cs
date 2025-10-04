// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensionsTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Collections;

using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions;
using Sundew.Base.Collections;
using Sundew.Base.Collections.Linq;
using Xunit;

public class EnumerableExtensionsTests
{
    private readonly IList<int> numberList = new List<int> { 1, 2, 4, 5, 6, 5, 6 };

    private readonly int[] numberArray;

    public EnumerableExtensionsTests()
    {
        this.numberArray = this.numberList.ToArray();
    }

    [Fact]
    public void WhereNotDefault_When_InputIsNullable_Then_ResultShouldBeExpectedResult()
    {
        var expectedResult = new[] { 1 };
        var list = new List<int?> { 0, null, 1 };

        var result = list.WhereNotDefault();

        result.Should().Equal(expectedResult);
    }

    [Fact]
    public void WhereNotDefault_Then_ResultShouldBeExpectedResult()
    {
        var expectedResult = new[] { 1 };
        var list = new List<int> { 0, 1 };

        var result = list.WhereNotDefault();

        result.Should().Equal(expectedResult);
    }

    [Fact]
    public void WhereNotNull_When_InputIsNullableValueType_Then_ResultShouldBeExpectedResult()
    {
        var expectedResult = new[] { 0, 1 };
        var list = new List<int?> { 0, null, 1 };

        var result = list.WhereNotNull();

        result.Should().Equal((IEnumerable<int>)expectedResult);
    }

    [Fact]
    public void WhereNotNull_Then_ResultShouldBeExpectedResult()
    {
        var expectedResult = new[] { "0", "1" };
        var list = new List<string?> { "0", null, "1" };

        var result = list.WhereNotNull();

        result.Should().Equal(expectedResult);
    }

    [Fact]
    public void ForEach_When_PassingEnumerableWithAction_Then_ActionIsCalledForAllElements()
    {
        var resultList = new List<int>();

        this.numberList.ForEachItem(x => resultList.Add(x + 1));

        resultList.ForEachItem((x, i) => x.Should().Be(this.numberList[i] + 1));
    }

    [Fact]
    public void ToReadOnlyCollection_When_PassingLinqQuery_Then_ResultShouldBeEquivalentToExpectedEnumerable()
    {
        var expectedEnumerable = this.numberList.Where(x => x > 2 && x < 6);

        var result = expectedEnumerable.ToReadOnlyCollection();

        result.Should().BeEquivalentTo(expectedEnumerable);
    }

    [Fact]
    public void ToReadOnlyCollection_When_PassingList_Then_ResultShouldContainTheSameList()
    {
        var result = this.numberList.ToReadOnlyCollection();

        result.Should().Equal(this.numberList);
    }

    [Fact]
    public void ToReadOnlyCollection_When_PassingArray_Then_ResultShouldContainTheSameArray()
    {
        var result = this.numberArray.ToReadOnlyCollection();

        result.Should().Equal(this.numberArray);
    }

    [Fact]
    public void ToReadOnlyCollection_When_PassingReadOnly_Then_ResultShouldBeTheSameIterable()
    {
        var expectedIterable = this.numberList.Where(x => x > 2 && x < 6).ToReadOnlyCollection();

        var result = expectedIterable.ToReadOnlyCollection();

        result.Should().Equal(expectedIterable);
    }

    [Fact]
    public void Concat_Then_ResultShouldBeExpectedResult()
    {
        var items1 = new List<int> { 1, 2, 3, 4 };
        var items2 = new[] { 5, 6, 7, 8 };
        var items3 = new List<int> { 9, 10, 11, 12 };
        var items4 = new List<int> { 13, 14, 15, 16 };
        var expectedResult = items1.Concat(items2).Concat(items3).Concat(items4);

        var result = items1.Concat(items2, items3, items4);

        result.Should().Equal(expectedResult);
    }

    [Fact]
    public void Concat_When_OneItemIsEnumerable_Then_ResultShouldBeExpectedResult()
    {
        var items1 = new List<int> { 1, 2, 3, 4 };
        var items2 = new[] { 5, 6, 7, 8 };
        var items3 = new List<int> { 9, 10, 11, 12 };
        var items4 = new List<int> { 13, 14, 15, 16 }.Where(_ => true);
        var expectedResult = items1.Concat(items2).Concat(items3).Concat(items4);

        var result = items1.Concat(items2, items3, items4);

        result.Should().Equal(expectedResult);
    }

    [Theory]
    [InlineData(1, new int[] { 1 })]
    [InlineData(null, new int[0])]
    public void ToReadOnlyList_When_PassingNullableStruct_Then_ResultShouldBeExpectedResult(int? value, int[] expectedResult)
    {
        var result = value.ToReadOnlyList();

        result.Should().Equal(expectedResult);
    }

    [Theory]
    [InlineData("hello", new string[] { "hello" })]
    [InlineData(null, new string[0])]
    public void ToReadOnlyList_When_PassingNullableClass_Then_ResultShouldBeExpectedResult(string? value, string[] expectedResult)
    {
        var result = value.ToReadOnlyList();

        result.Should().Equal(expectedResult);
    }

    [Theory]
    [InlineData("hello", new string[] { "hello" })]
    public void ToReadOnlyList_When_PassingNonNullableClass_Then_ResultShouldBeExpectedResult(string value, string[] expectedResult)
    {
        var result = value.ToReadOnlyList();

        result.Should().Equal(expectedResult);
    }
}