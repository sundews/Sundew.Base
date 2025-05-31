// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuestTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Primitives;

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Sundew.Base.Threading;
using Telerik.JustMock;
using Xunit;

public class QuestTests
{
    [Fact]
    public async Task Start_When_Completed_Then_IsCompletedSuccessfullyShouldBeTrue()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var quest = Quest.Create(__._, _ => Task.CompletedTask, cancellationTokenSource.Token);
        await quest.Start().Value;
        quest.Task.IsCompletedSuccessfully.Should().BeTrue();
    }

    [Fact]
    public async Task Start_When_TaskIsRunningAndCanceled_Then_IsCanceledShouldBeTrue()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var quest = Quest.Create(
            __._,
            Task.Run(() =>
            {
                Thread.Sleep(1000);
                return Task.CompletedTask;
            }),
            cancellationTokenSource.Token);
        cancellationTokenSource.CancelAfter(1);

        var start = quest.Start();

#pragma warning disable VSTHRD003
        var startTask = () => start.Value.Task;
#pragma warning restore VSTHRD003
        await startTask.Should().ThrowAsync<OperationCanceledException>();

        quest.Task.IsCanceled.Should().BeTrue();
    }

    [Fact]
    public async Task Start_When_TaskIsRunningAndExceptionThrow_Then_IsFaultedShouldBeTrue()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var quest = Quest.Create(
            __._,
            Task.Run(() => throw new InvalidOperationException()),
            cancellationTokenSource.Token);

        var start = quest.Start();

#pragma warning disable VSTHRD003
        var startTask = () => start.Value.Task;
#pragma warning restore VSTHRD003
        await startTask.Should().ThrowAsync<AggregateException>();

        quest.Task.IsFaulted.Should().BeTrue();
    }

    [Fact]
    public async Task Start_When_UnstartedAndCanceled_Then_IsCanceledShouldBeTrue()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var quest = Quest.Create(
            __._,
            _ =>
            {
                Thread.Sleep(1000);
                return Task.CompletedTask;
            },
            cancellationTokenSource.Token);
        cancellationTokenSource.CancelAfter(1);

        var start = quest.Start();

#pragma warning disable VSTHRD003
        var startTask = () => start.Value.Task;
#pragma warning restore VSTHRD003
        await startTask.Should().ThrowAsync<OperationCanceledException>();

        quest.Task.IsCanceled.Should().BeTrue();
    }

    [Fact]
    public async Task Start_When_UnstartedTaskAndExceptionThrow_Then_IsFaultedShouldBeTrue()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var quest = Quest.Create(
            __._,
            _ => throw new InvalidOperationException(),
            cancellationTokenSource.Token);

        var start = quest.Start();

#pragma warning disable VSTHRD003
        var startTask = () => start.Value.Task;
