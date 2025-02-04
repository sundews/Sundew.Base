// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuestStart.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base;

using System.Runtime.CompilerServices;
using System.Threading.Tasks;

/// <summary>
/// Represents the result of a quest start.
/// </summary>
/// <param name="WasStarted"><c>true</c>, if the quest was started, <c>false</c> if it was already running.</param>
/// <param name="Task">The task.</param>
public readonly record struct QuestStart(bool WasStarted, Task Task)
{
    /// <summary>
    /// Gets the task awaiter.
    /// </summary>
    /// <returns>The task awaiter.</returns>
    public TaskAwaiter GetAwaiter() => this.Task.GetAwaiter();

    /// <summary>
    /// Configures the await.
    /// </summary>
    /// <param name="continueOnCapturedContext">The continueOnCapturedContext.</param>
    /// <returns>A <see cref="ConfiguredTaskAwaitable"/>.</returns>
    public ConfiguredTaskAwaitable ConfigureAwait(bool continueOnCapturedContext) => this.Task.ConfigureAwait(continueOnCapturedContext);
}