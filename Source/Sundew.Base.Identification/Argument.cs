// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Argument.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

using System;
using System.Text;

/// <summary>
/// Represents an argument.
/// </summary>
/// <param name="Name">The Name.</param>
/// <param name="ValueId">The value id.</param>
public sealed record Argument(string? Name, ValueId ValueId)
{
    /// <summary>
    /// Appends this instance into the specified string builder.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="appendOptions">The append options.</param>
    public void AppendInto(StringBuilder stringBuilder, IFormatProvider formatProvider, AppendOptions appendOptions)
    {
        if (this.Name.HasValue)
        {
            stringBuilder.Append(this.Name);
        }

        this.ValueId.AppendInto(stringBuilder, formatProvider, appendOptions, this.Name.HasValue);
    }
}