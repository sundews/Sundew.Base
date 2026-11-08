// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Subscriptions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Notifications;

/// <summary>
/// Stand-in for the Sundew.Base.Notifications type of the same name.
/// </summary>
/// <remarks>
/// The generator cannot reference Sundew.Base.Notifications, so it compiles the linked <see cref="INotificationOwner"/> source instead
/// and needs this type for the property to compile. This lets the generator refer to the contract with <see langword="typeof"/> and
/// <see langword="nameof"/>, so a rename in the library is a compile error here rather than a silently idle generator.
/// An analyzer assembly is never referenced by consumer code, so this type does not clash with the real one.
/// </remarks>
public sealed class Subscriptions
{
}
