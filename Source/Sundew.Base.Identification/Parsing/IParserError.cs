// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IParserError.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Sundew.Base.Identification;

using Sundew.Base.Parsing;
using Sundew.DiscriminatedUnions;

/// <summary>
/// Interface for implementing a parser error.
/// </summary>
public interface IParserError;

#pragma warning disable SA1402

/// <summary>
/// Union for <see cref="IdRoute"/> errors.
/// </summary>
[DiscriminatedUnion]
public partial interface IIdRouteError : IParserError
{
    public sealed partial record IdRouteIdError(IIdError Error) : IIdRouteError;
}

/// <summary>
/// Union for <see cref="Id"/> errors.
/// </summary>
[DiscriminatedUnion]
public partial interface IIdError : IParserError
{
    public sealed partial record IdSourceError(ISourceError Error) : IIdError;

    public sealed partial record IdPathError(IPathError Error) : IIdError;

    public sealed partial record IdValueIdError(IArgumentsError Error) : IIdError;
}

/// <summary>
/// Union for <see cref="Source"/> errors.
/// </summary>
[DiscriminatedUnion]
public partial interface ISourceError : IParserError
{
    public sealed partial record SourceError(object Cause, LexerError Error) : ISourceError;
}

/// <summary>
/// Union for <see cref="Path"/> errors.
/// </summary>
[DiscriminatedUnion]
public partial interface IPathError : IParserError
{
    public sealed partial record SegmentNameError(LexerError? Error) : IPathError;

    public sealed partial record PathValueIdError(IArgumentsError Inner) : IPathError;

    public sealed partial record PathEndError(object Cause, LexerError? Error) : IPathError;
}

/// <summary>
/// Union for <see cref="ValueIdError"/> errors.
/// </summary>
[DiscriminatedUnion]
public partial interface IArgumentsError : IParserError
{
    public sealed partial record ValueIdError(IValueIdError Error) : IArgumentsError;

    public sealed partial record ValueError(IValueError Error) : IArgumentsError;

    public sealed partial record GroupValueIdError(object Cause, LexerError? Error) : IArgumentsError;
}

/// <summary>
/// Union for <see cref="ScalarValue"/> errors.
/// </summary>
[DiscriminatedUnion]
public partial interface IParseValueIdError : IParserError
{
}

/// <summary>
/// Union for <see cref="ScalarValue"/> errors.
/// </summary>
[DiscriminatedUnion]
public partial interface IValueIdError : IParserError
{
}

/// <summary>
/// Union for <see cref="IValue"/> errors.
/// </summary>
[DiscriminatedUnion]
public partial interface IValueError : IParserError
{
    /*public sealed partial record LiteralError(char Expected, LexerError Error) : IValueError;

    public sealed partial record GroupError(char Expected, LexerError Error) : IValueError;

    public sealed partial record ArrayError(char Expected, LexerError Error) : IValueError;*/

    public sealed partial record ArgumentsError(IArgumentsError Error) : IValueError;

    public sealed partial record ValueIdError(IValueIdError Error) : IValueError;

    public sealed partial record ValueError(LexerError? Error) : IValueError;
}

public sealed partial record ValueIdError(object Cause, LexerError? Error) : IValueIdError, IParseValueIdError;

public sealed partial record ValueIdValueError(IValueError Error) : IValueIdError, IParseValueIdError;

/// <summary>
/// Represents an error when there was still input to process.
/// </summary>
public sealed partial record NotAtEndError() : IIdError, IIdRouteError, IParseValueIdError;

/// <summary>
/// Represents an error when the input is empty or null.
/// </summary>
public sealed partial record EmptyOrNullError() : IIdError, IIdRouteError, IParseValueIdError;

/// <summary>
/// Represents a lexer error.
/// </summary>
/// <param name="Cause">The cause.</param>
/// <param name="LexerError">The lexer error.</param>
public sealed partial record ExpectedCharacterError(object Cause, LexerError LexerError) : IArgumentsError, IValueIdError, IValueError;
#pragma warning restore SA1402
