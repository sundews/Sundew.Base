// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AIdRouteParser.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification.Parsing;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Sundew.Base.Collections.Linq;
using Sundew.Base.Parsing;

internal static class AIdRouteParser
{
    private static readonly Lexer<Tokens> AidLexer;
    private static readonly Lexer<Tokens> AidRouteLexer;

    static AIdRouteParser()
    {
        var sourceNameTokenMatcher = new RegexLexerRule<Tokens>(Tokens.SourceName, new Regex("[^~]+", RegexOptions.Compiled));
        var sourcePathTokenMatcher = new RegexLexerRule<Tokens>(Tokens.SourcePath, new Regex("[^$~]+", RegexOptions.Compiled));
        var sourceOriginTokenMatcher = new RegexLexerRule<Tokens>(Tokens.SourceOrigin, new Regex("[^$~//>]*", RegexOptions.Compiled));
        var segmentNameTokenMatcher = new RegexLexerRule<Tokens>(Tokens.SegmentName, new Regex("[^$~//?(>]+", RegexOptions.Compiled));
        var valueIdNameTokenMatcher = new RegexLexerRule<Tokens>(Tokens.ValueIdName, new Regex("[^!=]", RegexOptions.Compiled));
        var valueIdMetadataTokenMatcher = new RegexLexerRule<Tokens>(Tokens.ValueIdMetadata, new Regex("[^=]+", RegexOptions.Compiled));
        var valueIdValueTokenMatcher = new RegexLexerRule<Tokens>(Tokens.ValueIdValue, new Regex("[^)&]+", RegexOptions.Compiled));
        AidRouteLexer = new Lexer<Tokens>(
            [sourceNameTokenMatcher,
            sourcePathTokenMatcher,
            sourceOriginTokenMatcher,
            segmentNameTokenMatcher,
            valueIdNameTokenMatcher,
            valueIdMetadataTokenMatcher,
            valueIdValueTokenMatcher]);
        AidLexer = new Lexer<Tokens>(
        [sourceNameTokenMatcher,
            sourcePathTokenMatcher,
            sourceOriginTokenMatcher,
            segmentNameTokenMatcher,
            valueIdNameTokenMatcher,
            valueIdMetadataTokenMatcher,
            valueIdValueTokenMatcher]);
    }

    public static bool TryGetAIdRoute(
        string? input,
        IFormatProvider? formatProvider,
        [MaybeNullWhen(false)] out AIdRoute aIdRoute)
    {
        if (!input.HasValue)
        {
            aIdRoute = null;
            return false;
        }

        var parser = new Parser<Tokens>(AidRouteLexer, input);
        if (TryGetAIdRoute(parser, out aIdRoute) && parser.AcceptEnd())
        {
            return true;
        }

        return false;
    }

    public static bool TryGetAId(string? input, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out AId aId)
    {
        if (!input.HasValue)
        {
            aId = null;
            return false;
        }

        var parser = new Parser<Tokens>(AidLexer, input);

        if (TryGetAId(parser, out aId) && parser.AcceptEnd())
        {
            return true;
        }

        return false;
    }

    private static bool TryGetAIdRoute(Parser<Tokens> parser, [MaybeNullWhen(false)] out AIdRoute aIdRoute)
    {
        var route = new List<AId>();
        while (TryGetAId(parser, out var aid))
        {
            route.Add(aid);
            if (!parser.Accept('>'))
            {
                break;
            }
        }

        if (route.Count > 0)
        {
            aIdRoute = new AIdRoute([..route]);
            return true;
        }

        aIdRoute = null;
        return false;
    }

    private static bool TryGetAId(Parser<Tokens> parser, [MaybeNullWhen(false)] out AId aid)
    {
        if (TryGetSource(parser, out var source))
        {
            Path? path = null;
            if (parser.Accept('/'))
            {
                if (TryGetPath(parser, out path))
                {
                }
                else
                {
                }
            }

            ValueId? valueId = null;
            if (parser.Accept('?'))
            {
                if (TryGetValueId(parser, out valueId))
                {
                }
                else
                {
                }
            }

            aid = new AId(source, path, valueId);
            return true;
        }

        aid = null;
        return false;
    }

