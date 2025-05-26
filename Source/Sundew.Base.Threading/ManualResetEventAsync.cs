// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManualResetEventAsync.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading;

using System.Threading.Tasks;

/// <summary>
/// An asynchronous manual reset event.
/// </summary>
/// <seealso cref="Sundew.Base.Threading.ResetEventAsync" />
public sealed class ManualResetEventAsync : ResetEventAsync
{
    private TaskCompletionSource<bool>? taskCompletionSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="ManualResetEventAsync"/> class.
    /// </summary>
    public ManualResetEventAsync()
        : this(false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ManualResetEventAsync"/> class.
    /// </summary>
    /// <param name="isSet">if set to <c>true</c> [initial state].</param>
    public ManualResetEventAsync(bool isSet)
        : base(isSet)
    {
    }

    /// <summary>
    /// Sets the event and signals all waiters.
    /// </summary>
    public override void Set()
    {
        lock (this.lockObject)
        {
            this.privateIsSet = true;
            if (this.taskCompletionSource != null)
            {
                this.taskCompletionSource.TrySetResult(true);
            }
        }
    }

    /// <summary>
    /// Called during a Wait or WaitAsync call when the event is set and the lock is acquired.
    /// </summary>
    /// <param name="isSet">if set to <c>true</c> [is set].</param>
    private protected override void OnIsSetDuringWaitWhileLocked(ref bool isSet)
    {
    }

    /// <inheritdoc/>
    private protected override Task<bool> ConfigureAwaiterWhileLocked(Cancellation externalCancellation)
    {
        this.taskCompletionSource ??= new TaskCompletionSource<bool>();
        var enabler = externalCancellation.EnableCancellation();
        enabler.Token.Register(() => this.taskCompletionSource.TrySetResult(false));
        this.taskCompletionSource.Task.ContinueWith(_ => enabler.Dispose());
        return this.taskCompletionSource.Task;
    }

    private protected override void OnResetWhileLocked()
    {
        this.taskCompletionSource = null;
    }
}