// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEvent.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Notifications;

/// <summary>
/// Interface for implementing an event.
/// </summary>
/// <typeparam name="TEvent">The type of the event data passed to event handlers.</typeparam>
public interface IEvent<TEvent> : INotifier<TEvent>, INotify<TEvent>
{
}