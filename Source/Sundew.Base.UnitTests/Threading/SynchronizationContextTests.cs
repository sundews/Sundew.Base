// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SynchronizationContextTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Threading;

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Sundew.Base.Threading;
using Xunit;
using SingleThreadedSynchronizationContext = Microsoft.VisualStudio.Threading.SingleThreadedSynchronizationContext;

public class SynchronizationContextTests
{
    [Fact]
    public async Task SendAsync()
    {
        var currentThread = new CurrentThread();
        var currentThreadList = new List<int> { currentThread.ManagedThreadId };
        var synchronizationContext = new SingleThreadedSynchronizationContext();
        var frame = new SingleThreadedSynchronizationContext.Frame();
        var task = Task.Run(async () =>
        {
            await synchronizationContext.SendAsync(() =>
            {
                currentThreadList.Add(currentThread.ManagedThreadId);
            });
            frame.Continue = false;
        });

        synchronizationContext.PushFrame(frame);
        await task.ConfigureAwait(true);
        currentThreadList.Add(currentThread.ManagedThreadId);

        currentThreadList.Should().Equal(new List<int> { currentThread.ManagedThreadId, currentThread.ManagedThreadId, currentThread.ManagedThreadId });
    }
}