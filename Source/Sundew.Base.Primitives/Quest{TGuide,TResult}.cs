// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Quest{TGuide,TResult}.cs" company="Sundews">
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
/// Represents a quest for processing some type of operation.
/// </summary>
/// <typeparam name="TGuide">The Guide type.</typeparam>
/// <typeparam name="TResult">The Result type.</typeparam>
public sealed class Quest<TGuide, TResult> : IAsyncDisposable, IDisposable
{
    private readonly Task<TResult> task;
    private readonly Task<TResult> continuation;
    private readonly object? disposable;
    private readonly CancellationToken cancellationToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="Quest{TGuide, TResult}"/> class.
    /// </summary>
    /// <param name="guide">The guide.</param>
    /// <param name="task">The task.</param>
    /// <param name="disposable">The disposable.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    internal Quest(TGuide guide, Task<TResult> task, object? disposable, CancellationToken cancellationToken)
    {
        this.Guide = guide;
        this.task = task;
        this.continuation = this.task.ContinueWith(this.Completion, cancellationToken);
        this.disposable = ReferenceEquals(guide, disposable) ? default : disposable;
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
    /// Converts from a  <see cref="Quest{TGuide}"/> to an <see cref="Task"/>.
    /// </summary>
    /// <param name="quest">The quest.</param>
    public static implicit operator Task(Quest<TGuide, TResult> quest)
    {
        return quest.Task;
    }

    /// <summary>
    /// Converts from a  <see cref="Quest{TGuide}"/> to an <see cref="Task{TResult}"/>.
    /// </summary>
    /// <param name="quest">The quest.</param>
    public static implicit operator Task<TResult>(Quest<TGuide, TResult> quest)
    {
        return quest.Task;
    }

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

    /// <inheritdoc/>
    public void Dispose()
    {
        if (this.task.Status != TaskStatus.Created)
        {
            try
            {
                this.task.Wait(this.cancellationToken);
                var result = this.task.Result;
                Quest.TryDispose(result);
            }
            catch (OperationCanceledException)
            {
            }
        }

        Quest.TryDispose(this.Guide);
        Quest.TryDispose(this.disposable);
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (this.task.Status != TaskStatus.Created)
        {
            try
            {
                var result = await this.task.ConfigureAwait(false);
                await Quest.TryDisposeAsync(result).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
        }

        await Quest.TryDisposeAsync(this.Guide).ConfigureAwait(false);
        await Quest.TryDisposeAsync(this.disposable).ConfigureAwait(false);
    }

    private TResult Completion(Task<TResult> task)
    {
        if (task is { IsFaulted: true, Exception: not null })
        {
            throw task.Exception!;
        }

        task.Wait(this.cancellationToken);
        var result = task.Result;
        return result;
    }
}