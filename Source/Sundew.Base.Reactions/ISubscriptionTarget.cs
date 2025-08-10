// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISubscriptionTarget.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Reactions;

/// <summary>
/// Interface for implementing a subscription target.
/// </summary>
public interface ISubscriptionTarget
{
    /// <summary>
    /// Gets the subscriptions.
    /// </summary>
    /// <returns>The subscriptions.</returns>
    Subscriptions ReactionSubscriptions { get; }
}