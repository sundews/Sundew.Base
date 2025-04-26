// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensionsTakeIfAsyncTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Collections;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FluentAssertions;
using Sundew.Base.Collections;
using Xunit;

public class EnumerableExtensionsTakeIfAsyncTests
{
    public static readonly IEnumerable<object?[]> NullableIntTestCases = new object?[][]
    {
        [Array.Empty<int?>(), null, -1, false, null],
        [new int?[] { 1 }, null, 0, true, null],
        [Array.Empty<int?>(), 1, 0, false, null],
        [new int?[] { 1 }, 2, 0, true, 2],
        [Array.Empty<int?>(), 1, TooLateAddDelay, false, null],
        [new int?[] { 1 }, 2, TooLateAddDelay, false, null],
    };

    private const long DefaultTimeoutMilliseconds = 60;
    private const int TooLateAddDelay = 70;

    [Theory]
    [InlineData(new int[0], null, -1, false, 0)]
    [InlineData(new int[0], 1, 0, true, 1)]
    [InlineData(new int[] { 1 }, 2, 0, true, 1)]
    [InlineData(new int[0], 1, TooLateAddDelay, false, 0)]
    [InlineData(new int[] { 1 }, 2, TooLateAddDelay, true, 1)]
    public async Task FirstAsync_ForInt_When_UsingListWithInitialListPassedAndAddMaybeMade_Then_ResultIsExpectedResult(int[] initialList, int? adds, int addDelayMilliSeconds, bool expectedResult, int? expectedValue)
    {
        var testee = new List<int>(initialList);

        var taskResult = testee.FirstAsync(TimeSpan.FromMilliseconds(20));
        await TryAddToAfterDelay(testee, addDelayMilliSeconds, adds);

        var result = await taskResult;

        result.IsSuccess.Should().Be(expectedResult);
        result.Value.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData(new int[0], null, -1, false, 0)]
    [InlineData(new int[0], 1, 0, true, 1)]
    [InlineData(new int[] { 1 }, 2, 0, true, 1)]
    [InlineData(new int[0], 1, TooLateAddDelay, false, 0)]
    [InlineData(new int[] { 1 }, 2, TooLateAddDelay, true, 1)]
    public async Task FirstAsync_ForInt_When_UsingObservableCollectionWithInitialListPassedAndAddMaybeMadeAfterDelay_Then_ResultIsExpectedResult(int[] initialList, int? adds, int addDelayMilliSeconds, bool expectedResult, int? expectedValue)
    {
        var testee = new ObservableCollection<int>(initialList);

        var taskResult = testee.FirstAsync(TimeSpan.FromMilliseconds(10));
        await TryAddToAfterDelay(testee, addDelayMilliSeconds, adds);

        var result = await taskResult;

        result.IsSuccess.Should().Be(expectedResult);
        result.Value.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData(new string[0], null, -1, false, null)]
    [InlineData(new string[0], "1", 0, true, "1")]
    [InlineData(new string[] { "1" }, "2", 0, true, "1")]
    [InlineData(new string[0], "1", TooLateAddDelay, false, null)]
    [InlineData(new string[] { "1" }, "2", TooLateAddDelay, true, "1")]
    public async Task FirstAsync_ForString_When_UsingListWithInitialListPassedAndAddMaybeMade_Then_ResultIsExpectedResult(string[] initialList, string? adds, int addDelayMilliSeconds, bool expectedResult, string? expectedValue)
    {
        var testee = new List<string>(initialList);

        var taskResult = testee.FirstAsync(TimeSpan.FromMilliseconds(20));
        await TryAddToAfterDelay(testee, addDelayMilliSeconds, adds);

        var result = await taskResult;

        result.IsSuccess.Should().Be(expectedResult);
        result.Value.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData(new string[0], null, -1, false, null)]
    [InlineData(new string[0], "1", 0, true, "1")]
    [InlineData(new string[] { "1" }, "2", 0, true, "1")]
    [InlineData(new string[0], "1", TooLateAddDelay, false, null)]
    [InlineData(new string[] { "1" }, "2", TooLateAddDelay, true, "1")]
    public async Task FirstAsync_ForString_When_UsingObservableCollectionWithInitialListPassedAndAddMaybeMadeAfterDelay_Then_ResultIsExpectedResult(string[] initialList, string? adds, int addDelayMilliSeconds, bool expectedResult, string? expectedValue)
    {
        var testee = new ObservableCollection<string>(initialList);

        var taskResult = testee.FirstAsync(TimeSpan.FromMilliseconds(20));
        await TryAddToAfterDelay(testee, addDelayMilliSeconds, adds);

        var result = await taskResult;

        result.IsSuccess.Should().Be(expectedResult);
        result.Value.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData(new string[0], null, -1, false, null)]
    [InlineData(new string[0], "1", 0, false, null)]
    [InlineData(new string[] { "1" }, "2", 0, true, "2")]
    [InlineData(new string[0], "1", TooLateAddDelay, false, null)]
    [InlineData(new string[] { "1" }, "2", TooLateAddDelay, false, null)]
    public async Task SecondAsync_ForString_When_UsingObservableCollectionWithInitialListPassedAndAddMaybeMadeAfterDelay_Then_ResultIsExpectedResult(string[] initialList, string? adds, int addDelayMilliSeconds, bool expectedResult, string? expectedValue)
    {
        var testee = new ObservableCollection<string>(initialList);

        var taskResult = testee.SecondAsync(TimeSpan.FromMilliseconds(20));
        await TryAddToAfterDelay(testee, addDelayMilliSeconds, adds);

        var result = await taskResult;

        result.IsSuccess.Should().Be(expectedResult);
        result.Value.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData(new string?[0], null, 0, false, null)]
    [InlineData(new string?[] { "1" }, null, 0, true, null)]
    [InlineData(new string?[] { "1" }, "2", 0, true, "2")]
    [InlineData(new string?[0], "1", TooLateAddDelay, false, null)]
    [InlineData(new string?[] { "1" }, "2", TooLateAddDelay, false, null)]
    public async Task SecondAsync_ForStringOption_When_UsingObservableCollectionWithInitialListPassedAndAddMaybeMadeAfterDelay_Then_ResultIsExpectedResult(string[] initialList, string? adds, int addDelayMilliSeconds, bool expectedResult, string? expectedValue)
    {
        var testee = new ObservableCollection<string?>(initialList);

        var taskResult = testee.SecondAsync(TimeSpan.FromMilliseconds(DefaultTimeoutMilliseconds));
        await TryAddToAfterDelay(testee, addDelayMilliSeconds, adds);

        var result = await taskResult;

        result.IsSuccess.Should().Be(expectedResult);
        result.Value.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData(new int[0], null, -1, false, 0)]
    [InlineData(new int[0], 1, 0, false, 0)]
    [InlineData(new int[] { 1 }, 2, 0, true, 2)]
    [InlineData(new int[0], 1, TooLateAddDelay, false, 0)]
    [InlineData(new int[] { 1 }, 2, TooLateAddDelay, false, 0)]
    public async Task SecondAsync_ForInt_When_UsingObservableCollectionWithInitialListPassedAndAddMaybeMadeAfterDelay_Then_ResultIsExpectedResult(int[] initialList, int? adds, int addDelayMilliSeconds, bool expectedResult, int expectedValue)
    {
        var testee = new ObservableCollection<int>(initialList);

        var taskResult = testee.SecondAsync(TimeSpan.FromMilliseconds(DefaultTimeoutMilliseconds));
        await TryAddToAfterDelay(testee, addDelayMilliSeconds, adds);

        var result = await taskResult;

        result.IsSuccess.Should().Be(expectedResult);
        result.Value.Should().Be(expectedValue);
    }

