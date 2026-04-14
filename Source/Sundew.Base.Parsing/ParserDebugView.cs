// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParserDebugView.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Parsing;

using System.Diagnostics;

internal class ParserDebugView<TToken>
    where TToken : notnull
{
    private readonly Parser<TToken> parser;

    public ParserDebugView(Parser<TToken> parser)
    {
        this.parser = parser;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public string State => this.parser.Input.Substring(this.parser.CurrentState().Position);
}