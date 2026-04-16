// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdRouteParser.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification.Parsing;

using System;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Parsing;

internal static class IdRouteParser
{
    private static readonly Lexer<Tokens> IdRouteLexer;
    private static readonly Lexer<Tokens> IdLexer;

    static IdRouteParser()
    {
        var sourceNameLexerRule = new RegexLexerRule<Tokens>(Tokens.SourceName, new Regex("[^~]+", RegexOptions.Compiled));
        var sourcePathLexerRule = new RegexLexerRule<Tokens>(Tokens.SourcePath, new Regex("[^$~]+", RegexOptions.Compiled));
        var sourceOriginLexerRule = new RegexLexerRule<Tokens>(Tokens.SourceOrigin, new Regex("[^$~//>]*", RegexOptions.Compiled));
        var segmentNameLexerRule = new RegexLexerRule<Tokens>(Tokens.PathSegmentName, new Regex("[^$~//?(>]+", RegexOptions.Compiled));
        var argumentNameLexerRule = new RegexLexerRule<Tokens>(Tokens.ArgumentName, new Regex(@"\G[^!=(]+", RegexOptions.Compiled));
        var valueIdMetadataLexerRule = new RegexLexerRule<Tokens>(Tokens.ValueIdMetadata, new Regex("[^=]+", RegexOptions.Compiled));
        var valueIdValueLexerRule = new RegexLexerRule<Tokens>(Tokens.ValueIdValue, new Regex(@"[^),\]&\#]+", RegexOptions.Compiled));
        var fragmentLexerRule = new RegexLexerRule<Tokens>(Tokens.Fragment, new Regex(@"[^),\]&]+", RegexOptions.Compiled));
        IdRouteLexer = new Lexer<Tokens>(
            [sourceNameLexerRule,
            sourcePathLexerRule,
            sourceOriginLexerRule,
            segmentNameLexerRule,
            argumentNameLexerRule,
            valueIdMetadataLexerRule,
            valueIdValueLexerRule,
            fragmentLexerRule]);
        IdLexer = new Lexer<Tokens>(
        [sourceNameLexerRule,
            sourcePathLexerRule,
            sourceOriginLexerRule,
            segmentNameLexerRule,
            argumentNameLexerRule,
            valueIdMetadataLexerRule,
            valueIdValueLexerRule,
            fragmentLexerRule]);
    }

    public static R<IdRoute, IIdRouteError> ParseIdRoute(string? input, IFormatProvider? formatProvider)
    {
        if (!input.HasValue)
        {
            return R.Error(IIdRouteError.EmptyOrNullError);
        }

        var parser = new Parser<Tokens>(IdRouteLexer, input, formatProvider);
        var idRouteResult = IdRoute(parser);
        if (idRouteResult.IsSuccess && parser.IsEnd())
        {
            return idRouteResult;
        }

        return R.Error(IIdRouteError.NotAtEndError);
    }

    public static R<Id, IIdError> ParseId(string? input, IFormatProvider? formatProvider)
    {
        if (!input.HasValue)
        {
            return R.Error(IIdError.EmptyOrNullError);
        }

        var parser = new Parser<Tokens>(IdLexer, input, formatProvider);
        var idResult = Id(parser);
        if (idResult.IsSuccess && parser.IsEnd())
        {
            return idResult;
        }

        return R.Error(IIdError.NotAtEndError);
    }

    public static R<ValueId, IParseValueIdError> ParseValueId(string? input, IFormatProvider? formatProvider)
    {
        if (!input.HasValue)
        {
            return R.Error(IParseValueIdError.EmptyOrNullError);
        }

        var parser = new Parser<Tokens>(IdLexer, input, formatProvider);
        var valueIdResult = ValueId(parser);
        if (valueIdResult.IsSuccess && parser.IsEnd())
        {
            return valueIdResult.MapError(x => (IParseValueIdError)x);
        }

        return R.Error(IParseValueIdError.NotAtEndError);
    }

    private static R<IdRoute, IIdRouteError> IdRoute(Parser<Tokens> parser)
    {
        var builder = ImmutableArray.CreateBuilder<Id>();
        do
        {
            var idResult = Id(parser);
            if (idResult.IsSuccess)
            {
                builder.Add(idResult.Value);
            }
            else
            {
                return R.Error(IIdRouteError._IdRouteIdError(idResult.Error));
            }
        }
        while (!parser.TryAccept(Grammar.IdSeparator));
        return R.Success(new IdRoute(builder.ToValueList()));
    }

