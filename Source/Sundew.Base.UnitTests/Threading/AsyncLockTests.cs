// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncLockTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Sundew.Base.Threading;
    using Sundew.Base.Threading.Jobs;
    using Xunit;

    public class AsyncLockTests
    {
        private const int ExpectedResult = 5;

        [Fact]
        public void TryLockAsync_When_CheckIsNotCalled_Then_LockNotConfirmedExceptionShouldBeThrown()
        {
            static async Task<int> LockTest()
            {
                var testee = new AsyncLock();
                using (await testee.TryLockAsync().ConfigureAwait(false))
                {
                    return ExpectedResult;
                }
            }

            var task = LockTest();
            Action test = () =>
            {
                var ignored = task.Result;
            };

            test.Should().Throw<LockNotConfirmedException>();
        }

        [Fact]
        public void TryLockAsync_When_CheckIsCalled_Then_ResultShouldBeExpectedResult()
        {
            static async Task<int> LockTest()
            {
                var testee = new AsyncLock();
                using (var lockResult = await testee.TryLockAsync().ConfigureAwait(false))
                {
                    if (lockResult)
                    {
                    }

                    return ExpectedResult;
                }
            }

            var task = LockTest();
            var result = task.Result;

            result.Should().Be(ExpectedResult);
        }

        [Fact]
        public async Task TryLockAsync_When_AddingAnItemTwiceForEachOfTwoThreads_Then_ResultShouldContainItemsWhereTwoConsecutiveItemsAreEqual()
        {
            var list = new List<int>();
            var asyncLock = new AsyncLock();
            var count = int.MaxValue;
            using (var testee = new ContinuousJob(
                async c =>
                {
                    using (var result = await asyncLock.TryLockAsync(c).ConfigureAwait(false))
                    {
                        if (result)
                        {
                            list.Add(count);
                            list.Add(count);
                            count--;
                        }
                    }
                }))
            {
                var startResult = testee.Start();
                for (int i = 0; i < 100; i++)
                {
                    var value = i;
                    using (var result = await asyncLock.TryLockAsync(startResult.Value).ConfigureAwait(false))
                    {
                        if (result)
                        {
                            list.Add(value);
                            list.Add(value);
                            Thread.Sleep(1);
                        }
                    }
                }
            }

            list.Contains(int.MaxValue).Should().BeTrue();
            AssertEachTwoItemShouldBeEqual(list);
        }

        private static void AssertEachTwoItemShouldBeEqual(List<int> list)
        {
            for (int i = 0; i < list.Count; i += 2)
            {
                list[i].Should().Be(list[i + 1]);
            }
        }
    }
}