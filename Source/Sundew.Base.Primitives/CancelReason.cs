// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CancelReason.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base;

/// <summary>
/// Defines the reasons for canceling.
/// </summary>
public enum CancelReason
{
    /// <summary>
    /// Indicates that the cancellation was requested by the externally linked token.
    /// </summary>
    External,

    /// <summary>
    /// Indicates that the cancellation was due to a timeout.
    /// </summary>
    Timeout,

    /// <summary>
    /// Indicates that the cancellation was internally requested.
    /// </summary>
    Internal,
}