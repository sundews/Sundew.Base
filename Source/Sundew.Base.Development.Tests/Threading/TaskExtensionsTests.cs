// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskExtensionsTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Threading;

using System;
using System.Threading.Tasks;
using AwesomeAssertions;
using Sundew.Base.Threading;

public class TaskExtensionsTests
{
    [Test]
    public void AsyncJoin_When_TaskCompletes_Then_ResultIsReturned()
    {
        var result = Task.FromResult(42).AsyncJoin();

        result.Should().Be(42);
    }

    [Test]
    public void AsyncJoin_When_TaskThrows_Then_ExceptionIsRethrown()
    {
        const string? expectedMessage = "Failed";
        var task = Task.FromException<int>(new InvalidOperationException(expectedMessage));

        var action = () => task.AsyncJoin();

        action.Should().Throw<InvalidOperationException>().WithMessage(expectedMessage);
    }

    [Test]
    public void AsyncJoin_When_TaskCompletesWithoutResult_Then_TaskCompletes()
    {
        var executed = false;
        var task = Task.Run(async () =>
        {
            await Task.Delay(10).ConfigureAwait(false);
            executed = true;
        });

        task.AsyncJoin();

        executed.Should().BeTrue();
    }

    [Test]
    public void AsyncJoin_When_FuncTaskCompletes_Then_ResultIsReturned()
    {
        Func<Task<int>> func = async () =>
        {
            await Task.Delay(10).ConfigureAwait(false);
            return 42;
        };

        var result = func.AsyncJoin();

        result.Should().Be(42);
    }

    [Test]
    public void AsyncJoin_When_FuncTaskThrows_Then_ExceptionIsRethrown()
    {
        const string? expectedMessage = "Failed";
        Func<Task<int>> func = () => Task.FromException<int>(new InvalidOperationException(expectedMessage));

        var action = () => func.AsyncJoin();

        action.Should().Throw<InvalidOperationException>().WithMessage(expectedMessage);
    }

    [Test]
    public void AsyncJoin_When_FuncTaskCompletesWithoutResult_Then_TaskCompletes()
    {
        var executed = false;
        Func<Task> func = async () =>
        {
            await Task.Delay(10).ConfigureAwait(false);
            executed = true;
        };

        func.AsyncJoin();

        executed.Should().BeTrue();
    }
}
