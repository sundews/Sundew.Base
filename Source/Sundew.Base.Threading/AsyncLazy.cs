// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncLazy.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
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
    private readonly Microsoft.VisualStudio.Threading.AsyncLazy<TValue> lazy;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncLazy{TValue}" /> class.
    /// </summary>
    /// <param name="valueFunc">The value function.</param>
    public AsyncLazy(Func<Task<TValue>> valueFunc)
    {
        this.lazy = new Microsoft.VisualStudio.Threading.AsyncLazy<TValue>(valueFunc);
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
    public bool IsValueCreated => this.lazy.IsValueCreated;

    /// <summary>
    /// Gets the value or default.
    /// </summary>
    /// <returns>The created value or default.</returns>
    [return: MaybeNull]
    public TValue GetValueOrDefault()
    {
        if (this.IsValueCreated)
        {
            return this.lazy.GetValue();
        }

        return default;
    }

    /// <summary>
    /// Add cancellation support to the async lazy.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A cancellable task.</returns>
    public Task<TValue> GetValueAsync(CancellationToken cancellationToken = default)
    {
        return this.lazy.GetValueAsync(cancellationToken);
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