// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationError.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Migrations;

/// <summary>
/// Specifies the possible errors of a migration operation.
/// </summary>
/// <typeparam name="TMigratable">The type of the migratable.</typeparam>
public sealed record MigrationError<TMigratable>(bool WasCancelled, TMigratable Failed);