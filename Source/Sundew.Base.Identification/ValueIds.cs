// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueIds.cs" company="Sundews">
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

/// <summary>
/// Represents arguments for an <see cref="AId"/>.
/// </summary>
/// <param name="Items">The value ids.</param>
public sealed record ValueIds(ValueArray<ValueId> Items) : IValue
{
    /// <summary>The value id separator.</summary>
    public const char ValueIdsSeparator = '&';

    /// <summary>The group start separator.</summary>
    public const char GroupStartSeparator = '(';

    /// <summary>The group end separator.</summary>
    public const char GroupEndSeparator = ')';

    /// <summary>
    /// Parses the specified input string into an instance of the <see cref="ValueIds"/> type.
    /// </summary>
    /// <param name="inputValueId">The string representation of the argument to be parsed. This value must be a valid format for the <see cref="ValueIds"/>> type.</param>
    /// <param name="formatProvider">The format formatProvider.</param>
    /// <returns>An instance of ValueId that represents the parsed value from the input string.</returns>
    /// <exception cref="FormatException">Thrown if the input string is not in a valid format for the <see cref="ValueIds"/>> type.</exception>
    public static ValueIds Parse(string inputValueId, IFormatProvider? formatProvider)
    {
        if (TryParse(inputValueId, formatProvider, out var result))
        {
            return result;
        }

        throw new FormatException($"The string: {inputValueId} is not a valid {nameof(ValueIds)}.");
    }

    /// <summary>
    /// Tries to parse the specified input string into an instance of the <see cref="ValueIds"/> type.
    /// </summary>
    /// <param name="inputValueId">The string representation of the argument to be parsed. This value must be a valid format for the <see cref="ValueIds"/>> type.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if parsing was successful, otherwise <c>false</c>.</returns>
    public static bool TryParse([NotNullWhen(true)] string? inputValueId, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out ValueIds result)
    {
        var args = ImmutableArray.CreateBuilder<ValueId>();
        if (inputValueId.HasValue)
        {
            //// var argStartIndex = 0;
            var index = 0;
            var level = 0;
            while (index < inputValueId.Length)
            {
                var character = inputValueId[index++];
                if (character == GroupStartSeparator)
                {
                    level++;
                }
                else if (character == GroupEndSeparator)
                {
                    level--;
                }
                else if (character == ValueIdsSeparator && level == 0)
                {
                    /*if (ValueId.TryParse(inputValueId.Substring(argStartIndex, index - argStartIndex - 1), formatProvider, out var arg))
                    {
                        args.Add(arg);
                        argStartIndex = index;
                    }
                    else
                    {
                        result = null;
                        return false;
                    }*/
                }
            }

            /*if (ValueId.TryParse(inputValueId.Substring(argStartIndex, index - argStartIndex), formatProvider, out var arg2))
            {
                args.Add(arg2);
            }*/

            result = new ValueIds(args.ToImmutable());
            return true;
        }

        result = null;
        return false;
    }

    /// <summary>
    /// Appends this <see cref="ValueIds"/> to the specified <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="formatProvider">The format provider.</param>
    public void AppendInto(StringBuilder stringBuilder, IFormatProvider formatProvider)
    {
        stringBuilder.AppendItems(
            this.Items,
            (builder) => builder.Append(GroupStartSeparator),
            (stringBuilder, arg) => arg.AppendInto(stringBuilder, formatProvider),
            (builder) => builder.Append(GroupEndSeparator),
            ValueIds.ValueIdsSeparator);
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
            return TValue.Parse(argument.Value.ToString() ?? string.Empty, formatProvider);
        }

        var firstDotIndex = referenceName.IndexOf('.');
        var fallback = firstDotIndex > -1
            ? referenceName.Substring(firstDotIndex + 1, referenceName.Length - firstDotIndex - 1)
            : null;
        argument = this.Items.FirstOrDefault(x => x.Name == fallback);
        if (argument.HasValue)
        {
            return TValue.Parse(argument.Value.ToString() ?? string.Empty, formatProvider);
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
            return TValue.From(defaultValue, argument);
        }

        var firstDotIndex = referenceName.IndexOf('.');
        var fallback = firstDotIndex > -1
            ? referenceName.Substring(firstDotIndex + 1, referenceName.Length - firstDotIndex - 1)
            : null;
        argument = this.Items.FirstOrDefault(x => x.Name == fallback);
        if (argument.HasValue)
        {
            return TValue.From(defaultValue, argument);
        }

        return defaultValue;
    }
}