// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueSynchronizerTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable SA1402
#pragma warning disable SA1201
namespace Sundew.Base.Development.Tests.Threading;

using System;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Sundew.Base.Collections.Concurrent;
using Sundew.Base.Notifications;
using Sundew.Base.Threading;
using Xunit;
using Xunit.Abstractions;

public class ValueSynchronizerTests
{
    private const string InitialName = "initial name";
    private const int InitialNumber = -1;

    private readonly SequenceBuilder eventSequence;

    public ValueSynchronizerTests(ITestOutputHelper output)
    {
        this.eventSequence = new SequenceBuilder(output);
    }

    [Fact]
    public async Task Value_Then_ResultShouldBeExpectedValue()
    {
        var expectedValue = new SynchronizedValue(InitialName, InitialNumber);

        var externalService = new ExternalService(this.eventSequence, InitialName, InitialNumber);

        var testee = new ValueSynchronizer<UpdateTarget, SynchronizedValue>(UpdateTarget.None, (target, _) => externalService.GetValueAsync(target));

        var result = await testee.Value;

        result.Should().Be(expectedValue);
    }

    [Fact]
    public async Task Value_When_ApplyingOneChangeWithRefresh_Then_ResultShouldBeExpectedResult()
    {
        const string newName = "new name";
        var expectedResult = new SynchronizedValue(newName, InitialNumber);

        var externalService = new ExternalService(this.eventSequence, InitialName, InitialNumber);
        using var testee = new ValueSynchronizer<UpdateTarget, SynchronizedValue>(UpdateTarget.None, (target, _) => externalService.GetValueAsync(target));
        var notificationTarget = new NotificationTarget();
        testee.Subscribe(notificationTarget, (SynchronizedValue value, CancellationToken token) =>
        {
            this.eventSequence.Notify(value);
            return ValueTask.CompletedTask;
        });

        _ = testee.TrySubmitAsync(
            1,
            async _ =>
            {
                await externalService.SetName(newName).ConfigureAwait(false);
                return PostSubmitAction<UpdateTarget, SynchronizedValue>._Refresh(UpdateTarget.None);
            },
            Cancellation.None);

        await this.eventSequence.AwaitEvent();

        var result = await testee.Value;

        Assert.Multiple(
            () => result.Should().Be(expectedResult),
            () => this.eventSequence.Sequence.Should().BeEquivalentTo(SequenceBuilder.AwaitEvent(1), SequenceBuilder.GetValue, SequenceBuilder.SetName(newName), SequenceBuilder.GetValue, SequenceBuilder.Notified(expectedResult), SequenceBuilder.ReceivedEvent(1)));
    }

    [Fact]
    public async Task Value_When_ApplyingMultipleChangesForTheSameKey_Then_OnlySingleUpdateShouldBeSubmitted()
    {
        const string newName1 = "new name 1";
        const string newName2 = "new name 2";
        const string newName3 = "new name 3";
        var expectedResult = new SynchronizedValue(newName3, InitialNumber);

        var externalService = new ExternalService(this.eventSequence, InitialName, InitialNumber);
        using var testee = new ValueSynchronizer<UpdateTarget, SynchronizedValue>(UpdateTarget.None, (target, _) => externalService.GetValueAsync(target));
        var notificationTarget = new NotificationTarget();
        testee.Subscribe(notificationTarget, (SynchronizedValue value, CancellationToken token) =>
        {
            this.eventSequence.Notify(value);
            return ValueTask.CompletedTask;
        });

        var submission1 = testee.TrySubmitAsync(
            1,
            async _ =>
            {
                await externalService.SetName(newName1).ConfigureAwait(false);
                return PostSubmitAction<UpdateTarget, SynchronizedValue>._Refresh(UpdateTarget.None);
            },
            Cancellation.None);

        var submission2 = testee.TrySubmitAsync(
            1,
            async _ =>
            {
                await externalService.SetName(newName2).ConfigureAwait(false);
                return PostSubmitAction<UpdateTarget, SynchronizedValue>._Refresh(UpdateTarget.None);
            },
            Cancellation.None);

        var submission3 = testee.TrySubmitAsync(
            1,
            async _ =>
            {
                await externalService.SetName(newName3).ConfigureAwait(false);
                return PostSubmitAction<UpdateTarget, SynchronizedValue>._Refresh(UpdateTarget.None);
            },
            Cancellation.None);

        await Task.WhenAll(submission1, submission2, submission3);

        var result = await testee.Value;

        Assert.Multiple(() => result.Should().Be(expectedResult));
    }