    private static R<Id, IIdError> Id(Parser<Tokens> parser)
    {
        var sourceResult = Source(parser);
        if (!sourceResult.IsSuccess)
        {
            return R.Error(IIdError._IdSourceError(sourceResult.Error));
        }

        Path? path = null;
        if (parser.TryAccept(Grammar.PathSegmentSeparator))
        {
            var pathResult = Path(parser);
            if (pathResult.IsSuccess)
            {
                path = pathResult.Value;
            }
            else
            {
                return R.Error(IIdError._IdPathError(pathResult.Error));
            }
        }

        Arguments? arguments = null;
        if (parser.TryAccept(Grammar.ArgumentsSeparator))
        {
            var valueIdsResult = Arguments(parser);
            if (valueIdsResult.IsSuccess)
            {
                arguments = valueIdsResult.Value;
            }
            else
            {
                return R.Error(IIdError._IdValueIdError(valueIdsResult.Error));
            }
        }

        string? fragment = null;
        if (parser.TryAccept(Grammar.LiteralSeparator))
        {
            var valueIdsResult = Arguments(parser);
            if (valueIdsResult.IsSuccess)
            {
                arguments = valueIdsResult.Value;
            }
            else
            {
                return R.Error(IIdError._IdValueIdError(valueIdsResult.Error));
            }
        }

        return R.Success(new Id(sourceResult.Value, path, arguments, fragment));
    }

    private static R<Source, ISourceError> Source(Parser<Tokens> parser)
    {
        return parser.Accept(Tokens.SourceName).MapError(lexerError => ISourceError._SourceError(Tokens.SourceName, lexerError))
                     .And(() => parser.Accept(Grammar.SourceNamePathSeparator).Map(lexerError => ISourceError._SourceError(Grammar.SourceNamePathSeparator, lexerError)))
                     .And(() => parser.Accept(Tokens.SourcePath).MapError(lexerError => ISourceError._SourceError(Tokens.SourcePath, lexerError)))
                     .And(() => parser.Accept(Grammar.SourcePathOriginSeparator).Map(lexerError => ISourceError._SourceError(Grammar.SourcePathOriginSeparator, lexerError)))
                     .And(() => parser.Accept(Tokens.SourceOrigin).MapError(lexerError => ISourceError._SourceError(Tokens.SourceOrigin, lexerError)))
                     .Map(x => new Source(x.Value3, x.Value2, x.Value1));
    }

    private static R<Path, IPathError> Path(Parser<Tokens> parser)
    {
        var segments = ImmutableArray.CreateBuilder<Segment>();
        while (!parser.IsNext(Grammar.ArgumentsSeparator) && !parser.IsEnd())
        {
            var segmentResult = parser.Accept(Tokens.PathSegmentName).MapError(IPathError._SegmentNameError);
            if (segmentResult.IsSuccess)
            {
                if (parser.TryAccept(Grammar.GroupStart))
                {
                    if (parser.TryAccept(Grammar.GroupEnd))
                    {
                        segments.Add(new Segment(segmentResult.Value, new Arguments(ValueArray<Argument>.Empty)));
                    }
                    else
                    {
                        var argumentsResult = Arguments(parser).MapError(IPathError._PathValueIdError)
                            .And(() => parser.Accept(Grammar.GroupEnd).Map(le => IPathError._PathEndError(Grammar.GroupStart, le)));
                        if (!argumentsResult.IsSuccess)
                        {
                            return R.Error(argumentsResult.Error);
                        }

                        segments.Add(new Segment(segmentResult.Value, argumentsResult.Value));
                    }
                }
                else
                {
                    segments.Add(new Segment(segmentResult.Value, null));
                }
            }
            else
            {
                return R.Error(segmentResult.Error);
            }

            if (!parser.TryAccept(Grammar.PathSegmentSeparator))
            {
                break;
            }
        }

        if (segments.Count == 0)
        {
            return R.Error(IPathError._SegmentNameError(null));
        }

        return R.Success(new Path(segments.ToValueArray()));
    }

    private static R<Arguments, IArgumentsError> Arguments(Parser<Tokens> parser)
    {
        var valueIds = ImmutableArray.CreateBuilder<Argument>();
        while (!parser.IsEnd())
        {
            var argumentResult = Argument(parser);
            if (argumentResult.IsSuccess)
            {
                valueIds.Add(argumentResult.Value);
                if (!parser.Accept(Grammar.ArgumentSeparator))
                {
                    break;
                }
            }
            else
            {
                return R.Error(argumentResult.Error);
            }
        }

        return R.Success(new Arguments(valueIds.ToValueArray()));
    }

