// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Argument.cs" company="Sundews">
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
/// Represents an argument in a <see cref="AId"/>.
/// </summary>
/// <param name="Name">The name.</param>
/// <param name="Value">The value.</param>
public record Argument(string? Name, string Value) : IParsable<Argument>
{
    /// <summary>Key Value separator.</summary>
    public const char KeyValueSeparator = '=';

    /// <summary>
    /// Parses the specified input string into an instance of the <see cref="Argument"/> type.
    /// </summary>
    /// <param name="inputArg">The string representation of the argument to be parsed. This value must be a valid format for the <see cref="Argument"/>> type.</param>
    /// <param name="provider">The format provider.</param>
    /// <returns>An instance of Argument that represents the parsed value from the input string.</returns>
    /// <exception cref="FormatException">Thrown if the input string is not in a valid format for the <see cref="Argument"/>> type.</exception>
    public static Argument Parse(string inputArg, IFormatProvider? provider)
    {
        if (TryParse(inputArg, provider, out var result))
        {
            return result;
        }

        throw new FormatException($"The string: {inputArg} is not a valid {nameof(Argument)}.");
    }

    /// <summary>
    /// Tries to parse the specified input string into an instance of the <see cref="Argument"/>> type.
    /// </summary>
    /// <param name="inputArg">The string representation of the argument to be parsed. This value must be a valid format for the <see cref="Argument"/>> type.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if parsing was successful, otherwise <c>false</c>.</returns>
    public static bool TryParse([NotNullWhen(true)] string? inputArg, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out Argument result)
    {
        if (inputArg.HasValue)
        {
            string key = string.Empty;
            var level = 0;
            var index = 0;
            while (index < inputArg.Length)
            {
                var character = inputArg[index++];
                if (character == Arguments.GroupStartSeparator)
                {
                    level++;
                }

                if (character == Arguments.GroupEndSeparator)
                {
                    level--;
                }

                if (character == KeyValueSeparator && level == 0)
                {
                    result = new Argument(inputArg.Substring(0, index - 1), inputArg.Substring(index));
                    return true;
                }
            }

            result = new Argument(null, inputArg);
            return true;
        }

        result = null;
        return false;
    }

    /// <summary>
    /// Appends this <see cref="Argument"/> to the specified <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="formatProvider">The format provider.</param>
    public void AppendInto(StringBuilder stringBuilder, IFormatProvider formatProvider)
    {
        if (!string.IsNullOrEmpty(this.Name))
        {
            stringBuilder.Append(this.Name).Append(KeyValueSeparator);
        }

        stringBuilder.Append(this.Value);
    }

    /// <summary>
    /// Creates a string representation of the <see cref="Argument"/>.
    /// </summary>
    /// <returns>A string.</returns>
    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        this.AppendInto(stringBuilder, CultureInfo.CurrentCulture);
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Attempts to parse the current value into an instance of the Arguments class.
    /// </summary>
    /// <returns>A result containing the parsed Arguments if successful.</returns>
    public R<Arguments> TryGetValueArguments()
    {
        return this.TryGetValueArguments(CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Attempts to parse the current value into an instance of the Arguments class.
    /// </summary>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns>A result containing the parsed Arguments if successful.</returns>
    public R<Arguments> TryGetValueArguments(IFormatProvider formatProvider)
    {
        return R.From(Arguments.TryParse(this.Value, formatProvider, out var args), args);
    }
}