    private static bool TryGetSource(Parser<Tokens> parser, [MaybeNullWhen(false)] out Source source)
    {
        if (parser.Accept(Tokens.SourceName, out var sourceName) && parser.Accept('~') &&
            parser.Accept(Tokens.SourcePath, out var sourcePath) && parser.Accept('$') &&
            parser.Accept(Tokens.SourceOrigin, out var sourceOrigin))
        {
            source = new Source(sourceName, sourcePath, sourceOrigin);
            return true;
        }

        source = null;
        return false;
    }

    private static bool TryGetPath(Parser<Tokens> parser, [MaybeNullWhen(false)] out Path path)
    {
        var segments = new List<Segment>();
        while (!parser.IsNext('?') && !parser.AcceptEnd())
        {
            if (parser.Accept(Tokens.SegmentName, out var segmentName))
            {
                if (parser.Accept('(') && TryGetValueId(parser, out var valueId) && parser.Accept(')'))
                {
                    segments.Add(new Segment(segmentName, valueId));
                }
                else
                {
                    segments.Add(new Segment(segmentName, null));
                }
            }
        }

        path = new Path([..segments]);
        return true;
    }

    private static bool TryGetValueId(Parser<Tokens> parser, [MaybeNullWhen(false)] out ValueId valueId)
    {
        if (parser.Accept('('))
        {
            if (!TrySingleValue(parser, out valueId))
            {
                valueId = null;
                return false;
            }

            if (parser.Accept(')'))
            {
                return true;
            }
        }
        else
        {
        }

        valueId = null;
        return false;
    }

    private static bool TrySingleValue(Parser<Tokens> parser, [MaybeNullWhen(false)] out ValueId valueId)
    {
        string? name = null;
        string? metadata = null;
        IValue? value = null;
        var lexemesState = parser.CurrentState();
        if (!parser.Accept(Tokens.ValueIdName, out name) || !parser.Accept('!') ||
            !parser.Accept(Tokens.ValueIdMetadata, out metadata) || !parser.Accept('=') ||
            !TryGetValue(parser, out value))
        {
            name = null;
            metadata = null;
            parser.RestoreState(lexemesState);
            if (!parser.Accept('!') ||
                !parser.Accept(Tokens.ValueIdMetadata, out metadata) || !parser.Accept('=') ||
                !TryGetValue(parser, out value))
            {
                name = null;
                metadata = null;
                parser.RestoreState(lexemesState);
                if (!parser.Accept(Tokens.ValueIdName, out name) || !parser.Accept('=') ||
                    !TryGetValue(parser, out value))
                {
                    parser.RestoreState(lexemesState);
                    if (!TryGetValue(parser, out value))
                    {
                        valueId = null;
                        return false;
                    }
                }
            }
        }

        valueId = new ValueId(name, metadata, value);
        return true;
    }

    private static bool TryGetValue(Parser<Tokens> parser, [MaybeNullWhen(false)] out IValue value)
    {
        if (parser.Accept('('))
        {
            var valueIds = new List<ValueId>();
            while (!parser.IsNext(')'))
            {
                if (TrySingleValue(parser, out var valueId) && parser.Accept(')'))
                {
                    valueIds.Add(valueId);
                    if (!parser.Accept('&'))
                    {
                        break;
                    }
                }
                else
                {
                    value = null;
                    return false;
                }
            }

            if (parser.Accept(')'))
            {
                value = valueIds.ByCardinality() switch
                {
                    Empty<ValueId> empty => null,
                    Multiple<ValueId> multiple => new ValueIds([..valueIds]),
                    Single<ValueId> single => single.Item.Value,
                };

                return value != null;
            }
        }

        if (parser.Accept(Tokens.ValueIdValue, out var rawValue))
        {
            value = new SingleValue(rawValue);
            return true;
        }

        value = null;
        return false;
    }
}