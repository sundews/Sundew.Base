// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Deferred{TResult}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

/// <summary>
/// Represents a result whose operation is triggered when it is awaited.
/// Like <see cref="ValueTask{TResult}"/>, a <see cref="Deferred{TResult}"/> must be consumed at most once:
/// awaiting it (or calling <see cref="AsValueTask"/> or <see cref="AsTask"/>) more than once triggers a function based operation again.
/// If it is never consumed, the operation never runs.
/// </summary>
/// <typeparam name="TResult">The result type.</typeparam>
public readonly struct Deferred<TResult> : IEquatable<Deferred<TResult>>
{
    // null: result is used, otherwise one of: Task<TResult>, Func<Task<TResult>> or Func<ValueTask<TResult>>
    private readonly object? source;
    private readonly TResult result;

    /// <summary>
    /// Initializes a new instance of the <see cref="Deferred{TResult}"/> struct.
    /// </summary>
    /// <param name="result">The computed result.</param>
    internal Deferred(TResult result)
    {
        this.source = null;
        this.result = result;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Deferred{TResult}"/> struct.
    /// </summary>
    /// <param name="source">The task or function.</param>
    internal Deferred(object source)
    {
        this.source = source;
        this.result = default!;
    }

    /// <summary>
    /// Converts a task to a <see cref="Deferred{TResult}"/>.
    /// If the task is not started, it is started when the deferred is awaited, otherwise it is just awaited.
    /// </summary>
    /// <param name="task">The task.</param>
    /// <returns>A <see cref="Deferred{TResult}"/>.</returns>
    public static implicit operator Deferred<TResult>(Task<TResult> task)
    {
        return new Deferred<TResult>(task);
    }

    /// <summary>
    /// Converts a task to a <see cref="Deferred{TResult}"/>.
    /// If the task is not started, it is started when the deferred is awaited, otherwise it is just awaited.
    /// </summary>
    /// <param name="task">The task.</param>
    /// <returns>A <see cref="Deferred{TResult}"/>.</returns>
    public static implicit operator Deferred<TResult>(ValueTask<TResult> task)
    {
        return new Deferred<TResult>(task);
    }

    /// <summary>
    /// Determines whether the two values are equal.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns><c>true</c>, if the values are equal, otherwise <c>false</c>.</returns>
    public static bool operator ==(Deferred<TResult> left, Deferred<TResult> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether the two values are not equal.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns><c>true</c>, if the values are not equal, otherwise <c>false</c>.</returns>
    public static bool operator !=(Deferred<TResult> left, Deferred<TResult> right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    /// Triggers the operation if necessary and gets an awaiter for the result.
    /// </summary>
    /// <returns>An awaiter for the result.</returns>
    public ValueTaskAwaiter<TResult> GetAwaiter()
    {
        return this.AsValueTask().GetAwaiter();
    }

    /// <summary>
    /// Triggers the operation if necessary and configures an awaiter for the result.
    /// </summary>
    /// <param name="continueOnCapturedContext"><c>true</c> to attempt to marshal the continuation back to the captured context; otherwise, <c>false</c>.</param>
    /// <returns>The configured awaitable.</returns>
    public ConfiguredValueTaskAwaitable<TResult> ConfigureAwait(bool continueOnCapturedContext)
    {
        return this.AsValueTask().ConfigureAwait(continueOnCapturedContext);
    }

    /// <summary>
    /// Triggers the operation if necessary and gets a <see cref="ValueTask{TResult}"/> representing it.
    /// Functions are invoked inline on the calling thread, tasks that are not started are started on the <see cref="TaskScheduler.Default"/>.
    /// </summary>
    /// <returns>The value task.</returns>
    public ValueTask<TResult> AsValueTask()
    {
        try
        {
            switch (this.source)
            {
                case null:
                    return new ValueTask<TResult>(this.result);
                case ValueTask<TResult> valueTask:
                    return valueTask;
                case Task<TResult> task:
                    if (task.Status == TaskStatus.Created)
                    {
                        try
                        {
                            task.Start(TaskScheduler.Default);
                        }
                        catch (InvalidOperationException)
                        {
                            // The task was started by someone else in the meantime.
                        }
                    }

                    return new ValueTask<TResult>(task);
                case Func<ValueTask<TResult>> valueTaskFunc:
                    return valueTaskFunc();
                default:
                    return new ValueTask<TResult>(((Func<Task<TResult>>)this.source)());
            }
        }
        catch (OperationCanceledException exception)
        {
            return new ValueTask<TResult>(FromCanceled(exception));
        }
        catch (Exception exception)
        {
            return new ValueTask<TResult>(Task.FromException<TResult>(exception));
        }
    }

    /// <summary>
    /// Triggers the operation if necessary and gets a <see cref="Task{TResult}"/> representing it.
    /// </summary>
    /// <returns>The task.</returns>
    public Task<TResult> AsTask()
    {
        return this.AsValueTask().AsTask();
    }

    /// <summary>
    /// Determines whether this instance wraps the same task, function or result as the other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c>, if the values are equal, otherwise <c>false</c>.</returns>
    public bool Equals(Deferred<TResult> other)
    {
        return ReferenceEquals(this.source, other.source) && EqualityComparer<TResult?>.Default.Equals(this.result, other.result);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is Deferred<TResult> other && this.Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return this.source?.GetHashCode() ?? this.result?.GetHashCode() ?? 0;
    }

    // An async method that throws an OperationCanceledException results in a canceled task that preserves the original exception.
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    private static async Task<TResult> FromCanceled(OperationCanceledException operationCanceledException)
#pragma warning restore CS1998
    {
        ExceptionDispatchInfo.Capture(operationCanceledException).Throw();
        return default!;
    }
}
