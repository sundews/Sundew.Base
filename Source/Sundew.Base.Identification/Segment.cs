// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Segment.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

using System;
using System.Text;

/// <summary>
/// Represents a segment with a specified name and optional associated value identifiers.
/// </summary>
/// <param name="Name">The name of the segment, which serves as its identifier.</param>
/// <param name="Arguments">A value for the segment.</param>
public sealed record Segment(string Name, Arguments? Arguments = null)
{
    /// <summary>
    /// Appends the name of the current instance to the specified StringBuilder, followed by parentheses.
    /// </summary>
    /// <param name="builder">The StringBuilder instance to which the name will be appended. This parameter cannot be null.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns>The updated StringBuilder instance containing the appended name.</returns>
    public StringBuilder AppendInto(StringBuilder builder, IFormatProvider formatProvider)
    {
        builder.Append(this.Name);

        if (this.Arguments.HasValue)
        {
            builder.Append('(');
            this.Arguments.AppendInto(builder, formatProvider, new AppendOptions(true));
            builder.Append(')');
        }

        return builder;
    }
}