// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeferredTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Threading;

using System;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Sundew.Base.Threading;

public class DeferredTests
{
    [Test]
    public async Task FromResult_When_Awaited_Then_ResultShouldBeExpectedValue()
    {
        var testee = Deferred.FromResult(42);

        var result = await testee;

        result.Should().Be(42);
    }

    [Test]
    public void FromResult_When_GettingAwaiter_Then_AwaiterShouldBeCompletedSynchronously()
    {
        var testee = Deferred.FromResult(42);

        var result = testee.GetAwaiter();

        result.IsCompleted.Should().BeTrue();
        result.GetResult().Should().Be(42);
    }

    [Test]
    public async Task Default_When_Awaited_Then_ResultShouldBeDefault()
    {
        var testee = default(Deferred<int>);

        var result = await testee;

        result.Should().Be(0);
    }

    [Test]
    public async Task FromTask_When_TaskHasNotStarted_Then_AwaitStartsTheTaskOnDemand()
    {
        var task = new Task<int>(() => 42);
        var testee = Deferred.FromTask(task);

        task.Status.Should().Be(TaskStatus.Created);

        var result = await testee;

        result.Should().Be(42);
        task.Status.Should().Be(TaskStatus.RanToCompletion);
    }

    [Test]
    public async Task FromTask_When_TaskIsAlreadyStarted_Then_AwaitUsesExistingTask()
    {
        var task = Task.Run(() => 42);
        var testee = Deferred.FromTask(task);

        var result = await testee;

        result.Should().Be(42);
    }

    [Test]
    public async Task FromTask_When_TaskIsFaulted_Then_AwaitShouldThrow()
    {
        var testee = Deferred.FromTask(Task.FromException<int>(new InvalidOperationException("Failed")));

        Func<Task> act = async () => await testee;

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Failed");
    }

    [Test]
    public async Task FromFunc_When_Awaited_Then_FunctionRunsOnDemand()
    {
        var started = false;
        var testee = Deferred.FromFunc(() =>
        {
            started = true;
            return Task.FromResult(42);
        });

        started.Should().BeFalse();

        var result = await testee;

        result.Should().Be(42);
        started.Should().BeTrue();
    }

    [Test]
    public void FromFunc_When_GettingAwaiter_Then_FunctionShouldRunInlineOnCallingThread()
    {
        var threadId = -1;
        var testee = Deferred.FromFunc(() =>
        {
            threadId = Environment.CurrentManagedThreadId;
            return Task.FromResult(42);
        });

        var result = testee.GetAwaiter();

        threadId.Should().Be(Environment.CurrentManagedThreadId);
        result.IsCompleted.Should().BeTrue();
    }

    [Test]
    public async Task FromFunc_When_FunctionThrowsSynchronously_Then_AwaitShouldThrow()
    {
        var testee = Deferred.FromFunc<int>(() => throw new InvalidOperationException("Failed"));

        Func<Task> act = async () => await testee;

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Failed");
    }

    [Test]
    public async Task FromFunc_When_FunctionIsAsynchronous_Then_ResultShouldBeExpectedValue()
    {
        var testee = Deferred.FromFunc(async () =>
        {
            await Task.Yield();
            return 42;
        });

        var result = await testee.ConfigureAwait(false);

        result.Should().Be(42);
    }

    [Test]
    public async Task FromFunc_When_FunctionReturnsValueTask_Then_FunctionRunsOnDemand()
    {
        var started = false;
        var testee = Deferred.FromFunc(() =>
        {
            started = true;
            return new ValueTask<int>(42);
        });

        started.Should().BeFalse();

        var result = await testee;

        result.Should().Be(42);
        started.Should().BeTrue();
    }

    [Test]
    public async Task FromFunc_When_NeverAwaited_Then_FunctionShouldNotRun()
    {
        var invocations = 0;
        _ = Deferred.FromFunc(() => Task.FromResult(Interlocked.Increment(ref invocations)));

        await Task.Yield();

        invocations.Should().Be(0);
    }

    [Test]
    public async Task AsTask_When_UsedWithWhenAll_Then_ResultsShouldBeExpectedValues()
    {
        var first = Deferred.FromResult(1);
        var second = Deferred.FromFunc(() => Task.FromResult(2));

        var result = await Task.WhenAll(first.AsTask(), second.AsTask());

        result.Should().Equal(1, 2);
    }

