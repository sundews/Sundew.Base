﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContinuousJob.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading.Jobs
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Sundew.Base.Primitives;
    using Sundew.Base.Primitives.Computation;

    /// <summary>
    /// A job that keeps running once started, until it is stopped or disposed.
    /// </summary>
    public sealed class ContinuousJob : IJob
    {
        private readonly ContinuousJob<ˍ> continuousJob;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContinuousJob" /> class.
        /// </summary>
        /// <param name="taskAction">The asynchronous task action.</param>
        /// <param name="onException">The on exception.</param>
        /// <param name="taskScheduler">The task scheduler.</param>
        public ContinuousJob(Func<CancellationToken, Task> taskAction, ContinuousJobExceptionHandler? onException = null, TaskScheduler? taskScheduler = null)
        {
            this.continuousJob = new ContinuousJob<ˍ>(
                (_, token) => taskAction.Invoke(token),
                ˍ._,
                onException,
                taskScheduler);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContinuousJob" /> class.
        /// </summary>
        /// <param name="taskAction">The task action.</param>
        /// <param name="onException">The on exception.</param>
        /// <param name="taskScheduler">The task scheduler.</param>
        public ContinuousJob(Action<CancellationToken> taskAction, ContinuousJobExceptionHandler? onException = null, TaskScheduler? taskScheduler = null)
        {
            this.continuousJob = new ContinuousJob<ˍ>(
                (_, token) => taskAction.Invoke(token),
                ˍ._,
                onException,
                taskScheduler);
        }

        /// <summary>
        /// Gets a value indicating whether the job is running.
        /// </summary>
        public bool IsRunning => this.continuousJob.IsRunning;

        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public AggregateException? Exception => this.continuousJob.Exception;

        /// <summary>
        /// Starts the job.
        /// </summary>
        /// <returns><c>true</c>, if the job was started, otherwise <c>false</c>, meaning the job is already running.</returns>
        public Result.IfSuccess<CancellationToken> Start()
        {
            return this.continuousJob.Start();
        }

        /// <summary>
        /// Stops the job and waits for it to complete.
        /// </summary>
        /// <returns>A result containing the exception in case of an error.</returns>
        public Result.IfError<AggregateException?> Stop()
        {
            return this.continuousJob.Stop();
        }

        /// <summary>
        /// Stops the job and awaits its completion.
        /// </summary>
        /// <returns>An async task.</returns>
        public Task<Result.IfError<AggregateException?>> StopAsync()
        {
            return this.continuousJob.StopAsync();
        }

        /// <summary>
        /// Waits for the job to finish asynchronously.
        /// </summary>
        /// <returns>
        /// An async task.
        /// </returns>
        public Task<Result.IfError<AggregateException?>> WaitAsync()
        {
            return this.continuousJob.WaitAsync();
        }

        /// <summary>
        /// Waits for the job to finish.
        /// </summary>
        /// <returns>The result.</returns>
        public Result.IfError<AggregateException?> Wait()
        {
            return this.continuousJob.Wait();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.continuousJob.Dispose();
        }
    }
}
