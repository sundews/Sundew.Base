// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AIdRoute.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

using System;
using System.Diagnostics.CodeAnalysis;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Identification.Parsing;

/// <summary>
/// Represents a route consisting of a path of <see cref="AId"/>.
/// </summary>
public sealed record AIdRoute(ValueList<AId> Path) : IParsable<AIdRoute>
{
    /// <summary>
    /// Parses the specified input string into an instance of the <see cref="AIdRoute"/> type.
    /// </summary>
    /// <param name="inputAIdRoute">The string representation of the argument to be parsed. This value must be a valid format for the <see cref="AIdRoute"/>> type.</param>
    /// <param name="provider">The format provider.</param>
    /// <returns>An instance of ValueId that represents the parsed value from the input string.</returns>
    /// <exception cref="FormatException">Thrown if the input string is not in a valid format for the <see cref="AId"/>> type.</exception>
    public static AIdRoute Parse(string inputAIdRoute, IFormatProvider? provider)
    {
        if (TryParse(inputAIdRoute, provider, out var result))
        {
            return result;
        }

        throw new FormatException($"The string: {inputAIdRoute} is not a valid {nameof(AId)}");
    }

    /// <summary>
    /// Tries to parse the specified input string into an instance of the <see cref="AId"/> type.
    /// </summary>
    /// <param name="inputAidRoute">The string representation of the argument to be parsed. This value must be a valid format for the <see cref="AId"/>> type.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if parsing was successful, otherwise <c>false</c>.</returns>
    public static bool TryParse([NotNullWhen(true)] string? inputAidRoute, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out AIdRoute result)
    {
        return AIdRouteParser.TryGetAIdRoute(inputAidRoute, formatProvider, out result);
    }
}