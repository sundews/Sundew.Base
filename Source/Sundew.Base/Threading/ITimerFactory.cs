// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITimerFactory.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading
{
    using System;

    /// <summary>
    /// Interface for implementing a timer factory.
    /// </summary>
    public interface ITimerFactory : IDisposable
    {
        /// <summary>
        /// Creates a new <see cref="ITimer"/>.
        /// </summary>
        /// <returns>A new <see cref="ITimer"/>.</returns>
        ITimer Create();

        /// <summary>
        /// Creates a new <see cref="ITimer{TState}"/>.
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <param name="state">The state.</param>
        /// <returns>A new <see cref="ITimer{TState}"/>.</returns>
        ITimer<TState> Create<TState>(TState state)
            where TState : notnull;

        /// <summary>
        /// Disposes the specified timer base.
        /// </summary>
        /// <param name="timerControl">The timer base.</param>
        void Dispose(ITimerControl timerControl);
    }
}