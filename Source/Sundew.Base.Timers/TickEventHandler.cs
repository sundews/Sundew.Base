// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TickEventHandler.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Timers
{
    /// <summary>
    /// Delegate for <see cref="ITimer{TState}" /> tick events.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <param name="timer">The timer.</param>
    /// <param name="state">The state.</param>
    public delegate void TickEventHandler<TState>(ITimer<TState> timer, TState state);

    /// <summary>
    /// Delegate for <see cref="ITimer"/> tick events.
    /// </summary>
    /// <param name="timer">The timer.</param>
    public delegate void TickEventHandler(ITimer timer);
}