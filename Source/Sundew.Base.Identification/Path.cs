// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Path.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Text;

/// <summary>
/// Represents a path composed of multiple segments separated by a forward slash ('/').
/// </summary>
/// <param name="Segments">The collection of segments that make up the path, in order from root to leaf.</param>
public sealed record Path(ValueArray<Segment> Segments)
{
    /// <summary>The path separator.</summary>
    public const char Separator = '/';
 /*
    /// <summary>
    /// GetScalar the <see cref="Path"/> from input path.
    /// </summary>
    /// <param name="inputPath">The input path.</param>
    /// <returns>The path.</returns>
    public static Path From(string inputPath)
    {
        return new Path(inputPath.Split(Separator));
    }

    /// <summary>
    /// Parses the specified input string into an instance of the <see cref="Path"/> type.
    /// </summary>
    /// <param name="inputPath">The string representation of the argument to be parsed. This value must be a valid format for the <see cref="Path"/>> type.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns>An instance of ValueId that represents the parsed value from the input string.</returns>
    /// <exception cref="FormatException">Thrown if the input string is not in a valid format for the <see cref="Path"/>> type.</exception>
    public static Path Parse(string inputPath, IFormatProvider? formatProvider)
    {
        if (TryParse(inputPath, formatProvider, out var result))
        {
            return result;
        }

        throw new FormatException($"The string: {inputPath} is not a valid {nameof(Path)}.");
    }

    /// <summary>
    /// Tries to parse the specified input string into an instance of the <see cref="Path"/> type.
    /// </summary>
    /// <param name="inputPath">The string representation of the argument to be parsed. This value must be a valid format for the <see cref="Path"/>> type.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if parsing was successful, otherwise <c>false</c>.</returns>
    public static bool TryParse([NotNullWhen(true)] string? inputPath, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out Path result)
    {
        if (inputPath.HasValue)
        {
            var parts = inputPath.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
            {
                result = new Path(parts);
                return true;
            }
        }

        result = null;
        return false;
    }*/

    /// <summary>
    /// Appends this <see cref="Path"/> to the specified <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="formatProvider">The format provider.</param>
    public void AppendInto(StringBuilder stringBuilder, IFormatProvider formatProvider)
    {
        stringBuilder.AppendItems(this.Segments, (builder, segment) => segment.AppendInto(builder, formatProvider), Separator);
    }

    /// <summary>
    /// Creates a string representation of the <see cref="Path"/>.
    /// </summary>
    /// <returns>A string.</returns>
    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        this.AppendInto(stringBuilder, CultureInfo.CurrentCulture);
        return stringBuilder.ToString();
    }
}