    private static R<Argument, IArgumentsError> Argument(Parser<Tokens> parser)
    {
        var argumentNameOption = parser.TryAccept(Tokens.ArgumentName);
        var argumentResult = ValueId(parser).MapError(IArgumentsError._ValueIdError)
            .Map(x => new Argument(argumentNameOption, x));
        if (argumentResult.IsSuccess)
        {
            return argumentResult;
        }

        argumentResult = parser.TryAccept(
            Grammar.KeyValueSeparator,
            result => result.MapError(x => IArgumentsError.ExpectedCharacterError(Grammar.KeyValueSeparator, x)))
                .And(() => Value(parser).MapError(IArgumentsError._ValueError))
                .Map(x => new Argument(argumentNameOption, new ValueId(null, x.Value2)));
        if (argumentResult.IsSuccess)
        {
            return argumentResult;
        }

        var valueIdResult = Value(parser);
        if (valueIdResult.IsSuccess)
        {
            return R.Success(new Argument(argumentNameOption, new ValueId(null, valueIdResult.Value)));
        }

        parser.Undo();
        valueIdResult = Value(parser);
        if (valueIdResult.IsSuccess)
        {
            return R.Success(new Argument(null, new ValueId(null, valueIdResult.Value)));
        }

        return R.Error(IArgumentsError._ValueError(valueIdResult.Error));
    }

    private static R<ValueId, IValueIdError> ValueId(Parser<Tokens> parser)
    {
        var metadataResult = parser.TryAccept(
            Grammar.NameMetadataSeparator,
            result => result.MapError(lexerError => IValueIdError.ExpectedCharacterError(Grammar.NameMetadataSeparator, lexerError))
                .And(() => parser.Accept(Tokens.ValueIdMetadata).MapError(lexerError => IValueIdError.ValueIdError(Tokens.ValueIdMetadata, lexerError))));

        var valueIdResult = parser.TryAccept(Grammar.KeyValueSeparator).Map(lexerError => IValueIdError.ValueIdError(Grammar.KeyValueSeparator, lexerError))
                .And(() => Value(parser).MapError(IValueIdError.ValueIdValueError))
                .Map(x => new ValueId(metadataResult.Value.Value2, x));
        if (valueIdResult.IsSuccess)
        {
            return valueIdResult;
        }

        return Value(parser).Map(x => new ValueId(null, x), IValueIdError.ValueIdValueError);
    }

    private static R<IValue, IValueError> Value(Parser<Tokens> parser)
    {
        var valueResult = parser.TryAccept(
            Grammar.GroupStart,
            result =>
            {
                return result.MapError(lexerError => IValueError.ExpectedCharacterError(Grammar.GroupStart, lexerError))
                    .And(() => Arguments(parser).MapError(x => IValueError._ArgumentsError(x)))
                    .And(() => parser.Accept(Grammar.GroupEnd).Map(lexerError => IValueError.ExpectedCharacterError(Grammar.GroupEnd, lexerError)))
                    .Map(x => x.Value2);
            }).Map(x => IValue.ComplexValue(x.Items));
        if (valueResult.IsSuccess)
        {
            return valueResult;
        }

        valueResult = parser.TryAccept(
            Grammar.ArrayStart,
            result =>
            {
                return result.MapError(lexerError => IValueError.ExpectedCharacterError(Grammar.ArrayStart, lexerError))
                    .And(() =>
                    {
                        var valueIds = ImmutableArray.CreateBuilder<ValueId>();
                        while (!parser.IsNext(Grammar.GroupStart))
                        {
                            var singleValueIdResult = ValueId(parser);
                            if (singleValueIdResult.IsSuccess)
                            {
                                valueIds.Add(singleValueIdResult.Value);
                                if (!parser.Accept(Grammar.ArrayElementSeparator))
                                {
                                    break;
                                }
                            }
                            else
                            {
                                return R.Error(IValueError._ValueIdError(singleValueIdResult.Error));
                            }
                        }

                        return R.Success<IValue>(new ArrayValue(valueIds.ToValueArray())).Omits<IValueError>();
                    })
                    .And(() => parser.Accept(Grammar.ArrayEnd)
                        .Map(lexerError => IValueError.ExpectedCharacterError(Grammar.ArrayEnd, lexerError)))
                    .Map(x => x.Value2);
            });
        if (valueResult.IsSuccess)
        {
            return valueResult;
        }

        var literalValueResult = parser.TryAccept(
            Grammar.LiteralSeparator,
            result => result.MapError(lexerError => IValueError.ExpectedCharacterError(Grammar.LiteralSeparator, lexerError))
                .And(() => parser.Accept(Tokens.ValueIdValue).Map(x => IValue.LiteralValue(x), IValueError._ValueError)))
            .Map(x => x.Value2);
        if (literalValueResult.IsSuccess)
        {
            return literalValueResult;
        }

        return parser.Accept(Tokens.ValueIdValue).Map(x => IValue.ScalarValue(Uri.UnescapeDataString(x)), IValueError._ValueError);
    }
}