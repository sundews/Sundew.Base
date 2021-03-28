// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CancellableJob{TState}.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading.Jobs
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Sundew.Base.Computation;
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
        private readonly object lockObject = new object();
        private readonly Func<TState, CancellationToken, Task> taskAction;
        private readonly JobExceptionHandler? onException;
        private readonly TaskScheduler? taskScheduler;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "False positive, it is disposed.")]
        private CancellationTokenSource? cancellationTokenSource;
        private Task? jobContinuationTask;
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
        /// <returns><c>true</c>, if the job was started, otherwise <c>false</c>, meaning the job is already running.</returns>
        public Result.IfSuccess<CancellationToken> Start()
        {
            lock (this.lockObject)
            {
                if (this.jobContinuationTask == null)
                {
                    this.aggregateException = null;
                    this.cancellationTokenSource = new CancellationTokenSource();
                    const TaskCreationOptions taskCreationOptions = TaskCreationOptions.RunContinuationsAsynchronously | TaskCreationOptions.DenyChildAttach;
                    this.jobContinuationTask = Task.Factory.StartNew(this.TaskAction, this.cancellationTokenSource.Token, taskCreationOptions, this.taskScheduler ?? TaskScheduler.Default).Unwrap().ContinueWith(this.DisposeTask, this.taskScheduler ?? TaskScheduler.Default);

                    return Result.Success(this.cancellationTokenSource.Token);
                }

                return Result.Error();
            }
        }

        /// <summary>
        /// Stops the job and waits for it to complete.
        /// </summary>
        /// <returns>A result containing the exception in case of an error.</returns>
        public Result.IfError<AggregateException?> Stop()
        {
            var task = this.StopAsync();
            task.Wait();
            return task.Result;
        }

        /// <summary>
        /// Stops the job and awaits its completion.
        /// </summary>
        /// <returns>An async task.</returns>
        public async Task<Result.IfError<AggregateException?>> StopAsync()
        {
            var actualTask = this.jobContinuationTask;
            this.cancellationTokenSource?.Cancel();
            this.jobContinuationTask = null;
            if (actualTask != null)
            {
                await actualTask.ConfigureAwait(false);
            }

            return Result.FromError(this.aggregateException);
        }

        /// <summary>
        /// Waits for the job to finish asynchronously.
        /// </summary>
        /// <returns>
        /// An async task.
        /// </returns>
        public async Task<Result.IfError<AggregateException?>> WaitAsync()
        {
            if (this.jobContinuationTask != null)
            {
                await this.jobContinuationTask.ConfigureAwait(false);
            }

            return Result.FromError(this.aggregateException);
        }

        /// <summary>
        /// Waits for the job to finish.
        /// </summary>
        /// <returns>A result.</returns>
        public Result.IfError<AggregateException?> Wait()
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
            lock (this.lockObject)
            {
                this.cancellationTokenSource?.Dispose();
                this.cancellationTokenSource = null;
                this.aggregateException = jobTask.Exception;
            }
        }

        private async Task TaskAction()
        {
            this.IsRunning = true;
            var cancellationToken = this.cancellationTokenSource!.Token;
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
    }
}