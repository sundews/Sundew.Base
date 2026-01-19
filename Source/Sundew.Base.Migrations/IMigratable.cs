// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMigratable.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Migrations;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Defines a contract for types that can provide migration version information.
/// </summary>
/// <remarks>Implementing this interface allows a type to expose its supported migration versions through a static
/// method. This is typically used in scenarios where versioned migrations or upgrades are required, such as database
/// schema evolution or data format changes.</remarks>
public interface IMigratable
{
    /// <summary>
    /// Gets a read-only collection containing version information for the implementing type.
    /// </summary>
    /// <returns>A read-only collection of <see cref="Migrations.MigrationInfo"/> objects that describe the available versions. The collection
    /// is empty if no version information is available.</returns>
    static abstract IReadOnlyCollection<MigrationInfo> GetMigrationInfo();
}

/// <summary>
/// Defines a contract for types that support migration using a specified defaults provider.
/// </summary>
/// <typeparam name="TMigratable">The type of the object to be migrated.</typeparam>
/// <typeparam name="TValueProvider">The type of the provider that supplies values during migration.</typeparam>
public interface IMigratable<TMigratable, in TValueProvider> : IMigratable
{
    /// <summary>
    /// Migrates the specified object to the latest supported version, applying default values as needed.
    /// </summary>
    /// <remarks>Use this method to ensure that objects created with earlier versions are compatible with the
    /// current version. The migration process may apply default values to fields that did not exist in previous
    /// versions, as provided by the defaults provider.</remarks>
    /// <param name="migratable">The object to migrate. Represents the source instance to be upgraded to the latest version.</param>
    /// <param name="valueProvider">An object that provides values for any new or missing fields during migration.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A migration result.</returns>
    static abstract ValueTask<MigrationResult<TMigratable>> Migrate(TMigratable migratable, TValueProvider valueProvider, CancellationToken cancellationToken);
}