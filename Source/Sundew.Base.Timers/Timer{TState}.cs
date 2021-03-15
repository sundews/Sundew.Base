// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Timer{TState}.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Timers
{
#nullable disable
    /// <summary>
    /// Implementation of a timer with state.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    public sealed class Timer<TState> : TimerBase, ITimer<TState>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Timer{TState}"/> class.
        /// </summary>
        /// <param name="state">The state.</param>
        public Timer(TState state)
            : base(state)
        {
        }

        /// <summary>
        /// Occurs when the timer ticks.
        /// </summary>
        public event TickEventHandler<TState> Tick;

        /// <summary>
        /// Called when the timer ticks.
        /// </summary>
        /// <param name="state">The state.</param>
        protected override void OnTick(object state)
        {
            this.Tick?.Invoke(this, (TState)state);
        }
    }
}