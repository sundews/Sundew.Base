// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdRoute.cs" company="Sundews">
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
/// Represents a route consisting of a path of <see cref="Id"/>.
/// </summary>
public sealed record IdRoute(ValueList<Id> Path) : IParsable<IdRoute>
{
    /// <summary>
    /// Parses the specified input string into an instance of the <see cref="IdRoute"/> type.
    /// </summary>
    /// <param name="inputIdRoute">The string representation of the argument to be parsed. This value must be a valid format for the <see cref="IdRoute"/>> type.</param>
    /// <param name="provider">The format provider.</param>
    /// <returns>An instance of ValueId that represents the parsed value from the input string.</returns>
    /// <exception cref="FormatException">Thrown if the input string is not in a valid format for the <see cref="Id"/>> type.</exception>
    public static IdRoute Parse(string inputIdRoute, IFormatProvider? provider)
    {
        if (TryParse(inputIdRoute, provider, out var result))
        {
            return result;
        }

        throw new FormatException($"The string: {inputIdRoute} is not a valid {nameof(Id)}");
    }

    /// <summary>
    /// Tries to parse the specified input string into an instance of the <see cref="Id"/> type.
    /// </summary>
    /// <param name="inputIdRoute">The string representation of the argument to be parsed. This value must be a valid format for the <see cref="Id"/>> type.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if parsing was successful, otherwise <c>false</c>.</returns>
    public static bool TryParse([NotNullWhen(true)] string? inputIdRoute, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out IdRoute result)
    {
        return IdRouteParser.ParseIdRoute(inputIdRoute, formatProvider).TryGet(out result, out _);
    }
}