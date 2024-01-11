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
#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
public sealed record ArgumentsContainedNullValues : FormattedStringResult
#else
public sealed class ArgumentsContainedNullValues : FormattedStringResult
#endif
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