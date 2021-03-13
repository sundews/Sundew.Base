// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITimerControl.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading
{
    using System;

    /// <summary>
    /// Interface for controlling a timer.
    /// </summary>
    public interface ITimerControl : ITimerInfo, IDisposable
    {
        /// <summary>
        /// Starts or restarts the timer.
        /// </summary>
        /// <param name="startDelay">The delay before the first occurence.</param>
        void StartOnce(TimeSpan startDelay);

        /// <summary>
        /// Starts or restarts the timer.
        /// </summary>
        /// <param name="interval">The interval.</param>
        void Start(TimeSpan interval);

        /// <summary>
        /// Starts or restarts the timer.
        /// </summary>
        /// <param name="startDelay">The delay before the first occurence.</param>
        /// <param name="interval">The interval.</param>
        void Start(TimeSpan startDelay, TimeSpan interval);

        /// <summary>
        /// Stops the timer.
        /// </summary>
        void Stop();
    }
}