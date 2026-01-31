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
using AwesomeAssertions.Execution;
using Sundew.Base.Collections.Concurrent;
using Sundew.Base.Development.Tests.Assertions;
using Sundew.Base.Notifications;
using Sundew.Base.Threading;

[Repeat(100)]
public class ValueSynchronizerTests
{
    private const string InitialName = "initial name";
    private const int InitialNumber = -1;

    private readonly SequenceBuilder eventSequence = new SequenceBuilder();

    [Test]
    public async Task Value_Then_ResultShouldBeExpectedValue()
    {
        var expectedValue = new SynchronizedValue(InitialName, InitialNumber);

        var externalService = new ExternalService(this.eventSequence, InitialName, InitialNumber);

        var testee = new ValueSynchronizer<UpdateTarget, SynchronizedValue>(UpdateTarget.None, (target, _) => externalService.GetValueAsync(target));

        var result = await testee.Value;

        result.Should().Be(expectedValue);
    }

    [Test]
    public async Task TrySubmitAsync_When_SubmittingOneChangeWithRefresh_Then_ResultShouldBeExpectedResult()
    {
        const string newName = "new name";
        var expectedResult = new SynchronizedValue(newName, InitialNumber);

        var externalService = new ExternalService(this.eventSequence, InitialName, InitialNumber);
        using var testee = new ValueSynchronizer<UpdateTarget, SynchronizedValue>(UpdateTarget.None, (target, _) => externalService.GetValueAsync(target));
        var notificationTarget = new NotificationTarget();
        var results = new ConcurrentList<SynchronizedValue>();
        testee.Subscribe(notificationTarget, (SynchronizedValue value, CancellationToken token) =>
        {
            results.Add(value);
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

        await submission1;

        using (new AssertionScope())
        {
            results.Should().Equal(expectedResult);
            this.eventSequence.Sequence.Should().AtLeastContain(
                SequenceBuilder.GetValue,
                SequenceBuilder.SetName(newName),
                SequenceBuilder.GetValue);
        }
    }

    [Test]
    public async Task TrySubmitAsync_When_SubmittingMultipleChangesForTheSameKey_Then_OnlySingleUpdateShouldBeSubmitted()
    {
        const string newName1 = "new name 1";
        const string newName2 = "new name 2";
        const string newName3 = "new name 3";
        var expectedResult = new SynchronizedValue(newName3, InitialNumber);

        var externalService = new ExternalService(this.eventSequence, InitialName, InitialNumber);
        using var testee = new ValueSynchronizer<UpdateTarget, SynchronizedValue>(UpdateTarget.None, (target, _) => externalService.GetValueAsync(target));
        var notificationTarget = new NotificationTarget();
        var results = new ConcurrentList<SynchronizedValue>();
        testee.Subscribe(notificationTarget, (SynchronizedValue value, CancellationToken token) =>
        {
            results.Add(value);
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

        using (new AssertionScope())
        {
            results.Should().EndWith(expectedResult);
            this.eventSequence.Sequence.Should().AtLeastContain(
                SequenceBuilder.GetValue,
                SequenceBuilder.SetName(expectedResult.Name),
                SequenceBuilder.GetValue);
        }
    }

    [Test]
    public async Task TrySubmitAsync_When_SubmittingMultipleChangesWithRefreshForTheDifferentKeys_Then_ResultShouldBeExpectedResult()
    {
        const string newName = "new name";
        const int newNumber = 42;
        var firstExpectedValue = new SynchronizedValue(newName, InitialNumber);
        var secondExpectedValue = new SynchronizedValue(newName, newNumber);

        var externalService = new ExternalService(this.eventSequence, InitialName, InitialNumber);
        var notificationTarget = new NotificationTarget();
        using var testee = new ValueSynchronizer<UpdateTarget, SynchronizedValue>(UpdateTarget.None, (target, _) => externalService.GetValueAsync(target));
        var results = new ConcurrentList<SynchronizedValue>();
        testee.Subscribe(notificationTarget, (SynchronizedValue value, CancellationToken token) =>
        {
            results.Add(value);
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

        await Task.WhenAll(submission1, submission2);

        using (new AssertionScope())
        {
            results.Should().Contain(firstExpectedValue);
            results.Should().EndWith(secondExpectedValue);
            this.eventSequence.Sequence.Should().AtLeastContain(
                SequenceBuilder.GetValue,
                SequenceBuilder.SetName(firstExpectedValue.Name),
                SequenceBuilder.GetValue,
                SequenceBuilder.SetNumber(secondExpectedValue.Number),
                SequenceBuilder.GetValue);
        }
    }

    [Test]
    public async Task TrySubmitAsync_When_SubmittingMultipleChangesWithRefreshOnIdleForDifferentKeys_Then_ResultShouldBeExpectedResult()
    {
        const string newName = "new name";
        const int newNumber = 42;
        var firstExpectedValue = new SynchronizedValue(newName, InitialNumber);
        var expectedResult = new SynchronizedValue(newName, newNumber);

        var externalService = new ExternalService(this.eventSequence, InitialName, InitialNumber);
        var notificationTarget = new NotificationTarget();
        using var testee = new ValueSynchronizer<UpdateTarget, SynchronizedValue>(
            UpdateTarget.None,
            (target, _) => externalService.GetValueAsync(target));
        var results = new ConcurrentList<SynchronizedValue>();

        testee.Subscribe(notificationTarget, (SynchronizedValue value, CancellationToken token) =>
        {
            results.Add(value);
            return ValueTask.CompletedTask;
        });

        var submission1 = testee.TrySubmitAsync(
            1,
            async _ =>
            {
                await externalService.SetName(newName).ConfigureAwait(false);
                return PostSubmitAction<UpdateTarget, SynchronizedValue>._RefreshOnIdle(UpdateTarget.None);
            },
            Cancellation.None);
        var submission2 = testee.TrySubmitAsync(
            2,
            async _ =>
            {
                await externalService.SetNumber(newNumber).ConfigureAwait(false);
                return PostSubmitAction<UpdateTarget, SynchronizedValue>._RefreshOnIdle(UpdateTarget.None);
            },
            Cancellation.None);

        await Task.WhenAll(submission1, submission2);

        using (new AssertionScope())
        {
            results.Should().EndWith(expectedResult);
            this.eventSequence.Sequence.Should().AtLeastContain(
                SequenceBuilder.GetValue,
                SequenceBuilder.SetName(firstExpectedValue.Name),
                SequenceBuilder.SetNumber(expectedResult.Number),
                SequenceBuilder.GetValue);
        }
    }

    [Test]
    public async Task TrySubmitAsync_When_SubmittingMultipleChangesWithSetValueForDifferentKeys_Then_ResultShouldBeExpectedResult()
    {
        const string newName = "new name";
        const int newNumber = 42;
        var firstExpectedValue = new SynchronizedValue(newName, InitialNumber);
        var expectedResult = new SynchronizedValue(newName, newNumber);

        var externalService = new ExternalService(this.eventSequence, InitialName, InitialNumber);
        var notificationTarget = new NotificationTarget();
        using var testee = new ValueSynchronizer<UpdateTarget, SynchronizedValue>(UpdateTarget.None, (target, _) => externalService.GetValueAsync(target));
        var results = new ConcurrentList<SynchronizedValue>();

        testee.Subscribe(notificationTarget, (SynchronizedValue value, CancellationToken token) =>
        {
            results.Add(value);
            return ValueTask.CompletedTask;
        });

        var submission1 = testee.TrySubmitAsync(
            1,
            async _ =>
            {
                await externalService.SetName(newName).ConfigureAwait(false);
                return PostSubmitAction<UpdateTarget, SynchronizedValue>._SetValue(new SynchronizedValue(externalService.ExternalName, externalService.ExternalNumber));
            },
            Cancellation.None);
        var submission2 = testee.TrySubmitAsync(
            2,
            async _ =>
            {
                await externalService.SetNumber(newNumber).ConfigureAwait(false);
                return PostSubmitAction<UpdateTarget, SynchronizedValue>._SetValue(new SynchronizedValue(externalService.ExternalName, externalService.ExternalNumber));
            },
            Cancellation.None);

        await Task.WhenAll(submission1, submission2);
        var result = await testee.Value;

        using (new AssertionScope())
        {
            results.Should().Contain(firstExpectedValue);
            results.Should().EndWith(expectedResult);
            result.Should().Be(expectedResult);
            this.eventSequence.Sequence.Should().AtLeastContain(
                SequenceBuilder.GetValue,
                SequenceBuilder.SetName(firstExpectedValue.Name),
                SequenceBuilder.SetNumber(expectedResult.Number));
        }
    }

    [Test]
    public async Task TrySubmitAsync_When_SubmittingChangeAndCancelling_Then_ResultShouldBeExpectedResult()
    {
        const string newName = "new name";
        var expectedResult = new SynchronizedValue(newName, InitialNumber);

        var externalService = new ExternalService(this.eventSequence, InitialName, InitialNumber);
        var notificationTarget = new NotificationTarget();
        using var testee = new ValueSynchronizer<UpdateTarget, SynchronizedValue>(UpdateTarget.None, (target, _) => externalService.GetValueAsync(target));
        var results = new ConcurrentList<SynchronizedValue>();

        testee.Subscribe(notificationTarget, (SynchronizedValue value, CancellationToken token) =>
        {
            results.Add(value);
            return ValueTask.CompletedTask;
        });

        var submission1 = testee.TrySubmitAsync(
            1,
            async _ =>
            {
                await externalService.SetName(newName).ConfigureAwait(false);
                return PostSubmitAction<UpdateTarget, SynchronizedValue>._SetValue(new SynchronizedValue(externalService.ExternalName, externalService.ExternalNumber));
            },
            Cancellation.None);
        var submission2 = testee.TrySubmitAsync(
            2,
            token => throw new OperationCanceledException(token),
            new Cancellation(TimeSpan.FromMilliseconds(2)));

        await Task.WhenAll(submission1, submission2);

        using (new AssertionScope())
        {
            results.Should().EndWith(expectedResult);
            this.eventSequence.Sequence.Should().AtLeastContain(
                SequenceBuilder.SetName(expectedResult.Name));
        }
    }

    [Test]
    public async Task RefreshAsync_Then_ResultShouldBeExpectedResult()
    {
        const string newName = "new name";
        const int newNumber = 42;
        var expectedResult = new SynchronizedValue(newName, newNumber);

        var externalService = new ExternalService(this.eventSequence, InitialName, InitialNumber);
        using var testee = new ValueSynchronizer<UpdateTarget, SynchronizedValue>(UpdateTarget.None, (target, _) => externalService.GetValueAsync(target));

        externalService.ExternalName = newName;
        externalService.ExternalNumber = newNumber;
        var refresh1 = testee.RefreshAsync(UpdateTarget.None);

        await Task.WhenAll(refresh1);
        var result = await testee.Value;

        using (new AssertionScope())
        {
            result.Should().Be(expectedResult);
            this.eventSequence.Sequence.Should().AtLeastContain(
                SequenceBuilder.GetValue);
        }
    }
}

public enum UpdateTarget
{
    None,
}

public record SynchronizedValue(string Name, int Number);

public class ExternalService(SequenceBuilder sequenceBuilder, string initialName, int initialNumber)
{
    public string ExternalName { get; set; } = initialName;

    public int ExternalNumber { get; set; } = initialNumber;

    public async Task<SynchronizedValue> GetValueAsync(UpdateTarget target)
    {
        await Task.Delay(100).ConfigureAwait(false);
        sequenceBuilder.Add(SequenceBuilder.GetValue);
        return new SynchronizedValue(this.ExternalName, this.ExternalNumber);
    }

    public async Task SetName(string newName)
    {
        Console.WriteLine("SetName");
        await Task.Delay(100).ConfigureAwait(false);
        this.ExternalName = newName;
        Console.WriteLine("Name set");
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

    public static string SetName(string name) => $"Set Name {name}";

    public static string SetNumber(int number) => $"Set Number {number}";

    public ConcurrentList<string> Sequence { get; } = new();

    public void Add(string item)
    {
        Console.WriteLine(item);
        this.Sequence.Add(item);
    }
}