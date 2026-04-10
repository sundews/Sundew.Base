// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Target.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

/// <summary>
/// Represents a <see cref="Target"/> composed of a source and an optional path.
/// </summary>
/// <param name="Source">The source component of the target.</param>
/// <param name="Path">The path associated with the target.</param>
public sealed record Target(Source Source, Path? Path) : IParsable<Target>
{
    /// <summary>
    /// Parses the specified input string into an instance of the <see cref="ComplexValue"/> type.
    /// </summary>
    /// <param name="inputTarget">The string representation of the argument to be parsed. This value must be a valid format for the <see cref="Target"/>> type.</param>
    /// <param name="formatProvider">The format formatProvider.</param>
    /// <returns>An instance of ValueId that represents the parsed value from the input string.</returns>
    /// <exception cref="FormatException">Thrown if the input string is not in a valid format for the <see cref="Target"/>> type.</exception>
    public static Target Parse(string inputTarget, IFormatProvider? formatProvider)
    {
        if (TryParse(inputTarget, formatProvider, out var result))
        {
            return result;
        }

        throw new FormatException($"The string: {inputTarget} is not a valid {nameof(Target)}");
    }

    /// <summary>
    /// Tries to parse the specified input string into an instance of the <see cref="ComplexValue"/> type.
    /// </summary>
    /// <param name="inputTarget">The string representation of the argument to be parsed. This value must be a valid format for the <see cref="Target"/>> type.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if parsing was successful, otherwise <c>false</c>.</returns>
    public static bool TryParse([NotNullWhen(true)] string? inputTarget, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out Target result)
    {
        if (inputTarget.HasValue)
        {
            var argumentsSeparatorIndex = inputTarget.IndexOf(Path.Separator);
            if (argumentsSeparatorIndex > -1)
            {
                var targetString = inputTarget.Substring(0, argumentsSeparatorIndex);
                var argumentsString = inputTarget.Substring(argumentsSeparatorIndex + 1);
                if (Source.TryParse(targetString, formatProvider, out var entry) /* && Path.TryParse(argumentsString, formatProvider, out var path)*/)
                {
                    result = new Target(entry, null);
                    return true;
                }
            }
            else if (Source.TryParse(inputTarget, formatProvider, out var entry))
            {
                result = new Target(entry, null);
                return true;
            }
        }

        result = null;
        return false;
    }

    /// <summary>
    /// Appends this <see cref="Target"/> to the specified <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="formatProvider">The format provider.</param>
    public void AppendInto(StringBuilder stringBuilder, IFormatProvider formatProvider)
    {
        this.Source.AppendInto(stringBuilder, formatProvider);
        if (this.Path.HasValue)
        {
            stringBuilder.Append(Path.Separator);
            this.Path.AppendInto(stringBuilder, formatProvider);
        }
    }

    /// <summary>
    /// Creates a string representation of the <see cref="Target"/>.
    /// </summary>
    /// <returns>A string.</returns>
    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        this.AppendInto(stringBuilder, CultureInfo.CurrentCulture);
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Tries to get the source type.
    /// </summary>
    /// <returns>A result containing the source type if successful.</returns>
    public R<Type> TryGetSourceType()
    {
        return this.Source.TryGetType();
    }

    /// <summary>
    /// Tries to get the result type.
    /// </summary>
    /// <returns>A result containing the source type if successful.</returns>
    public R<Type> TryGetResultType()
    {
        return TargetEvaluator.GetResultType(this.Source, this.Path);
    }

    /// <summary>
    /// Tries to get the input types.
    /// </summary>
    /// <returns>A result containing the input types if successful.</returns>
    public R<IReadOnlyList<Type>> TryGetInputTypes()
    {
        return TargetEvaluator.GetInputTypes(this.Source, this.Path, null);
    }

    /// <summary>
    /// Tries to get the target type.
    /// </summary>
    /// <returns>A result containing the source type if successful.</returns>
    public R<Type> TryGetContainingType()
    {
        return TargetEvaluator.GetDeclaringType(this.Source, this.Path);
    }
}