// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RevisionId.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

/// <summary>
/// Default implementation of <see cref="ISequenceId{TId}"/> into a revision id.
/// </summary>
/// <param name="Number">The code.</param>
public readonly record struct RevisionId(uint Number) : ISequenceId<RevisionId>
{
    /// <summary>
    /// Creates a new revision id.
    /// </summary>
    /// <param name="number">The number.</param>
    /// <returns>The new revision id.</returns>
    public static RevisionId Create(uint number)
    {
        return new RevisionId(number);
    }
}