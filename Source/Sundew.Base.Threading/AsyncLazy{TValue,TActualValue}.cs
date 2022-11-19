// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncLazy{TValue,TActualValue}.cs" company="Hukano">
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
/// <typeparam name="TActualValue">The type of the actual value.</typeparam>
public sealed class AsyncLazy<TValue, TActualValue> : IAsyncLazy<TValue>
    where TActualValue : TValue
{
    private readonly AsyncLock asyncLock = new();
    private readonly Func<CancellationToken, Task<TActualValue>> factory;
    private Task<TActualValue>? task;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncLazy{TValue, TActualValue}" /> class.
    /// </summary>
    /// <param name="valueFunc">The value function.</param>
    public AsyncLazy(Func<CancellationToken, Task<TActualValue>> valueFunc)
    {
        this.factory = valueFunc;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncLazy{TValue, TActualValue}" /> class.
    /// </summary>
    /// <param name="valueFunc">The value function.</param>
    public AsyncLazy(Func<Task<TActualValue>> valueFunc)
    {
        this.factory = _ => valueFunc();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncLazy{TValue, TActualValue}" /> class.
    /// </summary>
    /// <param name="valueFunc">The value function.</param>
    public AsyncLazy(Func<CancellationToken, TActualValue> valueFunc)
     : this(cancellationToken => Task.FromResult(valueFunc(cancellationToken)))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncLazy{TValue, TActualValue}" /> class.
    /// </summary>
    /// <param name="valueFunc">The value function.</param>
    public AsyncLazy(Func<TActualValue> valueFunc)
     : this(() => Task.FromResult(valueFunc()))
    {
    }

    /// <summary>
    /// Gets a value indicating whether the value has been created.
    /// </summary>
    public bool IsValueCreated => this.task is { IsCompleted: true, IsCanceled: false };

    /// <summary>
    /// Gets the value or default.
    /// </summary>
    /// <returns>The created value or default.</returns>
    [return: MaybeNull]
    public TActualValue GetValueOrDefault()
    {
        if (this.task != null && this.IsValueCreated)
        {
            return this.task.Result;
        }

        return default;
    }

    /// <summary>
    /// Gets the value or default.
    /// </summary>
    /// <returns>The created value or default.</returns>
    [return: MaybeNull]
    TValue IAsyncLazy<TValue>.GetValueOrDefault()
    {
        return this.GetValueOrDefault();
    }

    /// <summary>
    /// Add cancellation support to the async lazy.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A cancellable task.</returns>
    public async Task<TActualValue> GetValueAsync(CancellationToken cancellationToken = default)
    {
        using var unlocker = await this.asyncLock.LockAsync(cancellationToken).ConfigureAwait(false);
        var task = this.task;
        if (task is { IsCanceled: false })
        {
#if NET6_0_OR_GREATER
            return await task.WaitAsync(cancellationToken).ConfigureAwait(false);
#else
            var cancelTaskCompletionSource = new TaskCompletionSource<TActualValue>();
#if NETSTANDARD1_3
            using var cancellationTokenRegistration = cancellationToken.Register(() => cancelTaskCompletionSource.SetResult(default!));
#else
            await using var cancellationTokenRegistration = cancellationToken.Register(() => cancelTaskCompletionSource.SetResult(default!));
#endif
            var completedTask = await Task.WhenAny(task, cancelTaskCompletionSource.Task);
            if (completedTask == task)
            {
                return await task.ConfigureAwait(false);
            }

            return await Task.FromCanceled<TActualValue>(cancellationToken).ConfigureAwait(false);
#endif
        }

        task = this.task = Task.Run(() => this.factory(cancellationToken), cancellationToken);
        return await task.ConfigureAwait(false);
    }

    /// <summary>Configures the await.</summary>
    /// <param name="continueOnCapturedContext">if set to <c>true</c> [continue on captured context].</param>
    /// <returns>A <see cref="ConfiguredTaskAwaitable{TResult}"/>.</returns>
    public ConfiguredTaskAwaitable<TActualValue> ConfigureAwait(bool continueOnCapturedContext)
    {
        return this.GetValueAsync(CancellationToken.None).ConfigureAwait(continueOnCapturedContext);
    }

    /// <summary>
    /// Gets the awaiter.
    /// </summary>
    /// <returns>
    /// A task awaiter.
    /// </returns>
    public TaskAwaiter<TActualValue> GetAwaiter()
    {
        return this.GetValueAsync(CancellationToken.None).GetAwaiter();
    }

    /// <summary>
    /// Add cancellation support to the async lazy.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A cancellable task.</returns>
    async Task<TValue> IAsyncLazy<TValue>.GetValueAsync(CancellationToken cancellationToken)
    {
        return await this.GetValueAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Configures the await.</summary>
    /// <param name="continueOnCapturedContext">if set to <c>true</c> [continue on captured context].</param>
    /// <returns>A <see cref="ConfiguredTaskAwaitable{TResult}"/>.</returns>
    ConfiguredTaskAwaitable<TValue> IAsyncLazy<TValue>.ConfigureAwait(bool continueOnCapturedContext)
    {
        return this.GetTask(continueOnCapturedContext).ConfigureAwait(continueOnCapturedContext);
    }

    /// <summary>
    /// Gets the awaiter.
    /// </summary>
    /// <returns>A task awaiter.</returns>
    TaskAwaiter<TValue> IAsyncLazy<TValue>.GetAwaiter()
    {
        return this.GetTask(false).GetAwaiter();
    }

    private async Task<TValue> GetTask(bool continueOnCapturedContext)
    {
        return await this.ConfigureAwait(continueOnCapturedContext);
    }
}