// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimerFactory.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Timers
{
    using Sundew.Base.Disposal;

    /// <summary>
    /// Factory for creating timers.
    /// </summary>
    /// <seealso cref="ITimerFactory" />
    public sealed class TimerFactory : ITimerFactory
    {
        private readonly DisposingList<ITimerControl> timers = new();

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>A new <see cref="Timer"/>.</returns>
        public ITimer Create()
        {
            return this.timers.Add(new Timer());
        }

        /// <summary>
        /// Creates the specified state.
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <param name="state">The state.</param>
        /// <returns>A new <see cref="Timer{TState}"/>.</returns>
        public ITimer<TState> Create<TState>(TState state)
            where TState : notnull
        {
            return this.timers.Add(new Timer<TState>(state));
        }

        /// <summary>
        /// Disposes the specified timer base.
        /// </summary>
        /// <param name="timerControl">The timer base.</param>
        public void Dispose(ITimerControl timerControl)
        {
            this.timers.Dispose(timerControl);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            this.timers.Dispose();
        }
    }
}