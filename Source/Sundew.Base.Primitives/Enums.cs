// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Enums.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives;

using System;
using System.Collections.Generic;

/// <summary>
/// Enum methods.
/// </summary>
public static class Enums
{
    /// <summary>
    /// Gets the values.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <returns>The enum values.</returns>
    public static IReadOnlyList<TEnum> GetValues<TEnum>()
        where TEnum : Enum
    {
        return (TEnum[])Enum.GetValues(typeof(TEnum));
    }

    /// <summary>
    /// Gets the names.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <returns>The enum names.</returns>
    public static IReadOnlyList<string> GetNames<TEnum>()
        where TEnum : Enum
    {
        return Enum.GetNames(typeof(TEnum));
    }
}