// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormattedStringResult.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text;

using System.Collections.Generic;

/// <summary>
/// Represents a result of formatting a <see cref="NamedFormatString"/>.
/// </summary>
[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record FormattedStringResult
{
    /// <summary>
    /// Creates a successfully formatted string.
    /// </summary>
    /// <param name="formattedString">The formatted string.</param>
    /// <returns>The result.</returns>
    public static FormattedStringResult StringFormatted(string formattedString) => new StringFormatted(formattedString);

    /// <summary>
    /// Creates a failed formatted string due to arguments containing null values.
    /// </summary>
    /// <param name="nullArguments">The null arguments.</param>
    /// <returns>The result.</returns>
    public static FormattedStringResult ArgumentsContainedNullValues(IReadOnlyList<(string Name, int Index)> nullArguments) => new ArgumentsContainedNullValues(nullArguments);

    /// <summary>
    /// Creates a failed formatted string due unexpected names.
    /// </summary>
    /// <param name="names">The unknown names.</param>
    /// <returns>The result.</returns>
    public static FormattedStringResult UnexpectedNames(IReadOnlyList<string> names) => new UnexpectedNames(names);
}

/// <summary>
/// Represents a successfully formatted string.
/// </summary>
/// <param name="Value">The formatted string.</param>
public sealed record StringFormatted(string Value) : FormattedStringResult;

/// <summary>
/// Represents a failed formatted string due to arguments containing null values.
/// </summary>
/// <param name="NullArguments">The arguments that contained null values.</param>
public sealed record ArgumentsContainedNullValues(IReadOnlyList<(string Name, int Index)> NullArguments) : FormattedStringResult;

/// <summary>
/// Represents a failed formatted string due to unknown names in format string.
/// </summary>
/// <param name="Names">The unknown names.</param>
public sealed record UnexpectedNames(IReadOnlyList<string> Names) : FormattedStringResult;