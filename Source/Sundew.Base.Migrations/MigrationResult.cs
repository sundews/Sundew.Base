// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationResult.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Migrations;

/// <summary>
/// Represents the result of a migration operation, including the outcome and the migrated object.
/// </summary>
/// <typeparam name="TMigratable">The type of the object being migrated.</typeparam>
/// <param name="MigrationOutcome">The outcome of the migration operation.</param>
/// <param name="Migratable">The migrated object associated with the result.</param>
public readonly record struct MigrationResult<TMigratable>(MigrationOutcome MigrationOutcome, TMigratable Migratable);
