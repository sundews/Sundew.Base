// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Deferred.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading;

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

/// <summary>
/// Exposes factory methods for creating <see cref="Deferred{TResult}"/>.
/// </summary>
public static class Deferred
{
    /// <summary>
    /// Creates a <see cref="Deferred{TResult}"/> from an already computed value.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="result">The computed result.</param>
    /// <returns>A <see cref="Deferred{TResult}"/> that completes synchronously.</returns>
    public static Deferred<TResult> FromResult<TResult>(TResult result)
    {
        return new Deferred<TResult>(result);
    }

    /// <summary>
    /// Creates a <see cref="Deferred{TResult}"/> from a task.
    /// If the task is not started, it is started when the deferred is awaited, otherwise it is just awaited.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="task">The task.</param>
    /// <returns>A <see cref="Deferred{TResult}"/>.</returns>
    public static Deferred<TResult> FromTask<TResult>(Task<TResult> task)
    {
        return new Deferred<TResult>(task);
    }

    /// <summary>
    /// Creates a <see cref="Deferred{TResult}"/> from a function that is invoked when the deferred is awaited.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="func">The function that creates the task.</param>
    /// <returns>A <see cref="Deferred{TResult}"/>.</returns>
    public static Deferred<TResult> FromFunc<TResult>(Func<Task<TResult>> func)
    {
        return new Deferred<TResult>(func);
    }

    /// <summary>
    /// Creates a <see cref="Deferred{TResult}"/> from a function that is invoked when the deferred is awaited.
    /// This overload is preferred for async lambdas, so that they avoid allocating a task when completing synchronously.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="func">The function that creates the value task.</param>
    /// <returns>A <see cref="Deferred{TResult}"/>.</returns>
    [OverloadResolutionPriority(1)]
    public static Deferred<TResult> FromFunc<TResult>(Func<ValueTask<TResult>> func)
    {
        return new Deferred<TResult>(func);
    }
}
