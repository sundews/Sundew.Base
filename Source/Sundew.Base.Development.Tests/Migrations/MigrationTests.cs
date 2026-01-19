// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable SA1402
#pragma warning disable SA1201 // Elements should appear in the correct order
namespace Sundew.Base.Development.Tests.Migrations;

using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Sundew.Base.Migrations;
using Sundew.Base.Migrations.System.Text.Json;
using Sundew.DiscriminatedUnions;
using Xunit;

public class MigrationTests
{
    [Fact]
    public async Task Migrate_When_MigratingV1_Then_ResultShouldBeValidV3()
    {
        var personDefaultsProvider = new PersonValueProvider(new AddressValueProvider());
        var testee = new Migrator<Person, Person.V3, IPersonValueProvider>(personDefaultsProvider);

        var person = new Person.V1("John");
        var jsonSerializerOptions = new JsonSerializerOptions { Converters = { new MigratableJsonConverter<Person>() } };
        var person_text = JsonSerializer.Serialize<Person>(person, jsonSerializerOptions);
        var person_deserialized = JsonSerializer.Deserialize<Person>(person_text, jsonSerializerOptions)!;

        var migrationResult = await testee.Migrate(person_deserialized);

        migrationResult.IsSuccess.Should().BeTrue();
        migrationResult.Value!.Name.Should().Be(person.Name);
        migrationResult.Value!.LastName.Should().Be(string.Empty);
        migrationResult.Value!.Address.Should().Be(await personDefaultsProvider.GetAddressV2(null, person.Name, string.Empty));
        migrationResult.Value.Age.Should().Be(0);
    }

    [Fact]
    public async Task Migrate_When_MigratingV2_Then_ResultShouldBeValidV3()
    {
        var personDefaultsProvider = new PersonValueProvider(new AddressValueProvider());
        var testee = new Migrator<Person, Person.V3, IPersonValueProvider>(personDefaultsProvider);

        var person = new Person.V2("John", "Doe", new Address.V1("Some Street", "Some Number", "Some Code", "Some City"));
        var jsonSerializerOptions = new JsonSerializerOptions { Converters = { new MigratableJsonConverter<Person>() } };
        var person_text = JsonSerializer.Serialize<Person>(person, jsonSerializerOptions);
        var person_deserialized = JsonSerializer.Deserialize<Person>(person_text, jsonSerializerOptions)!;

        var migrationResult = await testee.Migrate(person_deserialized);

        migrationResult.IsSuccess.Should().BeTrue();
        migrationResult.Value!.Name.Should().Be(person.Name);
        migrationResult.Value!.LastName.Should().Be(person.LastName);
        migrationResult.Value!.Address.Should().Be(await personDefaultsProvider.GetAddressV2(person.Address, person.Name, person.LastName));
        migrationResult.Value.Age.Should().Be(0);
    }

    [Fact]
    public async Task Migrate_When_MigratingV3_Then_ResultShouldBeValidV3()
    {
        var personDefaultsProvider = new PersonValueProvider(new AddressValueProvider());
        var testee = new Migrator<Person, Person.V3, IPersonValueProvider>(personDefaultsProvider);

        var person = new Person.V3("John", "Doe", 45, new Address.V2("Some Street", "Some Number", "Some Code", "Some City", "Some Apartment", "Some second line"));
        var jsonSerializerOptions = new JsonSerializerOptions { Converters = { new MigratableJsonConverter<Person>() } };
        var person_text = JsonSerializer.Serialize<Person>(person, jsonSerializerOptions);
        var person_deserialized = JsonSerializer.Deserialize<Person>(person_text, jsonSerializerOptions)!;

        var migrationResult = await testee.Migrate(person_deserialized);

        migrationResult.IsSuccess.Should().BeTrue();
        migrationResult.Value!.Name.Should().Be(person.Name);
        migrationResult.Value!.LastName.Should().Be(person.LastName);
        migrationResult.Value!.Address.Should().Be(person.Address);
        migrationResult.Value.Age.Should().Be(person.Age);
    }

