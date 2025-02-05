// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobStartStatus.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading.Jobs;

/// <summary>
/// The job start status.
/// </summary>
public enum JobStartStatus
{
    /// <summary>
    /// The job was started.
    /// </summary>
    Started = 0,

    /// <summary>
    /// The job was already running.
    /// </summary>
    WasAlreadyRunning = 1,

    /// <summary>
    /// The job was canceled.
    /// </summary>
    Canceled = 2,
}