// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Arguments.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Text;

#pragma warning disable CS8907, CS1591
/// <summary>
/// Represents arguments for an <see cref="AId"/>.
/// </summary>
/// <param name="Items">The arguments.</param>
public readonly record struct Arguments(ValueArray<Argument> Items) : IParsable<Arguments>
{
    /// <summary>The argument separator.</summary>
    public const char ArgumentsSeparator = '&';

    /// <summary>The group start separator.</summary>
    public const char GroupStartSeparator = '(';

    /// <summary>The group end separator.</summary>
    public const char GroupEndSeparator = ')';

    /// <summary>
    /// Parses the specified input string into an instance of the <see cref="Arguments"/> type.
    /// </summary>
    /// <param name="inputArgs">The string representation of the argument to be parsed. This value must be a valid format for the <see cref="Arguments"/>> type.</param>
    /// <param name="formatProvider">The format formatProvider.</param>
    /// <returns>An instance of Argument that represents the parsed value from the input string.</returns>
    /// <exception cref="FormatException">Thrown if the input string is not in a valid format for the <see cref="Arguments"/>> type.</exception>
    public static Arguments Parse(string inputArgs, IFormatProvider? formatProvider)
    {
        if (TryParse(inputArgs, formatProvider, out var result))
        {
            return result;
        }

        throw new FormatException($"The string: {inputArgs} is not a valid {nameof(Arguments)}.");
    }

    /// <summary>
    /// Tries to parse the specified input string into an instance of the <see cref="Arguments"/> type.
    /// </summary>
    /// <param name="inputArguments">The string representation of the argument to be parsed. This value must be a valid format for the <see cref="Arguments"/>> type.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if parsing was successful, otherwise <c>false</c>.</returns>
    public static bool TryParse([NotNullWhen(true)] string? inputArguments, IFormatProvider? formatProvider, out Arguments result)
    {
        var args = ImmutableArray.CreateBuilder<Argument>();
        if (inputArguments.HasValue)
        {
            var argStartIndex = 0;
            var index = 0;
            var level = 0;
            while (index < inputArguments.Length)
            {
                var character = inputArguments[index++];
                if (character == GroupStartSeparator)
                {
                    level++;
                }
                else if (character == GroupEndSeparator)
                {
                    level--;
                }
                else if (character == ArgumentsSeparator && level == 0)
                {
                    if (Argument.TryParse(inputArguments.Substring(argStartIndex, index - argStartIndex - 1), formatProvider, out var arg))
                    {
                        args.Add(arg);
                        argStartIndex = index;
                    }
                    else
                    {
                        result = default;
                        return false;
                    }
                }
            }

            if (Argument.TryParse(inputArguments.Substring(argStartIndex, index - argStartIndex), formatProvider, out var arg2))
            {
                args.Add(arg2);
            }

            result = new Arguments(args.ToImmutable());
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Appends this <see cref="Arguments"/> to the specified <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="formatProvider">The format provider.</param>
    public void AppendInto(StringBuilder stringBuilder, IFormatProvider formatProvider)
    {
        stringBuilder.AppendItems(this.Items, (stringBuilder, arg) => arg.AppendInto(stringBuilder, formatProvider), Arguments.ArgumentsSeparator);
    }

    /// <summary>
    /// Creates a string representation of the <see cref="Arguments"/>.
    /// </summary>
    /// <returns>A string.</returns>
    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        this.AppendInto(stringBuilder, CultureInfo.CurrentCulture);
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Creates an <see cref="Arguments"/> from the specified builder func.
    /// </summary>
    /// <param name="valueIdFunc">The value id func.</param>
    /// <returns>A new <see cref="Arguments"/>.</returns>
    public static Arguments From(Action<ValueIdBuilder> valueIdFunc)
    {
        var valueIdBuilder = new ValueIdBuilder();
        valueIdFunc(valueIdBuilder);
        return valueIdBuilder.Build();
    }

    /// <summary>
    /// Gets the value from the arguments.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="referenceName">The argument name.</param>
    /// <returns>The retrieved value or the default value.</returns>
    public TValue Get<TValue>(TValue defaultValue, IFormatProvider formatProvider, [CallerArgumentExpression(nameof(defaultValue))] string? referenceName = null)
        where TValue : IParsable<TValue>
    {
        if (!referenceName.HasValue)
        {
            throw new NotSupportedException("ReferenceName should be filled by compiler.");
        }

        var argument = this.Items.FirstOrDefault(x => x.Name == referenceName);
        if (argument.HasValue)
        {
            return TValue.Parse(argument.Value, formatProvider);
        }

        var firstDotIndex = referenceName.IndexOf('.');
        var fallback = firstDotIndex > -1
            ? referenceName.Substring(firstDotIndex + 1, referenceName.Length - firstDotIndex - 1)
            : null;
        argument = this.Items.FirstOrDefault(x => x.Name == fallback);
        if (argument.HasValue)
        {
            return TValue.Parse(argument.Value, formatProvider);
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
    public TValue Get2<TValue>(TValue defaultValue, IFormatProvider formatProvider, [CallerArgumentExpression(nameof(defaultValue))] string? referenceName = null)
        where TValue : IValueIdentifiable<TValue>
    {
        if (!referenceName.HasValue)
        {
            throw new NotSupportedException("ReferenceName should be filled by compiler.");
        }

        var argument = this.Items.FirstOrDefault(x => x.Name == referenceName);
        if (argument.HasValue)
        {
            var innerArguments = argument.TryGetValueArguments();
            if (innerArguments.IsSuccess)
            {
                return TValue.From(defaultValue, innerArguments.Value);
            }
        }

        var firstDotIndex = referenceName.IndexOf('.');
        var fallback = firstDotIndex > -1
            ? referenceName.Substring(firstDotIndex + 1, referenceName.Length - firstDotIndex - 1)
            : null;
        argument = this.Items.FirstOrDefault(x => x.Name == fallback);
        if (argument.HasValue)
        {
            var innerArguments = argument.TryGetValueArguments();
            if (innerArguments.IsSuccess)
            {
                return TValue.From(defaultValue, innerArguments.Value);
            }
        }

        return defaultValue;
    }
}