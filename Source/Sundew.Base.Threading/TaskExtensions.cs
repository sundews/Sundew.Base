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
/// Provides helpers for synchronously running asynchronous work using a <see cref="JoinableTaskFactory"/>.
/// </summary>
/// <remarks>
/// The asynchronous work is started inside the <see cref="JoinableTaskFactory"/>, so continuations that would otherwise be posted to a blocked thread are processed by the blocked thread instead.
/// Tasks started before calling these methods are not covered, which is why only functions are accepted.
/// </remarks>
public static class TaskExtensions
{
    /// <summary>
    /// Extends <see cref="Task"/> with static members for synchronously running asynchronous work.
    /// </summary>
    extension(Task)
    {
        /// <summary>
        /// Blocks the calling thread while invoking the specified function and waiting for its task to complete.
        /// </summary>
        /// <remarks>
        /// Uses <see cref="MainThreadContext.Current"/> if available, otherwise a <see cref="JoinableTaskContext"/> treating the calling thread as the main thread is created for the call.
        /// </remarks>
        /// <param name="func">The function starting the asynchronous work.</param>
        public static void RunAndJoin(Func<Task> func)
        {
            var mainThreadContext = MainThreadContext.Current;
            if (mainThreadContext.HasValue)
            {
                mainThreadContext.Factory.Run(func);
                return;
            }

            using var context = new JoinableTaskContext();
            context.Factory.Run(func);
        }

        /// <summary>
        /// Blocks the calling thread while invoking the specified function and waiting for its task to complete.
        /// </summary>
        /// <remarks>
        /// Uses <see cref="MainThreadContext.Current"/> if available, otherwise a <see cref="JoinableTaskContext"/> treating the calling thread as the main thread is created for the call.
        /// </remarks>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="func">The function starting the asynchronous work.</param>
        /// <returns>The task result.</returns>
        public static TResult RunAndJoin<TResult>(Func<Task<TResult>> func)
        {
            var mainThreadContext = MainThreadContext.Current;
            if (mainThreadContext.HasValue)
            {
                return mainThreadContext.Factory.Run(func);
            }

            using var context = new JoinableTaskContext();
            return context.Factory.Run(func);
        }
    }
}
