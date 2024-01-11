// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringFormatted.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text;

/// <summary>
/// Represents a successfully formatted string.
/// </summary>
#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
public sealed record StringFormatted : FormattedStringResult
#else
public sealed class StringFormatted : FormattedStringResult
#endif
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StringFormatted"/> class.
    /// </summary>
    /// <param name="result">The result.</param>
    public StringFormatted(string result)
    {
        this.Result = result;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    public string Result { get; }
}