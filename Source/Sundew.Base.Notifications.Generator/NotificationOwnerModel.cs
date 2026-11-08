// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationOwnerModel.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Notifications.Generator;

/// <summary>
/// What is needed to implement INotificationOwner for a partial class.
/// </summary>
/// <param name="HintName">The hint name of the generated file.</param>
/// <param name="Namespace">The namespace, or an empty string for the global namespace.</param>
/// <param name="Declarations">The partial declarations from the outermost type to the target type, one per line, such as <c>partial class Outer&lt;T&gt;</c>.</param>
/// <param name="GenerateTargetSubscriptions">Whether to generate TargetSubscriptions, which the class does not implement itself.</param>
/// <param name="SubscribeToModifier">The access modifier of the SubscribeTo methods, or null when the class or a base class already has them.</param>
/// <param name="EventTypeParameter">The name of the event type parameter, chosen not to clash with the type parameters of the class.</param>
/// <param name="SubscribedEventTypeParameter">The name of the subscribed event type parameter, chosen not to clash with the type parameters of the class.</param>
internal sealed record NotificationOwnerModel(
    string HintName,
    string Namespace,
    string Declarations,
    bool GenerateTargetSubscriptions,
    string? SubscribeToModifier,
    string EventTypeParameter,
    string SubscribedEventTypeParameter);
