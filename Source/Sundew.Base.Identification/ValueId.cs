// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueId.cs" company="Sundews">
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
/// Represents a value id for an <see cref="AId"/> argument.
/// </summary>
/// <param name="Name">The name.</param>
/// <param name="Metadata">The metadata.</param>
/// <param name="Value">The value.</param>
public sealed partial record ValueId(string? Name, string? Metadata, IValue Value)
{
    /// <summary>Key Value separator.</summary>
    public const char KeyValueSeparator = '=';

    /// <summary>Metadata separator.</summary>
    public const char MetadataSeparator = '!';

    /// <summary>
    /// Gets the type of the source.
    /// </summary>
    /// <returns>A result containing the type is successful.</returns>
    public R<Type> TryGetType()
    {
        if (Source.TryParse(this.Metadata, CultureInfo.InvariantCulture, out var source))
        {
            return source.TryGetType();
        }

        return R.Error();
    }

    /// <summary>
    /// Creates a string representation of the <see cref="ValueIds"/>.
    /// </summary>
    /// <returns>A string.</returns>
    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        this.AppendInto(stringBuilder, CultureInfo.CurrentCulture);
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Appends this <see cref="ValueIds"/> to the specified <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="formatProvider">The format provider.</param>
    public void AppendInto(StringBuilder stringBuilder, IFormatProvider formatProvider)
    {
        bool TryAppendMetadata()
        {
            if (!string.IsNullOrEmpty(this.Metadata))
            {
                stringBuilder.Append(MetadataSeparator);
                stringBuilder.Append(this.Metadata);
                return true;
            }

            return false;
        }

        if (!string.IsNullOrEmpty(this.Name))
        {
            stringBuilder.Append(this.Name);
            TryAppendMetadata();
            stringBuilder.Append(KeyValueSeparator);
        }
        else
        {
            if (TryAppendMetadata())
            {
                stringBuilder.Append(KeyValueSeparator);
            }
        }

        this.Value.AppendInto(stringBuilder, formatProvider);
    }

    /// <summary>
    /// Creates an <see cref="ValueIds"/> from the specified builder func.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="valueIdFunc">The value id func.</param>
    /// <param name="isRoot">Indicated whether this is a root id.</param>
    /// <returns>A new <see cref="ValueIds"/>.</returns>
    public static ValueId From<TValue>(TValue value, Action<TValue, ValueIdBuilder> valueIdFunc, bool isRoot)
    {
        var valueIdBuilder = new ValueIdBuilder(value?.GetType() ?? typeof(TValue), isRoot);
        valueIdFunc(value, valueIdBuilder);
        return valueIdBuilder.Build();
    }

    /// <summary>
    /// Parses the specified input string into an instance of the <see cref="ValueId"/> type.
    /// </summary>
    /// <param name="inputArg">The string representation of the argument to be parsed. This value must be a valid format for the <see cref="ValueId"/>> type.</param>
    /// <param name="provider">The format provider.</param>
    /// <returns>An instance of ValueId that represents the parsed value from the input string.</returns>
    /// <exception cref="FormatException">Thrown if the input string is not in a valid format for the <see cref="ValueId"/>> type.</exception>
    public static ValueId Parse(string inputArg, IFormatProvider? provider)
    {
        if (TryParse(inputArg, provider, out var result))
        {
            return result;
        }

        throw new FormatException($"The string: {inputArg} is not a valid {nameof(ValueId)}.");
    }

    /// <summary>
    /// Tries to parse the specified input string into an instance of the <see cref="ValueId"/>> type.
    /// </summary>
    /// <param name="inputArg">The string representation of the argument to be parsed. This value must be a valid format for the <see cref="ValueId"/>> type.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if parsing was successful, otherwise <c>false</c>.</returns>
    public static bool TryParse([NotNullWhen(true)] string? inputArg, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out ValueId result)
    {
        if (inputArg.HasValue)
        {
            string key = string.Empty;
            var level = 0;
            var index = 0;
            var metadataIndex = -1;
            while (index < inputArg.Length)
            {
                var character = inputArg[index++];
                if (character == ValueIds.GroupStartSeparator)
                {
                    level++;
                }

                if (character == ValueIds.GroupEndSeparator)
                {
                    level--;
                }

                if (character == MetadataSeparator && level == 0)
                {
                    metadataIndex = index;
                }

                if (character == KeyValueSeparator && level == 0)
                {
                    var (nameLength, metadataStart, metadataLength) = metadataIndex > -1 ? (metadataIndex - 1, metadataIndex, index - metadataIndex - 1) : (index - 1, 0, 0);
                    result = new ValueId(inputArg.Substring(0, nameLength), inputArg.Substring(metadataStart, metadataLength), new SingleValue(inputArg.Substring(index)));
                    return true;
                }
            }

            result = new ValueId(null, null, new SingleValue(inputArg));
            return true;
        }

        result = null;
        return false;
    }

    /// <summary>
    /// Converts the specified initial value to a value of the specified type, using the current instance as context.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to convert. Must implement the <see cref="IIdentifiable{TValue}"/> interface.</typeparam>
    /// <param name="defaultValue">The default value to be converted. Must be of type TValue.</param>
    /// <returns>A value of type TValue that is derived from the initial value and the current instance.</returns>
    public TValue ToValue<TValue>(TValue defaultValue)
        where TValue : IValueIdentifiable<TValue>
    {
        return TValue.From(defaultValue, this);
    }
}