// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives;

using System;

/// <summary>
/// Extension methods for <see cref="object"/>.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Tries to cast the value to a type and otherwise calls GetType.
    /// </summary>
    /// <param name="value">The @value.</param>
    /// <returns>The type.</returns>
    public static Type AsType(this object value)
    {
        if (value is Type type)
        {
            return type;
        }

        return value.GetType();
    }

    /// <summary>
    /// Converts the value to string or an empty string.
    /// </summary>
    /// <typeparam name="TObject">The type of the object.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>A string.</returns>
    public static string ToStringOrEmpty<TObject>(this TObject? value)
    {
        if (value == null)
        {
            return string.Empty;
        }

        return value.ToString() ?? string.Empty;
    }
}