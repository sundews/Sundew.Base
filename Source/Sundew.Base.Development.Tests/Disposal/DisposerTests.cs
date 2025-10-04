// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposerTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Disposal;

using System;
using System.Threading.Tasks;
using global::Disposal.Interfaces;
using Sundew.Base.Disposal;
using Telerik.JustMock;
using Xunit;

public class DisposerTests
{
    [Fact]
    public void Dispose_Then_DisposedShouldBeCalledOncePerDisposableInOrder()
    {
        var disposableReporter = Mock.Create<IDisposalReporter>();
        var disposable1 = Mock.Create<IDisposable>();
        var disposable2 = Mock.Create<IDisposable>();
        Mock.Arrange(() => disposableReporter.Disposed(Arg.AnyObject, disposable1)).InOrder().Occurs(1);
        Mock.Arrange(() => disposableReporter.Disposed(Arg.AnyObject, disposable2)).InOrder().Occurs(1);

        var testee = new Disposer.SynchronousDisposables(disposable1, disposable2);

        testee.Dispose(disposableReporter);

        Mock.Assert(disposableReporter);
    }

    [Fact]
    public void Dispose_Then_DisposedAndOnDisposedAsyncShouldBeCalledOnceInOrder()
    {
        var disposableReporter = Mock.Create<IDisposalReporter>();
        var disposable = Mock.Create<IDisposable>();
        var asyncDisposable = Mock.Create<IAsyncDisposable>();
        Mock.Arrange(() => disposableReporter.Disposed(Arg.AnyObject, disposable)).InOrder().Occurs(1);
        Mock.Arrange(() => disposableReporter.Disposed(Arg.AnyObject, asyncDisposable)).InOrder().Occurs(1);
        var disposer = new Disposer.Disposers(new Disposer.Synchronous(disposable), new Disposer.Asynchronous(asyncDisposable));

        disposer.Dispose(disposableReporter);

        Mock.Assert(disposableReporter);
    }

    [Fact]
    public async Task DisposeAsync_Then_DisposedShouldBeCalledOncePerDisposableInOrder()
    {
        var disposableReporter = Mock.Create<IDisposalReporter>();
        var disposable1 = Mock.Create<IDisposable>();
        var disposable2 = Mock.Create<IDisposable>();
        Mock.Arrange(() => disposableReporter.Disposed(Arg.AnyObject, disposable1)).InOrder().Occurs(1);
        Mock.Arrange(() => disposableReporter.Disposed(Arg.AnyObject, disposable2)).InOrder().Occurs(1);

        var testee = new Disposer.SynchronousDisposables(disposable1, disposable2);

        await testee.DisposeAsync(disposableReporter);

        Mock.Assert(disposableReporter);
    }

    [Fact]
    public async Task DisposeAsync_Then_DisposedAndOnDisposedAsyncShouldBeCalledOnceInOrder()
    {
        var disposableReporter = Mock.Create<IDisposalReporter>();
        var disposable = Mock.Create<IDisposable>();
        var asyncDisposable = Mock.Create<IAsyncDisposable>();
        Mock.Arrange(() => disposableReporter.Disposed(Arg.AnyObject, disposable)).InOrder().Occurs(1);
        Mock.Arrange(() => disposableReporter.Disposed(Arg.AnyObject, asyncDisposable)).InOrder().Occurs(1);
        var disposer = new Disposer.Disposers(new Disposer.Synchronous(disposable), new Disposer.Asynchronous(asyncDisposable));

        await disposer.DisposeAsync(disposableReporter);

        Mock.Assert(disposableReporter);
    }
}