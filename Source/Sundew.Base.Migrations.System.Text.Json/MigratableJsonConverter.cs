// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigratableJsonConverter.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Migrations.System.Text.Json;

using global::System;
using global::System.Linq;
using global::System.Text.Json;
using global::System.Text.Json.Serialization;

/// <summary>
/// Converts migratable objects to and from JSON, supporting versioned deserialization and migration scenarios.
/// </summary>
/// <typeparam name="TMigratable">The type of object to convert. Must implement <see cref="IMigratable"/> to provide versioning and migration
/// information.</typeparam>
public class MigratableJsonConverter<TMigratable> : JsonConverter<TMigratable>
    where TMigratable : IMigratable
{
    private const string VersionName = "version";

    /// <summary>
    /// Reads a migratable object from the provided JSON reader, deserializing it according to its version information.
    /// </summary>
    /// <param name="reader">A reference to the <see cref="Utf8JsonReader"/> positioned at the start of the migratable object to read. The
    /// reader will be advanced past the consumed JSON value.</param>
    /// <param name="typeToConvert">The type of object to convert. This parameter is required by the base class but is not used in this
    /// implementation.</param>
    /// <param name="options">The serializer options to use when deserializing the migratable object. These options may be modified internally
    /// to avoid recursion.</param>
    /// <returns>An instance of <typeparamref name="TMigratable"/> deserialized from the JSON input, corresponding to the
    /// detected version.</returns>
    public override TMigratable Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        reader.Read();
        reader.Read();
        var version = reader.GetInt32();
        reader.Read();
        reader.Read();
        var newOptions = new JsonSerializerOptions(options);
        newOptions.Converters.Remove(this);
        var migratableData = JsonSerializer.Deserialize(ref reader, TMigratable.GetMigrationInfo().FirstOrDefault(x => x.Version == version).Type, newOptions);
        reader.Read();
        if (migratableData is TMigratable migratable)
        {
            return migratable;
        }

        throw new JsonException();
    }

    /// <summary>
    /// Writes the JSON representation of a TMigratable object, including version information and serialized data, to
    /// the specified Utf8JsonWriter.
    /// </summary>
    /// <param name="writer">The Utf8JsonWriter to which the JSON output is written. Must not be null.</param>
    /// <param name="value">The TMigratable object to serialize. Must not be null.</param>
    /// <param name="options">The options to use when serializing the object, such as custom converters or formatting settings.</param>
    public override void Write(Utf8JsonWriter writer, TMigratable value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        // Manually serialize your special property
        writer.WriteNumber(VersionName, TMigratable.GetMigrationInfo().FirstOrDefault(x => x.Type == value.GetType()).Version);

        const string dataName = "data";
        writer.WritePropertyName(dataName);

        // Serialize everything else using reflection
        var newOptions = new JsonSerializerOptions(options);
        newOptions.Converters.Remove(this);
        JsonSerializer.Serialize(writer, value, value.GetType(), newOptions);

        writer.WriteEndObject();
    }
}