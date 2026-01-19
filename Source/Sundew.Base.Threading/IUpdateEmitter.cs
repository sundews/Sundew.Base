// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUpdateEmitter.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading;

using System.Threading.Tasks;

/// <summary>
/// Interface for implementing an update emitter.
/// </summary>
/// <typeparam name="TValue">The value type.</typeparam>
public interface IUpdateEmitter<in TValue>
{
    /// <summary>
    /// Schedules the specified value for processing.
    /// </summary>
    /// <param name="value">The value type.</param>
    /// <returns>A task representing the update operation.</returns>
    ValueTask Update(TValue value);
}