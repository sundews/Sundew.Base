// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimerBase.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Timers;

using System;
using System.Threading;

/// <summary>
/// Abstract base class for timers.
/// </summary>
/// <seealso cref="ITimerControl" />
public abstract class TimerBase : ITimerControl
{
    private readonly object lockObject = new();
    private readonly System.Threading.Timer timer;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimerBase" /> class.
    /// </summary>
    /// <param name="state">The state.</param>
    protected TimerBase(object? state)
    {
        this.Interval = Timeout.InfiniteTimeSpan;
        this.timer = new System.Threading.Timer(this.PrivateOnTick, state, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
    }

    /// <summary>
    /// Gets a value indicating whether this instance is running.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is running; otherwise, <c>false</c>.
    /// </value>
    public bool IsEnabled { get; private set; }

    /// <summary>
    /// Gets the interval.
    /// </summary>
    /// <value>
    /// The interval.
    /// </value>
    public TimeSpan Interval { get; private set; }

    /// <summary>
    /// Starts or restarts the timer.
    /// </summary>
    /// <param name="interval">The interval.</param>
    public void Start(TimeSpan interval)
    {
        this.Start(TimeSpan.Zero, interval);
    }

    /// <summary>
    /// Starts or restarts the timer.
    /// </summary>
    /// <param name="startDelay">The delay before the first occurence.</param>
    public void StartOnce(TimeSpan startDelay)
    {
        this.Start(startDelay, Timeout.InfiniteTimeSpan);
    }

    /// <summary>
    /// Starts or restarts the timer.
    /// </summary>
    /// <param name="startDelay">The delay before the first occurence.</param>
    /// <param name="interval">The interval.</param>
    public void Start(TimeSpan startDelay, TimeSpan interval)
    {
        lock (this.lockObject)
        {
            this.Interval = interval;
            this.timer.Change(startDelay, interval);
            this.IsEnabled = true;
        }
    }

    /// <summary>
    /// Stops the timer.
    /// </summary>
    public void Stop()
    {
        lock (this.lockObject)
        {
            this.Interval = Timeout.InfiniteTimeSpan;
            this.timer.Change(Timeout.InfiniteTimeSpan, this.Interval);
            this.IsEnabled = false;
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Called when [tick].
    /// </summary>
    /// <param name="state">The state.</param>
    protected abstract void OnTick(object state);

    /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
    /// <param name="disposing">
    ///   <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            lock (this.lockObject)
            {
                this.timer.Dispose();
            }
        }
    }

    private void PrivateOnTick(object state)
    {
        lock (this.lockObject)
        {
            if (this.Interval != Timeout.InfiniteTimeSpan)
            {
                this.IsEnabled = true;
            }
        }

        this.OnTick(state);
    }
}