// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationInfo.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Migrations;

using global::System;

/// <summary>
/// Represents version information associated with a specific type.
/// </summary>
/// <param name="Type">The type for which the version information applies. Cannot be null.</param>
/// <param name="Version">The version number associated with the specified type.</param>
public readonly record struct MigrationInfo(Type Type, int Version);