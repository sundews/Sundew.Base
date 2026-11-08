// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Diagnostics.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Notifications.Generator;

using Microsoft.CodeAnalysis;

/// <summary>
/// The diagnostics reported by <see cref="NotificationOwnerGenerator"/>.
/// </summary>
internal static class Diagnostics
{
    /// <summary>
    /// Reported when a class implements <see cref="INotificationOwner"/> without <see cref="INotificationOwner.TargetSubscriptions"/>, but is not partial, so nothing is generated for it.
    /// A class that implements the property and only misses the SubscribeTo methods is not reported, since those are a convenience.
    /// Generating is optional, so this is informational: implementing the property by hand is just as valid.
    /// </summary>
    public static readonly DiagnosticDescriptor NotPartial = new(
        "SBN0001",
        "Notification owner is not partial",
        "'{0}' implements " + nameof(INotificationOwner) + " without " + nameof(INotificationOwner.TargetSubscriptions) + ". Make it partial to have " + nameof(INotificationOwner.TargetSubscriptions) + " generated.",
        Category,
        DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    private const string Category = "Sundew.Base.Notifications";
}
