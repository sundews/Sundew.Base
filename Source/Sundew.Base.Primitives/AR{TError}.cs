// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AR{TError}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives;

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

#if NOT_SUPPORT_BY_LANGUAGE
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

#pragma warning disable SA1600 // Elements should be documented
[AsyncMethodBuilder(typeof(ARMethodBuilder<,>))]
public readonly struct AR<TError>
{
    private readonly ValueTask<R<TError>> task;

    public AR(ValueTask<R<TError>> task)
    {
        this.task = task;
    }

    public AR(R<TError> result)
        : this(new ValueTask<R<TError>>(result))
    {
    }

    public static implicit operator AR<TError>(ValueTask<R<TError>> task)
    {
        return task.IsCompletedSuccessfully ? new AR<TError>(new ValueTask<R<TError>>(task.Result)) : new AR<TError>(task);
    }

    public static implicit operator AR<TError>(ValueTask<R.SuccessResult> task)
    {
        if (task.IsCompletedSuccessfully)
        {
            return new AR<TError>(new ValueTask<R<TError>>(task.Result));
        }

        var resultTask = task.AsTask().ContinueWith(t => (R<TError>)t.Result, TaskScheduler.Default);
        return new AR<TError>(new ValueTask<R<TError>>(resultTask));
    }

    public static implicit operator AR<TError>(ValueTask<R.ErrorResult<TError>> task)
    {
        if (task.IsCompletedSuccessfully)
        {
            return new AR<TError>(new ValueTask<R<TError>>(task.Result));
        }

        var resultTask = task.AsTask().ContinueWith(t => (R<TError>)t.Result, TaskScheduler.Default);
        return new AR<TError>(new ValueTask<R<TError>>(resultTask));
    }

    public static implicit operator AR<TError>(Task<R<TError>> task)
    {
        return task.Status == TaskStatus.RanToCompletion ? new AR<TError>(new ValueTask<R<TError>>(task.Result)) : new AR<TError>(new ValueTask<R<TError>>(task));
    }

    public static implicit operator AR<TError>(Task<R.SuccessResult> task)
    {
        if (task.Status == TaskStatus.RanToCompletion)
        {
            return new AR<TError>(new ValueTask<R<TError>>(task.Result));
        }

        var resultTask = task.ContinueWith(t => (R<TError>)t.Result, TaskScheduler.Default);
        return new AR<TError>(new ValueTask<R<TError>>(resultTask));
    }

    public static implicit operator AR<TError>(Task<R.ErrorResult<TError>> task)
    {
        if (task.Status == TaskStatus.RanToCompletion)
        {
            return new AR<TError>(new ValueTask<R<TError>>(task.Result));
        }

        var resultTask = task.ContinueWith(t => (R<TError>)t.Result, TaskScheduler.Default);
        return new AR<TError>(new ValueTask<R<TError>>(resultTask));
    }

    public static implicit operator AR<TError>(R<TError> result)
    {
        return new AR<TError>(result);
    }

    public ValueTaskAwaiter<R<TError>> GetAwaiter()
    {
        return this.task.GetAwaiter();
    }
}

[StructLayout(LayoutKind.Auto)]
public struct ARMethodBuilder<TResult, TError>
{
    private AsyncValueTaskMethodBuilder<TResult> asyncValueTaskMethodBuilder;

    internal ARMethodBuilder(AsyncValueTaskMethodBuilder<R<TError>> asyncValueTaskMethodBuilder)
    {
        this.asyncValueTaskMethodBuilder = asyncValueTaskMethodBuilder;
    }

    public AR<TError> Task => new AR<TError>(this.asyncValueTaskMethodBuilder.Task);

    public static ARMethodBuilder<R<TError>, TError> Create() => default;

    public void Start<TStateMachine>(ref TStateMachine stateMachine)
        where TStateMachine : IAsyncStateMachine
    {
        this.asyncValueTaskMethodBuilder.SetStateMachine(stateMachine);
    }

    public void SetStateMachine(IAsyncStateMachine stateMachine)
    {
        this.asyncValueTaskMethodBuilder.SetStateMachine(stateMachine);
    }

    public void SetResult(TResult result)
    {
        this.asyncValueTaskMethodBuilder.SetResult(result);
    }

    /// <summary>Marks the value task as failed and binds the specified exception to the value task.</summary>
    /// <param name="exception">The exception to bind to the value task.</param>
    public void SetException(Exception exception)
    {
        this.asyncValueTaskMethodBuilder.SetException(exception);
    }

    public void AwaitOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter,
        ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        this.asyncValueTaskMethodBuilder.AwaitOnCompleted(ref awaiter, ref stateMachine);
    }

    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter,
        ref TStateMachine stateMachine)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        this.asyncValueTaskMethodBuilder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning restore SA1600 // Elements should be documented
#endif