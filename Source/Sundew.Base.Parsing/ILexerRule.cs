// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILexerRule.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Parsing;

/// <summary>
/// Defines a contract for a lexer rule that identifies and extracts tokens from input text based on the current parser
/// state.
/// </summary>
/// <typeparam name="TToken">The type of token produced by this lexer rule.</typeparam>
public interface ILexerRule<TToken>
    where TToken : notnull
{
    /// <summary>
    /// Gets the token associated with this instance.
    /// </summary>
    TToken Token { get; }

    /// <summary>
    /// Attempts to extract a lexeme from the specified input string based on the current parser state.
    /// </summary>
    /// <param name="input">The input string from which to extract the lexeme. This parameter must not be null or empty.</param>
    /// <param name="state">The current parser state that determines how the lexeme is identified. This parameter must be a valid and
    /// initialized state.</param>
    /// <returns>A result containing a tuple with the extracted lexeme and the number of characters consumed from the input. If
    /// extraction fails, returns an error describing the reason.</returns>
    R<(string Lexeme, int ConsumedLength), LexerError> TryGetLexeme(string input, Parser<TToken>.State state);
}