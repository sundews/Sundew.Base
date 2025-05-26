// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoResetEventAsync.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading;

using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// An asynchronous auto reset event.
/// </summary>
public sealed class AutoResetEventAsync : ResetEventAsync
{
    private readonly LinkedList<TaskCompletionSource<bool>> taskCompletionSources = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoResetEventAsync"/> class.
    /// </summary>
    public AutoResetEventAsync()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoResetEventAsync"/> class.
    /// </summary>
    /// <param name="isSet">if set to <c>true</c> [initial state].</param>
    public AutoResetEventAsync(bool isSet)
        : base(isSet)
    {
    }

    /// <summary>
    /// Sets the event and signals all waiters.
    /// </summary>
    public override void Set()
    {
        LinkedListNode<TaskCompletionSource<bool>>? taskCompletionSource;
        lock (this.lockObject)
        {
            taskCompletionSource = this.taskCompletionSources.First;
            this.privateIsSet = !taskCompletionSource.HasValue();
        }

        if (!taskCompletionSource.HasValue())
        {
            return;
        }

        taskCompletionSource.Value.TrySetResult(true);
    }

    /// <summary>
    /// Called during a Wait or WaitAsync call when the event is set and the lock is acquired.
    /// </summary>
    /// <param name="isSet">if set to <c>true</c> [is set].</param>
    private protected override void OnIsSetDuringWaitWhileLocked(ref bool isSet)
    {
        isSet = false;
    }

    private protected override Task<bool> ConfigureTaskWhileLocked(Cancellation externalCancellation)
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();
        var enabler = externalCancellation.EnableCancellation();
        enabler.Token.Register(() => taskCompletionSource.TrySetResult(false));
        taskCompletionSource.Task.ContinueWith(_ =>
        {
            lock (this.lockObject)
            {
                this.taskCompletionSources.Remove(taskCompletionSource);
            }

            enabler.Dispose();
        });
        this.taskCompletionSources.AddLast(taskCompletionSource);

        return taskCompletionSource.Task;
    }
}