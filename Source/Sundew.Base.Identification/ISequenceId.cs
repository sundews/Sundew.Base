// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISequenceId.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

/// <summary>
/// Interface for implementing an incremental id.
/// </summary>
/// <typeparam name="TId">The id type.</typeparam>
public interface ISequenceId<TId>
    where TId : ISequenceId<TId>
{
    /// <summary>
    /// Gets the code.
    /// </summary>
    uint Number { get; }

    /// <summary>
    /// Creates an Id.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <returns>The new id.</returns>
    static abstract TId Create(uint id);
}