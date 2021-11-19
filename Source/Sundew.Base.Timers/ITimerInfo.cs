// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITimerInfo.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Timers;

using System;

/// <summary>
/// Interface for getting info for timers.
/// </summary>
public interface ITimerInfo
{
    /// <summary>
    /// Gets a value indicating whether this instance is running.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is running; otherwise, <c>false</c>.
    /// </value>
    bool IsEnabled { get; }

    /// <summary>
    /// Gets the interval.
    /// </summary>
    /// <value>
    /// The interval.
    /// </value>
    TimeSpan Interval { get; }
}