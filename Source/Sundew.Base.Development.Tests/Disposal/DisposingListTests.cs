// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposingListTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Disposal;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Sundew.Base.Disposal;

public class DisposingListTests
{
    [Test]
    public void Dispose_Then_ItemsShouldBeDisposedInExpectedOrder()
    {
        var expectedOrder = new[] { 1, 2 };
        var testee = new DisposingList();
        var disposeOrder = new List<int>();
        testee.Add(new DisposeAction(() => disposeOrder.Add(1)));
        testee.Add(new DisposeAction(() => disposeOrder.Add(2)));

        testee.Dispose();

        testee.GetDisposers().Should().BeEmpty();
        disposeOrder.Should().Equal(expectedOrder);
    }

    [Test]
    public void Dispose_When_AddedThroughAddRange_Then_ItemsShouldBeDisposedInExpectedOrder()
    {
        var expectedOrder = new[] { 1, 2 };
        var testee = new DisposingList<DisposeAction>();
        var disposeOrder = new List<int>();
        testee.AddRange([new DisposeAction(() => disposeOrder.Add(1)), new DisposeAction(() => disposeOrder.Add(2))]);

        testee.Dispose();

        testee.GetDisposers().Should().BeEmpty();
        disposeOrder.Should().Equal(expectedOrder);
    }

    [Test]
    public async Task Dispose_When_ItemsWereAddedConcurrently_Then_EachItemShouldBeDisposedExactlyOnce()
    {
        const int taskCount = 8;
        const int itemsPerTask = 500;
        var testee = new DisposingList<CountingDisposable>();
        var disposables = Enumerable.Range(0, taskCount * itemsPerTask).Select(_ => new CountingDisposable()).ToArray();

        await Task.WhenAll(Enumerable.Range(0, taskCount).Select(taskIndex => Task.Run(() =>
        {
            for (var i = 0; i < itemsPerTask; i++)
            {
                testee.Add(disposables[(taskIndex * itemsPerTask) + i]);
            }
        })));

#pragma warning disable VSTHRD103
        testee.Dispose();
#pragma warning restore VSTHRD103

        using (new AssertionScope())
        {
            testee.GetDisposers().Should().BeEmpty();
            disposables.Should().OnlyContain(x => x.DisposeCount == 1);
        }
    }

    [Test]
    public async Task Dispose_When_CalledWhileItemsAreBeingAdded_Then_NoItemShouldBeLostOrDisposedTwice()
    {
        const int itemCount = 2000;
        var testee = new DisposingList<CountingDisposable>();
        var disposables = Enumerable.Range(0, itemCount).Select(_ => new CountingDisposable()).ToArray();

        var addTask = Task.Run(() =>
        {
            foreach (var disposable in disposables)
            {
                testee.Add(disposable);
            }
        });

#pragma warning disable VSTHRD103
        while (!addTask.IsCompleted)
        {
            testee.Dispose();
        }

        await addTask;
        testee.Dispose();
#pragma warning restore VSTHRD103

        disposables.Should().OnlyContain(x => x.DisposeCount == 1);
    }

    private sealed class CountingDisposable : IDisposable
    {
        private int disposeCount;

        public int DisposeCount => Volatile.Read(ref this.disposeCount);

        public void Dispose()
        {
            Interlocked.Increment(ref this.disposeCount);
        }
    }
}