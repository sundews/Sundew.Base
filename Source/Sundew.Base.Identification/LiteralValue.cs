// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiteralValue.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

using System;
using System.Text;
using Sundew.Base.Identification.Parsing;

/// <summary>
/// Represents a literal value.
/// </summary>
/// <param name="Value">The literal value.</param>
public sealed partial record LiteralValue(string Value) : IValue
{
    /// <summary>
    /// None value.
    /// </summary>
    public const string Null = "null";

    /// <summary>
    /// Appends the content of the literal value into the string builder.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="appendOptions">The append options.</param>
    public void AppendInto(StringBuilder stringBuilder, IFormatProvider formatProvider, AppendOptions appendOptions)
    {
        stringBuilder
            .Append(Grammar.LiteralSeparator)
            .Append(Uri.EscapeDataString(this.Value));
    }
}