// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITimer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading
{
    /// <summary>
    /// Interface for implementing a stateless timer.
    /// </summary>
    /// <seealso cref="Sundew.Base.Threading.ITimerControl" />
    public interface ITimer : ITimerControl
    {
        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        event TickEventHandler Tick;
    }
}