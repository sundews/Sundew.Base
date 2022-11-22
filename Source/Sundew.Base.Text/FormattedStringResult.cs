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
public abstract class FormattedStringResult
{
    /// <summary>
    /// Creates a successfully formatted string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The result.</returns>
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(StringFormatted))]
    public static FormattedStringResult StringFormatted(string value) => new StringFormatted(value);

    /// <summary>
    /// Creates a failed formatted string due unexpected names.
    /// </summary>
    /// <param name="names">The unknown names.</param>
    /// <returns>The result.</returns>
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(UnexpectedNames))]
    public static FormattedStringResult UnexpectedNames(IReadOnlyList<string> names) => new UnexpectedNames(names);

    /// <summary>
    /// Creates a failed formatted string due to arguments containing null values.
    /// </summary>
    /// <param name="nullArguments">The null arguments.</param>
    /// <returns>The result.</returns>
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(ArgumentsContainedNullValues))]
    public static FormattedStringResult ArgumentsContainedNullValues(IReadOnlyList<(string Name, int Index)> nullArguments) => new ArgumentsContainedNullValues(nullArguments);
}