    [Fact]
    public async Task Value_When_ApplyingMultipleChangesWithRefreshForTheDifferentKeys_Then_ResultShouldBeExpectedResult()
    {
        const string newName = "new name";
        const int newNumber = 42;
        var firstExpectedValue = new SynchronizedValue(newName, InitialNumber);
        var secondExpectedValue = new SynchronizedValue(newName, newNumber);

        var externalService = new ExternalService(this.eventSequence, InitialName, InitialNumber);
        var notificationTarget = new NotificationTarget();
        using var testee = new ValueSynchronizer<UpdateTarget, SynchronizedValue>(UpdateTarget.None, (target, _) => externalService.GetValueAsync(target));

        testee.Subscribe(notificationTarget, (SynchronizedValue value, CancellationToken token) =>
        {
            this.eventSequence.Notify(value);
            return ValueTask.CompletedTask;
        });

        var submission1 = testee.TrySubmitAsync(
            1,
            async _ =>
            {
                await externalService.SetName(newName).ConfigureAwait(false);
                return PostSubmitAction<UpdateTarget, SynchronizedValue>._Refresh(UpdateTarget.None);
            },
            Cancellation.None);
        var submission2 = testee.TrySubmitAsync(
            2,
            async _ =>
            {
                await externalService.SetNumber(newNumber).ConfigureAwait(false);
                return PostSubmitAction<UpdateTarget, SynchronizedValue>._Refresh(UpdateTarget.None);
            },
            Cancellation.None);

        await this.eventSequence.AwaitEvent();

        var result1 = await testee.Value;

        await this.eventSequence.AwaitEvent();

        var result2 = await testee.Value;

        Assert.Multiple(
            () => result1.Should().Be(firstExpectedValue),
            () => result2.Should().Be(secondExpectedValue),
            () => this.eventSequence.Sequence.Should().BeEquivalentTo(
                SequenceBuilder.AwaitEvent(1),
                SequenceBuilder.GetValue,
                SequenceBuilder.SetName(firstExpectedValue.Name),
                SequenceBuilder.GetValue,
                SequenceBuilder.Notified(firstExpectedValue),
                SequenceBuilder.ReceivedEvent(1),
                SequenceBuilder.AwaitEvent(2),
                SequenceBuilder.SetNumber(secondExpectedValue.Number),
                SequenceBuilder.GetValue,
                SequenceBuilder.Notified(secondExpectedValue),
                SequenceBuilder.ReceivedEvent(2)));
    }

    [Fact]
    public async Task Value_When_ApplyingMultipleChangesWithRefreshOnIdleForDifferentKeys_Then_ResultShouldBeExpectedResult()
    {
        const string newName = "new name";
        const int newNumber = 42;
        var firstExpectedValue = new SynchronizedValue(newName, InitialNumber);
        var expectedResult = new SynchronizedValue(newName, newNumber);

        var externalService = new ExternalService(this.eventSequence, InitialName, InitialNumber);
        var notificationTarget = new NotificationTarget();
        using var testee = new ValueSynchronizer<UpdateTarget, SynchronizedValue>(UpdateTarget.None, (target, _) => externalService.GetValueAsync(target));

        testee.Subscribe(notificationTarget, (SynchronizedValue value, CancellationToken token) =>
        {
            this.eventSequence.Notify(value);
            return ValueTask.CompletedTask;
        });

        _ = testee.TrySubmitAsync(
            1,
            async _ =>
            {
                await externalService.SetName(newName).ConfigureAwait(false);
                return PostSubmitAction<UpdateTarget, SynchronizedValue>._RefreshOnIdle(UpdateTarget.None);
            },
            Cancellation.None);
        _ = testee.TrySubmitAsync(
            2,
            async _ =>
            {
                await externalService.SetNumber(newNumber).ConfigureAwait(false);
                return PostSubmitAction<UpdateTarget, SynchronizedValue>._RefreshOnIdle(UpdateTarget.None);
            },
            Cancellation.None);

        await this.eventSequence.AwaitEvent();

        var result1 = await testee.Value;

        Assert.Multiple(
            () => result1.Should().Be(expectedResult),
            () => this.eventSequence.Sequence.Should().BeEquivalentTo(
                SequenceBuilder.AwaitEvent(1),
                SequenceBuilder.GetValue,
                SequenceBuilder.SetName(firstExpectedValue.Name),
                SequenceBuilder.SetNumber(expectedResult.Number),
                SequenceBuilder.GetValue,
                SequenceBuilder.Notified(expectedResult),
                SequenceBuilder.ReceivedEvent(1)));
    }

