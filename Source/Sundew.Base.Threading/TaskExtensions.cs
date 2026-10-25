// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading;

using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;

/// <summary>
/// Provides helpers for joining asynchronous work on a dedicated <see cref="JoinableTaskContext"/>.
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// Extends functions that return a task to provide a synchronous join operation.
    /// </summary>
    /// <typeparam name="TResult">The task result type.</typeparam>
    extension<TResult>(Func<Task<TResult>> func)
    {
        /// <summary>
        /// Invokes the supplied factory and waits for the result to complete on a joinable context.
        /// </summary>
        /// <returns>The task result.</returns>
#pragma warning disable SA1101
        public TResult AsyncJoin()
#pragma warning restore SA1101
        {
            using var context = new JoinableTaskContext();
#pragma warning disable SA1101
            var joinableTask = context.Factory.RunAsync(async () => await func().ConfigureAwait(false));
#pragma warning restore SA1101
            var result = joinableTask.Join();
            return result;
        }
    }

    /// <summary>
    /// Extends functions that return a task to provide a synchronous join operation.
    /// </summary>
    extension(Func<Task> func)
    {
        /// <summary>
        /// Invokes the supplied factory and waits for the task to complete on a joinable context.
        /// </summary>
#pragma warning disable SA1101
        public void AsyncJoin()
#pragma warning restore SA1101
        {
            using var context = new JoinableTaskContext();
#pragma warning disable SA1101
            var joinableTask = context.Factory.RunAsync(async () => await func().ConfigureAwait(false));
#pragma warning restore SA1101
            joinableTask.Join();
        }
    }

    /// <summary>
    /// Synchronously waits for the supplied task to complete while running it inside a joinable context.
    /// </summary>
    /// <typeparam name="TResult">The task result type.</typeparam>
    /// <param name="task">The task to await synchronously.</param>
    /// <returns>The task result when the operation completes.</returns>
    public static TResult AsyncJoin<TResult>(this Task<TResult> task)
    {
        using var context = new JoinableTaskContext();
        var joinableTask = context.Factory.RunAsync(async () => await task.ConfigureAwait(false));
        var result = joinableTask.Join();
        return result;
    }

    /// <summary>
    /// Synchronously waits for the supplied task to complete while running it inside a joinable context.
    /// </summary>
    /// <param name="task">The task to await synchronously.</param>
    public static void AsyncJoin(this Task task)
    {
        using var context = new JoinableTaskContext();
        var joinableTask = context.Factory.RunAsync(async () => await task.ConfigureAwait(false));
        joinableTask.Join();
    }
}
