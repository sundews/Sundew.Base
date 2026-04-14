// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Parser.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Parsing;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

/// <summary>
/// Provides functionality to parse a sequence of tokens from an input string using a specified lexer.
/// </summary>
/// <typeparam name="TToken">The type of tokens produced by the lexer.</typeparam>
[DebuggerTypeProxy(typeof(ParserDebugView<>))]
[DebuggerDisplay("{Current}")]
public class Parser<TToken>
    where TToken : notnull
{
    private readonly ILexer<TToken> lexer;
    private readonly Stack<State> stateStack = new Stack<State>();

    /// <summary>
    /// Initializes a new instance of the <see cref="Parser{TToken}" /> class.
    /// </summary>
    /// <param name="lexer">The lexer used to tokenize the input string. This parameter must not be null.</param>
    /// <param name="input">The input string to be parsed. This parameter cannot be null or empty.</param>
    /// <param name="formatProvider">The format provider.</param>
    public Parser(ILexer<TToken> lexer, string input, IFormatProvider? formatProvider)
    {
        this.lexer = lexer;
        this.Input = input;
        this.FormatProvider = formatProvider ?? CultureInfo.CurrentCulture;
        this.stateStack.Push(new State(0));
    }

    /// <summary>
    /// Gets the input being processed.
    /// </summary>
    public string Input { get; }

    /// <summary>
    /// Gets the format provider used to control formatting operations for values such as numbers and dates.
    /// </summary>
    public IFormatProvider FormatProvider { get; }

    internal string Current => this.Input.Substring(this.stateStack.Peek().Position);

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
        var state = this.stateStack.Peek();
        if (this.lexer.TryGetLexeme(token, this.Input, state, out lexeme, out var consumedLength))
        {
            this.stateStack.Push(new State(state.Position + consumedLength));
            return true;
        }

        lexeme = string.Empty;
        return false;
    }

    /// <summary>
    /// Determines whether the specified token is accepted at the current position and retrieves the corresponding
    /// lexeme if accepted.
    /// </summary>
    /// <param name="token">The token to evaluate for acceptance at the current input position.</param>
    /// <returns>true if the token is accepted at the current position; otherwise, false.</returns>
    public R<string, LexerError> Accept(TToken token)
    {
        var state = this.stateStack.Peek();
        if (this.lexer.TryGetLexeme(token, this.Input, state, out var lexeme, out var consumedLength))
        {
            this.stateStack.Push(new State(state.Position + consumedLength));
            return R.Success(lexeme);
        }

        return R.Error(LexerError._TokenTypeError(token, state.Position, 0));
    }

    /// <summary>
    /// Determines whether the specified token is accepted at the current position and retrieves the corresponding
    /// lexeme if accepted.
    /// </summary>
    /// <param name="token">The token to evaluate for acceptance at the current input position.</param>
    /// <returns>true if the token is accepted at the current position; otherwise, false.</returns>
    public string? TryAccept(TToken token)
    {
        var state = this.stateStack.Peek();
        if (this.lexer.TryGetLexeme(token, this.Input, state, out var lexeme, out var consumedLength))
        {
            this.stateStack.Push(new State(state.Position + consumedLength));
            return lexeme;
        }

        return null;
    }

    /// <summary>
    /// Determines whether the specified token is accepted at the current position and retrieves the corresponding
    /// lexeme if accepted.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the first output.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="token">The token to evaluate for acceptance at the current input position.</param>
    /// <param name="matchNextFunc">The next match func.</param>
    /// <returns>true if the token is accepted at the current position; otherwise, false.</returns>
    public R<TSuccess, TError> TryAccept<TSuccess, TError>(TToken token, Func<R<string, LexerError>, R<TSuccess, TError>> matchNextFunc)
    {
        var state = this.stateStack.Peek();
        if (this.lexer.TryGetLexeme(token, this.Input, state, out var lexeme, out var consumedLength))
        {
            this.stateStack.Push(new State(state.Position + consumedLength));
            var result = matchNextFunc(R.Success(lexeme));
            if (result.IsSuccess)
            {
                return result;
            }

            this.stateStack.Pop();
            return R.Error(result.Error);
        }

        return matchNextFunc(R.Error(LexerError._TokenTypeError(token, state.Position, 0)));
    }

    /// <summary>
    /// Determines whether the specified token is accepted at the current position and retrieves the corresponding
    /// lexeme if accepted.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the first output.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="input">The character to evaluate against the expected input for the current parsing state.</param>
    /// <param name="matchNextFunc">The next match func.</param>
    /// <returns>true if the token is accepted at the current position; otherwise, false.</returns>
    public R<TSuccess, TError> TryAccept<TSuccess, TError>(char input, Func<R<string, LexerError>, R<TSuccess, TError>> matchNextFunc)
    {
        var state = this.stateStack.Peek();
        if (this.IsNext(input))
        {
            this.stateStack.Push(new State(state.Position + 1));
            var result = matchNextFunc(R.Success(input.ToString()));
            if (result.IsSuccess)
            {
                return result;
            }

            this.stateStack.Pop();
            return R.Error(result.Error);
        }

        return matchNextFunc(R.Error(LexerError._TokenError(input.ToString(), state.Position, 0)));
    }

    /// <summary>
    /// Determines whether the specified character matches the expected input for the current parsing state and advances
    /// the state if a match is found.
    /// </summary>
    /// <remarks>If the input character matches the expected value, the parsing state is updated to the next
    /// position. Otherwise, the state remains unchanged.</remarks>
    /// <param name="input">The character to evaluate against the expected input for the current parsing state.</param>
    /// <returns>true if the input character matches the expected input and the state is advanced; otherwise, false.</returns>
    public RoE<LexerError> TryAccept(char input)
    {
        var state = this.stateStack.Peek();
        if (this.IsNext(input))
        {
            this.stateStack.Push(new State(state.Position + 1));
            return R.Success();
        }

        return R.Error(LexerError._TokenError(input.ToString(), state.Position, 1));
    }

    /// <summary>
    /// Determines whether the specified character matches the expected input for the current parsing state and advances
    /// the state if a match is found.
    /// </summary>
    /// <remarks>If the input character matches the expected value, the parsing state is updated to the next
    /// position. Otherwise, the state remains unchanged.</remarks>
    /// <param name="input">The character to evaluate against the expected input for the current parsing state.</param>
    /// <returns>true if the input character matches the expected input and the state is advanced; otherwise, false.</returns>
    public RoE<LexerError> Accept(char input)
    {
        var state = this.stateStack.Peek();
        if (this.IsNext(input))
        {
            this.stateStack.Push(new State(state.Position + 1));
            return R.Success();
        }

        return R.Error(LexerError._TokenError(input.ToString(), state.Position, 1));
    }

    /// <summary>
    /// Determines whether the specified input matches the expected next input and advances the internal state if the
    /// match is successful.
    /// </summary>
    /// <param name="input">The input string to evaluate against the expected next input. Cannot be null.</param>
    /// <returns>true if the input matches the expected next input and the state is advanced; otherwise, false.</returns>
    public RoE<LexerError> Accept(string input)
    {
        var state = this.stateStack.Peek();
        if (this.IsNext(input))
        {
            this.stateStack.Push(new State(state.Position + input.Length));
            return R.Success();
        }

        return R.Error(LexerError._TokenError(input.ToString(), state.Position, 1));
    }

    /// <summary>
    /// Determines whether the specified character matches the character at the current position in the input.
    /// </summary>
    /// <param name="input">The character to compare with the character at the current input position.</param>
    /// <returns>true if the specified character matches the current character in the input; otherwise, false.</returns>
    public bool IsNext(char input)
    {
        var state = this.stateStack.Peek();
        if (this.Input.Length > state.Position && this.Input[state.Position] == input)
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
        var state = this.stateStack.Peek();
        if (this.Input.Length > state.Position + input.Length - 1 &&
            this.Input.AsSpan(state.Position, input.Length).SequenceEqual(input.AsSpan()))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the current position in the input has reached the end of the input sequence.
    /// </summary>
    /// <returns>true if the current position is at the end of the input; otherwise, false.</returns>
    public RoE<LexerError> AcceptEnd()
    {
        var state = this.stateStack.Peek();
        return R.FromError(state.Position == this.Input.Length, () => LexerError._End(state.Position));
    }

    /// <summary>
    /// Determines whether the current position in the input has reached the end of the input sequence.
    /// </summary>
    /// <returns>true if the current position is at the end of the input; otherwise, false.</returns>
    public RoE<LexerError> IsEnd()
    {
        var state = this.stateStack.Peek();
        return R.FromError(state.Position == this.Input.Length, () => LexerError._End(state.Position));
    }

    /// <summary>
    /// Undoes the last accepted change.
    /// </summary>
    /// <returns>A value indicating whether the undo was successful.</returns>
    public bool Undo()
    {
        return this.stateStack.TryPop(out var _);
    }

    /// <summary>
    /// Gets the current state of the parser.
    /// </summary>
    /// <returns>The current state represented as a <see cref="State"/> instance.</returns>
    public State CurrentState()
    {
        return this.stateStack.Peek();
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