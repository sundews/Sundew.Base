// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LastUpdateEmitter.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sundew.Base.Reporting;

/// <summary>
/// An implementation of an <see cref="IUpdateEmitter{TValue}"/> the only emits new values when the previous update has been processed.
/// </summary>
/// <typeparam name="TValue">The value type.</typeparam>
public sealed class LastUpdateEmitter<TValue> : IUpdateEmitter<TValue>
{
    private readonly Func<TValue?, ValueTask> emitterFunc;
    private readonly IUpdateEmitterReporter<TValue>? updateEmitterReporter;
    private readonly IEqualityComparer<TValue?> comparer;
#if NET9_0_OR_GREATER
    private readonly Lock lockObject = new Lock();
#else
    private readonly object lockObject = new object();
#endif

    private TValue? nextValue;
    private TValue? currentValue;

    private Task? currentTask;

    /// <summary>
    /// Initializes a new instance of the <see cref="LastUpdateEmitter{TValue}"/> class.
    /// </summary>
    /// <param name="emitterFunc">The emitter func.</param>
    /// <param name="comparer">The comparer.</param>
    /// <param name="updateEmitterReporter">The update emitter reporter.</param>
    public LastUpdateEmitter(Func<TValue?, ValueTask> emitterFunc, IEqualityComparer<TValue?>? comparer = null, IUpdateEmitterReporter<TValue>? updateEmitterReporter = null)
    {
        this.emitterFunc = emitterFunc;
        this.updateEmitterReporter = updateEmitterReporter;
        this.updateEmitterReporter?.SetSource(this);
        this.comparer = comparer ?? EqualityComparer<TValue?>.Default;
    }

    /// <summary>
    /// Schedules the specified value for processing.
    /// </summary>
    /// <param name="value">The value.</param>
    public void Update(TValue value)
    {
        var needUpdate = !this.comparer.Equals(value, this.currentValue);
        lock (this.lockObject)
        {
            this.nextValue = value;
            if (this.currentTask == null && needUpdate)
            {
                this.currentTask = Task.Run(() => this.ProcessAsync(value));
            }
        }
    }

    private async Task ProcessAsync(TValue? value)
    {
        try
        {
            while (!this.comparer.Equals(this.currentValue, value))
            {
                this.currentValue = value;

                try
                {
                    await this.emitterFunc(value).ConfigureAwait(false);
                    this.updateEmitterReporter?.Emitted(value);
                }
                catch (Exception ex)
                {
                    this.updateEmitterReporter?.ErrorDuringUpdate(ex);
                }
#if NET9_0_OR_GREATER
                this.lockObject.Enter();
#else
                Monitor.Enter(this.lockObject);
#endif
                value = this.nextValue;
            }
        }
        finally
        {
#if NET9_0_OR_GREATER
            if (!this.lockObject.IsHeldByCurrentThread)
            {
                this.lockObject.Enter();
            }
#else
            if (!Monitor.IsEntered(this.lockObject))
            {
                Monitor.Enter(this.lockObject);
            }
#endif

            this.currentTask = null;
#if NET9_0_OR_GREATER
            this.lockObject.Exit();
#else
            Monitor.Exit(this.lockObject);
#endif
        }
    }
}