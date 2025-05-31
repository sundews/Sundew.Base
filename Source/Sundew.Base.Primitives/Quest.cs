// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Quest.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base;

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Factory for creating quests.
/// </summary>
public static class Quest
{
    /// <summary>
    /// Creates a quest.
    /// </summary>
    /// <typeparam name="TGuide">The guide type.</typeparam>
    /// <param name="guide">The guide.</param>
    /// <param name="task">The task.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The quest.</returns>
    public static Quest<TGuide> Create<TGuide>(TGuide guide, Task task, CancellationToken cancellationToken = default)
    {
        return new Quest<TGuide>(guide, task, new TryDisposeAction<TGuide, IDisposable>(guide, null), cancellationToken, null);
    }

    /// <summary>
    /// Creates a quest.
    /// </summary>
    /// <typeparam name="TGuide">The guide type.</typeparam>
    /// <param name="guide">The guide.</param>
    /// <param name="task">The task.</param>
    /// <param name="disposable">The disposable.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The quest.</returns>
    public static Quest<TGuide> Create<TGuide>(TGuide guide, Task task, IDisposable? disposable, CancellationToken cancellationToken = default)
    {
        return new Quest<TGuide>(guide, task, new TryDisposeAction<TGuide, IDisposable>(guide, disposable), cancellationToken, null);
    }

    /// <summary>
    /// Creates a quest.
    /// </summary>
    /// <typeparam name="TGuide">The guide type.</typeparam>
    /// <param name="guide">The guide.</param>
    /// <param name="startFunc">The start func.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The quest.</returns>
    public static Quest<TGuide> Create<TGuide>(TGuide guide, Func<CancellationToken, Task> startFunc, CancellationToken cancellationToken = default)
    {
        return new Quest<TGuide>(guide, new Task(() => startFunc(cancellationToken)), new TryDisposeAction<TGuide, IDisposable>(guide, null), cancellationToken, null);
    }

    /// <summary>
    /// Creates a quest.
    /// </summary>
    /// <typeparam name="TGuide">The guide type.</typeparam>
    /// <param name="guide">The guide.</param>
    /// <param name="startAction">The start func.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The quest.</returns>
    [OverloadResolutionPriority(-1)]
    public static Quest<TGuide> Create<TGuide>(TGuide guide, Action startAction, CancellationToken cancellationToken = default)
    {
        return new Quest<TGuide>(guide, new Task(startAction), new TryDisposeAction<TGuide, IDisposable>(guide, null), cancellationToken, null);
    }

    /// <summary>
    /// Creates a quest.
    /// </summary>
    /// <typeparam name="TGuide">The guide type.</typeparam>
    /// <param name="guide">The guide.</param>
    /// <param name="startFunc">The start func.</param>
    /// <param name="disposable">The disposable.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The quest.</returns>
    public static Quest<TGuide> Create<TGuide>(TGuide guide, Func<CancellationToken, Task> startFunc, IDisposable? disposable, CancellationToken cancellationToken = default)
    {
        return new Quest<TGuide>(guide, new Task(() => startFunc(cancellationToken)), new TryDisposeAction<TGuide, IDisposable>(guide, disposable), cancellationToken, null);
    }

    /// <summary>
    /// Creates a quest.
    /// </summary>
    /// <typeparam name="TGuide">The guide type.</typeparam>
    /// <param name="guide">The guide.</param>
    /// <param name="startAction">The start action.</param>
    /// <param name="disposable">The disposable.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The quest.</returns>
    [OverloadResolutionPriority(-1)]
    public static Quest<TGuide> Create<TGuide>(TGuide guide, Action startAction, IDisposable? disposable, CancellationToken cancellationToken = default)
    {
        return new Quest<TGuide>(guide, new Task(startAction), new TryDisposeAction<TGuide, IDisposable>(guide, disposable), cancellationToken, null);
    }

    /// <summary>
    /// Creates a quest.
    /// </summary>
    /// <typeparam name="TGuide">The guide type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="guide">The guide.</param>
    /// <param name="task">The task.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The quest.</returns>
    public static Quest<TGuide, TResult> Create<TGuide, TResult>(TGuide guide, Task<TResult> task, CancellationToken cancellationToken = default)
    {
        return new Quest<TGuide, TResult>(guide, task, new TryDisposeAction<TGuide, IDisposable>(guide, null), cancellationToken, null);
    }

