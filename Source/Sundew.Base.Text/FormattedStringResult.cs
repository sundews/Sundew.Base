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
    /// <param name="result">The result.</param>
    /// <returns>The string formatted value.</returns>
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(StringFormatted))]
    public static FormattedStringResult StringFormatted(string result) => new StringFormatted(result);

    /// <summary>
    /// Creates a failed formatted string result due format containing unknown names.
    /// </summary>
    /// <param name="names">The unknown names.</param>
    /// <returns>The result.</returns>
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(FormatContainedUnknownNames))]
    public static FormattedStringResult FormatContainedUnknownNames(IReadOnlyList<string> names) => new FormatContainedUnknownNames(names);

    /// <summary>
    /// Creates a failed formatted string due to arguments containing null values.
    /// </summary>
    /// <param name="nullArguments">The null arguments.</param>
    /// <returns>The result.</returns>
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(ArgumentsContainedNullValues))]
    public static FormattedStringResult ArgumentsContainedNullValues(IReadOnlyList<(string Name, int Index)> nullArguments) => new ArgumentsContainedNullValues(nullArguments);
}