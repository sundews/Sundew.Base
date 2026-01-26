// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Subscription.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Notifications;

using System;
using Sundew.Base.Disposal;

/// <summary>
/// Delegate representing an unsubscribe action.
/// </summary>
public struct Subscription
{
    private Action<Subscription>? unsubscribe;

    /// <summary>
    /// Initializes a new instance of the <see cref="Subscription"/> struct.
    /// </summary>
    /// <param name="unsubscribe">The action to invoke when unsubscribing. Cannot be null.</param>
    public Subscription(Action<Subscription> unsubscribe)
    {
        this.unsubscribe = unsubscribe;
    }

    /// <summary>
    /// Unsubscribes from the associated event or notification, preventing further callbacks or notifications from being
    /// received.
    /// </summary>
    /// <remarks>After calling this method, subsequent invocations have no effect. This method is typically
    /// used to release resources or stop receiving updates when they are no longer needed.</remarks>
    public void Unsubscribe()
    {
        this.unsubscribe?.Invoke(this);
        this.unsubscribe = null;
    }

    /// <summary>
    /// Returns an <see cref="IDisposable"/> that unsubscribes from the current subscription when disposed.
    /// </summary>
    /// <remarks>Use this method to integrate the subscription with <see langword="using"/> statements or
    /// other disposal patterns, ensuring that unsubscription occurs automatically when the returned object is
    /// disposed.</remarks>
    /// <returns>An <see cref="IDisposable"/> that, when disposed, unsubscribes from the subscription. If the subscription has
    /// already been unsubscribed, disposing the returned object has no effect.</returns>
    public IDisposable AsDisposable()
    {
        var subscription = this;
        return new DisposeAction(() => subscription.unsubscribe?.Invoke(subscription));
    }
}