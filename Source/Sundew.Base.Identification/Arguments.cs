// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Arguments.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

using System;
using System.Text;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Identification.Parsing;
using Sundew.Base.Text;

/// <summary>
/// Represents a list of arguments.
/// </summary>
/// <param name="Items">The arguments.</param>
public sealed record Arguments(ValueArray<Argument> Items)
{
    /// <summary>
    /// Appends this <see cref="ValueId"/> to the specified <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="appendOptions">The append options.</param>
    public void AppendInto(StringBuilder stringBuilder, IFormatProvider formatProvider, AppendOptions appendOptions)
    {
        stringBuilder.AppendItems(this.Items, (builder, argument) => argument.AppendInto(builder, formatProvider, appendOptions), Grammar.ArgumentSeparator);
    }
}