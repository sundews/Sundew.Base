// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrayValue.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Identification.Parsing;
using Sundew.Base.Text;

/// <summary>
/// Represents arguments for an <see cref="Id"/>.
/// </summary>
/// <param name="Items">The value ids.</param>
public sealed partial record ArrayValue(ValueArray<ValueId> Items) : IValue
{
    /// <summary>
    /// Appends this <see cref="ComplexValue"/> to the specified <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="appendOptions">The append options.</param>
    public void AppendInto(StringBuilder stringBuilder, IFormatProvider formatProvider, AppendOptions appendOptions)
    {
        stringBuilder.AppendItems(
            this.Items,
            (stringBuilder, arg) => arg.AppendInto(stringBuilder, formatProvider, appendOptions with { IsRoot = false }),
            Grammar.ValueIdsSeparator);
    }

    /// <summary>
    /// Creates a string representation of the <see cref="ComplexValue"/>.
    /// </summary>
    /// <returns>A string.</returns>
    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        this.AppendInto(stringBuilder, CultureInfo.CurrentCulture, new AppendOptions(true));
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