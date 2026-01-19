// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationOutcome.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Migrations;

using Sundew.DiscriminatedUnions;

/// <summary>
/// Specifies the possible outcomes of a migration operation.
/// </summary>
/// <remarks>Use this enumeration to determine whether an entity was successfully migrated or if the migration
/// operation failed. The values can be used to control application flow or to provide feedback to users regarding the
/// result of a migration attempt.</remarks>
[DiscriminatedUnion]
public enum MigrationOutcome
{
    /// <summary>
    /// Indicates the migration was successful.
    /// </summary>
    Migrated,

    /// <summary>
    /// Indicates that the operation did not complete successfully.
    /// </summary>
    Failed,
}