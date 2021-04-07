// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncLazy{TValue,TActualValue}.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading
{
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
        private readonly Lazy<Task<TActualValue>> lazy;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncLazy{TValue,TActualValue}" /> class.
        /// </summary>
        /// <param name="valueFunc">The value function.</param>
        /// <param name="runOnThreadPool">if set to <c>true</c> [run on thread pool].</param>
        public AsyncLazy(Func<Task<TActualValue>> valueFunc, bool runOnThreadPool = false)
        {
            var actualValueFunc = valueFunc;
            if (runOnThreadPool)
            {
                actualValueFunc = () => Task.Run(valueFunc);
            }

            this.lazy = new Lazy<Task<TActualValue>>(actualValueFunc, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncLazy{TValue,TActualValue}" /> class.
        /// </summary>
        /// <param name="valueFunc">The value function.</param>
        public AsyncLazy(Func<TActualValue> valueFunc)
        {
            this.lazy = new Lazy<Task<TActualValue>>(() => Task.Run(valueFunc), LazyThreadSafetyMode.ExecutionAndPublication);
        }

        /// <summary>
        /// Gets the value or default.
        /// </summary>
        /// <returns>The created value or default.</returns>
        [return: MaybeNull]
        public TActualValue GetValueOrDefault()
        {
            if (this.lazy.IsValueCreated)
            {
                return this.lazy.Value.Result;
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

        /// <summary>Configures the await.</summary>
        /// <param name="continueOnCapturedContext">if set to <c>true</c> [continue on captured context].</param>
        /// <returns>A <see cref="ConfiguredTaskAwaitable{TResult}"/>.</returns>
        public ConfiguredTaskAwaitable<TActualValue> ConfigureAwait(bool continueOnCapturedContext)
        {
            return this.lazy.Value.ConfigureAwait(continueOnCapturedContext);
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
        /// <returns>
        /// A task awaiter.
        /// </returns>
        public TaskAwaiter<TActualValue> GetAwaiter()
        {
            return this.lazy.Value.GetAwaiter();
        }

        /// <summary>
        /// Gets the awaiter.
        /// </summary>
        /// <returns>A task awaiter.</returns>
        TaskAwaiter<TValue> IAsyncLazy<TValue>.GetAwaiter()
        {
            return this.GetTask(true).GetAwaiter();
        }

        private async Task<TValue> GetTask(bool continueOnCapturedContext)
        {
            return await this.ConfigureAwait(continueOnCapturedContext);
        }
    }
}