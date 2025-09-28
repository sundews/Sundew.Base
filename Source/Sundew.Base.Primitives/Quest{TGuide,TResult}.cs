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
using static Sundew.Base.Quest;

/// <summary>
/// Represents a quest for processing some type of operation.
/// </summary>
/// <typeparam name="TGuide">The Guide type.</typeparam>
/// <typeparam name="TResult">The Result type.</typeparam>
public sealed class Quest<TGuide, TResult> : IQuest
{
    private readonly Task<TResult> task;
    private readonly Task<TResult> continuation;
    private readonly TryDisposeAction<TGuide, IDisposable> disposable;
    private readonly CancellationToken cancellationToken;
    private readonly IQuest? ancestorQuest;

    /// <summary>
    /// Initializes a new instance of the <see cref="Quest{TGuide, TResult}"/> class.
    /// </summary>
    /// <param name="guide">The guide.</param>
    /// <param name="task">The task.</param>
    /// <param name="disposable">The disposable.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="ancestorQuest">The ancestor quest.</param>
    internal Quest(TGuide guide, Task<TResult> task, TryDisposeAction<TGuide, IDisposable> disposable, CancellationToken cancellationToken, IQuest? ancestorQuest)
    {
        this.Guide = guide;
        this.task = task;
        this.continuation = this.task.ContinueWith(this.Completion, cancellationToken);
        this.disposable = disposable;
        this.cancellationToken = cancellationToken;
        this.ancestorQuest = ancestorQuest;
    }

    /// <summary>
    /// Gets the Guide.
    /// </summary>
    public TGuide Guide { get; }

    /// <summary>
    /// Gets the goal task.
    /// </summary>
    public Task<TResult> Task => this.continuation;

    /// <summary>
    /// Gets a value indicating whether the quest has started.
    /// </summary>
    public bool IsStarted
    {
        get
        {
            if (this.ancestorQuest.HasValue)
            {
                return this.ancestorQuest.IsStarted;
            }

            return this.task.Status != TaskStatus.Created;
        }
    }

    /// <summary>
    /// Gets the goal task.
    /// </summary>
    Task IQuest.Task => this.Task;

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
        if (this.ancestorQuest != null)
        {
            var ancestorStart = this.ancestorQuest.Start();
            return ancestorStart.Map(x => x with { Task = this.continuation });
        }

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
    /// Transforms the result of the current quest into a new result by applying the specified asynchronous function.
    /// </summary>
    /// <remarks>The transformation is performed asynchronously, ensuring that the current quest completes
    /// before invoking the <paramref name="nextQuest"/> function. The returned quest maintains the same guide and
    /// cancellation token as the original quest.</remarks>
    /// <typeparam name="TNewGuide">The type of the new guide.</typeparam>
    /// <typeparam name="TNewResult">The type of the result produced by the next quest.</typeparam>
    /// <param name="nextGuide">The next guide.</param>
    /// <param name="nextQuest">A function that defines the next quest, returning a task that produces the new result.</param>
    /// <returns>A new <see cref="Quest{TNewGuide, TNewResult}"/> instance representing the transformed quest.</returns>
    public Quest<TNewGuide, TNewResult> Map<TNewGuide, TNewResult>(Func<TGuide, TNewGuide> nextGuide, Func<TResult, Task<TNewResult>> nextQuest)
    {
        async Task<TNewResult> Continuation()
        {
            var result = await this.Task.ConfigureAwait(false);
            return await nextQuest(result).ConfigureAwait(false);
        }

        var newGuide = nextGuide(this.Guide);
        return new Quest<TNewGuide, TNewResult>(
            newGuide,
            Continuation(),
            new TryDisposeAction<TNewGuide, IDisposable>(newGuide, null),
            this.cancellationToken,
            this);
    }

    /// <summary>
    /// Transforms the result of the current quest into a new result by applying the specified asynchronous function.
    /// </summary>
    /// <remarks>The transformation is performed asynchronously, ensuring that the current quest completes
    /// before invoking the <paramref name="nextQuest"/> function. The returned quest maintains the same guide and
    /// cancellation token as the original quest.</remarks>
    /// <typeparam name="TNewGuide">The type of the new guide.</typeparam>
    /// <typeparam name="TNewResult">The type of the result produced by the next quest.</typeparam>
    /// <param name="nextGuide">The next guide.</param>
    /// <param name="nextQuest">A function that defines the next quest, returning a task that produces the new result.</param>
    /// <returns>A new <see cref="Quest{TNewGuide, TNewResult}"/> instance representing the transformed quest.</returns>
    public Quest<TNewGuide, TNewResult> Map<TNewGuide, TNewResult>(Func<TGuide, TNewGuide> nextGuide, Func<TResult, TNewResult> nextQuest)
    {
        async Task<TNewResult> Continuation()
        {
            var result = await this.Task.ConfigureAwait(false);
            return nextQuest(result);
        }

        var newGuide = nextGuide(this.Guide);
        return new Quest<TNewGuide, TNewResult>(
            newGuide,
            Continuation(),
            new TryDisposeAction<TNewGuide, IDisposable>(newGuide, null),
            this.cancellationToken,
            this);
    }

