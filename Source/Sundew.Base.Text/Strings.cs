// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Strings.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text;

using System;

/// <summary>Contains reusable strings.</summary>
public static class Strings
{
    /// <summary>A empty string.</summary>
    public const string Empty = "";

    /// <summary>
    /// The unix new line.
    /// </summary>
    public const string UnixNewLine = "\n";

    /// <summary>
    /// The windows new line.
    /// </summary>
    public const string WindowsNewLine = "\r\n";

    /// <summary>
    /// The environment new line.
    /// </summary>
    public static readonly string NewLine = Environment.NewLine;
}