// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AR{TValue,TError}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives;

using System.Runtime.CompilerServices;
using System.Threading.Tasks;

#if NOT_SUPPORT_BY_LANGUAGE

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
[AsyncMethodBuilder(typeof(AsyncValueTaskMethodBuilder<>))]
public readonly struct AR<TValue, TError>
{
    private readonly ValueTask<R<TValue, TError>> task;

    public AR(ValueTask<R<TValue, TError>> task)
    {
        this.task = task;
    }

    public AR(R<TValue, TError> result)
        : this(new ValueTask<R<TValue, TError>>(result))
    {
    }

    public static implicit operator AR<TValue, TError>(ValueTask<R<TValue, TError>> task)
    {
        return task.IsCompletedSuccessfully ? new AR<TValue, TError>(new ValueTask<R<TValue, TError>>(task.Result)) : new AR<TValue, TError>(task);
    }

    public static implicit operator AR<TValue, TError>(ValueTask<R.SuccessResult<TValue>> task)
    {
        if (task.IsCompletedSuccessfully)
        {
            return new AR<TValue, TError>(new ValueTask<R<TValue, TError>>(task.Result));
        }

        var resultTask = task.AsTask().ContinueWith(t => (R<TValue, TError>)t.Result, TaskScheduler.Default);
        return new AR<TValue, TError>(new ValueTask<R<TValue, TError>>(resultTask));
    }

    public static implicit operator AR<TValue, TError>(ValueTask<R.ErrorResult<TError>> task)
    {
        if (task.IsCompletedSuccessfully)
        {
            return new AR<TValue, TError>(new ValueTask<R<TValue, TError>>(task.Result));
        }

        var resultTask = task.AsTask().ContinueWith(t => (R<TValue, TError>)t.Result, TaskScheduler.Default);
        return new AR<TValue, TError>(new ValueTask<R<TValue, TError>>(resultTask));
    }

    public static implicit operator AR<TValue, TError>(Task<R<TValue, TError>> task)
    {
        return task.Status == TaskStatus.RanToCompletion ? new AR<TValue, TError>(new ValueTask<R<TValue, TError>>(task.Result)) : new AR<TValue, TError>(new ValueTask<R<TValue, TError>>(task));
    }

    public static implicit operator AR<TValue, TError>(Task<R.SuccessResult<TValue>> task)
    {
        if (task.Status == TaskStatus.RanToCompletion)
        {
            return new AR<TValue, TError>(new ValueTask<R<TValue, TError>>(task.Result));
        }

        var resultTask = task.ContinueWith(t => (R<TValue, TError>)t.Result, TaskScheduler.Default);
        return new AR<TValue, TError>(new ValueTask<R<TValue, TError>>(resultTask));
    }

    public static implicit operator AR<TValue, TError>(Task<R.ErrorResult<TError>> task)
    {
        if (task.Status == TaskStatus.RanToCompletion)
        {
            return new AR<TValue, TError>(new ValueTask<R<TValue, TError>>(task.Result));
        }

        var resultTask = task.ContinueWith(t => (R<TValue, TError>)t.Result, TaskScheduler.Default);
        return new AR<TValue, TError>(new ValueTask<R<TValue, TError>>(resultTask));
    }

    public ValueTaskAwaiter<R<TValue, TError>> GetAwaiter()
    {
        return this.task.GetAwaiter();
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning restore SA1600 // Elements should be documented
#endif