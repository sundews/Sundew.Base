// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeserializationTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable SA1302
#pragma warning disable SA1402
#pragma warning disable SA1201 // Elements should appear in the correct order
namespace Sundew.Base.Development.Tests.Migrations.System_Text_Json;

using global::AwesomeAssertions;
using global::Sundew.Base.Migrations;
using global::Sundew.Base.Migrations.System.Text.Json;
using global::Sundew.DiscriminatedUnions;
using global::System.Collections.Generic;
using global::System.Text.Json;
using global::System.Threading;
using global::System.Threading.Tasks;

public class DeserializationTests
{
    [Test]
    public void Deserialize_When_SerializedWithNewerBuildAndDeserializingWithOldVersion_Then_JsonExceptionShouldBeThrow()
    {
        var person = new Person("John", "Doe", 45, new Address.V2("Some Street", "Some Number", "Some Code", "Some City", "Some Apartment", "Some second line"));
        var currentJsonSerializerOptions = new JsonSerializerOptions { Converters = { new MigratableJsonConverter<PersonDto>() } };
        var pastJsonSerializerOptions = new JsonSerializerOptions { Converters = { new MigratableJsonConverter<PersonPastDto>() } };
        var person_text = JsonSerializer.Serialize<PersonDto>(person, currentJsonSerializerOptions);
        var person_deserialize = () => JsonSerializer.Deserialize<PersonPastDto>(person_text, pastJsonSerializerOptions)!;

        person_deserialize.Should().Throw<UnsupportedVersionJsonException>();
    }

    [Test]
    public void Deserialize_When_SerializedWithNewerBuildAndDeserializingWithOldVersion_Then_ResultShouldBeExpectedResult()
    {
        var person = new Person("John", "Doe", 45, new Address.V2("Some Street", "Some Number", "Some Code", "Some City", "Some Apartment", "Some second line"));
        var expectedResult = new PersonPast(person.Name);
        var currentJsonSerializerOptions = new JsonSerializerOptions { Converters = { new MigratableJsonConverter<PersonDto>() } };
        var pastJsonSerializerOptions = new JsonSerializerOptions { Converters = { new MigratableJsonConverter<PersonPastDto>(true) } };
        var person_text = JsonSerializer.Serialize<PersonDto>(person, currentJsonSerializerOptions);
        var person_deserialized = JsonSerializer.Deserialize<PersonPastDto>(person_text, pastJsonSerializerOptions)!;

        person_deserialized.Should().Be(expectedResult);
    }
}

[DiscriminatedUnion]
public abstract partial record PersonPastDto : IMigratable<PersonPastDto, __>
{
    public static ValueTask<MigrationResult<PersonPastDto>> Migrate(PersonPastDto personPastDto, __ valueProvider, CancellationToken cancellationToken)
    {
        return personPastDto switch
        {
            PersonPast person => ValueTask.FromResult(new MigrationResult<PersonPastDto>(MigrationOutcome.Migrated, person)),
        };
    }

    public static IReadOnlyCollection<MigrationInfo> GetMigrationInfo() => DiscriminatedUnionMigrations.FromVersionNamedUnion<PersonPastDto>();
}

public sealed record PersonPast(string Name) : PersonPastDto;