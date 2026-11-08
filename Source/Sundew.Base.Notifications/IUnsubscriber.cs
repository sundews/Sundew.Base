// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUnsubscriber.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Notifications;

/// <summary>
/// A source that a <see cref="Subscription"/> can be removed from.
/// </summary>
internal interface IUnsubscriber
{
    /// <summary>
    /// Removes the specified subscription from the source.
    /// </summary>
    /// <param name="subscription">The subscription.</param>
    void Unsubscribe(Subscription subscription);
}