    /// <summary>
    /// Creates a quest.
    /// </summary>
    /// <typeparam name="TGuide">The guide type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="guide">The guide.</param>
    /// <param name="task">The task.</param>
    /// <param name="disposable">The disposable.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The quest.</returns>
    public static Quest<TGuide, TResult> Create<TGuide, TResult>(TGuide guide, Task<TResult> task, IDisposable? disposable, CancellationToken cancellationToken = default)
    {
        return new Quest<TGuide, TResult>(guide, task, new TryDisposeAction<TGuide, IDisposable>(guide, disposable), cancellationToken, null);
    }

    /// <summary>
    /// Creates a quest.
    /// </summary>
    /// <typeparam name="TGuide">The guide type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="guide">The guide.</param>
    /// <param name="startFunc">The start func.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The quest.</returns>
    public static Quest<TGuide, TResult> Create<TGuide, TResult>(TGuide guide, Func<TResult> startFunc, CancellationToken cancellationToken = default)
    {
        return new Quest<TGuide, TResult>(guide, new Task<TResult>(startFunc), new TryDisposeAction<TGuide, IDisposable>(guide, null), cancellationToken, null);
    }

    /// <summary>
    /// Creates a quest.
    /// </summary>
    /// <typeparam name="TGuide">The guide type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="guide">The guide.</param>
    /// <param name="startFunc">The start func.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The quest.</returns>
    public static Quest<TGuide, TResult> Create<TGuide, TResult>(TGuide guide, Func<CancellationToken, Task<TResult>> startFunc, CancellationToken cancellationToken = default)
    {
        return new Quest<TGuide, TResult>(guide, new Task<TResult>(() => startFunc(cancellationToken).Result), new TryDisposeAction<TGuide, IDisposable>(guide, null), cancellationToken, null);
    }

    /// <summary>
    /// Creates a quest.
    /// </summary>
    /// <typeparam name="TGuide">The guide type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="guide">The guide.</param>
    /// <param name="startFunc">The start func.</param>
    /// <param name="disposable">The disposable.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The quest.</returns>
    public static Quest<TGuide, TResult> Create<TGuide, TResult>(TGuide guide, Func<TResult> startFunc, IDisposable? disposable, CancellationToken cancellationToken = default)
    {
        return new Quest<TGuide, TResult>(guide, new Task<TResult>(startFunc), new TryDisposeAction<TGuide, IDisposable>(guide, disposable), cancellationToken, null);
    }

    /// <summary>
    /// Creates a quest.
    /// </summary>
    /// <typeparam name="TGuide">The guide type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="guide">The guide.</param>
    /// <param name="startFunc">The start func.</param>
    /// <param name="disposable">The disposable.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The quest.</returns>
    public static Quest<TGuide, TResult> Create<TGuide, TResult>(TGuide guide, Func<CancellationToken, Task<TResult>> startFunc, IDisposable? disposable, CancellationToken cancellationToken = default)
    {
        return new Quest<TGuide, TResult>(guide, new Task<TResult>(() => startFunc(cancellationToken).Result), new TryDisposeAction<TGuide, IDisposable>(guide, disposable), cancellationToken, null);
    }

    internal static async ValueTask TryDisposeAsync(object? disposableCandidate)
    {
        if (disposableCandidate is IAsyncDisposable asyncDisposable)
        {
            try
            {
                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
        }
        else if (disposableCandidate is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    internal static void TryDispose(object? disposableCandidate)
    {
        if (disposableCandidate is IAsyncDisposable asyncDisposable)
        {
            try
            {
                asyncDisposable.DisposeAsync().AsTask().Wait(CancellationToken.None);
            }
            catch (OperationCanceledException)
            {
            }
        }
        else if (disposableCandidate is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    internal readonly struct TryDisposeAction<TCandidate1, TCandidate2> : IDisposable, IAsyncDisposable
    {
        private readonly TCandidate1? disposableCandidate1;
        private readonly TCandidate2? disposableCandidate2;

        internal TryDisposeAction(TCandidate1? disposableCandidate1, TCandidate2? disposableCandidate2)
        {
            if (!typeof(TCandidate1).IsValueType && !typeof(TCandidate2).IsValueType && ReferenceEquals(disposableCandidate1, disposableCandidate2))
            {
                this.disposableCandidate1 = disposableCandidate1;
            }
            else
            {
                this.disposableCandidate1 = disposableCandidate1;
                this.disposableCandidate2 = disposableCandidate2;
            }
        }

        public void Dispose()
        {
            Quest.TryDispose(this.disposableCandidate1);
            Quest.TryDispose(this.disposableCandidate2);
        }

        public async ValueTask DisposeAsync()
        {
            await Quest.TryDisposeAsync(this.disposableCandidate1).ConfigureAwait(false);
            await Quest.TryDisposeAsync(this.disposableCandidate2).ConfigureAwait(false);
        }
    }
}