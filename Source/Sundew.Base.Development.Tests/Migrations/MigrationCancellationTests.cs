// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationCancellationTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable SA1402
#pragma warning disable SA1201 // Elements should appear in the correct order
namespace Sundew.Base.Development.Tests.Migrations;

using System.Diagnostics;
using AwesomeAssertions.Execution;
using global::AwesomeAssertions;
using global::Sundew.Base.Migrations;
using global::Sundew.Base.Migrations.System.Text.Json;
using global::Sundew.DiscriminatedUnions;
using global::System;
using global::System.Collections.Generic;
using global::System.Text.Json;
using global::System.Threading;
using global::System.Threading.Tasks;

public class MigrationCancellationTests
{
    [Test]
    public async Task Migrate_When_Cancelling_Then_ResultShouldBeErrorIndicatingCancellation()
    {
        var testee = new Migrator<CancelledPerson, CancelledPerson.V3, __>(__._);

        var person = new CancelledPerson.V1("John");
        var jsonSerializerOptions = new JsonSerializerOptions { Converters = { new MigratableJsonConverter<CancelledPerson>() } };
        var person_text = JsonSerializer.Serialize<CancelledPerson>(person, jsonSerializerOptions);
        var person_deserialized = JsonSerializer.Deserialize<CancelledPerson>(person_text, jsonSerializerOptions)!;

        var migrationResult = await testee.Migrate(person_deserialized, new Cancellation(TimeSpan.FromMilliseconds(10)));

        using (new AssertionScope())
        {
            migrationResult.IsSuccess.Should().BeFalse();
            migrationResult.Error!.WasCancelled.Should().BeTrue();
        }
    }

    [Test]
    public async Task Migrate_When_IsListAndCancelling_Then_ResultShouldBeErrorIndicatingCancellation()
    {
        var testee = new Migrator<CancelledPerson, CancelledPerson.V3, __>(__._);

        var persons = new CancelledPerson[] { new CancelledPerson.V1("John"), new CancelledPerson.V2("Jane", "Doe"), new CancelledPerson.V3("John", "Doe", 30) };
        var jsonSerializerOptions = new JsonSerializerOptions { Converters = { new MigratableJsonConverter<CancelledPerson>() } };
        var persons_text = JsonSerializer.Serialize(persons, jsonSerializerOptions);
        var persons_deserialized = JsonSerializer.Deserialize<CancelledPerson[]>(persons_text, jsonSerializerOptions)!;

        var migrationResult = await testee.Migrate(persons_deserialized, new Cancellation(TimeSpan.FromMilliseconds(10)));

        try
        {
            using (new AssertionScope())
            {
                migrationResult.IsSuccess.Should().BeFalse();
                migrationResult.Value.Should().BeNull();
                migrationResult.Error!.WasCancelled.Should().BeTrue();
            }
        }
        catch (Exception e)
        {
            Debugger.Break();
            Console.WriteLine(e);
            throw;
        }
    }
}

[DiscriminatedUnion]
public abstract partial record CancelledPerson : IMigratable<CancelledPerson, __>
{
    public sealed record V1(string Name) : CancelledPerson;

    public sealed record V2(string Name, string LastName) : CancelledPerson;

    public sealed record V3(string Name, string LastName, int Age) : CancelledPerson;

    public static async ValueTask<MigrationResult<CancelledPerson>> Migrate(CancelledPerson person, __ valueProvider, CancellationToken cancellationToken)
    {
        if (person is V2)
        {
            await Task.Delay(20000, cancellationToken).ConfigureAwait(false);
        }

        return person switch
        {
            V1 v1 => new MigrationResult<CancelledPerson>(MigrationOutcome.Migrated, new V2(v1.Name, string.Empty)),
            V2 v2 => new MigrationResult<CancelledPerson>(MigrationOutcome.Migrated, new V3(v2.Name, v2.LastName, 0)),
            V3 v3 => new MigrationResult<CancelledPerson>(MigrationOutcome.Migrated, v3),
        };
    }

    public static IReadOnlyCollection<MigrationInfo> GetMigrationInfo() => DiscriminatedUnionMigrations.FromVersionNamedUnion<CancelledPerson>();
}