// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamedValues.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text;

/// <summary>
/// Represents a list of values, each with a name.
/// </summary>
public readonly struct NamedValues
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NamedValues"/> struct.
    /// </summary>
    /// <param name="namedStrings">The named strings.</param>
    public NamedValues(params (string Name, object Value)[] namedStrings)
    {
        this.Pairs = namedStrings;
    }

    /// <summary>
    /// Gets the pairs.
    /// </summary>
    public (string Name, object Value)[] Pairs { get; }

    /// <summary>
    /// Creates a <see cref="NamedValues"/>.
    /// </summary>
    /// <param name="namedStrings">The named string pairs.</param>
    /// <returns>The named strings.</returns>
    public static NamedValues Create(params (string Name, object Value)[] namedStrings)
    {
        return new NamedValues(namedStrings);
    }
}