// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncLazyTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Threading
{
    using System.Collections.Generic;
    using FluentAssertions;
    using Sundew.Base.Threading;
    using Xunit;

    public class AsyncLazyTests
    {
        [Fact]
        public async void Await_Then_ResultShouldBeExpectedResult()
        {
            var expectedResult = 3;
            var asyncLazy = new AsyncLazy<int>(() => expectedResult);

            var result = await asyncLazy;

            result.Should().Be(expectedResult);
        }

        [Fact]
        public async void Value_Then_TaskResultShouldBeExpectedResult()
        {
            var expectedResult = new List<int> { 3 };
            var asyncLazy = new AsyncLazy<IList<int>, List<int>>(() => expectedResult);
            IAsyncLazy<IList<int>> asyncLazyBase = asyncLazy;

            var result = await asyncLazyBase;

            result.Should().BeSameAs(expectedResult);
        }
    }
}