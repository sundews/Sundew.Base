// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Quest{TGuide,TResult}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Represents a quest for processing some type of operation.
/// </summary>
/// <typeparam name="TGuide">The Guide type.</typeparam>
/// <typeparam name="TResult">The Result type.</typeparam>
public sealed class Quest<TGuide, TResult> : IMayBe<IDisposable>
{
    private readonly Task<TResult> task;
    private readonly Task<TResult> continuation;
    private readonly IDisposable? disposable;
    private readonly CancellationToken cancellationToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="Quest{TGuide, TResult}"/> class.
    /// </summary>
    /// <param name="guide">The guide.</param>
    /// <param name="task">The task.</param>
    /// <param name="disposable">The disposable.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    internal Quest(TGuide guide, Task<TResult> task, IDisposable? disposable, CancellationToken cancellationToken)
    {
        this.Guide = guide;
        this.task = task;
        this.continuation = this.task.ContinueWith(this.Completion, cancellationToken);
        this.disposable = Quest.NestedDisposable.Create(disposable, guide);
        this.cancellationToken = cancellationToken;
    }

    /// <summary>
    /// Gets the Guide.
    /// </summary>
    public TGuide Guide { get; }

    /// <summary>
    /// Gets the Goal task.
    /// </summary>
    public Task<TResult> Task => this.continuation;

    /// <summary>
    /// Starts the quest.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public R<QuestStart, InvalidOperationException> Start()
    {
        if (this.task.Status == TaskStatus.Created)
        {
            try
            {
                this.task.Start(TaskScheduler.Default);
                return R.Success(new QuestStart(true, this.continuation));
            }
            catch (InvalidOperationException e)
            {
                return R.Error(e);
            }
        }

        return R.Success(new QuestStart(false, this.continuation));
    }

    /// <summary>
    /// Gets the <see cref="IDisposable"/> if this quest is disposable.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns><c>true</c>, if this quest is disposable, otherwise <c>false</c>.</returns>
    public bool TryGetTarget([NotNullWhen(true)] out IDisposable? target)
    {
        target = this.disposable;
        return target != default;
    }

    /// <summary>
    /// Gets the task awaiter.
    /// </summary>
    /// <returns>The task awaiter.</returns>
    public TaskAwaiter<TResult> GetAwaiter() => this.Task.GetAwaiter();

    /// <summary>
    /// Configures the await.
    /// </summary>
    /// <param name="continueOnCapturedContext">The continueOnCapturedContext.</param>
    /// <returns>A <see cref="ConfiguredTaskAwaitable"/>.</returns>
    public ConfiguredTaskAwaitable<TResult> ConfigureAwait(bool continueOnCapturedContext) => this.Task.ConfigureAwait(continueOnCapturedContext);

    private TResult Completion(Task<TResult> task)
    {
        if (task is { IsFaulted: true, Exception: not null })
        {
            throw task.Exception!;
        }

        task.Wait(this.cancellationToken);
        return task.Result;
    }
}