// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LexerError.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Parsing;

/// <summary>
/// Represents a lexer error.
/// </summary>
public readonly record struct LexerError(string Input, int Position, int Length)
{
    /// <summary>
    /// Gets the message.
    /// </summary>
    /// <returns>
    /// The error message.
    /// </returns>
    public string GetMessage()
    {
        return $"Invalid input: {this.Input} at position: {this.Position}";
    }
}