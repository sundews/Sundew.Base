// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContinuousJob{TState}.cs" company="Sundews">
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
/// Delegate for handling continuous job exceptions.
/// </summary>
/// <param name="exception">The exception.</param>
/// <param name="handled">if set to <c>true</c> [handled].</param>
public delegate void ContinuousJobExceptionHandler(Exception exception, ref bool handled);

/// <summary>
/// A job that keeps running once started, until it is stopped or disposed.
/// </summary>
/// <typeparam name="TState">The type of the state.</typeparam>
public sealed class ContinuousJob<TState> : IJob
{
    private readonly Func<TState, CancellationToken, Task> taskAction;
    private readonly CancellableJob<TState> cancellableJob;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContinuousJob{TState}" /> class.
    /// </summary>
    /// <param name="taskAction">The asynchronous task action.</param>
    /// <param name="state">The state.</param>
    /// <param name="onException">The on exception.</param>
    /// <param name="taskScheduler">The task scheduler.</param>
    /// <exception cref="ArgumentNullException">The task action.</exception>
    public ContinuousJob(Func<TState, CancellationToken, Task> taskAction, TState state, ContinuousJobExceptionHandler? onException = null, TaskScheduler? taskScheduler = null)
    {
        this.taskAction = taskAction ?? throw new ArgumentNullException(nameof(taskAction));
        this.cancellableJob = new CancellableJob<TState>(this.Job, state, (Exception exception, ref bool retry) => onException?.Invoke(exception, ref retry), taskScheduler);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContinuousJob{TState}" /> class.
    /// </summary>
    /// <param name="taskAction">The task action.</param>
    /// <param name="state">The state.</param>
    /// <param name="onException">The on exception.</param>
    /// <param name="taskScheduler">The task scheduler.</param>
    public ContinuousJob(Action<TState, CancellationToken> taskAction, TState state, ContinuousJobExceptionHandler? onException = null, TaskScheduler? taskScheduler = null)
        : this(GetAsyncTaskAction(taskAction), state, onException, taskScheduler)
    {
    }

    /// <summary>
    /// Gets the state.
    /// </summary>
    public TState State => this.cancellableJob.State;

    /// <summary>
    /// Gets a value indicating whether the job is running.
    /// </summary>
    public bool IsRunning => this.cancellableJob.IsRunning;

    /// <summary>
    /// Gets the exception.
    /// </summary>
    /// <value>
    /// The exception.
    /// </value>
    public AggregateException? Exception => this.cancellableJob.Exception;

    /// <summary>
    /// Starts the job.
    /// </summary>
    /// <returns>The job start result.</returns>
    public Task<JobStartResult> StartAsync()
    {
        return this.cancellableJob.StartAsync();
    }

    /// <summary>
    /// Starts the job.
    /// </summary>
    /// <returns>The job start result.</returns>
    public JobStartResult Start()
    {
        return this.cancellableJob.Start();
    }

    /// <summary>
    /// Stops the job and waits for it to complete.
    /// </summary>
    /// <returns>A result containing the exception in case of an error.</returns>
    public RoE<AggregateException> Stop()
    {
        return this.cancellableJob.Stop();
    }

    /// <summary>
    /// Stops the job and awaits its completion.
    /// </summary>
    /// <returns>An async task.</returns>
    public Task<RoE<AggregateException>> StopAsync()
    {
        return this.cancellableJob.StopAsync();
    }

    /// <summary>
    /// Waits for the job to finish asynchronously.
    /// </summary>
    /// <returns>
    /// An async task.
    /// </returns>
    public Task<RoE<AggregateException>> WaitAsync()
    {
        return this.cancellableJob.WaitAsync();
    }

    /// <summary>
    /// Waits for the job to finish.
    /// </summary>
    /// <returns>The result.</returns>
    public RoE<AggregateException> Wait()
    {
        return this.cancellableJob.Wait();
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        this.Stop();
        this.cancellableJob.Dispose();
    }

    private static Func<TState, CancellationToken, Task> GetAsyncTaskAction(
        Action<TState, CancellationToken> taskAction)
    {
        return (actualState, token) =>
        {
            taskAction(actualState, token);
            return TaskHelper.CompletedTrueTask;
        };
    }

    private async Task Job(TState state, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await this.taskAction.Invoke(state, cancellationToken).ConfigureAwait(false);
        }
    }
}