// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Source.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

/// <summary>
/// Represents a source for an <see cref="AId"/>.
/// </summary>
/// <param name="Origin">The Origin.</param>
/// <param name="Path">The Path.</param>
/// <param name="Name">The Name.</param>
public sealed record Source(string Origin, string Path, string Name) : IParsable<Source>
{
    /// <summary>The origin separator.</summary>
    public const char OriginSeparator = '$';

    /// <summary>The name separator.</summary>
    public const char NameSeparator = '.';

    /// <summary>
    /// Parses the specified input string into an instance of the <see cref="Source"/> type.
    /// </summary>
    /// <param name="inputSource">The string representation of the argument to be parsed. This value must be a valid format for the <see cref="Source"/>> type.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns>An instance of Argument that represents the parsed value from the input string.</returns>
    /// <exception cref="FormatException">Thrown if the input string is not in a valid format for the <see cref="Source"/>> type.</exception>
    public static Source Parse(string inputSource, IFormatProvider? formatProvider)
    {
        if (TryParse(inputSource, formatProvider, out var result))
        {
            return result;
        }

        throw new FormatException($"The string: {inputSource} is not a valid {nameof(Source)}");
    }

    /// <summary>
    /// Tries to parse the specified input string into an instance of the <see cref="Source"/> type.
    /// </summary>
    /// <param name="inputSource">The string representation of the argument to be parsed. This value must be a valid format for the <see cref="Source"/>> type.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if parsing was successful, otherwise <c>false</c>.</returns>
    public static bool TryParse([NotNullWhen(true)] string? inputSource, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out Source result)
    {
        if (string.IsNullOrEmpty(inputSource))
        {
            result = null;
            return false;
        }

        var originStartIndex = inputSource.IndexOf(OriginSeparator);
        var nameEndIndex = inputSource.IndexOf(NameSeparator);
        if (originStartIndex > -1 && nameEndIndex > -1)
        {
            var origin = inputSource.Substring(originStartIndex + 1);
            var name = inputSource.Substring(0, nameEndIndex);
            result = new Source(origin, inputSource.Substring(nameEndIndex + 1, originStartIndex - nameEndIndex - 1), name);
            return true;
        }

        result = null;
        return false;
    }

    /// <summary>
    /// Appends this <see cref="Source"/> to the specified <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="formatProvider">The format provider.</param>
    public void AppendInto(StringBuilder stringBuilder, IFormatProvider formatProvider)
    {
        stringBuilder.Append($"{this.Name}{NameSeparator}{this.Path}{OriginSeparator}{this.Origin}");
    }

    /// <summary>
    /// Creates a string representation of the <see cref="Source"/>.
    /// </summary>
    /// <returns>A string.</returns>
    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        this.AppendInto(stringBuilder, CultureInfo.CurrentCulture);
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Creates an <see cref="Source"/> for the specified type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>A new <see cref="Source"/>>.</returns>
    public static Source FromType(Type type)
    {
        var stringBuilder = new StringBuilder();
        GetName(type, false);

        void GetName(Type type, bool isParent)
        {
            if (type.DeclaringType.HasValue)
            {
                GetName(type.DeclaringType, true);
            }

            stringBuilder.Append(type.Name);
            if (isParent)
            {
                stringBuilder.Append('+');
            }
        }

        return new Source(type.Assembly.GetName().Name ?? string.Empty, type.Namespace ?? string.Empty, stringBuilder.ToString());
    }

    /// <summary>
    /// Tries to get the type for the <see cref="Source"/>.
    /// </summary>
    /// <returns>A result containing the type if successful.</returns>
    public R<Type> TryGetType()
    {
        var type = Type.GetType($"{this.Path}.{this.Name}, {this.Origin}");
        return R.From(type);
    }
}