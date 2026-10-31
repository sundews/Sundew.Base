// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainThreadContextTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Threading;

using System;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Microsoft.VisualStudio.Threading;
using Sundew.Base.Threading;

public class MainThreadContextTests
{
    [Test]
    public void Current_When_NoContextIsRegisteredForCurrentThread_Then_ResultShouldBeNull()
    {
        MainThreadContext.Current.Should().BeNull();
    }

    [Test]
    public void Current_When_ContextIsRegisteredForCurrentThread_Then_ResultShouldBeContext()
    {
        using var context = new JoinableTaskContext();
        MainThreadContext.Register(context);
        try
        {
            MainThreadContext.Current.Should().BeSameAs(context);
        }
        finally
        {
            MainThreadContext.Unregister(context);
        }
    }

    [Test]
    public void Current_When_ContextIsUnregistered_Then_ResultShouldBeNull()
    {
        using var context = new JoinableTaskContext();
        MainThreadContext.Register(context);

        var wasRegistered = MainThreadContext.Unregister(context);

        wasRegistered.Should().BeTrue();
        MainThreadContext.Current.Should().BeNull();
    }

    [Test]
    public void Unregister_When_ContextIsNotRegistered_Then_ResultShouldBeFalse()
    {
        using var context = new JoinableTaskContext();

        var wasRegistered = MainThreadContext.Unregister(context);

        wasRegistered.Should().BeFalse();
    }

    [Test]
    public void Register_When_ContextIsAlreadyRegisteredForMainThread_Then_InvalidOperationExceptionShouldBeThrown()
    {
        using var context = new JoinableTaskContext();
        using var otherContext = new JoinableTaskContext();
        MainThreadContext.Register(context);
        try
        {
            var action = () => MainThreadContext.Register(otherContext);

            action.Should().Throw<InvalidOperationException>();
        }
        finally
        {
            MainThreadContext.Unregister(context);
        }
    }

    [Test]
    public void Current_When_MainThreadHasTerminated_Then_ContextShouldBeUnregistered()
    {
        var mainThread = new Thread(() => { });
        using var context = new JoinableTaskContext(mainThread, new SynchronizationContext());
        mainThread.Start();
        mainThread.Join();
        MainThreadContext.Register(context);

        _ = MainThreadContext.Current;

        MainThreadContext.Unregister(context).Should().BeFalse();
    }

    [Test]
    public void Current_When_OnBackgroundThreadWithinJoinableTaskOfRegisteredContext_Then_ResultShouldBeContext()
    {
        using var context = new JoinableTaskContext();
        MainThreadContext.Register(context);
        try
        {
            var result = context.Factory.Run(() => Task.Run(() => MainThreadContext.Current));

            result.Should().BeSameAs(context);
        }
        finally
        {
            MainThreadContext.Unregister(context);
        }
    }

    [Test]
    public void Current_When_OnBackgroundThreadOutsideJoinableTask_Then_ResultShouldBeNull()
    {
        using var context = new JoinableTaskContext();
        MainThreadContext.Register(context);
        try
        {
            JoinableTaskContext? result = context;
            var thread = new Thread(() => result = MainThreadContext.Current);
            thread.Start();
            thread.Join();

            result.Should().BeNull();
        }
        finally
        {
            MainThreadContext.Unregister(context);
        }
    }

    [Test]
    public void Current_When_OnBackgroundThreadWithinJoinableTasksOfTwoRegisteredContexts_Then_ResultShouldBeNull()
    {
        using var otherMainThreadCanExit = new ManualResetEventSlim();
        var otherMainThread = new Thread(() => otherMainThreadCanExit.Wait());
        otherMainThread.Start();
        using var context = new JoinableTaskContext();
        using var otherContext = new JoinableTaskContext(otherMainThread, new SynchronizationContext());
        MainThreadContext.Register(context);
        MainThreadContext.Register(otherContext);
        try
        {
            var result = context.Factory.Run(async () => await otherContext.Factory.RunAsync(() => Task.Run(() => MainThreadContext.Current)));

            result.Should().BeNull();
        }
        finally
        {
            MainThreadContext.Unregister(context);
            MainThreadContext.Unregister(otherContext);
            otherMainThreadCanExit.Set();
            otherMainThread.Join();
        }
    }
}
