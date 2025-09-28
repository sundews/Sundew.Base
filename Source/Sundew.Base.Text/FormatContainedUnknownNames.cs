// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormatContainedUnknownNames.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text;

using System.Collections.Generic;

/// <summary>
/// Represents a failed formatted string due to unknown names in format string.
/// </summary>
#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
public sealed record FormatContainedUnknownNames
#else
public sealed class FormatContainedUnknownNames
#endif
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FormatContainedUnknownNames"/> class.
    /// </summary>
    /// <param name="names">The names.</param>
    public FormatContainedUnknownNames(IReadOnlyCollection<string> names)
    {
        this.Names = names;
    }

    /// <summary>
    /// Gets the names.
    /// </summary>
    public IReadOnlyCollection<string> Names { get; }
}