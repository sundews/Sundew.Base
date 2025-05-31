// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQuest.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base;

using System;
using System.Threading.Tasks;

/// <summary>
/// Interface for implementing quests.
/// </summary>
public interface IQuest : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Gets the Goal task.
    /// </summary>
    public Task Task { get; }

    /// <summary>
    /// Gets a value indicating whether the quest has started.
    /// </summary>
    public bool IsStarted { get; }

    /// <summary>
    /// Starts the quest.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    R<QuestStart, InvalidOperationException> Start();
}