    /// <summary>
    /// Transforms the result of the current quest into a new result by applying the specified asynchronous function.
    /// </summary>
    /// <remarks>The transformation is performed asynchronously, ensuring that the current quest completes
    /// before invoking the <paramref name="nextQuest"/> function. The returned quest maintains the same guide and
    /// cancellation token as the original quest.</remarks>
    /// <typeparam name="TNewResult">The type of the result produced by the next quest.</typeparam>
    /// <param name="nextQuest">A function that defines the next quest, returning a task that produces the new result.</param>
    /// <returns>A new <see cref="Quest{TGuide, TNewResult}"/> instance representing the transformed quest.</returns>
    public Quest<TGuide, TNewResult> Map<TNewResult>(Func<TResult, Task<TNewResult>> nextQuest)
    {
        async Task<TNewResult> Continuation()
        {
            var result = await this.Task.ConfigureAwait(false);
            return await nextQuest(result).ConfigureAwait(false);
        }

        return new Quest<TGuide, TNewResult>(
            this.Guide,
            Continuation(),
            new TryDisposeAction<TGuide, IDisposable>(default, null),
            this.cancellationToken,
            this);
    }

    /// <summary>
    /// Transforms the result of the current quest into a new result by applying the specified asynchronous function.
    /// </summary>
    /// <remarks>The transformation is performed asynchronously, ensuring that the current quest completes
    /// before invoking the <paramref name="nextQuest"/> function. The returned quest maintains the same guide and
    /// cancellation token as the original quest.</remarks>
    /// <typeparam name="TNewResult">The type of the result produced by the next quest.</typeparam>
    /// <param name="nextQuest">A function that defines the next quest, returning a task that produces the new result.</param>
    /// <returns>A new <see cref="Quest{TGuide, TNewResult}"/> instance representing the transformed quest.</returns>
    public Quest<TGuide, TNewResult> Map<TNewResult>(Func<TResult, TNewResult> nextQuest)
    {
        async Task<TNewResult> Continuation()
        {
            var result = await this.Task.ConfigureAwait(false);
            return nextQuest(result);
        }

        return new Quest<TGuide, TNewResult>(
            this.Guide,
            Continuation(),
            new TryDisposeAction<TGuide, IDisposable>(default, null),
            this.cancellationToken,
            this);
    }

    /// <summary>
    /// Transforms the result of the current quest into a new result by applying the specified asynchronous function.
    /// </summary>
    /// <remarks>The transformation is performed asynchronously, ensuring that the current quest completes
    /// before invoking the <paramref name="nextQuest"/> function. The returned quest maintains the same guide and
    /// cancellation token as the original quest.</remarks>
    /// <typeparam name="TNewGuide">The type of the new guide.</typeparam>
    /// <param name="nextGuide">The next guide.</param>
    /// <param name="nextQuest">A function that defines the next quest, returning a task that produces the new result.</param>
    /// <returns>A new <see cref="Quest{TNewGuide}"/> instance representing the transformed quest.</returns>
    public Quest<TNewGuide> Map<TNewGuide>(Func<TGuide, TNewGuide> nextGuide, Func<TResult, Task> nextQuest)
    {
        async Task Continuation()
        {
            var result = await this.Task.ConfigureAwait(false);
            await nextQuest(result).ConfigureAwait(false);
        }

        var newGuide = nextGuide(this.Guide);
        return new Quest<TNewGuide>(
            newGuide,
            Continuation(),
            new TryDisposeAction<TNewGuide, IDisposable>(newGuide, null),
            this.cancellationToken,
            this);
    }

    /// <summary>
    /// Transforms the result of the current quest into a new result by applying the specified asynchronous function.
    /// </summary>
    /// <remarks>The transformation is performed asynchronously, ensuring that the current quest completes
    /// before invoking the <paramref name="nextQuest"/> function. The returned quest maintains the same guide and
    /// cancellation token as the original quest.</remarks>
    /// <param name="nextQuest">A function that defines the next quest, returning a task that produces the new result.</param>
    /// <returns>A new <see cref="Quest{TGuide}"/> instance representing the transformed quest.</returns>
    public Quest<TGuide> Map(Func<TResult, Task> nextQuest)
    {
        async Task Continuation()
        {
            var result = await this.Task.ConfigureAwait(false);
            await nextQuest(result).ConfigureAwait(false);
        }

        return new Quest<TGuide>(
            this.Guide,
            Continuation(),
            new TryDisposeAction<TGuide, IDisposable>(default, null),
            this.cancellationToken,
            this);
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
        if (this.IsStarted)
        {
            try
            {
                this.task.Wait(this.cancellationToken);
            }
            catch (OperationCanceledException)
            {
            }
        }

        this.ancestorQuest?.Dispose();
        Quest.TryDispose(this.disposable);
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (this.IsStarted)
        {
            try
            {
                await this.task.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
        }

        if (this.ancestorQuest != null)
        {
            await this.ancestorQuest.DisposeAsync().ConfigureAwait(false);
        }

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