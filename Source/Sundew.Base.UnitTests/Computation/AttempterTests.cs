// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttempterTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Computation;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Sundew.Base.Computation;
using Xunit;

public class AttempterTests
{
    [Fact]
    public void Attempt_Then_ResultsShouldEqualExpectedResults()
    {
        var expectedResults = new[] { true, true, false };
        var testee = new Attempter(2);

        var results = new List<bool>();
        foreach (var ignored in expectedResults)
        {
            results.Add(testee.Attempt());
        }

        results.Should().Equal(expectedResults);
    }

    [Fact]
    public void Attempt_When_SucceedingAtFirstAttempt_Then_ResultShouldBeExpectedResult()
    {
        const int expectedResult = 5;
        var testee = new Attempter(2);

        var numberOfCalls = 0;
        var result = testee.Attempt(
            x =>
            {
                numberOfCalls = x.CurrentAttempt;
                return expectedResult;
            },
            ExceptionFilter.HandleAll());

        numberOfCalls.Should().Be(1);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedResult);
    }

    [Fact]
    public void Attempt_When_SucceedingAtSecondAttempt_Then_ResultShouldBeExpectedResult()
    {
        const int expectedResult = 5;
        var testee = new Attempter(2);

        var numberOfCalls = 0;
        var result = testee.Attempt(
            x =>
            {
                numberOfCalls = x.CurrentAttempt;
                if (numberOfCalls == 1)
                {
                    throw new InvalidOperationException("Something went right");
                }

                return expectedResult;
            },
            ExceptionFilter.HandleOnly(typeof(SystemException)));

        numberOfCalls.Should().Be(2);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedResult);
    }

    [Fact]
    public void Attempt_When_HandlingSystemExceptionAndInvalidOperationExceptionIsThrown_Then_AggregateExceptionShouldBeThrownAfter2Attempts()
    {
        var testee = new Attempter(2);

        var numberOfCalls = 0;
        var test = () => testee.Attempt(
            x =>
            {
                numberOfCalls = x.CurrentAttempt;
                throw new InvalidOperationException("Something went right");
            },
            ExceptionFilter.HandleOnly(typeof(SystemException)));

        test.Should().Throw<AggregateException>().Which.InnerExceptions.Should().Contain(x => x is InvalidOperationException);
        numberOfCalls.Should().Be(2);
    }

    [Fact]
    public void Attempt_When_HandlingAllExceptionsAndTaskCancelledExceptionIsThrown_Then_ItShouldBeThrownAfter1Attempt()
    {
        var testee = new Attempter(2);

        var numberOfCalls = 0;
        var test = () => testee.Attempt(
            x =>
            {
                numberOfCalls = x.CurrentAttempt;
                throw new TaskCanceledException("Something went right");
            },
            ExceptionFilter.HandleAll());

        test.Should().Throw<TaskCanceledException>();
        numberOfCalls.Should().Be(1);
    }

    [Fact]
    public void Attempt_When_HandlingAllExceptionOperationCancelledExceptionAndDifferentExceptionIsThrown_Then_AggregateExceptionShouldBeThrownAfter2Attempts()
    {
        var testee = new Attempter(2);

        var numberOfCalls = 0;
        var test = () => testee.Attempt(
            x =>
            {
                numberOfCalls = x.CurrentAttempt;
                throw new DivideByZeroException("Something went right");
            },
            ExceptionFilter.HandleAllExcept(typeof(InvalidOperationException)));

        test.Should().Throw<AggregateException>().Which.InnerExceptions.Should().Contain(x => x is DivideByZeroException);
        numberOfCalls.Should().Be(2);
    }
}