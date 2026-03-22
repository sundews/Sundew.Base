// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegexLexerRule.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Parsing;

using System.Text.RegularExpressions;

/// <summary>
/// Represents a lexer rule that matches input text using a specified regular expression and produces a corresponding
/// token when a match is found.
/// </summary>
/// <typeparam name="TToken">The type of token produced by the lexer rule.</typeparam>
/// <param name="token">The token to associate with input that matches the regular expression.</param>
/// <param name="regex">The regular expression used to identify matching lexemes in the input text.</param>
public class RegexLexerRule<TToken>(TToken token, Regex regex) : ILexerRule<TToken>
{
    private const string TokenGroupName = "TOKEN";

    /// <summary>
    /// Gets the token associated with the current instance.
    /// </summary>
    public TToken Token { get; } = token;

    /// <summary>
    /// Attempts to extract a lexeme from the specified input string, starting at the position indicated by the parser
    /// state.
    /// </summary>
    /// <param name="input">The input string from which to extract the lexeme. Cannot be null or empty.</param>
    /// <param name="state">The current parser state, which specifies the position in the input string at which to begin matching.</param>
    /// <returns>An R containing a tuple with the extracted lexeme and the number of characters consumed if a match is found;
    /// otherwise, an error indicating the failure to retrieve a lexeme.</returns>
    public R<(string Lexeme, int ConsumedLength), LexerError> TryGetLexeme(string input, Parser<TToken>.State state)
    {
        var match = regex.Match(input, state.Position);
        if (match.Success)
        {
            if (match.Groups.TryGetValue(TokenGroupName, out var matchingGroup))
            {
                if (matchingGroup.Success)
                {
                    return R.Success((matchingGroup.Value, match.Length));
                }

                return R.Error(new LexerError(input, state.Position, -1));
            }

            return R.Success((match.Value, match.Length));
        }

        return R.Error(new LexerError(input, state.Position, -1));
    }
}