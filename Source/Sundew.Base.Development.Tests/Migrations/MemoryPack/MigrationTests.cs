// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable SA1302
#pragma warning disable SA1402
#pragma warning disable SA1201 // Elements should appear in the correct order
namespace Sundew.Base.Development.Tests.Migrations.MemoryPack;

using global::AwesomeAssertions;
using global::MemoryPack;
using global::Sundew.Base.Migrations;
using global::Sundew.DiscriminatedUnions;
using global::System;
using global::System.Collections.Generic;
using global::System.Threading;
using global::System.Threading.Tasks;
using global::TUnit.Generated;

public class MigrationTests
{
    [Test]
    public async Task Migrate_When_MigratingV1_Then_ResultShouldBeValidV3()
    {
        var personDefaultsProvider = new PersonValueProvider(new AddressValueProvider());
        var testee = new Migrator<PersonDto, Person, IPersonValueProvider>(personDefaultsProvider);

        var person = new PersonDto.V1("John");
        var person_bytes = global::MemoryPack.MemoryPackSerializer.Serialize<PersonDto>(person, new MemoryPackSerializerOptions());
        var person_deserialized = global::MemoryPack.MemoryPackSerializer.Deserialize<PersonDto>(person_bytes, new MemoryPackSerializerOptions())!;

        var migrationResult = await testee.Migrate(person_deserialized);

        migrationResult.IsSuccess.Should().BeTrue();
        migrationResult.Value!.Name.Should().Be(person.Name);
        migrationResult.Value!.LastName.Should().Be(string.Empty);
        migrationResult.Value!.Address.Should().Be(await personDefaultsProvider.GetAddressV2(null, person.Name, string.Empty));
        migrationResult.Value.Age.Should().Be(0);
    }

    [Test]
    public async Task Migrate_When_MigratingV2_Then_ResultShouldBeValidV3()
    {
        var personDefaultsProvider = new PersonValueProvider(new AddressValueProvider());
        var testee = new Migrator<PersonDto, Person, IPersonValueProvider>(personDefaultsProvider);

        var person = new PersonDto.V2("John", "Doe", new Address.V1("Some Street", "Some Number", "Some Code", "Some City"));
        var person_bytes = global::MemoryPack.MemoryPackSerializer.Serialize<PersonDto>(person, new MemoryPackSerializerOptions());
        var person_deserialized = global::MemoryPack.MemoryPackSerializer.Deserialize<PersonDto>(person_bytes, new MemoryPackSerializerOptions())!;

        var migrationResult = await testee.Migrate(person_deserialized);

        migrationResult.IsSuccess.Should().BeTrue();
        migrationResult.Value!.Name.Should().Be(person.Name);
        migrationResult.Value!.LastName.Should().Be(person.LastName);
        migrationResult.Value!.Address.Should().Be(await personDefaultsProvider.GetAddressV2(person.Address, person.Name, person.LastName));
        migrationResult.Value.Age.Should().Be(0);
    }

    [Test]
    public async Task Migrate_When_MigratingV3_Then_ResultShouldBeValidV3()
    {
        var personDefaultsProvider = new PersonValueProvider(new AddressValueProvider());
        var testee = new Migrator<PersonDto, Person, IPersonValueProvider>(personDefaultsProvider);

        var person = new Person("John", "Doe", 45, new Address.V2("Some Street", "Some Number", "Some Code", "Some City", "Some Apartment", "Some second line"));
        var person_bytes = global::MemoryPack.MemoryPackSerializer.Serialize<PersonDto>(person, new MemoryPackSerializerOptions());
        var person_deserialized = global::MemoryPack.MemoryPackSerializer.Deserialize<PersonDto>(person_bytes, new MemoryPackSerializerOptions())!;

        var migrationResult = await testee.Migrate(person_deserialized);

        migrationResult.IsSuccess.Should().BeTrue();
        migrationResult.Value!.Name.Should().Be(person.Name);
        migrationResult.Value!.LastName.Should().Be(person.LastName);
        migrationResult.Value!.Address.Should().Be(person.Address);
        migrationResult.Value.Age.Should().Be(person.Age);
    }

    [Test]
    public async Task Migrate_When_MigratingList_Then_ResultShouldBeValidListOfV3()
    {
        var personDefaultsProvider = new PersonValueProvider(new AddressValueProvider());
        var testee = new Migrator<PersonDto, Person, IPersonValueProvider>(personDefaultsProvider);

        var persons = new PersonDto[] { new PersonDto.V1("John"), new PersonDto.V2("Jane", "Doe", new Address.V1("Some Street", "Some Number", "Some Code", "Some City")), new Person("John", "Doe", 30, new Address.V2("Some Street", "Some Number", "Some Code", "Some City", "Some Apartment", "Some second line")) };
        var persons_bytes = global::MemoryPack.MemoryPackSerializer.Serialize(persons, new MemoryPackSerializerOptions());
        var persons_deserialized = global::MemoryPack.MemoryPackSerializer.Deserialize<PersonDto[]>(persons_bytes, new MemoryPackSerializerOptions())!;

        var migrationResult = await testee.Migrate(persons_deserialized);

        migrationResult.IsSuccess.Should().BeTrue();
        migrationResult.Value!.Should().HaveCount(persons.Length);
    }
}

[DiscriminatedUnion]
[MemoryPackable]
[MemoryPackUnion(1, typeof(V1))]
[MemoryPackUnion(2, typeof(V2))]
[MemoryPackUnion(3, typeof(Person))]
#pragma warning disable IDE1006 // Naming Styles
public abstract partial record PersonDto : IMigratable<PersonDto, IPersonValueProvider>
{
    [MemoryPackable]
    public sealed partial record V1(string Name) : PersonDto;

    [MemoryPackable]
    public sealed partial record V2(string Name, string LastName, Address.V1? Address) : PersonDto;

    public static async ValueTask<MigrationResult<PersonDto>> Migrate(PersonDto personDto, IPersonValueProvider valueProvider, CancellationToken cancellationToken)
    {
        return personDto switch
        {
            V1 v1 => new MigrationResult<PersonDto>(MigrationOutcome.Migrated, new V2(v1.Name, valueProvider.GetDefaultLastName(), await valueProvider.GetAddressV1(v1.Name, valueProvider.GetDefaultLastName()))),
            V2 v2 => new MigrationResult<PersonDto>(MigrationOutcome.Migrated, new Person(v2.Name, v2.LastName, valueProvider.GetDefaultAge(), await valueProvider.GetAddressV2(v2.Address, v2.Name, v2.LastName))),
            Person v3 => new MigrationResult<PersonDto>(MigrationOutcome.Migrated, v3),
        };
    }

    public static IReadOnlyCollection<MigrationInfo> GetMigrationInfo() => DiscriminatedUnionMigrations.FromVersionNamedUnion<PersonDto>();
}

[MemoryPackable]
public sealed partial record Person(string Name, string LastName, int Age, Address.V2? Address) : PersonDto;

[DiscriminatedUnion]
[MemoryPackable]
[MemoryPackUnion(1, typeof(V1))]
[MemoryPackUnion(2, typeof(V2))]
public abstract partial record Address
{
    [MemoryPackable]
    public sealed partial record V1(string Street, string Number, string PostalCode, string City) : Address;

    [MemoryPackable]
    public sealed partial record V2(string Street, string Number, string PostalCode, string City, string? Apartment, string? SecondLine) : Address;
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