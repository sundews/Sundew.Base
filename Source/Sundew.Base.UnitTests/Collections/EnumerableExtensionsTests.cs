// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensionsTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Collections
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Sundew.Base.Collections;
    using Sundew.Base.Collections.Internal;
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
        public void ForEach_When_PassingEnumerableWithAction_Then_ActionIsCalledForAllElements()
        {
            var resultList = new List<int>();

            this.numberList.ForEach(x => resultList.Add(x + 1));

            resultList.ForEach((x, i) => x.Should().Be(this.numberList[i] + 1));
        }

        [Fact]
        public void ToReadOnly_When_PassingLinqQuery_Then_ResultShouldBeEquivalentToExpectedEnumerable()
        {
            var expectedEnumerable = this.numberList.Where(x => x > 2 && x < 6);

            var result = expectedEnumerable.ToReadOnly();

            result.Should().BeEquivalentTo(expectedEnumerable);
        }

        [Fact]
        public void ToReadOnly_When_PassingList_Then_ResultShouldContainTheSameList()
        {
            var result = this.numberList.ToReadOnly();

            result.Should().Equal(this.numberList);
        }

        [Fact]
        public void ToReadOnly_When_PassingArray_Then_ResultShouldContainTheSameArray()
        {
            var result = this.numberArray.ToReadOnly();

            result.Should().Equal(this.numberArray);
        }

        [Fact]
        public void ToReadOnly_When_PassingReadOnly_Then_ResultShouldBeTheSameIterable()
        {
            var expectedIterable = this.numberList.Where(x => x > 2 && x < 6).ToReadOnly();

            var result = expectedIterable.ToReadOnly();

            result.Should().Equal(expectedIterable);
        }

        [Fact]
        public void ToListIfNeeded_When_PassingEnumerable_Then_ResultShouldNotBeExpectedEnumerable()
        {
            var expectedEnumerable = this.numberList.Where(x => x > 2 && x < 6);

            var result = expectedEnumerable.ToListIfNeeded();

            result.Should().NotBeSameAs(expectedEnumerable);
        }

        [Fact]
        public void ToListIfNeeded_When_PassingEnumerable_Then_ResultShouldBeEquivalentToExpectedEnumerable()
        {
            var expectedEnumerable = this.numberList.Where(x => x > 2 && x < 6);

            var result = expectedEnumerable.ToListIfNeeded();

            result.Should().BeEquivalentTo(expectedEnumerable);
        }

        [Fact]
        public void ToListIfNeeded_When_PassingEnumerable_Then_ResultShouldBeIListOfInt32()
        {
            var enumerable = this.numberList.Where(x => x > 2 && x < 6);

            var result = enumerable.ToListIfNeeded();

            result.Should().BeAssignableTo<IList<int>>();
        }

        [Fact]
        public void ToListIfNeeded_When_PassingList_Then_ResultShouldBeTheSameList()
        {
            var enumerable = this.numberList;

            var result = enumerable.ToListIfNeeded();

            result.Should().Equal(enumerable);
        }

        [Fact]
        public void SelectFromNonGeneric_Then_ResultShouldContainTheSelectedItems()
        {
            var items = new List<int> { 5, 7, 9, 3 };

            var result = items.SelectFromNonGeneric(item => (int)item);

            result.Should().BeEquivalentTo(items);
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
    }
}