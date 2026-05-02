// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionMigrationsTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable SA1402 // File may only contain a single type
namespace Sundew.Base.Development.Tests.Migrations;

using AwesomeAssertions;
using Sundew.Base.Migrations;
using Sundew.DiscriminatedUnions;

public class DiscriminatedUnionMigrationsTests
{
    [Test]
    public void FromVersionNamedUnion_When_UnionOnlyHasOneCase_Then_OneMigrationInfoIsReturned()
    {
        var expectedMigrationInfos = new[]
        {
            new MigrationInfo(typeof(SingleVersion), 1),
        };

        var migrationInfos = DiscriminatedUnionMigrations.FromVersionNamedUnion<SingleVersionDto>();

        migrationInfos.Should().Equal(expectedMigrationInfos);
    }

    [Test]
    public void FromVersionNamedUnion_When_UnionOnlyHasTwoCases_Then_ExpectedMigrationInfosAreReturned()
    {
        var expectedMigrationInfos = new[]
        {
            new MigrationInfo(typeof(TwoVersionDto.V1), 1),
            new MigrationInfo(typeof(TwoVersion), 2),
        };

        var migrationInfos = DiscriminatedUnionMigrations.FromVersionNamedUnion<TwoVersionDto>();

        migrationInfos.Should().Equal(expectedMigrationInfos);
    }
}

[DiscriminatedUnion]
public abstract partial record SingleVersionDto
{
}

public sealed partial record SingleVersion(string Name) : SingleVersionDto;

[DiscriminatedUnion]
public abstract partial record TwoVersionDto
{
    public sealed partial record V1(string Name) : TwoVersionDto;
}

public sealed partial record TwoVersion(string Name, int Number) : TwoVersionDto;
