// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIdentifiable.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

using System;

/// <summary>
/// Interface for implementing an identifiable.
/// </summary>
/// <typeparam name="TId">The type of the value id.</typeparam>
public interface IIdentifiable<out TId>
    where TId : IEquatable<TId>
{
    /// <summary>
    /// Gets the value id.
    /// </summary>
    /// <returns>The value id.</returns>
    TId Id { get; }
}