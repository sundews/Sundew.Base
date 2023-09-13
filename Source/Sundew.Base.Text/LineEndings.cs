// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LineEndings.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text;

using System;

/// <summary>
/// Provides the proper line endings.
/// </summary>
public static class LineEndings
{
    /// <summary>
    /// The carriage return followed by new line.
    /// </summary>
    public const string CarriageReturnNewLine = "\r\n";

    /// <summary>
    /// The new line character.
    /// </summary>
    public const string NewLine = "\n";

    /// <summary>
    /// Gets the line endings for the specified operating system.
    /// </summary>
    /// <param name="platform">The operating system.</param>
    /// <returns>The line endings.</returns>
    public static string For(Platform platform)
    {
        return platform switch
        {
            Platform.Windows => CarriageReturnNewLine,
            Platform.Unix => NewLine,
            _ => Environment.NewLine,
        };
    }
}