// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SynchronizationContextTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Threading;

using System.Collections.Generic;
using System.Threading.Tasks;
using AwesomeAssertions;
using Sundew.Base.Threading;

using SingleThreadedSynchronizationContext = Microsoft.VisualStudio.Threading.SingleThreadedSynchronizationContext;

[Repeat(100)]
public class SynchronizationContextTests
{
    [Test]
    public async Task SendAsync_When_CalledMultipleTimesOnASingleThreadedSynchronizationContext_Then_AllShouldRunOnTheSameThread()
    {
        var currentThread = new CurrentThread();
        var expectedThreadId = currentThread.ManagedThreadId;
        var currentThreadList = new List<int> { currentThread.ManagedThreadId };
        var synchronizationContext = new SingleThreadedSynchronizationContext();
        var frame = new SingleThreadedSynchronizationContext.Frame();
        var pumpStarted = new TaskCompletionSource<bool>();

        var task = Task.Run(async () =>
        {
            // Wait for pump to start
#pragma warning disable VSTHRD003
            await pumpStarted.Task;
#pragma warning restore VSTHRD003

            await synchronizationContext.SendAsync(() =>
            {
                currentThreadList.Add(currentThread.ManagedThreadId);
            });

            await synchronizationContext.SendAsync(() =>
            {
                currentThreadList.Add(currentThread.ManagedThreadId);
            });

            frame.Continue = false;
        });

        // Signal that pump is about to start
        pumpStarted.SetResult(true);
        synchronizationContext.PushFrame(frame);
        await task.ConfigureAwait(true);

        currentThreadList.Should().Equal(new List<int> { expectedThreadId, expectedThreadId, expectedThreadId });
    }
}