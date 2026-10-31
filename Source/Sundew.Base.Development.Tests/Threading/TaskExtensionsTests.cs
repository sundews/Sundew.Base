// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskExtensionsTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Threading;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Microsoft.VisualStudio.Threading;
using Sundew.Base.Threading;

[SuppressMessage("Usage", "VSTHRD012:Provide JoinableTaskFactory where allowed", Justification = "Tests the overloads without a JoinableTaskFactory.")]
public class TaskExtensionsTests
{
    [Test]
    public void RunAndJoin_When_FuncTaskCompletes_Then_ResultIsReturned()
    {
        var result = Task.RunAndJoin(async () =>
        {
            await Task.Delay(10).ConfigureAwait(false);
            return 42;
        });

        result.Should().Be(42);
    }

    [Test]
    public void RunAndJoin_When_FuncTaskThrows_Then_ExceptionIsRethrown()
    {
        const string expectedMessage = "Failed";

        var action = () => Task.RunAndJoin(() => Task.FromException<int>(new InvalidOperationException(expectedMessage)));

        action.Should().Throw<InvalidOperationException>().WithMessage(expectedMessage);
    }

    [Test]
    public void RunAndJoin_When_FuncTaskCompletesWithoutResult_Then_TaskCompletes()
    {
        var executed = false;

        Task.RunAndJoin(async () =>
        {
            await Task.Delay(10).ConfigureAwait(false);
            executed = true;
        });

        executed.Should().BeTrue();
    }

    [Test]
    public void RunAndJoin_When_MainThreadContextIsRegistered_Then_ItIsUsed()
    {
        using var context = new JoinableTaskContext();
        MainThreadContext.Register(context);
        try
        {
            var wasWithinJoinableTask = Task.RunAndJoin(() => Task.FromResult(context.IsWithinJoinableTask));

            wasWithinJoinableTask.Should().BeTrue();
        }
        finally
        {
            MainThreadContext.Unregister(context);
        }
    }

    [Test]
    public void RunAndJoin_When_CalledOnThreadWithUnpumpedSynchronizationContext_Then_ContinuationRunsOnCallingThreadWithoutDeadlocking()
    {
        var callingThreadId = -1;
        var continuationThreadId = 0;
        Exception? exception = null;
        var thread = new Thread(() =>
        {
            try
            {
                // Like a blocked UI thread: Anything posted to this context is never processed.
                SynchronizationContext.SetSynchronizationContext(new SingleThreadedSynchronizationContext());
                callingThreadId = Environment.CurrentManagedThreadId;
                continuationThreadId = Task.RunAndJoin(async () =>
                {
                    await Task.Delay(10);
                    return Environment.CurrentManagedThreadId;
                });
            }
            catch (Exception e)
            {
                exception = e;
            }
        });

        thread.Start();
        var hasCompleted = thread.Join(TimeSpan.FromSeconds(10));

        hasCompleted.Should().BeTrue();
        exception.Should().BeNull();
        continuationThreadId.Should().Be(callingThreadId);
    }
}
