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
    private readonly AsyncLock asyncLock = new();
    private readonly Func<CancellationToken, Task<TValue>> factory;
    private Task<TValue>? task;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncLazy{TValue}" /> class.
    /// </summary>
    /// <param name="valueFunc">The value function.</param>
    public AsyncLazy(Func<CancellationToken, Task<TValue>> valueFunc)
    {
        this.factory = valueFunc;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncLazy{TValue}" /> class.
    /// </summary>
    /// <param name="valueFunc">The value function.</param>
    public AsyncLazy(Func<Task<TValue>> valueFunc)
    {
        this.factory = _ => valueFunc();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncLazy{TValue}" /> class.
    /// </summary>
    /// <param name="valueFunc">The value function.</param>
    public AsyncLazy(Func<CancellationToken, TValue> valueFunc)
        : this(cancellationToken => Task.FromResult(valueFunc(cancellationToken)))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncLazy{TValue}" /> class.
    /// </summary>
    /// <param name="valueFunc">The value function.</param>
    public AsyncLazy(Func<TValue> valueFunc)
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
    public TValue GetValueOrDefault()
    {
        if (this.task != null && this.IsValueCreated)
        {
            return this.task.Result;
        }

        return default;
    }

    /// <summary>
    /// Add cancellation support to the async lazy.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A cancellable task.</returns>
    public async Task<TValue> GetValueAsync(CancellationToken cancellationToken = default)
    {
        using var unlocker = await this.asyncLock.LockAsync(cancellationToken).ConfigureAwait(false);
        var task = this.task;
        if (task is { IsCanceled: false })
        {
#if NET6_0_OR_GREATER
            return await task.WaitAsync(cancellationToken).ConfigureAwait(false);
#else
            var cancelTaskCompletionSource = new TaskCompletionSource<TValue>();
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

            return await Task.FromCanceled<TValue>(cancellationToken).ConfigureAwait(false);
#endif
        }

        task = this.task = Task.Run(() => this.factory(cancellationToken), cancellationToken);
        return await task.ConfigureAwait(false);
    }

    /// <summary>Configures the await.</summary>
    /// <param name="continueOnCapturedContext">if set to <c>true</c> [continue on captured context].</param>
    /// <returns>A <see cref="ConfiguredTaskAwaitable{TResult}"/>.</returns>
    public ConfiguredTaskAwaitable<TValue> ConfigureAwait(bool continueOnCapturedContext)
    {
        return this.GetValueAsync(CancellationToken.None).ConfigureAwait(continueOnCapturedContext);
    }

    /// <summary>
    /// Gets the awaiter.
    /// </summary>
    /// <returns>
    /// A task awaiter.
    /// </returns>
    public TaskAwaiter<TValue> GetAwaiter()
    {
        return this.GetValueAsync(CancellationToken.None).GetAwaiter();
    }
}