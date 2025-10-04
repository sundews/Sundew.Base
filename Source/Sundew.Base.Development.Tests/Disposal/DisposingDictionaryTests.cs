// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposingDictionaryTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Disposal;

using System.Collections.Generic;
using System.Threading.Tasks;
using AwesomeAssertions;
using Sundew.Base.Disposal;
using Xunit;

public class DisposingDictionaryTests
{
    [Fact]
    public void Dispose_Then_ItemsShouldBeDisposedInExpectedOrder()
    {
        var expectedOrder = new[] { 1, 2 };
        var testee = new DisposingDictionary<string>();
        var disposeOrder = new List<int>();
        testee.Add("AnItem", new DisposeAction(() => disposeOrder.Add(1)));
        testee.Add("AnotherItem", new DisposeAction(() => disposeOrder.Add(2)));

        testee.Dispose();

        testee.GetDisposers().Should().BeEmpty();
        disposeOrder.Should().Equal(expectedOrder);
    }

    [Fact]
    public async Task DisposeAsync_Then_ItemsShouldBeDisposedInExpectedOrder()
    {
        var expectedOrder = new[] { 1, 2 };
        var testee = new DisposingDictionary<string>();
        var disposeOrder = new List<int>();
        testee.AddAsync("AnItem", new DisposeAsyncAction(async () =>
        {
            await Task.Delay(10);
            disposeOrder.Add(1);
        }));
        testee.AddAsync("AnotherItem", new DisposeAsyncAction(() =>
        {
            disposeOrder.Add(2);
            return default;
        }));

        await testee.DisposeAsync();

        testee.GetDisposers().Should().BeEmpty();
        disposeOrder.Should().Equal(expectedOrder);
    }

    [Fact]
    public async Task DisposeAsync_When_OneItemIsDisposed_Then_ItemsShouldBeDisposedInExpectedOrder()
    {
        var expectedOrder = new[] { 2 };
        var testee = new DisposingDictionary<string>();
        var disposeOrder = new List<int>();
        var expectedDisposable = new DisposeAsyncAction(async () =>
        {
            await Task.Delay(10);
            disposeOrder.Add(1);
        });
        testee.AddAsync("AnItem", expectedDisposable);
        testee.AddAsync("AnotherItem", new DisposeAsyncAction(() =>
        {
            disposeOrder.Add(2);
            return default;
        }));

        await testee.DisposeAsync("AnotherItem");

        testee.GetDisposers().Should().Equal(new Disposer.Asynchronous(expectedDisposable));
        disposeOrder.Should().Equal(expectedOrder);
    }

    [Fact]
    public async Task DisposeAsync_When_OneItemAndKeyIsDisposed_Then_ItemShouldBeDisposedInExpectedOrder()
    {
        var expectedOrder = new[] { 3, 2 };
        var testee = new DisposingDictionary<object>();
        var disposeOrder = new List<int>();
        var key = new DisposeAsyncAction(async () =>
        {
            await Task.Delay(10);
            disposeOrder.Add(2);
        });
        var expectedDisposable = new DisposeAsyncAction(async () =>
        {
            await Task.Delay(10);
            disposeOrder.Add(1);
        });
        testee.AddAsync("item", expectedDisposable);
        testee.AddAsync(
            key,
            new DisposeAsyncAction(() =>
            {
                disposeOrder.Add(3);
                return default;
            }),
            TryDisposeKey.AfterValue);

        await testee.DisposeAsync(key);

        testee.GetDisposers().Should().Equal(new Disposer.Asynchronous(expectedDisposable));
        disposeOrder.Should().Equal(expectedOrder);
    }
}