    [Fact]
    public async Task Value_When_ApplyingMultipleChangesWithSetValueForDifferentKeys_Then_ResultShouldBeExpectedResult()
    {
        const string newName = "new name";
        const int newNumber = 42;
        var firstExpectedValue = new SynchronizedValue(newName, InitialNumber);
        var expectedResult = new SynchronizedValue(newName, newNumber);

        var externalService = new ExternalService(this.eventSequence, InitialName, InitialNumber);
        var notificationTarget = new NotificationTarget();
        using var testee = new ValueSynchronizer<UpdateTarget, SynchronizedValue>(UpdateTarget.None, (target, _) => externalService.GetValueAsync(target));

        testee.Subscribe(notificationTarget, (SynchronizedValue value, CancellationToken token) =>
        {
            this.eventSequence.Notify(value);
            return ValueTask.CompletedTask;
        });

        _ = testee.TrySubmitAsync(
            1,
            async _ =>
            {
                await externalService.SetName(newName).ConfigureAwait(false);
                return PostSubmitAction<UpdateTarget, SynchronizedValue>._SetValue(new SynchronizedValue(externalService.ExternalName, externalService.ExternalNumber));
            },
            Cancellation.None);
        _ = testee.TrySubmitAsync(
            2,
            async _ =>
            {
                await externalService.SetNumber(newNumber).ConfigureAwait(false);
                return PostSubmitAction<UpdateTarget, SynchronizedValue>._SetValue(new SynchronizedValue(externalService.ExternalName, externalService.ExternalNumber));
            },
            Cancellation.None);

        await this.eventSequence.AwaitEvent();

        var result1 = await testee.Value;

        await this.eventSequence.AwaitEvent();

        var result2 = await testee.Value;

        Assert.Multiple(
            () => result1.Should().Be(firstExpectedValue),
            () => result2.Should().Be(expectedResult),
            () => this.eventSequence.Sequence.Should().BeEquivalentTo(
                SequenceBuilder.AwaitEvent(1),
                SequenceBuilder.GetValue,
                SequenceBuilder.SetName(firstExpectedValue.Name),
                SequenceBuilder.Notified(firstExpectedValue),
                SequenceBuilder.ReceivedEvent(1),
                SequenceBuilder.AwaitEvent(2),
                SequenceBuilder.SetNumber(expectedResult.Number),
                SequenceBuilder.Notified(expectedResult),
                SequenceBuilder.ReceivedEvent(2)));
    }
}

public enum UpdateTarget
{
    None,
}

public record SynchronizedValue(string Name, int Number);

public class ExternalService(SequenceBuilder sequenceBuilder, string initialName, int initialNumber)
{
    public string ExternalName { get; private set; } = initialName;

    public int ExternalNumber { get; private set; } = initialNumber;

    public async Task<SynchronizedValue> GetValueAsync(UpdateTarget target)
    {
        await Task.Delay(100).ConfigureAwait(false);
        sequenceBuilder.Add(SequenceBuilder.GetValue);
        return new SynchronizedValue(this.ExternalName, this.ExternalNumber);
    }

    public async Task SetName(string newName)
    {
        await Task.Delay(100).ConfigureAwait(false);
        this.ExternalName = newName;
        sequenceBuilder.Add(SequenceBuilder.SetName(newName));
    }

    public async Task SetNumber(int newNumber)
    {
        await Task.Delay(100).ConfigureAwait(false);
        this.ExternalNumber = newNumber;
        sequenceBuilder.Add(SequenceBuilder.SetNumber(newNumber));
    }
}

public class NotificationTarget : INotificationTarget
{
    public Subscriptions TargetSubscriptions { get; } = new();
}

public class SequenceBuilder
{
    public const string GetValue = "Get Value";

    private readonly ITestOutputHelper testOutputHelper;
    private readonly AutoResetEventAsync autoResetEvent = new AutoResetEventAsync();
    private int eventCounter = 0;

    public SequenceBuilder(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
    }

    public static string AwaitEvent(int id) => $"Await event {id}";

    public static string ReceivedEvent(int id) => $"Received event {id}";

    public static string SetName(string name) => $"Set Name {name}";

    public static string SetNumber(int number) => $"Set Number {number}";

    public static string Notified(object value) => $"Notified: {value}";

    public ConcurrentList<string> Sequence { get; } = new();

    public async Task AwaitEvent()
    {
        Interlocked.Increment(ref this.eventCounter);
        this.Add(SequenceBuilder.AwaitEvent(this.eventCounter));
        var awaitTask = await this.autoResetEvent.WaitAsync(TimeSpan.FromSeconds(10)).ConfigureAwait(false);
        this.Add(SequenceBuilder.ReceivedEvent(this.eventCounter));
    }

    public void Add(string item)
    {
        this.testOutputHelper.WriteLine(item);
        this.Sequence.Add(item);
    }

    public void Notify(SynchronizedValue value)
    {
        this.Add(SequenceBuilder.Notified(value));
        this.autoResetEvent.Set();
    }
}