// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITimer{TState}.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Timers
{
    /// <summary>
    /// Interface for implementing a timer with state.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <seealso cref="System.IDisposable" />
    public interface ITimer<TState> : ITimerControl
    {
        /// <summary>
        /// Occurs when the timer ticks.
        /// </summary>
        event TickEventHandler<TState> Tick;
    }
}