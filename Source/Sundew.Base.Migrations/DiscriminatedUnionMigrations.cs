// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionMigrations.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Migrations;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Sundew.Base.Collections;
using Sundew.DiscriminatedUnions;

/// <summary>
/// Provides helper methods for generating migration information for discriminated union types.
/// </summary>
/// <remarks>This class is intended for use with discriminated union types that implement the IDiscriminatedUnion
/// interface. It offers utilities to assist with versioned migrations, such as extracting version information from
/// union case names. All members are static and thread safe.</remarks>
public static partial class DiscriminatedUnionMigrations
{
    /// <summary>
    /// Creates a collection of migration information for each case in the specified discriminated union type, ordered
    /// by case name.
    /// </summary>
    /// <typeparam name="TDiscriminatedUnion">The discriminated union type whose cases are used to generate migration information. Must implement
    /// IDiscriminatedUnion.</typeparam>
    /// <returns>A read-only collection of MigrationInfo objects, each representing a case in the discriminated union type. The
    /// collection is ordered by case name.</returns>
    public static IReadOnlyCollection<MigrationInfo> FromVersionNamedUnion<TDiscriminatedUnion>()
        where TDiscriminatedUnion : IDiscriminatedUnion
    {
        var orderedCases = TDiscriminatedUnion.Cases
            .Select((caseType, index) =>
                {
                    const string version = "Version";
                    var match = GetVersionNameRegex().Match(caseType.Name);
                    if (match.Success)
                    {
                        var versionNumberGroup = match.Groups[version];
                        var versionNumber = versionNumberGroup.Success ? int.Parse(versionNumberGroup.Value, CultureInfo.InvariantCulture) : typeof(TDiscriminatedUnion).Name.Contains(caseType.Name) ? int.MaxValue : throw InvalidUnion(caseType);
                        return new MigrationInfo(caseType, versionNumber);
                    }

                    throw InvalidUnion(caseType);
                })
          .OrderBy(x => x.Version)
          .ToArray();
        return orderedCases
          .Select(migrationInfo => migrationInfo.Version == int.MaxValue ? new MigrationInfo(migrationInfo.Type, GetLatestVersion(migrationInfo, orderedCases)) : migrationInfo).ToArray();

        InvalidOperationException InvalidUnion(Type unionType)
        {
            return new InvalidOperationException($"The union case '{unionType.Name}' in discriminated union type '{typeof(TDiscriminatedUnion).FullName}' does not follow the expected version naming convention.");
        }
    }

    private static int GetLatestVersion(MigrationInfo migrationInfo, MigrationInfo[] migrationInfos)
    {
        var index = migrationInfos.IndexOf(migrationInfo);
        if (index >= 1)
        {
            return migrationInfos[index - 1].Version + 1;
        }

        return 1;
    }

    [GeneratedRegex(@"\D+(?<Version>\d+)?$")]
    private static partial Regex GetVersionNameRegex();
}