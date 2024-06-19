// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJob.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading.Jobs;

using System;
using System.Threading.Tasks;

/// <summary>
/// Interface for implementing a job.
/// </summary>
/// <seealso cref="System.IDisposable" />
public interface IJob : IDisposable
{
    /// <summary>
    /// Gets a value indicating whether the job is running.
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// Gets the exception.
    /// </summary>
    /// <value>
    /// The exception.
    /// </value>
    AggregateException? Exception { get; }

    /// <summary>
    /// Starts the job.
    /// </summary>
    /// <returns>The job start result.</returns>
    Task<JobStartResult> StartAsync();

    /// <summary>
    /// Starts the job.
    /// </summary>
    /// <returns>The job start result.</returns>
    JobStartResult Start();

    /// <summary>
    /// Stops the job and waits for it to complete.
    /// </summary>
    /// <returns>A result containing the exception in case of an error.</returns>
    RoE<AggregateException> Stop();

    /// <summary>
    /// Stops the job and awaits its completion.
    /// </summary>
    /// <returns>An async task.</returns>
    Task<RoE<AggregateException>> StopAsync();

    /// <summary>
    /// Waits for the job to finish asynchronously.
    /// </summary>
    /// <returns>
    /// An async task.
    /// </returns>
    Task<RoE<AggregateException>> WaitAsync();

    /// <summary>
    /// Waits for the job to finish.
    /// </summary>
    RoE<AggregateException> Wait();
}