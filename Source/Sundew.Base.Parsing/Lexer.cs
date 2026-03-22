// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Lexer.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Parsing;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Provides functionality to tokenize input strings based on defined lexer rules.
/// </summary>
/// <typeparam name="TToken">Specifies the type of tokens that the lexer will recognize. This type must be a non-nullable type.</typeparam>
public class Lexer<TToken> : ILexer<TToken>
    where TToken : notnull
{
    private readonly Dictionary<TToken, ILexerRule<TToken>> lexerRules;

    /// <summary>
    /// Initializes a new instance of the <see cref="Lexer{TToken}" /> class.
    /// </summary>
    /// <param name="lexerRules">An enumerable collection of lexer rules that define how tokens are recognized. Each rule must specify a unique
    /// token.</param>
    public Lexer(IEnumerable<ILexerRule<TToken>> lexerRules)
    {
        this.lexerRules = lexerRules.ToDictionary(x => x.Token);
    }

    /// <summary>
    /// Attempts to extract the lexeme corresponding to the specified token from the input string, using the current
    /// parser state.
    /// </summary>
    /// <param name="token">The token for which to retrieve the associated lexeme.</param>
    /// <param name="input">The input string from which the lexeme is to be extracted.</param>
    /// <param name="state">The current parser state, which may influence how the lexeme is determined.</param>
    /// <param name="lexeme">When this method returns, contains the extracted lexeme if successful; otherwise, an empty string.</param>
    /// <param name="consumedLength">When this method returns, contains the number of characters consumed from the input if successful; otherwise,
    /// zero.</param>
    /// <returns>true if the lexeme was successfully extracted for the specified token; otherwise, false.</returns>
    public bool TryGetLexeme(
        TToken token,
        string input,
        Parser<TToken>.State state,
        out string lexeme,
        out int consumedLength)
    {
        if (this.lexerRules.TryGetValue(token, out var lexerRule))
        {
            var result = lexerRule.TryGetLexeme(input, state);
            if (result.IsSuccess)
            {
                lexeme = result.Value.Lexeme;
                consumedLength = result.Value.ConsumedLength;
                return true;
            }
        }

        lexeme = string.Empty;
        consumedLength = 0;
        return false;
    }
}