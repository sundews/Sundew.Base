// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstanceId.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

/// <summary>
/// Default implementation of <see cref="ISequenceId{TId}"/> into an instance id.
/// </summary>
/// <param name="Number">The number.</param>
public readonly record struct InstanceId(uint Number) : ISequenceId<InstanceId>
{
    /// <summary>
    /// Creates a new instance id.
    /// </summary>
    /// <param name="number">The number.</param>
    /// <returns>The new instance id.</returns>
    public static InstanceId Create(uint number)
    {
        return new InstanceId(number);
    }

    /// <summary>
    /// Returns a string that represents this instance.
    /// </summary>
    /// <returns>The id as a string.</returns>
    public override string ToString()
    {
        return '#' + this.Number.ToString();
    }
}