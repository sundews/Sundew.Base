// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeakUnsubscriber.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Notifications;

using System;

/// <summary>
/// Forwards to an <see cref="IUnsubscriber"/> that it references only weakly.
/// </summary>
/// <remarks>
/// A source creates one instance and shares it among all of its subscriptions, so the subscriptions, and through them the
/// notification owners, never keep the source alive. Once the source has been collected there is nothing left to remove a
/// subscription from, and unsubscribing becomes a no-op.
/// </remarks>
internal sealed class WeakUnsubscriber : IUnsubscriber
{
    private readonly WeakReference<IUnsubscriber> unsubscriber;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeakUnsubscriber"/> class.
    /// </summary>
    /// <param name="unsubscriber">The source to forward to.</param>
    public WeakUnsubscriber(IUnsubscriber unsubscriber)
    {
        this.unsubscriber = new WeakReference<IUnsubscriber>(unsubscriber);
    }

    /// <inheritdoc/>
    public void Unsubscribe(Subscription subscription)
    {
        if (this.unsubscriber.TryGetTarget(out var target))
        {
            target.Unsubscribe(subscription);
        }
    }
}
