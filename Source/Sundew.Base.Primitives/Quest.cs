// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Quest.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base;

using System;
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
    /// <param name="startFunc">The start func.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The quest.</returns>
    public static Quest<TGuide> Create<TGuide>(TGuide guide, Func<CancellationToken, Task> startFunc, CancellationToken cancellationToken = default)
    {
        return new Quest<TGuide>(guide, startFunc, default, cancellationToken);
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
        return new Quest<TGuide>(guide, startFunc, disposable, cancellationToken);
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
        return new Quest<TGuide, TResult>(guide, startFunc, default, cancellationToken);
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
        return new Quest<TGuide, TResult>(guide, startFunc, disposable, cancellationToken);
    }
}