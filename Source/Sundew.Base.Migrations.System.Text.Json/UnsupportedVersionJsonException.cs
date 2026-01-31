// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnsupportedVersionJsonException.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Migrations.System.Text.Json;

using global::System.Text.Json;

/// <summary>
/// Represents an exception that is thrown when a JSON payload contains a version that is not supported by the current
/// processing logic.
/// </summary>
/// <remarks>This exception is typically used to indicate a version mismatch during JSON deserialization or
/// processing. It helps callers identify and handle cases where the data format version is incompatible with the
/// expected or supported versions. Consider catching this exception to provide a user-friendly error message or to
/// trigger a fallback mechanism when encountering unsupported JSON versions.</remarks>
public class UnsupportedVersionJsonException : JsonException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedVersionJsonException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public UnsupportedVersionJsonException(string message)
      : base(message, null)
    {
    }
}