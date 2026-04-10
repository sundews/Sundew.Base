// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LexerError.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Parsing;

using System.Collections.Generic;
using Sundew.DiscriminatedUnions;

/// <summary>
/// Represents a lexer error.
/// </summary>
[DiscriminatedUnion]
public abstract partial record LexerError
{
    /// <summary>
    /// Gets the message.
    /// </summary>
    /// <returns>
    /// The error message.
    /// </returns>
    public abstract string GetMessage();

    /// <summary>
    /// Represents a lexical analysis error that includes details about the invalid input segment and its location within the source text.
    /// </summary>
    /// <param name="TokenType">The token type.</param>
    /// <param name="Position">The zero-based index in the input string where the error was detected.</param>
    /// <param name="Length">The length of the invalid segment in the input string that triggered the error.</param>
    public sealed partial record TokenTypeError(object TokenType, int Position, int Length) : LexerError
    {
        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <returns>
        /// The error message.
        /// </returns>
        public override string GetMessage()
        {
            return $"Invalid token type: {this.TokenType} at position: {this.Position}";
        }
    }

    /// <summary>
    /// Represents a lexical analysis error that includes details about the invalid input segment and its location within the source text.
    /// </summary>
    /// <param name="Token">The input string that contains the invalid segment that caused the lexical error.</param>
    /// <param name="Position">The zero-based index in the input string where the error was detected.</param>
    /// <param name="Length">The length of the invalid segment in the input string that triggered the error.</param>
    public sealed partial record TokenError(string Token, int Position, int Length) : LexerError
    {
        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <returns>
        /// The error message.
        /// </returns>
        public override string GetMessage()
        {
            return $"Invalid input: {this.Token} at position: {this.Position}";
        }
    }

    /// <summary>
    /// Represents a lexical analysis error when the end was expected, but the input still contained more characters.
    /// </summary>
    /// <param name="Position">The zero-based index in the input string where the error was detected.</param>
    public sealed partial record End(int Position) : LexerError
    {
        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <returns>
        /// The error message.
        /// </returns>
        public override string GetMessage()
        {
            return $"Expected end at position: {this.Position}";
        }
    }

    /// <summary>
    /// Represents a collection of lexer errors that occurred during input processing.
    /// </summary>
    /// <param name="Errors">The collection of lexer errors encountered, providing context for the invalid input.</param>
    public sealed partial record Multiple(IEnumerable<LexerError> Errors) : LexerError
    {
        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <returns>
        /// The error message.
        /// </returns>
        public override string GetMessage()
        {
            return $"Invalid input:";
        }
    }
}