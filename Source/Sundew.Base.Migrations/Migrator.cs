// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Migrator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Migrations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sundew.Base.Collections;
using Sundew.Base.Collections.Linq;

/// <summary>
/// Provides functionality to migrate objects implementing the IMigratable interface to their latest version using a
/// specified defaults provider.
/// </summary>
/// <typeparam name="TMigratable">The type of the object to be migrated. Must implement IMigratable and define migration logic.</typeparam>
/// <typeparam name="TMigrated">The type representing the latest version of the migratable object. Must inherit from TMigratable.</typeparam>
/// <typeparam name="TValueProvider">The type of the provider that supplies values required during migration.</typeparam>
public class Migrator<TMigratable, TMigrated, TValueProvider>
    where TMigrated : TMigratable
    where TMigratable : IMigratable<TMigratable, TValueProvider>
{
    private readonly TValueProvider valueProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="Migrator{TMigratable, TMigrated, TDefaultsProvider}"/> class.
    /// </summary>
    /// <param name="valueProvider">The provider that supplies default values for migration operations. Cannot be null.</param>
    public Migrator(TValueProvider valueProvider)
    {
        this.valueProvider = valueProvider;
    }

    /// <summary>
    /// Attempts to migrate the specified object to the target type, applying one or more migration steps as needed.
    /// </summary>
    /// <remarks>The migration process may involve multiple steps if the input object requires intermediate
    /// migrations before reaching the target type. If migration fails at any step, an error result is
    /// returned.</remarks>
    /// <param name="migratable">The object to be migrated. Must implement the required migration interface.</param>
    /// <param name="cancellation">The cancellation.</param>
    /// <returns>A result containing the migrated object of type TMigrated if migration succeeds; otherwise, an error result.</returns>
    public async Task<R<TMigrated, MigrationError<TMigratable>>> Migrate(TMigratable migratable, Cancellation cancellation = default)
    {
        using var enabler = cancellation.EnableCancellation();
        try
        {
            return await this.PrivateMigrate(migratable, enabler);
        }
        catch (AggregateException aggregateException)
        {
            if (aggregateException.Flatten().InnerExceptions.Any(x => x is OperationCanceledException))
            {
                return R.Error(new MigrationError<TMigratable>(true, migratable));
            }

            throw;
        }
        catch (OperationCanceledException)
        {
            return R.Error(new MigrationError<TMigratable>(true, migratable));
        }
    }

    /// <summary>
    /// Attempts to migrate the specified object to the target type, applying one or more migration steps as needed.
    /// </summary>
    /// <remarks>The migration process may involve multiple steps if the input object requires intermediate
    /// migrations before reaching the target type. If migration fails at any step, an error result is
    /// returned.</remarks>
    /// <param name="migratables">The objects to be migrated. Must implement the required migration interface.</param>
    /// <param name="cancellation">The cancellation.</param>
    /// <returns>A result containing the migrated object of type TMigrated if migration succeeds; otherwise, an error result.</returns>
    public Task<R<IReadOnlyList<TMigrated>, MigrationError<IEnumerable<TMigratable>>>> Migrate(IEnumerable<TMigratable> migratables, Cancellation cancellation = default)
    {
        return this.Migrate(migratables, Parallelism.Default, cancellation);
    }

    /// <summary>
    /// Attempts to migrate the specified object to the target type, applying one or more migration steps as needed.
    /// </summary>
    /// <remarks>The migration process may involve multiple steps if the input object requires intermediate
    /// migrations before reaching the target type. If migration fails at any step, an error result is
    /// returned.</remarks>
    /// <param name="migratables">The objects to be migrated. Must implement the required migration interface.</param>
    /// <param name="parallelism">The parallelism.</param>
    /// <param name="cancellation">The cancellation.</param>
    /// <returns>A result containing the migrated object of type TMigrated if migration succeeds; otherwise, an error result.</returns>
    public async Task<R<IReadOnlyList<TMigrated>, MigrationError<IEnumerable<TMigratable>>>> Migrate(IEnumerable<TMigratable> migratables, Parallelism parallelism, Cancellation cancellation = default)
    {
        using var enabler = cancellation.EnableCancellation();
        try
        {
            var migrations = await migratables.SelectAsync(
                parallelism,
                async migratable => (migratable, result: await this.PrivateMigrate(migratable, enabler).ConfigureAwait(false)));
            var result = migrations.AllOrFailed(x => x.result.IsSuccess ? Item.Pass(x.result.Value) : Item.Fail());
            return result.Map(x => (IReadOnlyList<TMigrated>)x.Items, x => new MigrationError<IEnumerable<TMigratable>>(cancellation.IsCancellationRequested, x.Items.Select(x => x.Item.migratable)));
        }
        catch (AggregateException aggregateException)
        {
            if (aggregateException.Flatten().InnerExceptions.Any(x => x is OperationCanceledException))
            {
                return R.Error(new MigrationError<IEnumerable<TMigratable>>(true, migratables));
            }

            throw;
        }
        catch (OperationCanceledException)
        {
            return R.Error(new MigrationError<IEnumerable<TMigratable>>(true, migratables));
        }
    }

    private async Task<R<TMigrated, MigrationError<TMigratable>>> PrivateMigrate(TMigratable migratable, Cancellation.Enabler enabler)
    {
        var result = new MigrationResult<TMigratable>(MigrationOutcome.Migrated, migratable);
        while (result is { MigrationOutcome: MigrationOutcome.Migrated, Migratable: not TMigrated } && enabler.ContinueOrThrowIfCancellationRequested())
        {
            result = await TMigratable.Migrate(result.Migratable, this.valueProvider, enabler).ConfigureAwait(false);
        }

        return result.MigrationOutcome switch
        {
            MigrationOutcome.Migrated => result.Migratable is TMigrated migrated ? R.Success(migrated).Omits<MigrationError<TMigratable>>() : R.Error(new MigrationError<TMigratable>(enabler.IsCancellationRequested, result.Migratable)),
            MigrationOutcome.Failed => R.Error(new MigrationError<TMigratable>(false, result.Migratable)),
        };
    }
}