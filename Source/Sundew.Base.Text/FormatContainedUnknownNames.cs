// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormatContainedUnknownNames.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text;

using System.Collections.Generic;

/// <summary>
/// Represents a failed formatted string due to unknown names in format string.
/// </summary>
public sealed class FormatContainedUnknownNames : FormattedStringResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FormatContainedUnknownNames"/> class.
    /// </summary>
    /// <param name="names">The names.</param>
    public FormatContainedUnknownNames(IReadOnlyList<string> names)
    {
        this.Names = names;
    }

    /// <summary>
    /// Gets the names.
    /// </summary>
    public IReadOnlyList<string> Names { get; }
}