// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposerTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Disposal
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Sundew.Base.Disposal;
    using Sundew.Base.Disposal.Internal;
    using Telerik.JustMock;
    using Xunit;

    public class DisposerTests
    {
        [Fact]
        public void Dispose_Then_OnDisposedShouldBeCalledOncePerDisposableInOrder()
        {
            var disposableReporter = Mock.Create<IDisposableReporter>();
            var disposable1 = Mock.Create<IDisposable>();
            var disposable2 = Mock.Create<IDisposable>();
            Mock.Arrange(() => disposableReporter.Disposed(disposable1)).InOrder().Occurs(1);
            Mock.Arrange(() => disposableReporter.Disposed(disposable2)).InOrder().Occurs(1);

            var testee = new Disposer(disposableReporter, disposable1, disposable2);

            testee.Dispose();

            Mock.Assert(disposableReporter);
        }

        [Fact]
        public void Dispose_Then_OnDisposedAndOnDisposedAsyncShouldBeCalledOnceInOrder()
        {
            var disposableReporter = Mock.Create<IDisposableReporter>();
            var disposable = Mock.Create<IDisposable>();
            var asyncDisposable = Mock.Create<IAsyncDisposable>();
            Mock.Arrange(() => disposableReporter.Disposed(disposable)).InOrder().Occurs(1);
            Mock.Arrange(() => disposableReporter.Disposed(asyncDisposable)).InOrder().Occurs(1);
            var disposer = Disposer.Create(
                disposerBuilder =>
                {
                    disposerBuilder.Add(disposable);
                    disposerBuilder.AddAsync(asyncDisposable);
                },
                disposableReporter);

            disposer.Dispose();

            Mock.Assert(disposableReporter);
        }

        [Fact]
        public async Task DisposeAsync_Then_OnDisposedShouldBeCalledOncePerDisposableInOrder()
        {
            var disposableReporter = Mock.Create<IDisposableReporter>();
            var disposable1 = Mock.Create<IDisposable>();
            var disposable2 = Mock.Create<IDisposable>();
            Mock.Arrange(() => disposableReporter.Disposed(disposable1)).InOrder().Occurs(1);
            Mock.Arrange(() => disposableReporter.Disposed(disposable2)).InOrder().Occurs(1);

            var testee = new Disposer(disposableReporter, disposable1, disposable2);

            await testee.DisposeAsync().ConfigureAwait(false);

            Mock.Assert(disposableReporter);
        }

        [Fact]
        public async Task DisposeAsync_Then_OnDisposedAndOnDisposedAsyncShouldBeCalledOnceInOrder()
        {
            var disposableReporter = Mock.Create<IDisposableReporter>();
            var disposable = Mock.Create<IDisposable>();
            var asyncDisposable = Mock.Create<IAsyncDisposable>();
            Mock.Arrange(() => disposableReporter.Disposed(disposable)).InOrder().Occurs(1);
            Mock.Arrange(() => disposableReporter.Disposed(asyncDisposable)).InOrder().Occurs(1);
            var disposer = Disposer.Create(
                disposerBuilder =>
                {
                    disposerBuilder.Add(disposable);
                    disposerBuilder.AddAsync(asyncDisposable);
                },
                disposableReporter);

            await disposer.DisposeAsync().ConfigureAwait(false);

            Mock.Assert(disposableReporter);
        }

        [Fact]
        public void Dispose_When_SetReporterWasCalledMultipleTimes_Then_OnDisposedShouldBeCalledInOrder()
        {
            var callOrder = new List<IDisposableReporter>();
            var disposable = Mock.Create<IDisposable>();
            var disposableReporter1 = Mock.Create<IDisposableReporter>();
            var disposableReporter2 = Mock.Create<IDisposableReporter>();
            var disposableReporter3 = Mock.Create<IDisposableReporter>();
            Mock.Arrange(() => disposableReporter1.Disposed(disposable)).DoInstead(() => callOrder.Add(disposableReporter1));
            Mock.Arrange(() => disposableReporter2.Disposed(disposable)).DoInstead(() => callOrder.Add(disposableReporter2));
            Mock.Arrange(() => disposableReporter3.Disposed(disposable)).DoInstead(() => callOrder.Add(disposableReporter3));
            var testee = new Disposer(disposableReporter1, disposable);
            ((IReportingDisposable)testee).SetReporter(disposableReporter2);
            ((IReportingDisposable)testee).SetReporter(disposableReporter3);

            testee.Dispose();

            callOrder.Should().Equal(disposableReporter1, disposableReporter2, disposableReporter3);
        }
    }
}