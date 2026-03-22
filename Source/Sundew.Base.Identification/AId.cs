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
using Sundew.Base.Identification.Parsing;

/// <summary>
/// Represents any Id.
/// </summary>
public record AId(Source Source, Path? Path, ValueId? ValueId = null) : IParsable<AId>
{
    /// <summary>The value ids separator.</summary>
    public const char ValueIdsSeparator = '?';

    /// <summary>
    /// Parses the specified input string into an instance of the <see cref="AId"/> type.
    /// </summary>
    /// <param name="inputAId">The string representation of the argument to be parsed. This value must be a valid format for the <see cref="AId"/>> type.</param>
    /// <param name="provider">The format provider.</param>
    /// <returns>An instance of ValueId that represents the parsed value from the input string.</returns>
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
    /// Tries to parse the specified input string into an instance of the <see cref="AId"/> type.
    /// </summary>
    /// <param name="inputAId">The string representation of the argument to be parsed. This value must be a valid format for the <see cref="AId"/>> type.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if parsing was successful, otherwise <c>false</c>.</returns>
    public static bool TryParse([NotNullWhen(true)] string? inputAId, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out AId result)
    {
        return AIdRouteParser.TryGetAId(inputAId, formatProvider, out result);
        /*
        if (inputAId.HasValue)
        {
            var argumentsSeparatorIndex = inputAId.IndexOf(ValueIdsSeparator);
            if (argumentsSeparatorIndex > -1)
            {
                var targetString = inputAId.Substring(0, argumentsSeparatorIndex);
                var argumentsString = inputAId.Substring(argumentsSeparatorIndex + 1);
                if (TryParseTarget(targetString, formatProvider, out var target))
                {
                    result = new AId(target.Source, target.Path);
                    return true;
                }
            }
            else if (TryParseTarget(inputAId, formatProvider, out var target))
            {
                result = new AId(target.Source, target.Path);
                return true;
            }
        }

        result = null;
        return false;*/
    }

    /// <summary>
    /// Appends this <see cref="AId"/> to the specified <see cref="StringBuilder"/>.
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

        if (this.ValueId.HasValue)
        {
            stringBuilder.Append(ValueIdsSeparator);
            this.ValueId.AppendInto(stringBuilder, formatProvider);
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
        return this.Source.TryGetType();
    }

    /// <summary>
    /// Tries to get the result type.
    /// </summary>
    /// <returns>A result containing the result type if successful.</returns>
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
        return TargetEvaluator.GetInputTypes(this.Source, this.Path, this.ValueId);
    }

    /// <summary>
    /// Tries to get the target containing type.
    /// </summary>
    /// <returns>A result containing the containing type if successful.</returns>
    public R<Type> TryGetTargetContainingType()
    {
        return TargetEvaluator.GetDeclaringType(this.Source, this.Path);
    }

    /// <summary>
    /// Gets an <see cref="AId"/> from the specified source and expression.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <param name="targetExpression">The target expression.</param>
    /// <returns>A new <see cref="AId"/>.</returns>
    public static AId From<TSource>(Expression<Action<TSource>> targetExpression)
    {
        var (source, path, valueId) = ExpressionEvaluator.From(targetExpression);
        return new AId(source, path, valueId);
    }

    /// <summary>
    /// Gets an <see cref="AId"/> from the specified source and expression.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <param name="targetExpression">The target expression.</param>
    /// <returns>A new <see cref="AId"/>.</returns>
    public static AId From<TSource>(Expression<Func<TSource, object>> targetExpression)
    {
        var target = ExpressionEvaluator.From(targetExpression);
        return new AId(target.Source, target.Path);
    }

    /// <summary>
    /// Gets an <see cref="AId"/> from the specified source and expression.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <param name="targetExpression">The target expression.</param>
    /// <param name="value">The value.</param>
    /// <returns>A new <see cref="AId"/>.</returns>
    public static AId From<TSource>(Expression<Action<TSource>> targetExpression, IIdentifiable<ValueId> value)
    {
        var (source, path, valueId) = ExpressionEvaluator.From(targetExpression, value);
        return new AId(source, path, valueId);
    }

    /// <summary>
    /// Gets an <see cref="AId"/> from the specified source and expression.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <param name="targetExpression">The target expression.</param>
    /// <param name="value">The value.</param>
    /// <returns>A new <see cref="AId"/>.</returns>
    public static AId From<TSource>(Expression<Func<TSource, object>> targetExpression, IIdentifiable<ValueId> value)
    {
        var target = ExpressionEvaluator.From(targetExpression);
        return new AId(target.Source, target.Path, value.Id);
    }

    /// <summary>
    /// Tries to parse the specified input string into an instance of the <see cref="ValueIds"/> type.
    /// </summary>
    /// <param name="inputTarget">The string representation of the argument to be parsed. This value must be a valid format for the <see cref="Target"/>> type.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if parsing was successful, otherwise <c>false</c>.</returns>
    public static bool TryParseTarget([NotNullWhen(true)] string? inputTarget, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out Target result)
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
    /// Indicates an argument placeholder.
    /// </summary>
    /// <typeparam name="TArgument">The argument type.</typeparam>
    /// <returns>The default value.</returns>
    public static TArgument Argument<TArgument>()
    {
        return default!;
    }
}