    [Theory]
    [MemberData(nameof(NullableIntTestCases))]
    public async Task SecondAsync_ForIntOption_When_UsingObservableCollectionWithInitialListPassedAndAddMaybeMadeAfterDelay_Then_ResultIsExpectedResult(int?[] initialList, int? adds, int addDelayMilliSeconds, bool expectedResult, int? expectedValue)
    {
        var testee = new ObservableCollection<int?>(initialList);

        var taskResult = testee.SecondAsync(TimeSpan.FromMilliseconds(DefaultTimeoutMilliseconds));
        await TryAddToAfterDelay(testee, addDelayMilliSeconds, adds);

        var result = await taskResult;

        result.IsSuccess.Should().Be(expectedResult);
        result.Value.Should().Be(expectedValue);
    }

    [Fact]
    public async Task SecondAsync_ForStringOption_When_ObservableCollectionWithInitialListContainsNullAnd2ndItemAfterDelay_Then_ResultIsExpectedResult()
    {
        const string? expectedResult = "2";
        var testee = new ObservableCollection<string?>(new[] { null, null, "1", });

        var taskResult = testee.SecondAsync(x => x != null, TimeSpan.FromMilliseconds(DefaultTimeoutMilliseconds));
        await TryAddToAfterDelay(testee, 5, expectedResult);

        var result = await taskResult;

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(new string?[] { null, null }, new string[] { "1", "2", "3" }, "2")]
    public async Task SecondAsync_ForStringOption_When_ListWithInitialListContainsNullAnd2ndItemAfterDelay_Then_ResultIsExpectedResult(string?[] initialList, string?[] adds, string? expectedResult)
    {
        var testee = new List<string?>(initialList);

        var taskResult = testee.SecondAsync(x => x != null, TimeSpan.FromMilliseconds(DefaultTimeoutMilliseconds));
        await TryAddToAfterDelay(testee, 5, adds);

        var result = await taskResult;

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(new string?[] { null, null }, new string[] { "1", "2", "3" }, "2")]
    public async Task SecondAsync_ForStringOption_When_ObservableCollectionWithInitialListContainsNullAndItemsAddedAfterDelay_Then_ResultIsExpectedResult(string?[] initialList, string?[] adds, string? expectedResult)
    {
        var testee = new ObservableCollection<string?>(initialList);

        var taskResult = testee.SecondAsync(x => x != null, TimeSpan.FromMilliseconds(DefaultTimeoutMilliseconds));
        await TryAddToAfterDelay(testee, 5, adds);

        var result = await taskResult;

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(new string?[] { null, "1" }, new string?[] { null, "2", "3" }, new string?[] { null, "4", "5", "6" }, 50, false, new string[] { "1", "2", "3" })]
    [InlineData(new string?[] { null, "1" }, new string?[] { null, "2", "3" }, new string?[] { null, "4", "5", "6" }, 5, true, new string[] { "1", "2", "3", "4", "5" })]
    public async Task TakeIfAsync_ForStringOption_When_ObservableCollectionWithInitialListContainsNullAndMultipleRoundsOfAddingItemsAfterDelay_Then_ResultIsExpectedResult(string?[] initialList, string?[] firstAdds, string?[] secondAdds, int secondDelayMilliseconds, bool expectedResult, string[] expectedValue)
    {
        var testee = new ObservableCollection<string?>(initialList);

        var taskResult = testee.TakeIfAsync(x => x != null, 5, TimeSpan.FromMilliseconds(DefaultTimeoutMilliseconds));
        await TryAddToAfterDelay(testee, 5, firstAdds);
        await TryAddToAfterDelay(testee, secondDelayMilliseconds, secondAdds);

        var result = await taskResult;

        result.IsSuccess.Should().Be(expectedResult);
        if (result.TryGet(out var value, out var error))
        {
            value.Should().Equal(expectedValue);
        }
        else
        {
            error.Should().Equal(expectedValue);
        }
    }

    private static async Task TryAddToAfterDelay<TItem>(ICollection<TItem> testee, int addDelayMilliSeconds, params IEnumerable<object?> adds)
    {
        if (addDelayMilliSeconds >= 0)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(addDelayMilliSeconds)).ConfigureAwait(false);
            foreach (var add in adds)
            {
                if (add is null)
                {
                    testee.Add(default!);
                }
                else if (add is TItem item)
                {
                    testee.Add(item);
                }
            }
        }
    }
}