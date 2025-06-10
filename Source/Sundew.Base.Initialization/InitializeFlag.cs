// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializeFlag.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Initialization;

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

/// <summary>
/// Represents a thread safe initialize flag.
/// </summary>
public sealed class InitializeFlag
{
    private readonly TaskCompletionSource<bool> taskCompletionSource = new();

    /// <summary>
    /// Occurs when [initialized].
    /// </summary>
    public event EventHandler? Initialized;

    /// <summary>
    /// Gets a value indicating whether this instance is initialized.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
    /// </value>
    public bool IsInitialized => this.taskCompletionSource.Task is { Status: TaskStatus.RanToCompletion, Result: true };

    /// <summary>
    /// Performs an implicit conversion from <see cref="InitializeFlag" /> to <see cref="bool" />.
    /// </summary>
    /// <param name="initializeFlag">The interlocked boolean.</param>
    /// <value>
    ///   <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
    /// </value>
    public static implicit operator bool(InitializeFlag initializeFlag)
    {
        return initializeFlag.IsInitialized;
    }

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <returns>A value indicating whether the flag was just initialized.</returns>
    public bool Initialize()
    {
        var result = this.taskCompletionSource.TrySetResult(true);
        if (result)
        {
            this.Initialized?.Invoke(this, EventArgs.Empty);
        }

        return result;
    }

    /// <summary>
    /// Awaits the initialization of the flag.
    /// </summary>
    /// <returns>A <see cref="TaskAwaiter"/>.</returns>
    public TaskAwaiter<bool> GetAwaiter()
    {
        return this.taskCompletionSource.Task.GetAwaiter();
    }

    /// <summary>
    /// Enhances the initialization with cancellation support.
    /// </summary>
    /// <param name="cancellation">The cancellation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<bool> WhenInitialized(Cancellation cancellation = default)
    {
        var enabler = cancellation.EnableCancellation();
        enabler.Register(x => this.taskCompletionSource.TrySetResult(false));
        return this.taskCompletionSource.Task.ContinueWith(
            task =>
            {
                try
                {
                    return task.Result;
                }
                finally
                {
                    enabler.Dispose();
                }
            },
            TaskScheduler.Default);
    }
}