    [Test]
    public void Equals_When_CreatedFromSameResult_Then_ResultShouldBeTrue()
    {
        var lhs = Deferred.FromResult(42);
        var rhs = Deferred.FromResult(42);

        lhs.Equals(rhs).Should().BeTrue();
        (lhs == rhs).Should().BeTrue();
        lhs.GetHashCode().Should().Be(rhs.GetHashCode());
    }

    [Test]
    public void Equals_When_CreatedFromDifferentResults_Then_ResultShouldBeFalse()
    {
        var lhs = Deferred.FromResult(42);
        var rhs = Deferred.FromResult(43);

        lhs.Equals(rhs).Should().BeFalse();
        (lhs != rhs).Should().BeTrue();
    }

    [Test]
    public void Equals_When_CreatedFromSameFunc_Then_ResultShouldBeTrue()
    {
        Func<Task<int>> func = () => Task.FromResult(42);
        var lhs = Deferred.FromFunc(func);
        var rhs = Deferred.FromFunc(func);

        lhs.Equals(rhs).Should().BeTrue();
        lhs.GetHashCode().Should().Be(rhs.GetHashCode());
    }

    [Test]
    public void Equals_When_CreatedFromDifferentTasks_Then_ResultShouldBeFalse()
    {
        var lhs = Deferred.FromTask(Task.FromResult(42));
        var rhs = Deferred.FromTask(Task.FromResult(42));

        lhs.Equals(rhs).Should().BeFalse();
        lhs.Equals((object)rhs).Should().BeFalse();
    }

    [Test]
    public void AsValueTask_When_CreatedFromResult_Then_ValueTaskShouldBeCompletedSynchronously()
    {
        var testee = Deferred.FromResult(42);

        var result = testee.AsValueTask();

        result.IsCompletedSuccessfully.Should().BeTrue();
        result.Result.Should().Be(42);
    }

    [Test]
    public async Task AsValueTask_When_FunctionThrowsSynchronously_Then_ValueTaskShouldBeFaulted()
    {
        var testee = Deferred.FromFunc<int>(() => throw new InvalidOperationException("Failed"));

        var result = testee.AsValueTask();

        result.IsFaulted.Should().BeTrue();
        Func<Task> act = async () => await result;
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Failed");
    }

    [Test]
    public async Task AsValueTask_When_FunctionThrowsOperationCanceledExceptionSynchronously_Then_ValueTaskShouldBeCanceled()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        var cancellationToken = cancellationTokenSource.Token;
        var testee = Deferred.FromFunc<int>(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(42);
        });

        var result = testee.AsValueTask();

        result.IsCanceled.Should().BeTrue();
        Func<Task> act = async () => await result;
        (await act.Should().ThrowAsync<OperationCanceledException>()).Which.CancellationToken.Should().Be(cancellationToken);
    }

    [Test]
    public async Task AsTask_When_FunctionThrowsOperationCanceledExceptionWithoutToken_Then_TaskShouldBeCanceledAndPreserveException()
    {
        var expectedException = new OperationCanceledException("Canceled");
        var testee = Deferred.FromFunc<int>(() => throw expectedException);

        Task<int>? result = null;
        Func<Task> act = async () =>
        {
            result = testee.AsTask();
            await result;
        };

        (await act.Should().ThrowAsync<OperationCanceledException>()).Which.Should().BeSameAs(expectedException);
        result!.Status.Should().Be(TaskStatus.Canceled);
    }

    [Test]
    public async Task FromTask_When_TaskHasNotStartedAndAwaitedOnCustomScheduler_Then_TaskShouldRunOnDefaultScheduler()
    {
        var customScheduler = new ConcurrentExclusiveSchedulerPair().ExclusiveScheduler;
        TaskScheduler? awaitingScheduler = null;
        TaskScheduler? taskScheduler = null;
        var task = new Task<int>(() =>
        {
            taskScheduler = TaskScheduler.Current;
            return 42;
        });
        var testee = Deferred.FromTask(task);

        var result = await Task.Factory.StartNew(
            () =>
            {
                awaitingScheduler = TaskScheduler.Current;
                return testee.AsTask();
            },
            CancellationToken.None,
            TaskCreationOptions.None,
            customScheduler).Unwrap();

        result.Should().Be(42);
        awaitingScheduler.Should().BeSameAs(customScheduler);
        taskScheduler.Should().BeSameAs(TaskScheduler.Default);
    }

    [Test]
    public async Task ImplicitConversion_When_ConvertingFromTask_Then_ResultShouldBeExpectedValue()
    {
        var task = Task.FromResult(42);

        Deferred<int> testee = task;

        testee.Should().Be(Deferred.FromTask(task));
        (await testee).Should().Be(42);
    }
}
