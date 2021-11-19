// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposableState.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Disposal;

using System;
using System.Threading.Tasks;
using Sundew.Base.Threading;

/// <summary>
/// Helps implementing the dispose pattern.
/// </summary>
public readonly struct DisposableState
{
    private readonly object target;
    private readonly Flag flag;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisposableState"/> struct.
    /// </summary>
    /// <param name="target">The target.</param>
    public DisposableState(object target)
    {
        this.target = target;
        this.flag = new Flag();
    }

    /// <summary>
    /// Starts the specified is disposing.
    /// </summary>
    /// <param name="isDisposing">if set to <c>true</c> [is disposing].</param>
    /// <param name="disposeAction">The dispose action.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1816:TryDispose methods should call SuppressFinalize", Justification = "It is effectively called by the dispose method.")]
    public void Dispose(bool isDisposing, Action<bool> disposeAction)
    {
        if (!this.flag.Set())
        {
            return;
        }

        disposeAction.Invoke(isDisposing);
        GC.SuppressFinalize(this.target);
    }

    /// <summary>
    /// Starts the disposal asynchronous.
    /// </summary>
    /// <param name="isDisposing">if set to <c>true</c> [is disposing].</param>
    /// <param name="disposeActionAsync">The dispose action asynchronous.</param>
    /// <returns>
    /// A value task.
    /// </returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1816:TryDispose methods should call SuppressFinalize", Justification = "It is effectively called by the dispose method.")]
    public async ValueTask DisposeAsync(bool isDisposing, Func<bool, ValueTask> disposeActionAsync)
    {
        if (!this.flag.Set())
        {
            return;
        }

        await disposeActionAsync.Invoke(isDisposing).ConfigureAwait(false);
        GC.SuppressFinalize(this.target);
    }
}