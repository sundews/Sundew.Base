// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CancellableJob.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading.Jobs;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A job that keeps running once started, until it is stopped or disposed.
/// </summary>
public sealed class CancellableJob : IJob
{
    private readonly CancellableJob<__> cancellableJob;

    /// <summary>
    /// Initializes a new instance of the <see cref="CancellableJob" /> class.
    /// </summary>
    /// <param name="taskAction">The asynchronous task action.</param>
    /// <param name="onException">The on exception.</param>
    /// <param name="taskScheduler">The task scheduler.</param>
    public CancellableJob(Func<CancellationToken, Task> taskAction, JobExceptionHandler? onException = null, TaskScheduler? taskScheduler = null)
    {
        this.cancellableJob = new CancellableJob<__>(
            (_, token) => taskAction.Invoke(token),
            default,
            onException,
            taskScheduler);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CancellableJob" /> class.
    /// </summary>
    /// <param name="taskAction">The task action.</param>
    /// <param name="onException">The on exception.</param>
    /// <param name="taskScheduler">The task scheduler.</param>
    public CancellableJob(Action<CancellationToken> taskAction, JobExceptionHandler? onException = null, TaskScheduler? taskScheduler = null)
    {
        this.cancellableJob = new CancellableJob<__>(
            (_, token) => taskAction.Invoke(token),
            default,
            onException,
            taskScheduler);
    }

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
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The job start result.</returns>
    public Task<JobStartResult> StartAsync(CancellationToken cancellationToken = default)
    {
        return this.cancellableJob.StartAsync(cancellationToken);
    }

    /// <summary>
    /// Starts the job.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The job start result.</returns>
    public JobStartResult Start(CancellationToken cancellationToken = default)
    {
        return this.cancellableJob.Start(cancellationToken);
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
        this.cancellableJob.Dispose();
    }
}