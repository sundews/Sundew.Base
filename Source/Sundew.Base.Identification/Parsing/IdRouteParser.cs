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
    private static readonly Lexer<Tokens> IdLexer;
    private static readonly Lexer<Tokens> IdRouteLexer;

    static IdRouteParser()
    {
        var sourceNameLexerRule = new RegexLexerRule<Tokens>(Tokens.SourceName, new Regex("[^~]+", RegexOptions.Compiled));
        var sourcePathLexerRule = new RegexLexerRule<Tokens>(Tokens.SourcePath, new Regex("[^$~]+", RegexOptions.Compiled));
        var sourceOriginLexerRule = new RegexLexerRule<Tokens>(Tokens.SourceOrigin, new Regex("[^$~//>]*", RegexOptions.Compiled));
        var segmentNameLexerRule = new RegexLexerRule<Tokens>(Tokens.PathSegmentName, new Regex("[^$~//?(>]+", RegexOptions.Compiled));
        var valueIdNameLexerRule = new RegexLexerRule<Tokens>(Tokens.ValueIdName, new Regex("[^!=]+", RegexOptions.Compiled));
        var valueIdMetadataLexerRule = new RegexLexerRule<Tokens>(Tokens.ValueIdMetadata, new Regex("[^=]+", RegexOptions.Compiled));
        var valueIdValueLexerRule = new RegexLexerRule<Tokens>(Tokens.ValueIdValue, new Regex("[^)&]+", RegexOptions.Compiled));
        IdRouteLexer = new Lexer<Tokens>(
            [sourceNameLexerRule,
            sourcePathLexerRule,
            sourceOriginLexerRule,
            segmentNameLexerRule,
            valueIdNameLexerRule,
            valueIdMetadataLexerRule,
            valueIdValueLexerRule]);
        IdLexer = new Lexer<Tokens>(
        [sourceNameLexerRule,
            sourcePathLexerRule,
            sourceOriginLexerRule,
            segmentNameLexerRule,
            valueIdNameLexerRule,
            valueIdMetadataLexerRule,
            valueIdValueLexerRule]);
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

        IArguments? arguments = null;
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

        return R.Success(new Id(sourceResult.Value, path, arguments));
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
                    var argumentsResult = Arguments(parser).MapError(IPathError._PathValueIdError)
                        .And(() => parser.Accept(Grammar.GroupEnd).Map(le => IPathError._PathEndError(Grammar.GroupStart, le)));
                    if (!argumentsResult.IsSuccess)
                    {
                        return R.Error(argumentsResult.Error);
                    }

                    segments.Add(new Segment(segmentResult.Value, argumentsResult.Value));
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

    private static R<IArguments, IArgumentsError> Arguments(Parser<Tokens> parser)
    {
        var valueIds = ImmutableArray.CreateBuilder<ValueId>();
        while (!parser.IsEnd())
        {
            var value = Value(parser);
            if (value.IsSuccess)
            {
                valueIds.Add(new ValueId(null, null, value.Value));
                if (!parser.Accept(Grammar.ValueIdsSeparator))
                {
                    break;
                }

                continue;
            }

            var groupValueIdResult = parser.TryAccept(
                Grammar.GroupStart,
                r => r.MapError(lexerError => IArgumentsError._GroupValueIdError(Grammar.GroupStart, lexerError))
                    .And(() => ValueId(parser).MapError(IArgumentsError._ValueIdError))
                    .And(() => parser.Accept(Grammar.GroupEnd).Map(lexerError => IArgumentsError._GroupValueIdError(Grammar.GroupEnd, lexerError)))
                    .Map(x => x.Value2));

            /*var groupValueIdResult = parser.TryAccept(
                Grammar.GroupStart,
                r => r.MapError(lexerError => IArgumentsError._GroupValueIdError(Grammar.GroupStart, lexerError))
                    .And(() => ValueId(parser).MapError(IArgumentsError._ValueIdError))
                    .And(() => parser.Accept(Grammar.GroupEnd).Map(lexerError => IArgumentsError._GroupValueIdError(Grammar.GroupEnd, lexerError)))
                    .Map(x => x.Value2));*/
            if (groupValueIdResult.IsSuccess)
            {
                valueIds.Add(groupValueIdResult.Value);
            }
            else if (parser.IsNext(Grammar.GroupEnd))
            {
            }
            else
            {
                var valueIdResult = ValueId(parser);
                if (valueIdResult.IsSuccess)
                {
                    valueIds.Add(valueIdResult.Value);
                }
            }

            if (groupValueIdResult.Error is IArgumentsError.GroupValueIdError { Cause: Grammar.GroupEnd })
            {
                return R.Error(groupValueIdResult.Error);
            }

            if (!parser.Accept(Grammar.ValueIdsSeparator))
            {
                break;
            }
        }

        return R.Success(IArguments.ComplexValue(valueIds.ToValueArray()));
    }

    private static R<ValueId, IValueIdError> ValueId(Parser<Tokens> parser)
    {
        var fullValueIdResult = parser.TryAccept(
            Tokens.ValueIdName,
            result => result.MapError(lexerError => IValueIdError._ValueIdError(Tokens.ValueIdName, lexerError))
                .And(() => parser.Accept(Grammar.NameMetadataSeparator).Map(lexerError => IValueIdError._ValueIdError(Grammar.NameMetadataSeparator, lexerError)))
                .And(() => parser.Accept(Tokens.ValueIdMetadata).MapError(lexerError => IValueIdError._ValueIdError(Tokens.ValueIdMetadata, lexerError)))
                .And(() => parser.Accept(Grammar.KeyValueSeparator).Map(lexerError => IValueIdError._ValueIdError(Grammar.KeyValueSeparator, lexerError)))
                .And(() => Value(parser).MapError(IValueIdError._ValueIdValueError))
                .Map(x => new ValueId(x.Value1, x.Value2, x.Value3)));
        if (fullValueIdResult.IsSuccess)
        {
            return fullValueIdResult;
        }

        fullValueIdResult = parser.TryAccept(
            Grammar.NameMetadataSeparator,
            result => result.MapError(lexerError => IValueIdError._ValueIdError(Grammar.NameMetadataSeparator, lexerError))
                .And(() => parser.Accept(Tokens.ValueIdMetadata).MapError(lexerError => IValueIdError._ValueIdError(Tokens.ValueIdMetadata, lexerError)))
                .And(() => parser.Accept(Grammar.KeyValueSeparator).Map(lexerError => IValueIdError._ValueIdError(Grammar.KeyValueSeparator, lexerError)))
                .And(() => Value(parser).MapError(IValueIdError._ValueIdValueError))
                .Map(x => new ValueId(null, x.Value2, x.Value3)));
        if (fullValueIdResult.IsSuccess)
        {
            return fullValueIdResult;
        }

        fullValueIdResult = parser.TryAccept(
            Tokens.ValueIdName,
            result => result.MapError(lexerError => IValueIdError._ValueIdError(Tokens.ValueIdName, lexerError))
                .And(() => parser.Accept(Grammar.KeyValueSeparator).Map(lexerError => IValueIdError._ValueIdError(Grammar.KeyValueSeparator, lexerError)))
                .And(() => Value(parser).MapError(IValueIdError._ValueIdValueError))
                .Map(x => new ValueId(x.Value1, null, x.Value2)));
        if (fullValueIdResult.IsSuccess)
        {
            return fullValueIdResult;
        }

        return Value(parser).Map(x => new ValueId(null, null, x), IValueIdError._ValueIdValueError);
    }

    private static R<IValue, IValueError> Value(Parser<Tokens> parser)
    {
        var valueResult = parser.TryAccept(
            Grammar.GroupStart,
            result =>
            {
                return result.MapError(lexerError => IValueError._GroupError(Grammar.GroupStart, lexerError))
                    .And(() =>
                    {
                        var valueIds = ImmutableArray.CreateBuilder<ValueId>();
                        while (!parser.IsNext(Grammar.GroupStart))
                        {
                            var valueIdResult = ValueId(parser);
                            if (valueIdResult.IsSuccess)
                            {
                                valueIds.Add(valueIdResult.Value);
                                if (!parser.Accept(Grammar.ValueIdsSeparator))
                                {
                                    break;
                                }
                            }
                            else
                            {
                                return R.Error(IValueError._ValueIdError(valueIdResult.Error));
                            }
                        }

                        return R.Success<IValue>(new ComplexValue(valueIds.ToValueArray())).Omits<IValueError>();
                    })
                    .And(() => parser.Accept(Grammar.GroupEnd)
                        .Map(lexerError => IValueError._GroupError(Grammar.GroupEnd, lexerError)))
                    .Map(x => x.Value2);
            });
        if (valueResult.IsSuccess)
        {
            return valueResult;
        }

        valueResult = parser.TryAccept(
            Grammar.ArrayStart,
            result =>
            {
                return result.MapError(lexerError => IValueError._ArrayError(Grammar.GroupStart, lexerError))
                    .And(() =>
                    {
                        var valueIds = ImmutableArray.CreateBuilder<ValueId>();
                        while (!parser.IsNext(Grammar.GroupStart))
                        {
                            var singleValueIdResult = ValueId(parser);
                            if (singleValueIdResult.IsSuccess)
                            {
                                valueIds.Add(singleValueIdResult.Value);
                                if (!parser.Accept(Grammar.ValueIdsSeparator))
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
                        .Map(lexerError => IValueError._ArrayError(Grammar.ArrayEnd, lexerError)))
                    .Map(x => x.Value2);
            });
        if (valueResult.IsSuccess)
        {
            return valueResult;
        }

        return parser.Accept(Tokens.ValueIdValue).Map(x => IValue.ScalarValue(x), IValueError._ValueError);
    }
}