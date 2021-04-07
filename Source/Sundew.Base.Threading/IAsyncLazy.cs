// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAsyncLazy.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Interface for implementing an async lazy.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public interface IAsyncLazy<TValue>
    {
        /// <summary>
        /// Gets the value or default.
        /// </summary>
        /// <returns>The created value or default.</returns>
        [return: MaybeNull]
        TValue GetValueOrDefault();

        /// <summary>Configures the await.</summary>
        /// <param name="continueOnCapturedContext">if set to <c>true</c> [continue on captured context].</param>
        /// <returns>A <see cref="ConfiguredTaskAwaitable{TResult}"/>.</returns>
        ConfiguredTaskAwaitable<TValue> ConfigureAwait(bool continueOnCapturedContext);

        /// <summary>
        /// Gets the awaiter.
        /// </summary>
        /// <returns>A task awaiter.</returns>
        TaskAwaiter<TValue> GetAwaiter();
    }
}