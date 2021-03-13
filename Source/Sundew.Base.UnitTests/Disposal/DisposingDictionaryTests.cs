// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposingDictionaryTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Disposal
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
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

            disposeOrder.Should().Equal(expectedOrder);
        }

        [Fact]
        public async Task DisposeAsync_Then_ItemsShouldBeDisposedInExpectedOrder()
        {
            var expectedOrder = new[] { 1, 2 };
            var testee = new DisposingDictionary<string>();
            var disposeOrder = new List<int>();
            await testee.AddAsync("AnItem", new DisposeAction(() => disposeOrder.Add(1)));
            await testee.AddAsync("AnotherItem", new DisposeAction(() => disposeOrder.Add(2)));

            await testee.DisposeAsync();

            disposeOrder.Should().Equal(expectedOrder);
        }
    }
}