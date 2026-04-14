// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueId.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Sundew.Base.Identification.Parsing;

/// <summary>
/// Represents a value id for an <see cref="Id"/> argument.
/// </summary>
/// <param name="Metadata">The metadata.</param>
/// <param name="Value">The value.</param>
public sealed partial record ValueId(string? Metadata, IValue Value)
{
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
    /// Creates a string representation of the <see cref="ComplexValue"/>.
    /// </summary>
    /// <returns>A string.</returns>
    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        this.AppendInto(stringBuilder, CultureInfo.CurrentCulture, new AppendOptions(true), false);
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Appends this <see cref="ComplexValue"/> to the specified <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="appendOptions">The append options.</param>
    /// <param name="requiresKeySeparation">Indicates whether the value id has a name.</param>
    public void AppendInto(StringBuilder stringBuilder, IFormatProvider formatProvider, AppendOptions appendOptions, bool requiresKeySeparation)
    {
        bool TryAppendMetadata()
        {
            if (!string.IsNullOrEmpty(this.Metadata))
            {
                stringBuilder.Append(Grammar.NameMetadataSeparator);
                stringBuilder.Append(this.Metadata);
                return true;
            }

            return false;
        }

        if (TryAppendMetadata() || requiresKeySeparation)
        {
            stringBuilder.Append(Grammar.KeyValueSeparator);
        }

        this.Value.AppendInto(stringBuilder, formatProvider, appendOptions with { IsRoot = false });
    }

    /// <summary>
    /// Creates an <see cref="ComplexValue"/> from the specified builder func.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="valueIdFunc">The value id func.</param>
    /// <returns>A new <see cref="ComplexValue"/>.</returns>
    public static ValueId From<TValue>(TValue value, Action<TValue, ValueIdBuilder> valueIdFunc)
    {
        var valueIdBuilder = new ValueIdBuilder(value?.GetType() ?? typeof(TValue));
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
        var valueIdResult = IdRouteParser.ParseValueId(inputArg, formatProvider);
        result = valueIdResult.Value;
        return valueIdResult.IsSuccess;
    }

    /// <summary>
    /// Converts the specified initial value to a value of the specified type, using the current instance as context.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to convert. Must implement the <see cref="IIdentifiable{TValue}"/> interface.</typeparam>
    /// <param name="defaultValue">The default value to be converted. Must be of type TValue.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns>A value of type TValue that is derived from the initial value and the current instance.</returns>
    public TValue ToValue<TValue>(TValue defaultValue, IFormatProvider formatProvider)
        where TValue : IValueIdentifiable<TValue>
    {
        return TValue.From(defaultValue, this, formatProvider);
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
        return TValue.From(defaultValue, this, CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Gets the value from the arguments.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="referenceName">The argument name.</param>
    /// <returns>The retrieved value or the default value.</returns>
    public TValue GetScalar<TValue>(TValue defaultValue, IFormatProvider? formatProvider, [CallerArgumentExpression(nameof(defaultValue))] string? referenceName = null)
        where TValue : IParsable<TValue>
    {
        if (!referenceName.HasValue)
        {
            throw new NotSupportedException("ReferenceName should be filled by compiler.");
        }

        if (this.Value is not ComplexValue complexValue)
        {
            return defaultValue;
        }

        var argument = complexValue.Items.FirstOrDefault(x => x.Name == referenceName);
        if (argument.HasValue)
        {
            return TValue.Parse(argument.ValueId.Value.ToString() ?? string.Empty, formatProvider);
        }

        var firstDotIndex = referenceName.IndexOf('.');
        var fallback = firstDotIndex > -1
            ? referenceName.Substring(firstDotIndex + 1, referenceName.Length - firstDotIndex - 1)
            : null;
        argument = complexValue.Items.FirstOrDefault(x => x.Name == fallback);
        if (argument.HasValue)
        {
            return TValue.Parse(argument.ValueId.Value.ToString() ?? string.Empty, formatProvider);
        }

        return defaultValue;
    }

    /// <summary>
    /// Gets the value from the arguments.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="referenceName">The argument name.</param>
    /// <returns>The retrieved value or the default value.</returns>
    public TValue GetValue<TValue>(TValue defaultValue, IFormatProvider? formatProvider, [CallerArgumentExpression(nameof(defaultValue))] string? referenceName = null)
        where TValue : IValueIdentifiable<TValue>
    {
        if (!referenceName.HasValue)
        {
            throw new NotSupportedException("ReferenceName should be filled by compiler.");
        }

        if (this.Value is not ComplexValue complexValue)
        {
            return defaultValue;
        }

        var argument = complexValue.Items.FirstOrDefault(x => x.Name == referenceName);
        if (argument.HasValue)
        {
            return TValue.From(defaultValue, argument.ValueId, formatProvider);
        }

        var firstDotIndex = referenceName.IndexOf('.');
        var fallback = firstDotIndex > -1
            ? referenceName.Substring(firstDotIndex + 1, referenceName.Length - firstDotIndex - 1)
            : null;
        argument = complexValue.Items.FirstOrDefault(x => x.Name == fallback);
        if (argument.HasValue)
        {
            return TValue.From(defaultValue, argument.ValueId, formatProvider);
        }

        return defaultValue;
    }
}