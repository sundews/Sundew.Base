// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringFormatted.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text;

/// <summary>
/// Represents a successfully formatted string.
/// </summary>
public sealed class StringFormatted : FormattedStringResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StringFormatted"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public StringFormatted(string value)
    {
        this.Value = value;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    public string Value { get; }
}