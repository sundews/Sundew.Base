// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrayValue.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

using System;
using System.Globalization;
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
        stringBuilder.Append(Grammar.ArrayStart);
        stringBuilder.AppendItems(
            this.Items,
            (stringBuilder, valueId) => valueId.AppendInto(stringBuilder, formatProvider, appendOptions with { IsRoot = false }, false),
            Grammar.ArrayElementSeparator);
        stringBuilder.Append(Grammar.ArrayEnd);
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
}