// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncLazy.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Lazy initializer with support for async await.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
public sealed class AsyncLazy<TValue> : IAsyncLazy<TValue>
{
    private readonly Lazy<Task<TValue>> lazy;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncLazy{TValue}" /> class.
    /// </summary>
    /// <param name="valueFunc">The value function.</param>
    /// <param name="runOnThreadPool">if set to <c>true</c> [run on thread pool].</param>
    public AsyncLazy(Func<Task<TValue>> valueFunc, bool runOnThreadPool = false)
    {
        var actualValueFunc = valueFunc;
        if (runOnThreadPool)
        {
            actualValueFunc = () => Task.Run(valueFunc);
        }

        this.lazy = new Lazy<Task<TValue>>(actualValueFunc, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncLazy{TValue}" /> class.
    /// </summary>
    /// <param name="valueFunc">The value function.</param>
    public AsyncLazy(Func<TValue> valueFunc)
    {
        this.lazy = new Lazy<Task<TValue>>(() => Task.Run(valueFunc), LazyThreadSafetyMode.ExecutionAndPublication);
    }

    /// <summary>
    /// Gets the value if created and otherwise the default.
    /// </summary>
    /// <returns>The created value or default.</returns>
    [return: MaybeNull]
    public TValue GetValueOrDefault()
    {
        if (this.lazy.IsValueCreated)
        {
            return this.lazy.Value.Result;
        }

        return default;
    }

    /// <summary>Configures the await.</summary>
    /// <param name="continueOnCapturedContext">if set to <c>true</c> [continue on captured context].</param>
    /// <returns>A <see cref="ConfiguredTaskAwaitable{TResult}"/>.</returns>
    public ConfiguredTaskAwaitable<TValue> ConfigureAwait(bool continueOnCapturedContext)
    {
        return this.lazy.Value.ConfigureAwait(continueOnCapturedContext);
    }

    /// <summary>
    /// Gets the awaiter.
    /// </summary>
    /// <returns>
    /// A task awaiter.
    /// </returns>
    public TaskAwaiter<TValue> GetAwaiter()
    {
        return this.lazy.Value.GetAwaiter();
    }
}