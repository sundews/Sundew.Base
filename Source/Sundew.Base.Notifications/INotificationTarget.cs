// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INotificationTarget.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Notifications;

/// <summary>
/// Interface for implementing a subscription target.
/// </summary>
public interface INotificationTarget
{
    /// <summary>
    /// Gets the subscriptions.
    /// </summary>
    /// <returns>The subscriptions.</returns>
    Subscriptions TargetSubscriptions { get; }
}