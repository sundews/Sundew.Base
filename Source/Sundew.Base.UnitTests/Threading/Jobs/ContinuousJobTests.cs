// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContinuousJobTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Threading.Jobs
{
    using System;
    using System.Threading;
    using FluentAssertions;
    using Sundew.Base.Threading.Jobs;
    using Xunit;

    public class ContinuousJobTests
    {
        [Fact]
        public void Stop_When_NotStarted_Then_ResultShouldBeSuccess()
        {
            using var testee = new ContinuousJob(_ => { });
            var result = testee.Stop();

            ((bool)result).Should().BeTrue();
        }

        [Fact]
        public void Start_When_ExceptionIsThrownAndHandled4Times_Then_TesteExceptionShouldContainThrownException()
        {
            using var resetEvent = new ManualResetEventSlim();
            var errorCounter = 0;
            using var testee = new ContinuousJob(
                _ => throw new InvalidOperationException(),
                (Exception _, ref bool handled) => handled = errorCounter++ < 4);

            testee.Start();
            testee.Wait();

            Thread.Sleep(1);

            testee.Exception!.InnerException.Should().BeOfType<InvalidOperationException>();
        }

        [Fact]
        public void Stop_When_ExceptionIsThrown_Then_ResultShouldContainThrownException()
        {
            using var resetEvent = new ManualResetEventSlim();
            using var testee = new ContinuousJob(_ =>
            {
                resetEvent.Set();
                throw new InvalidOperationException();
            });
            testee.Start();
            resetEvent.Wait();
            Thread.Sleep(1);

            var result = testee.Stop();

            result.Error!.InnerException.Should().BeOfType<InvalidOperationException>();
        }

        [Fact]
        public void Stop_Then_ResultShouldBeSuccess()
        {
            using var resetEvent = new ManualResetEventSlim();
            using var testee = new ContinuousJob(_ => resetEvent.Set());
            testee.Start();
            resetEvent.Wait();
            Thread.Sleep(1);

            var result = testee.Stop();

            ((bool)result).Should().BeTrue();
        }
    }
}