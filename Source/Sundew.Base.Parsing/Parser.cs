// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Parser.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Parsing;

using System;

/// <summary>
/// Provides functionality to parse a sequence of tokens from an input string using a specified lexer.
/// </summary>
/// <typeparam name="TToken">The type of tokens produced by the lexer.</typeparam>
public class Parser<TToken>
{
    private readonly ILexer<TToken> lexer;
    private readonly string input;
    private State state;

    /// <summary>
    /// Initializes a new instance of the <see cref="Parser{TToken}" /> class.
    /// </summary>
    /// <param name="lexer">The lexer used to tokenize the input string. This parameter must not be null.</param>
    /// <param name="input">The input string to be parsed. This parameter cannot be null or empty.</param>
    public Parser(ILexer<TToken> lexer, string input)
    {
        this.lexer = lexer;
        this.input = input;
        this.state = new State(0);
    }

    /// <summary>
    /// Determines whether the specified token is accepted at the current position and retrieves the corresponding
    /// lexeme if accepted.
    /// </summary>
    /// <param name="token">The token to evaluate for acceptance at the current input position.</param>
    /// <param name="lexeme">When this method returns, contains the lexeme associated with the accepted token if the token is accepted;
    /// otherwise, an empty string.</param>
    /// <returns>true if the token is accepted at the current position; otherwise, false.</returns>
    public bool Accept(TToken token, out string lexeme)
    {
        if (this.lexer.TryGetLexeme(token, this.input, this.state, out lexeme, out var consumedLength))
        {
            this.state = new State(this.state.Position + consumedLength);
            return true;
        }

        lexeme = string.Empty;
        return false;
    }

    /// <summary>
    /// Determines whether the specified character matches the expected input for the current parsing state and advances
    /// the state if a match is found.
    /// </summary>
    /// <remarks>If the input character matches the expected value, the parsing state is updated to the next
    /// position. Otherwise, the state remains unchanged.</remarks>
    /// <param name="input">The character to evaluate against the expected input for the current parsing state.</param>
    /// <returns>true if the input character matches the expected input and the state is advanced; otherwise, false.</returns>
    public bool Accept(char input)
    {
        if (this.IsNext(input))
        {
            this.state = new State(this.state.Position + 1);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the specified input matches the expected next input and advances the internal state if the
    /// match is successful.
    /// </summary>
    /// <param name="input">The input string to evaluate against the expected next input. Cannot be null.</param>
    /// <returns>true if the input matches the expected next input and the state is advanced; otherwise, false.</returns>
    public bool Accept(string input)
    {
        if (this.IsNext(input))
        {
            this.state = new State(this.state.Position + input.Length);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the specified character matches the character at the current position in the input.
    /// </summary>
    /// <param name="input">The character to compare with the character at the current input position.</param>
    /// <returns>true if the specified character matches the current character in the input; otherwise, false.</returns>
    public bool IsNext(char input)
    {
        if (this.input[this.state.Position] == input)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the specified string matches the input sequence at the current position.
    /// </summary>
    /// <param name="input">The string to compare with the input sequence at the current position. The length of this string must not exceed
    /// the number of remaining characters in the input sequence.</param>
    /// <returns>true if the specified string matches the input sequence at the current position; otherwise, false.</returns>
    public bool IsNext(string input)
    {
        if (this.input.AsSpan(this.state.Position, input.Length).SequenceEqual(input.AsSpan()))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the current position in the input has reached the end of the input sequence.
    /// </summary>
    /// <returns>true if the current position is at the end of the input; otherwise, false.</returns>
    public bool AcceptEnd()
    {
        return this.state.Position == this.input.Length;
    }

    /// <summary>
    /// Gets the current state of the parser.
    /// </summary>
    /// <returns>The current state represented as a <see cref="State"/> instance.</returns>
    public State CurrentState()
    {
        return this.state;
    }

    /// <summary>
    /// Restores the object's state to the specified value.
    /// </summary>
    /// <param name="state">The state to restore. This parameter must not be null.</param>
    public void RestoreState(State state)
    {
        this.state = state;
    }

    /// <summary>
    /// Represents the current state of an item within its parent collection, including its position.
    /// </summary>
    public readonly record struct State
    {
        internal State(int position)
        {
            this.Position = position;
        }

        /// <summary>
        /// Gets the zero-based index that indicates the current position of the item within its parent collection.
        /// </summary>
        /// <remarks>The position reflects the item's index in the collection, where the first item has a
        /// position of 0. This property is read-only and updates automatically as the collection changes.</remarks>
        public int Position { get; }
    }
}