    [Fact]
    public async Task Migrate_When_MigratingList_Then_ResultShouldBeValidListOfV3()
    {
        var personDefaultsProvider = new PersonValueProvider(new AddressValueProvider());
        var testee = new Migrator<Person, Person.V3, IPersonValueProvider>(personDefaultsProvider);

        var persons = new Person[] { new Person.V1("John"), new Person.V2("Jane", "Doe", new Address.V1("Some Street", "Some Number", "Some Code", "Some City")), new Person.V3("John", "Doe", 30, new Address.V2("Some Street", "Some Number", "Some Code", "Some City", "Some Apartment", "Some second line")) };
        var jsonSerializerOptions = new JsonSerializerOptions { Converters = { new MigratableJsonConverter<Person>() } };
        var persons_text = JsonSerializer.Serialize(persons, jsonSerializerOptions);
        var persons_deserialized = JsonSerializer.Deserialize<Person[]>(persons_text, jsonSerializerOptions)!;

        var migrationResult = await testee.Migrate(persons_deserialized);

        migrationResult.IsSuccess.Should().BeTrue();
        migrationResult.Value!.Should().HaveCount(persons.Length);
    }
}

[DiscriminatedUnion]
public abstract partial record Person : IMigratable<Person, IPersonValueProvider>
{
    public sealed record V1(string Name) : Person;

    public sealed record V2(string Name, string LastName, Address.V1? Address) : Person;

    public sealed record V3(string Name, string LastName, int Age, Address.V2? Address) : Person;

    public static async ValueTask<MigrationResult<Person>> Migrate(Person person, IPersonValueProvider valueProvider, CancellationToken cancellationToken)
    {
        return person switch
        {
            V1 v1 => new MigrationResult<Person>(MigrationOutcome.Migrated, new V2(v1.Name, valueProvider.GetDefaultLastName(), await valueProvider.GetAddressV1(v1.Name, valueProvider.GetDefaultLastName()))),
            V2 v2 => new MigrationResult<Person>(MigrationOutcome.Migrated, new V3(v2.Name, v2.LastName, valueProvider.GetDefaultAge(), await valueProvider.GetAddressV2(v2.Address, v2.Name, v2.LastName))),
            V3 v3 => new MigrationResult<Person>(MigrationOutcome.Migrated, v3),
        };
    }

    public static IReadOnlyCollection<MigrationInfo> GetMigrationInfo() => DiscriminatedUnionMigrations.FromVersionNamedUnion<Person>();
}

[DiscriminatedUnion]
public abstract record Address
{
    public sealed record V1(string Street, string Number, string PostalCode, string City) : Address;

    public sealed record V2(string Street, string Number, string PostalCode, string City, string? Apartment, string? SecondLine) : Address;
}

public interface IPersonValueProvider
{
    string GetDefaultLastName();

    int GetDefaultAge();

    ValueTask<Address.V1?> GetAddressV1(string name, string lastName);

    ValueTask<Address.V2?> GetAddressV2(Address.V1? address, string name, string lastName);
}

public class PersonValueProvider : IPersonValueProvider
{
    private readonly IAddressValueProvider addressValueProvider;

    public PersonValueProvider(IAddressValueProvider addressValueProvider)
    {
        this.addressValueProvider = addressValueProvider;
    }

    public string GetDefaultLastName()
    {
        return string.Empty;
    }

    public int GetDefaultAge()
    {
        return 0;
    }

    public async ValueTask<Address.V1?> GetAddressV1(string name, string lastName)
    {
        var addressResult = await this.addressValueProvider.SearchAddress(name, lastName);
        return addressResult.HasValue ? new Address.V1(addressResult.Street, addressResult.Number, addressResult.PostalCode, addressResult.City) : null;
    }

    public async ValueTask<Address.V2?> GetAddressV2(Address.V1? address, string name, string lastName)
    {
        var addressResult = await this.addressValueProvider.SearchAddress(address, name, lastName);
        return addressResult;
    }
}

public interface IAddressValueProvider
{
    ValueTask<Address.V2?> SearchAddress(string name, string lastName);

    ValueTask<Address.V2?> SearchAddress(Address.V1? address, string name, string lastName);
}

public class AddressValueProvider : IAddressValueProvider
{
    public ValueTask<Address.V2?> SearchAddress(string name, string lastName)
    {
        return ValueTask.FromResult<Address.V2?>(new Address.V2("Street", "1", "1234", "City", null, null));
    }

    public ValueTask<Address.V2?> SearchAddress(Address.V1? address, string name, string lastName)
    {
        if (address.HasValue)
        {
            return ValueTask.FromResult<Address.V2?>(new Address.V2(address.Street, address.Number, address.PostalCode, address.City, null, null));
        }

        return this.SearchAddress(name, lastName);
    }
}