#pragma warning restore VSTHRD003
        await startTask.Should().ThrowAsync<AggregateException>();

        quest.Task.IsFaulted.Should().BeTrue();
    }

    [Fact]
    public void Dispose_When_GuideIsDisposable_Then_DisposeShouldBeCalledOnce()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var mock = Mock.Create<IDisposable>();
        var quest = Quest.Create(
            mock,
            _ => Task.CompletedTask,
            cancellationTokenSource.Token);

        quest.Dispose();

        Mock.Assert(() => mock.Dispose(), Occurs.Once());
    }

    [Fact]
    public void Dispose_When_GuideAndDisposableAreTheSame_Then_DisposeShouldBeCalledOnce()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var mock = Mock.Create<IDisposable>();
        var quest = Quest.Create(
            mock,
            _ => Task.CompletedTask,
            mock,
            cancellationTokenSource.Token);

        quest.Dispose();

        Mock.Assert(() => mock.Dispose(), Occurs.Once());
    }

    [Fact]
    public void Dispose_When_GuideAndDisposableAreDifferent_Then_DisposeShouldBeCalledOnceForEach()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var mock1 = Mock.Create<IDisposable>();
        var mock2 = Mock.Create<IDisposable>();
        var quest = Quest.Create(
            mock1,
            _ => Task.CompletedTask,
            mock2,
            cancellationTokenSource.Token);

        quest.Dispose();

        Mock.Assert(() => mock1.Dispose(), Occurs.Once());
        Mock.Assert(() => mock2.Dispose(), Occurs.Once());
    }

    [Fact]
    public void Dispose_When_GuideAndDisposableAreDifferentAndResultIsDisposableButQuestNot_Then_DisposeShouldBeCalledOnceButNotForResult()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var mock1 = Mock.Create<IDisposable>();
        var mock2 = Mock.Create<IDisposable>();
        var mock3 = Mock.Create<IDisposable>();
        var quest = Quest.Create(
            mock1,
            _ => Task.FromResult(mock3),
            mock2,
            cancellationTokenSource.Token);

        quest.Dispose();

        Mock.Assert(() => mock1.Dispose(), Occurs.Once());
        Mock.Assert(() => mock2.Dispose(), Occurs.Once());
        Mock.Assert(() => mock3.Dispose(), Occurs.Never());
    }

    [Fact]
    public void Dispose_When_GuideAndDisposableAreDifferentAndResultIsDisposableAndQuestStarted_Then_DisposeShouldBeCalledOnceForAll()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var mock1 = Mock.Create<IDisposable>();
        var mock2 = Mock.Create<IDisposable>();
        var mock3 = Mock.Create<IDisposable>();
        var quest = Quest.Create(
            mock1,
            _ => Task.FromResult(mock3),
            mock2,
            cancellationTokenSource.Token);
        quest.Start();

        quest.Dispose();

        Mock.Assert(() => mock1.Dispose(), Occurs.Once());
        Mock.Assert(() => mock2.Dispose(), Occurs.Once());
        Mock.Assert(() => mock3.Dispose(), Occurs.Once());
    }

    [Fact]
    public async Task Start_When_AsyncAndCompleted_Then_IsCompletedSuccessfullyShouldBeTrueAndResultShouldBeExpected()
    {
        const int expected = 42;
        using var cancellationTokenSource = new CancellationTokenSource();
        var quest = Quest.Create(
            __._,
            async cancellationToken =>
            {
                await Task.Delay(10, cancellationToken);
                return expected;
            },
            cancellationTokenSource.Token);

        await quest.Start().Value;

        quest.Task.IsCompletedSuccessfully.Should().BeTrue();
        (await quest.Task).Should().Be(expected);
    }

    [Fact]
    public async Task Start_When_Completed_Then_IsCompletedSuccessfullyShouldBeTrueAndResultShouldBeExpected()
    {
        const int expected = 42;
        using var cancellationTokenSource = new CancellationTokenSource();
        var quest = Quest.Create(
            __._,
            () =>
            {
                Thread.Sleep(10);
                return expected;
            },
            cancellationTokenSource.Token);

        await quest.Start().Value;

        quest.Task.IsCompletedSuccessfully.Should().BeTrue();
        (await quest.Task).Should().Be(expected);
    }

    [Fact]
    public async Task Start_When_TaskIsRunningAndCanceledShortlyAfter_Then_IsCanceledShouldBeTrueAndResultShouldThrow()
    {
        var manualResetEvent = new ManualResetEventAsync();
        using var cancellationTokenSource = new CancellationTokenSource();
        var quest = Quest.Create(
            __._,
            Task.Run(() =>
            {
                manualResetEvent.Wait(TimeSpan.FromMilliseconds(2000));
                return 42;
            }),
            cancellationTokenSource.Token);
        cancellationTokenSource.Token.Register(() => manualResetEvent.Set());
        cancellationTokenSource.CancelAfter(10);

        var start = quest.Start();
#pragma warning disable VSTHRD003
        var startTask = () => start.Value.Task;
#pragma warning restore VSTHRD003
        await startTask.Should().ThrowAsync<OperationCanceledException>();

        quest.Task.IsCanceled.Should().BeTrue();
#pragma warning disable VSTHRD003
        var result = async () => await quest.Task.ConfigureAwait(false);
#pragma warning restore VSTHRD003
        await result.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task Start_When_TaskIsRunningAndCanceled_Then_IsCanceledShouldBeTrueAndResultShouldThrow()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var quest = Quest.Create(
            __._,
            Task.Run(() =>
            {
                Thread.Sleep(1000);
                return 42;
            }),
            cancellationTokenSource.Token);
        await cancellationTokenSource.CancelAsync();

        var start = quest.Start();
#pragma warning disable VSTHRD003
        var startTask = () => start.Value.Task;
#pragma warning restore VSTHRD003
        await startTask.Should().ThrowAsync<OperationCanceledException>();

        quest.Task.IsCanceled.Should().BeTrue();
#pragma warning disable VSTHRD003
        var result = async () => await quest.Task.ConfigureAwait(false);
#pragma warning restore VSTHRD003
        await result.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task Start_When_TaskIsRunningAndExceptionThrow_Then_IsFaultedShouldBeTrueAndResultShouldThrow()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var quest = Quest.Create(
            __._,
            Task.Run(new Func<Task<int>?>(() => throw new InvalidOperationException())),
            cancellationTokenSource.Token);

        var start = quest.Start();
#pragma warning disable VSTHRD003
        var startTask = () => start.Value.Task;
#pragma warning restore VSTHRD003
        await startTask.Should().ThrowAsync<AggregateException>();

        quest.Task.IsFaulted.Should().BeTrue();
#pragma warning disable VSTHRD003
        var result = async () => await quest.Task.ConfigureAwait(false);
#pragma warning restore VSTHRD003
        await result.Should().ThrowAsync<AggregateException>();
    }

    [Fact]
    public async Task Start_When_UnstartedAndCanceled_Then_IsCanceledShouldBeTrueAndResultShouldThrow()
    {
        const int expected = 42;
        using var cancellationTokenSource = new CancellationTokenSource();
        var quest = Quest.Create(
            __._,
            () =>
            {
                Thread.Sleep(1000);
                return expected;
            },
            cancellationTokenSource.Token);
        cancellationTokenSource.CancelAfter(10);

        var start = quest.Start();
#pragma warning disable VSTHRD003
        var startTask = () => start.Value.Task;
#pragma warning restore VSTHRD003
        await startTask.Should().ThrowAsync<OperationCanceledException>();

        quest.Task.IsCanceled.Should().BeTrue();
#pragma warning disable VSTHRD003
        var result = async () => await quest.Task.ConfigureAwait(false);
#pragma warning restore VSTHRD003
        await result.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task Start_When_UnstartedTaskAndExceptionThrow_Then_IsFaultedShouldBeTrueAndResultShouldThrow()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var quest = Quest.Create<__, int>(
            __._,
            _ => throw new InvalidOperationException(),
            cancellationTokenSource.Token);

        var start = quest.Start();
#pragma warning disable VSTHRD003
        var startTask = () => start.Value.Task;
#pragma warning restore VSTHRD003
        await startTask.Should().ThrowAsync<AggregateException>();

        quest.Task.IsFaulted.Should().BeTrue();
#pragma warning disable VSTHRD003
        var result = async () => await quest.Task.ConfigureAwait(false);
#pragma warning restore VSTHRD003
        await result.Should().ThrowAsync<AggregateException>();
    }

    [Fact]
    public async Task Start_When_MappedAndCompleted_Then_IsCompletedSuccessfullyShouldBeTrueAndResultShouldBeExpected()
    {
        const int expected = 42;
        using var cancellationTokenSource = new CancellationTokenSource();
        var quest = Quest.Create(
            __._,
            () =>
            {
                Thread.Sleep(10);
                return expected;
            },
            cancellationTokenSource.Token);

        var newQuest = quest.Map(x => x.ToString());

        await newQuest.Start().Value;

        newQuest.Task.IsCompletedSuccessfully.Should().BeTrue();
        quest.Task.IsCompletedSuccessfully.Should().BeTrue();
        (await newQuest.Task).Should().Be(expected.ToString());
    }

    [Fact]
    public async Task Start_When_TriggeredAncestorAndCompleted_Then_IsCompletedSuccessfullyShouldBeTrueAndResultShouldBeExpected()
    {
        const int expected = 42;
        using var cancellationTokenSource = new CancellationTokenSource();
        var quest = Quest.Create(
            __._,
            () =>
            {
                Thread.Sleep(10);
                return expected;
            },
            cancellationTokenSource.Token);

        var newQuest = quest.Map(x => x.ToString());
        await quest.Start().Value;

        await newQuest;
        newQuest.Task.IsCompletedSuccessfully.Should().BeTrue();
        quest.Task.IsCompletedSuccessfully.Should().BeTrue();
        (await newQuest.Task).Should().Be(expected.ToString());
    }

    [Fact]
    public void Dispose_When_MappedAndGuideIsDisposable_Then_DisposeShouldBeCalledOnce()
    {
        const int expected = 42;
        using var cancellationTokenSource = new CancellationTokenSource();
        var mock = Mock.Create<IDisposable>();
        var quest = Quest.Create(
            mock,
            _ => Task.CompletedTask,
            cancellationTokenSource.Token);

        var newQuest = quest.Map(() => expected);

        newQuest.Dispose();

        Mock.Assert(() => mock.Dispose(), Occurs.Once());
    }

    [Fact]
    public void Dispose_When_MappedAndGuideAndDisposableAreTheSame_Then_DisposeShouldBeCalledOnce()
    {
        const int expected = 42;
        using var cancellationTokenSource = new CancellationTokenSource();
        var mock = Mock.Create<IDisposable>();
        var quest = Quest.Create(
            mock,
            _ => Task.CompletedTask,
            mock,
            cancellationTokenSource.Token);

        var newQuest = quest.Map(() => expected);

        newQuest.Dispose();

        Mock.Assert(() => mock.Dispose(), Occurs.Once());
    }

    [Fact]
    public void Dispose_When_MappedAndGuideAndDisposableAreDifferent_Then_DisposeShouldBeCalledOnceForEach()
    {
        const int expected = 42;
        using var cancellationTokenSource = new CancellationTokenSource();
        var mock1 = Mock.Create<IDisposable>();
        var mock2 = Mock.Create<IDisposable>();
        var quest = Quest.Create(
            mock1,
            _ => Task.CompletedTask,
            mock2,
            cancellationTokenSource.Token);

        var newQuest = quest.Map(() => expected);

        newQuest.Dispose();

        Mock.Assert(() => mock1.Dispose(), Occurs.Once());
        Mock.Assert(() => mock2.Dispose(), Occurs.Once());
    }

    [Fact]
    public void Dispose_When_MappedAndGuideAndDisposableAreDifferentAndResultIsDisposableButQuestNot_Then_DisposeShouldBeCalledOnceButNotForResult()
    {
        const int expected = 42;
        using var cancellationTokenSource = new CancellationTokenSource();
        var mock1 = Mock.Create<IDisposable>();
        var mock2 = Mock.Create<IDisposable>();
        var mock3 = Mock.Create<IDisposable>();
        var quest = Quest.Create(
            mock1,
            _ => Task.FromResult(mock3),
            mock2,
            cancellationTokenSource.Token);

        var newQuest = quest.Map(_ => expected);

        newQuest.Dispose();

        Mock.Assert(() => mock1.Dispose(), Occurs.Once());
        Mock.Assert(() => mock2.Dispose(), Occurs.Once());
        Mock.Assert(() => mock3.Dispose(), Occurs.Never());
    }

    [Fact]
    public void Dispose_When_MappedAndGuideAndDisposableAreDifferentAndResultIsDisposableAndQuestStarted_Then_DisposeShouldBeCalledOnceForAll()
    {
        const int expected = 42;
        using var cancellationTokenSource = new CancellationTokenSource();
        var mock1 = Mock.Create<IDisposable>();
        var mock2 = Mock.Create<IDisposable>();
        var mock3 = Mock.Create<IDisposable>();
        var quest = Quest.Create(
            mock1,
            _ => Task.FromResult(mock3),
            mock2,
            cancellationTokenSource.Token);
        quest.Start();

        var newQuest = quest.Map(_ => expected);

        newQuest.Dispose();

        Mock.Assert(() => mock1.Dispose(), Occurs.Once());
        Mock.Assert(() => mock2.Dispose(), Occurs.Once());
        Mock.Assert(() => mock3.Dispose(), Occurs.Once());
    }

    [Fact]
    public void Dispose_When_MappedAndGuideAndDisposableAreDifferentAndResultIsDisposableAndNewResultIsDisposableQuestStarted_Then_DisposeShouldBeCalledOnceForAll()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var mock1 = Mock.Create<IDisposable>();
        var mock2 = Mock.Create<IDisposable>();
        var mock3 = Mock.Create<IDisposable>();
        var mock4 = Mock.Create<IDisposable>();
        var quest = Quest.Create(
            mock1,
            _ => Task.FromResult(mock3),
            mock2,
            cancellationTokenSource.Token);
        quest.Start();

        var newQuest = quest.Map(_ => mock4);

        newQuest.Dispose();

        Mock.Assert(() => mock1.Dispose(), Occurs.Once());
        Mock.Assert(() => mock2.Dispose(), Occurs.Once());
        Mock.Assert(() => mock3.Dispose(), Occurs.Once());
        Mock.Assert(() => mock4.Dispose(), Occurs.Once());
    }
}