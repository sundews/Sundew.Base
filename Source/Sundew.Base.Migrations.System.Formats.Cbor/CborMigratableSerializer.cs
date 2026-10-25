// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CborMigratableSerializer.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Migrations.System.Formats.Cbor;

using global::System;
using global::System.Collections.Generic;
using global::System.Formats.Cbor;
using global::System.Linq;
using global::System.Text.Json;

/// <summary>
/// Serializes and deserializes migratable values using CBOR with versioned payloads.
/// </summary>
public static class CborMigratableSerializer
{
    /// <summary>
    /// Serializes the specified value to a CBOR payload.
    /// </summary>
    /// <typeparam name="TMigratable">The migratable type.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>The serialized payload.</returns>
    public static byte[] Serialize<TMigratable>(TMigratable value)
        where TMigratable : IMigratable
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var writer = new CborWriter(CborConformanceMode.Ctap2Canonical, false, false);
        WriteVersionedValue(writer, value, value.GetType());
        return writer.Encode();
    }

    /// <summary>
    /// Serializes the specified values to a CBOR array payload.
    /// </summary>
    /// <typeparam name="TMigratable">The migratable type.</typeparam>
    /// <param name="values">The values.</param>
    /// <returns>The serialized payload.</returns>
    public static byte[] Serialize<TMigratable>(IEnumerable<TMigratable> values)
        where TMigratable : IMigratable
    {
        if (values is null)
        {
            throw new ArgumentNullException(nameof(values));
        }

        var writer = new CborWriter(CborConformanceMode.Ctap2Canonical, false, false);
        var list = values.ToArray();
        writer.WriteStartArray(list.Length);
        foreach (var value in list)
        {
            WriteVersionedValue(writer, value, value.GetType());
        }

        writer.WriteEndArray();
        return writer.Encode();
    }

    /// <summary>
    /// Deserializes the specified payload into a migratable value.
    /// </summary>
    /// <typeparam name="TMigratable">The migratable type.</typeparam>
    /// <param name="payload">The payload.</param>
    /// <returns>The deserialized value.</returns>
    public static TMigratable Deserialize<TMigratable>(byte[] payload)
        where TMigratable : IMigratable
    {
        if (payload is null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        var reader = new CborReader(payload, CborConformanceMode.Ctap2Canonical, false);
        return ReadVersionedValue<TMigratable>(reader);
    }

    /// <summary>
    /// Deserializes the specified payload into a list of migratable values.
    /// </summary>
    /// <typeparam name="TMigratable">The migratable type.</typeparam>
    /// <param name="payload">The payload.</param>
    /// <returns>The deserialized values.</returns>
    public static IReadOnlyList<TMigratable> DeserializeMany<TMigratable>(byte[] payload)
        where TMigratable : IMigratable
    {
        if (payload is null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        var reader = new CborReader(payload, CborConformanceMode.Ctap2Canonical, false);
        reader.ReadStartArray();
        var result = new List<TMigratable>();
        while (reader.PeekState() != CborReaderState.EndArray)
        {
            result.Add(ReadVersionedValue<TMigratable>(reader));
        }

        reader.ReadEndArray();
        return result;
    }

    private static void WriteVersionedValue<TMigratable>(CborWriter writer, TMigratable value, Type valueType)
        where TMigratable : IMigratable
    {
        var migrationInfos = TMigratable.GetMigrationInfo();
        var migrationInfo = migrationInfos.SingleOrDefault(x => x.Type == valueType);
        if (migrationInfo.Equals(default))
        {
            throw new InvalidOperationException($"Type '{valueType.FullName}' is not part of the migratable hierarchy.");
        }

        writer.WriteStartMap(2);
        writer.WriteTextString("version");
        writer.WriteInt32(migrationInfo.Version);
        writer.WriteTextString("data");
        writer.WriteTextString(JsonSerializer.Serialize(value, valueType));
        writer.WriteEndMap();
    }

    private static TMigratable ReadVersionedValue<TMigratable>(CborReader reader)
        where TMigratable : IMigratable
    {
        reader.ReadStartMap();
        int version = 0;
        string? json = null;

        while (reader.PeekState() != CborReaderState.EndMap)
        {
            var key = reader.ReadTextString();
            switch (key)
            {
                case "version":
                    version = reader.ReadInt32();
                    break;
                case "data":
                    json = reader.ReadTextString();
                    break;
                default:
                    reader.SkipValue();
                    break;
            }
        }

        reader.ReadEndMap();

        var migrationInfos = TMigratable.GetMigrationInfo();
        var migrationInfo = migrationInfos.FirstOrDefault(x => x.Version == version);
        if (migrationInfo.Equals(default))
        {
            if (version > migrationInfos.Max(x => x.Version))
            {
                migrationInfo = migrationInfos.OrderByDescending(x => x.Version).First();
            }
            else
            {
                throw new InvalidOperationException($"Unsupported version {version} for {typeof(TMigratable).FullName}.");
            }
        }

        if (json is null)
        {
            throw new InvalidOperationException($"Failed to deserialize CBOR payload for {typeof(TMigratable).FullName}.");
        }

        var data = JsonSerializer.Deserialize(json, migrationInfo.Type);
        if (data is TMigratable migratable)
        {
            return migratable;
        }

        throw new InvalidOperationException($"Failed cast deserialized data to {typeof(TMigratable).FullName}.");
    }
}
