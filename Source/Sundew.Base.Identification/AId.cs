// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AId.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;

/// <summary>
/// Represents any Id.
/// </summary>
public record AId(Target Target, Arguments? Arguments) : IParsable<AId>
{
    /// <summary>The arguments separator.</summary>
    public const char ArgumentsSeparator = '?';

    /// <summary>
    /// Parses the specified input string into an instance of the <see cref="AId"/> type.
    /// </summary>
    /// <param name="inputAId">The string representation of the argument to be parsed. This value must be a valid format for the <see cref="AId"/>> type.</param>
    /// <param name="provider">The format provider.</param>
    /// <returns>An instance of Argument that represents the parsed value from the input string.</returns>
    /// <exception cref="FormatException">Thrown if the input string is not in a valid format for the <see cref="AId"/>> type.</exception>
    public static AId Parse(string inputAId, IFormatProvider? provider)
    {
        if (TryParse(inputAId, provider, out var result))
        {
            return result;
        }

        throw new FormatException($"The string: {inputAId} is not a valid {nameof(AId)}");
    }

    /// <summary>
    /// Tries to parse the specified input string into an instance of the <see cref="Arguments"/> type.
    /// </summary>
    /// <param name="inputAId">The string representation of the argument to be parsed. This value must be a valid format for the <see cref="AId"/>> type.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if parsing was successful, otherwise <c>false</c>.</returns>
    public static bool TryParse([NotNullWhen(true)] string? inputAId, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out AId result)
    {
        if (inputAId.HasValue)
        {
            var argumentsSeparatorIndex = inputAId.IndexOf(ArgumentsSeparator);
            if (argumentsSeparatorIndex > -1)
            {
                var targetString = inputAId.Substring(0, argumentsSeparatorIndex);
                var argumentsString = inputAId.Substring(argumentsSeparatorIndex + 1);
                if (Target.TryParse(targetString, formatProvider, out var target) && Identification.Arguments.TryParse(argumentsString, formatProvider, out var args))
                {
                    result = new AId(target, args);
                    return true;
                }
            }
            else if (Target.TryParse(inputAId, formatProvider, out var target))
            {
                result = new AId(target, null);
                return true;
            }
        }

        result = null;
        return false;
    }

    /// <summary>
    /// Appends this <see cref="AId"/> to the specified <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="formatProvider">The format provider.</param>
    public void AppendInto(StringBuilder stringBuilder, IFormatProvider formatProvider)
    {
        this.Target.AppendInto(stringBuilder, formatProvider);
        if (this.Arguments.HasValue)
        {
            stringBuilder.Append(ArgumentsSeparator);
            this.Arguments.Value.AppendInto(stringBuilder, formatProvider);
        }
    }

    /// <summary>
    /// Creates a string representation of the <see cref="AId"/>.
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
        return this.Target.TryGetSourceType();
    }

    /// <summary>
    /// Tries to get the result type.
    /// </summary>
    /// <returns>A result containing the result type if successful.</returns>
    public R<Type> TryGetResultType()
    {
        return this.Target.TryGetResultType();
    }

    /// <summary>
    /// Tries to get the input types.
    /// </summary>
    /// <returns>A result containing the input types if successful.</returns>
    public R<IReadOnlyList<Type>> TryGetInputTypes()
    {
        return this.Target.TryGetInputTypes();
    }

    /// <summary>
    /// Tries to get the target containing type.
    /// </summary>
    /// <returns>A result containing the containing type if successful.</returns>
    public R<Type> TryGetTargetContainingType()
    {
        return this.Target.TryGetContainingType();
    }

    /// <summary>
    /// Gets an <see cref="AId"/> from the specified source and expression.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <param name="targetExpression">The target expression.</param>
    /// <returns>A new <see cref="AId"/>.</returns>
    public static AId From<TSource>(Expression<Action<TSource>> targetExpression)
    {
        var target = Target.From(targetExpression);
        return new AId(target, null);
    }

    /// <summary>
    /// Gets an <see cref="AId"/> from the specified source and expression.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <param name="targetExpression">The target expression.</param>
    /// <returns>A new <see cref="AId"/>.</returns>
    public static AId From<TSource>(Expression<Func<TSource, object>> targetExpression)
    {
        var target = Target.From(targetExpression);
        return new AId(target, null);
    }
}