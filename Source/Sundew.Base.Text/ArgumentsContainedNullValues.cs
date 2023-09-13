// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArgumentsContainedNullValues.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text;

using System.Collections.Generic;

/// <summary>
/// Represents a failed formatted string due to arguments containing null values.
/// </summary>
public sealed class ArgumentsContainedNullValues : FormattedStringResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArgumentsContainedNullValues"/> class.
    /// </summary>
    /// <param name="nullArguments">The null arguments.</param>
    public ArgumentsContainedNullValues(IReadOnlyList<(string Name, int Index)> nullArguments)
    {
        this.NullArguments = nullArguments;
    }

    /// <summary>
    /// Gets the null arguments.
    /// </summary>
    public IReadOnlyList<(string Name, int Index)> NullArguments { get; }
}