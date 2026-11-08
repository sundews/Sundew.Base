// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INotificationOwner.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Notifications;

/// <summary>
/// A type that owns subscriptions.
/// </summary>
/// <remarks>
/// The <see cref="TargetSubscriptions"/> of a notification owner own the subscriptions made with it, which lets the owner unsubscribe
/// everything it owns with <see cref="Subscriptions.UnsubscribeAll(INotificationOwner)"/>, and which is what keeps a subscription alive
/// for an event that does not keep its subscribers alive: such a subscription ends when the <see cref="TargetSubscriptions"/> become
/// unreachable, or when it is unsubscribed, and nothing needs to be disposed for that to happen. An owner whose <see cref="TargetSubscriptions"/>
/// serve no other purpose controls the lifetime of its subscriptions explicitly.
/// The source generator shipped with this package implements <see cref="TargetSubscriptions"/> for partial classes, and adds protected
/// SubscribeTo methods to them, which subscribe the owner to an INotify or INotifyAny event source without passing the owner along.
/// </remarks>
public interface INotificationOwner
{
    /// <summary>
    /// Gets the subscriptions owned by this target.
    /// </summary>
    Subscriptions TargetSubscriptions { get; }
}
