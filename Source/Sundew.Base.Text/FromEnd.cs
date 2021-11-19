// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FromEnd.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text;

/// <summary>
/// Struct that contains an offset from the end.
/// </summary>
public readonly struct FromEnd
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FromEnd"/> struct.
    /// </summary>
    /// <param name="offset">The offset.</param>
    public FromEnd(int offset)
    {
        this.Value = offset;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    internal int Value { get; }

    /// <summary>
    /// Performs an implicit conversion from <see cref="string"/> to <see cref="FromEnd"/>.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    public static implicit operator FromEnd(string input)
    {
        return FromStringLength(input);
    }

    /// <summary>
    /// Froms the string length.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>The end offset.</returns>
    public static FromEnd FromStringLength(string input)
    {
        return new FromEnd(input.Length);
    }
}