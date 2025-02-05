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
        return new Quest<TGuide>(guide, task, default, cancellationToken);
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
        return new Quest<TGuide>(guide, task, disposable, cancellationToken);
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
        return new Quest<TGuide>(guide, new Task(() => startFunc(cancellationToken)), default, cancellationToken);
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
        return new Quest<TGuide>(guide, new Task(startAction), default, cancellationToken);
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
        return new Quest<TGuide>(guide, new Task(() => startFunc(cancellationToken)), disposable, cancellationToken);
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
        return new Quest<TGuide>(guide, new Task(startAction), disposable, cancellationToken);
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
        return new Quest<TGuide, TResult>(guide, task, default, cancellationToken);
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
        return new Quest<TGuide, TResult>(guide, task, disposable, cancellationToken);
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
        return new Quest<TGuide, TResult>(guide, new Task<TResult>(startFunc), default, cancellationToken);
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
        return new Quest<TGuide, TResult>(guide, new Task<TResult>(() => startFunc(cancellationToken).Result), default, cancellationToken);
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
        return new Quest<TGuide, TResult>(guide, new Task<TResult>(startFunc), disposable, cancellationToken);
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
        return new Quest<TGuide, TResult>(guide, new Task<TResult>(() => startFunc(cancellationToken).Result), disposable, cancellationToken);
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
}