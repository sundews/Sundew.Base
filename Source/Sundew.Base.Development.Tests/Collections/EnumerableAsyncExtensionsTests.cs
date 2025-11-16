// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableAsyncExtensionsTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Collections;

using System.Threading.Tasks;
using AwesomeAssertions;
using Sundew.Base.Collections;
using Xunit;

public class EnumerableAsyncExtensionsTests
{
    [Fact]
    public async Task ForEachAsync_When_UsingADelayProcessingItems_Then_ProcessingShouldAtLeastTakeTheItemsTimesTheDelay()
    {
        var numbers = new[] { 1, 2, 3, 4, 5 };
        var stopWatch = System.Diagnostics.Stopwatch.StartNew();
        var millisecondsDelay = 100;
        await numbers.ForEachAsync(async number =>
        {
            await Task.Delay(millisecondsDelay).ConfigureAwait(false);
        });

        stopWatch.Stop();
        stopWatch.ElapsedMilliseconds.Should().BeGreaterThanOrEqualTo(millisecondsDelay * numbers.Length);
    }
}