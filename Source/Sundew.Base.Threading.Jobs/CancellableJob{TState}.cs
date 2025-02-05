// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CancellableJob{TState}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading.Jobs;

using System;
using System.Threading;
using System.Threading.Tasks;
using Sundew.Base.Threading.Internal;

/// <summary>
/// Delegate for handling job exceptions.
/// </summary>
/// <param name="exception">The exception.</param>
/// <param name="retry">if set to <c>true</c> [continue execution].</param>
public delegate void JobExceptionHandler(Exception exception, ref bool retry);

/// <summary>
/// Represents a cancellable job.
/// </summary>
/// <typeparam name="TState">The type of the state.</typeparam>
public sealed class CancellableJob<TState> : IJob
{
    private readonly AsyncLock @lock = new();
    private readonly Func<TState, CancellationToken, Task> taskAction;
    private readonly JobExceptionHandler? onException;
    private readonly TaskScheduler? taskScheduler;

    private JobContext? jobContext;
    private AggregateException? aggregateException;

    /// <summary>
    /// Initializes a new instance of the <see cref="CancellableJob{TState}" /> class.
    /// </summary>
    /// <param name="taskAction">The asynchronous task action.</param>
    /// <param name="state">The state.</param>
    /// <param name="onException">The action call, if an exception occurs.</param>
    /// <param name="taskScheduler">The task scheduler.</param>
    /// <exception cref="ArgumentNullException">The taskAction.</exception>
    public CancellableJob(Func<TState, CancellationToken, Task> taskAction, TState state, JobExceptionHandler? onException = null, TaskScheduler? taskScheduler = null)
    {
        this.taskAction = taskAction ?? throw new ArgumentNullException(nameof(taskAction));
        this.State = state;
        this.onException = onException;
        this.taskScheduler = taskScheduler;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CancellableJob{TState}" /> class.
    /// </summary>
    /// <param name="taskAction">The task action.</param>
    /// <param name="state">The state.</param>
    /// <param name="onException">The on exception.</param>
    /// <param name="taskScheduler">The task scheduler.</param>
    public CancellableJob(Action<TState, CancellationToken> taskAction, TState state, JobExceptionHandler? onException = null, TaskScheduler? taskScheduler = null)
        : this(GetAsyncTaskAction(taskAction), state, onException, taskScheduler)
    {
    }

    /// <summary>
    /// Gets the state.
    /// </summary>
    public TState State { get; }

    /// <summary>
    /// Gets a value indicating whether the job is running.
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// Gets the exception.
    /// </summary>
    /// <value>
    /// The exception.
    /// </value>
    public AggregateException? Exception => this.aggregateException;

    /// <summary>
    /// Starts the job.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The job start result.</returns>
    public JobStartResult Start(CancellationToken cancellationToken = default)
    {
        return this.StartAsync(cancellationToken).Result;
    }

    /// <summary>
    /// Starts the job.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The job start result.</returns>
    public async Task<JobStartResult> StartAsync(CancellationToken cancellationToken)
    {
        using (await this.@lock.LockAsync())
        {
            if (!this.jobContext.HasValue())
            {
                this.aggregateException = null;
                var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                const TaskCreationOptions taskCreationOptions = TaskCreationOptions.RunContinuationsAsynchronously | TaskCreationOptions.DenyChildAttach;
                this.jobContext = new JobContext(
                    cancellationTokenSource,
                    Task.Factory
                        .StartNew(
                            () => this.TaskAction(cancellationTokenSource.Token),
                            cancellationTokenSource.Token,
                            taskCreationOptions,
                            this.taskScheduler ?? TaskScheduler.Default).Unwrap().ContinueWith(this.DisposeTask, this.taskScheduler ?? TaskScheduler.Default));

                return new JobStartResult(this.jobContext.CancellationTokenSource.Token, false);
            }

            return new JobStartResult(this.jobContext.CancellationTokenSource.Token, true);
        }
    }

    /// <summary>
    /// Stops the job and waits for it to complete.
    /// </summary>
    /// <returns>A result containing the exception in case of an error.</returns>
    public RoE<AggregateException> Stop()
    {
        var task = this.StopAsync();
        task.Wait();
        return task.Result;
    }

    /// <summary>
    /// Stops the job and awaits its completion.
    /// </summary>
    /// <returns>An async task.</returns>
    public async Task<RoE<AggregateException>> StopAsync()
    {
        Task? task = null;
        using (await this.@lock.LockAsync())
        {
            if (this.jobContext.HasValue())
            {
#if NET7_0_OR_GREATER
                await this.jobContext.CancellationTokenSource.CancelAsync();
#else
                this.jobContext.CancellationTokenSource.Cancel();
#endif
                task = this.jobContext.JobContinuationTask;
            }
        }

        if (task.HasValue())
        {
            await task.ConfigureAwait(false);
        }

        return R.FromError(this.aggregateException);
    }

    /// <summary>
    /// Waits for the job to finish asynchronously.
    /// </summary>
    /// <returns>
    /// An async task.
    /// </returns>
    public async Task<RoE<AggregateException>> WaitAsync()
    {
        Task? task = null;
        using (await this.@lock.LockAsync())
        {
            if (this.jobContext.HasValue())
            {
                task = this.jobContext.JobContinuationTask;
            }
        }

        if (task.HasValue())
        {
            await task.ConfigureAwait(false);
        }

        return R.FromError(this.aggregateException);
    }

    /// <summary>
    /// Waits for the job to finish.
    /// </summary>
    /// <returns>A result.</returns>
    public RoE<AggregateException> Wait()
    {
        var task = this.WaitAsync();
        task.Wait();
        return task.Result;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        this.Stop();
    }

    private static Func<TState, CancellationToken, Task> GetAsyncTaskAction(Action<TState, CancellationToken> taskAction)
    {
        return (actualState, token) =>
        {
            taskAction(actualState, token);
            return TaskHelper.CompletedTrueTask;
        };
    }

    private void DisposeTask(Task jobTask)
    {
        using (var lockResult = this.@lock.TryLock())
        {
            if (lockResult.Check())
            {
                this.jobContext?.CancellationTokenSource.Dispose();
                this.aggregateException = jobTask.Exception;
                this.jobContext = null;
            }
        }
    }

    private async Task TaskAction(CancellationToken cancellationToken)
    {
        this.IsRunning = true;
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await this.taskAction.Invoke(this.State, cancellationToken).ConfigureAwait(false);
                    return;
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception e)
                {
                    var continueExecution = false;
                    this.onException?.Invoke(e, ref continueExecution);
                    if (continueExecution)
                    {
                        continue;
                    }

                    throw;
                }
            }
        }
        finally
        {
            this.IsRunning = false;
        }
    }

    private sealed class JobContext
    {
        public JobContext(CancellationTokenSource cancellationTokenSource, Task jobContinuationTask)
        {
            this.CancellationTokenSource = cancellationTokenSource;
            this.JobContinuationTask = jobContinuationTask;
        }

        public CancellationTokenSource CancellationTokenSource { get; }

        public Task JobContinuationTask { get; }
    }
}