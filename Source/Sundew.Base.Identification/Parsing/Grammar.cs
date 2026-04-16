// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Grammar.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification.Parsing;

internal static class Grammar
{
    /// <summary>The Source name/path separator.</summary>
    public const char SourceNamePathSeparator = '~';

    /// <summary>The Source path/origin separator.</summary>
    public const char SourcePathOriginSeparator = '$';

    /// <summary>The path segment separator.</summary>
    public const char PathSegmentSeparator = '/';

    /// <summary>The arguments separator.</summary>
    public const char ArgumentsSeparator = '?';

    /// <summary>The literal separator.</summary>
    public const char LiteralSeparator = '^';

    /// <summary>Metadata separator.</summary>
    public const char NameMetadataSeparator = '!';

    /// <summary>Key Value separator.</summary>
    public const char KeyValueSeparator = '=';

    /// <summary>The array element separator.</summary>
    public const char ArrayElementSeparator = ',';

    /// <summary>The start of and array.</summary>
    public const char ArrayStart = '[';

    /// <summary>The end of an array.</summary>
    public const char ArrayEnd = ']';

    /// <summary>The argument separator.</summary>
    public const char ArgumentSeparator = '&';

    /// <summary>The group start.</summary>
    public const char GroupStart = '(';

    /// <summary>The group end.</summary>
    public const char GroupEnd = ')';

    /// <summary>The Id separator.</summary>
    public const char IdSeparator = '>';
}