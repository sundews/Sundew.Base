// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILexer.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Parsing;

/// <summary>
/// Defines a contract for lexers that extract lexemes from input strings based on tokens.
/// </summary>
/// <typeparam name="TToken">Specifies the type of tokens that the lexer processes.</typeparam>
public interface ILexer<TToken>
    where TToken : notnull
{
    /// <summary>
    /// Attempts to extract the lexeme associated with the specified token from the input string.
    /// </summary>
    /// <param name="token">The token for which to retrieve the corresponding lexeme.</param>
    /// <param name="input">The input string from which the lexeme is to be extracted.</param>
    /// <param name="state">The current parser state, which may influence how the lexeme is determined.</param>
    /// <param name="lexeme">When this method returns <see langword="true"/>, contains the extracted lexeme for the specified token;
    /// otherwise, the value is undefined.</param>
    /// <param name="consumedLength">When this method returns <see langword="true"/>, contains the number of characters consumed from the input to
    /// extract the lexeme; otherwise, the value is undefined.</param>
    /// <returns><see langword="true"/> if the lexeme was successfully extracted; otherwise, <see langword="false"/>.</returns>
    bool TryGetLexeme(TToken token, string input, Parser<TToken>.State state, out string lexeme, out int consumedLength);
}