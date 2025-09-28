// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringFormatted.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Represents a successfully formatted string.
/// </summary>
#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
public sealed record StringFormatted
#else
public sealed class StringFormatted
#endif
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StringFormatted"/> class.
    /// </summary>
    /// <param name="nullArguments">The null arguments.</param>
    /// <param name="result">The result.</param>
    public StringFormatted(string result, IReadOnlyList<(string Name, int Index)> nullArguments)
    {
        this.Result = result;
        this.NullArguments = nullArguments;
    }

    /// <summary>
    /// Gets a value indicating whether the string was formatted using null arguments.
    /// </summary>
    public bool HasNulls => this.NullArguments.Any();

    /// <summary>
    /// Gets the value.
    /// </summary>
    public string Result { get; }

    /// <summary>
    /// Gets a read-only list of argument names and their corresponding indexes for arguments that are null.
    /// </summary>
    /// <remarks>Each tuple in the list contains the name and index of an argument that was detected as null.
    /// The list is empty if no arguments are null.</remarks>
    public IReadOnlyList<(string Name, int Index)> NullArguments { get; }
}