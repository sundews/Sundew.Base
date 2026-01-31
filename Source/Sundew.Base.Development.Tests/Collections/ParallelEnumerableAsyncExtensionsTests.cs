// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParallelEnumerableAsyncExtensionsTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Collections;

using System.Threading.Tasks;
using AwesomeAssertions;
using Sundew.Base.Collections;
using Sundew.Base.Collections.Concurrent;

public class ParallelEnumerableAsyncExtensionsTests
{
    [Test]
    public async Task ForEachAsync_When_UsingADelayProcessingItems_Then_ProcessingShouldTakeLessThanTheItemsTimesTheDelay()
    {
        var millisecondsDelay = 500;
        var numbers = new[] { 1, 2, 3, 4, 5 };
        var results = new ConcurrentList<int>();
        await numbers.ForEachAsync(
            Parallelism.Default,
            async number =>
            {
                await Task.Delay(millisecondsDelay).ConfigureAwait(false);
                results.Add(number);
            });

        results.Should().BeEquivalentTo(numbers);
    }

    [Test]
    public async Task SelectAsync_When_UsingADelayProcessingItems_Then_AllItemsShouldBeProcessed()
    {
        var millisecondsDelay = 500;
        var numbers = new[] { 1, 2, 3, 4, 5 };
        var result = await numbers.SelectAsync(
            Parallelism.Default,
            async number =>
            {
                await Task.Delay(millisecondsDelay).ConfigureAwait(false);
                return ++number;
            });

        result.Should().Equal(2, 3, 4, 5, 6);
    }
}