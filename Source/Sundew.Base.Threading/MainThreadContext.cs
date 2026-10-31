// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainThreadContext.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading;

using System;
using System.Threading;
using Microsoft.VisualStudio.Threading;

/// <summary>
/// Holds the application's <see cref="JoinableTaskContext"/> instances, one per main thread (e.g. one per UI thread with its own dispatcher).
/// </summary>
/// <remarks>
/// The application creates, registers, unregisters and disposes the contexts, typically on each main thread at startup and shutdown.
/// Library code only looks them up through <see cref="Current"/>.
/// Contexts whose main thread has terminated are removed automatically.
/// </remarks>
public static class MainThreadContext
{
    // Copy on write: readers use the current array without locking, writers replace it atomically.
    private static JoinableTaskContext[] registeredContexts = [];

    /// <summary>
    /// Gets the <see cref="JoinableTaskContext"/> applicable for the caller, if any.
    /// </summary>
    /// <remarks>
    /// When called on a registered main thread, the context of that thread is returned.
    /// Otherwise, the context in which the caller is running a joinable task is returned, if exactly one registered context applies.
    /// </remarks>
    public static JoinableTaskContext? Current
    {
        get
        {
            var contexts = Volatile.Read(ref registeredContexts);
            var hasTerminatedMainThreads = false;
            JoinableTaskContext? ambientContext = null;
            var isAmbiguous = false;
            foreach (var context in contexts)
            {
                if (!context.MainThread.IsAlive)
                {
                    hasTerminatedMainThreads = true;
                    continue;
                }

                if (context.IsOnMainThread)
                {
                    return context;
                }

                if (context.IsWithinJoinableTask)
                {
                    isAmbiguous = ambientContext != null;
                    ambientContext = context;
                }
            }

            if (hasTerminatedMainThreads)
            {
                RemoveTerminatedMainThreads();
            }

            return isAmbiguous ? null : ambientContext;
        }
    }

    /// <summary>
    /// Registers the specified <see cref="JoinableTaskContext"/> for its main thread.
    /// </summary>
    /// <remarks>The context is still owned by the caller, which is responsible for disposing it.</remarks>
    /// <param name="joinableTaskContext">The joinable task context.</param>
    /// <exception cref="InvalidOperationException">Thrown if a context is already registered for the same main thread.</exception>
    public static void Register(JoinableTaskContext joinableTaskContext)
    {
        Update(contextsSnapshot =>
        {
            foreach (var context in contextsSnapshot)
            {
                if (context.MainThread == joinableTaskContext.MainThread)
                {
                    throw new InvalidOperationException($"A {nameof(JoinableTaskContext)} is already registered for the main thread: {joinableTaskContext.MainThread.ManagedThreadId}.");
                }
            }

            var contexts = new JoinableTaskContext[contextsSnapshot.Length + 1];
            contextsSnapshot.CopyTo(contexts, 0);
            contexts[^1] = joinableTaskContext;
            return contexts;
        });
    }

    /// <summary>
    /// Unregisters the specified <see cref="JoinableTaskContext"/>.
    /// </summary>
    /// <remarks>The context is not disposed.</remarks>
    /// <param name="joinableTaskContext">The joinable task context.</param>
    /// <returns><c>true</c>, if the context was registered, otherwise <c>false</c>.</returns>
    public static bool Unregister(JoinableTaskContext joinableTaskContext)
    {
        return Update(contextsSnapshot =>
        {
            var index = Array.IndexOf(contextsSnapshot, joinableTaskContext);
            if (index < 0)
            {
                return null;
            }

            var contexts = new JoinableTaskContext[contextsSnapshot.Length - 1];
            Array.Copy(contextsSnapshot, 0, contexts, 0, index);
            Array.Copy(contextsSnapshot, index + 1, contexts, index, contexts.Length - index);
            return contexts;
        });
    }

    private static void RemoveTerminatedMainThreads()
    {
        Update(static contextsSnapshot => Array.FindAll(contextsSnapshot, context => context.MainThread.IsAlive));
    }

    /// <summary>
    /// Atomically replaces the registered contexts, retrying if another thread replaced them concurrently.
    /// </summary>
    /// <param name="getNewContexts">Gets the new contexts from a snapshot of the current ones, or <c>null</c> to leave them unchanged.</param>
    /// <returns><c>true</c>, if the contexts were replaced, otherwise <c>false</c>.</returns>
    private static bool Update(Func<JoinableTaskContext[], JoinableTaskContext[]?> getNewContexts)
    {
        while (true)
        {
            var contextsSnapshot = Volatile.Read(ref registeredContexts);
            var newContexts = getNewContexts(contextsSnapshot);
            if (newContexts == null)
            {
                return false;
            }

            if (Interlocked.CompareExchange(ref registeredContexts, newContexts, contextsSnapshot) == contextsSnapshot)
            {
                return true;
            }
        }
    }
}
