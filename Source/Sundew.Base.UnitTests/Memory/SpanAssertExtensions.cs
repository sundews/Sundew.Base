// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpanAssertExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Memory;

using System;

public static class SpanAssertExtensions
{
    public static SpanAssertions<TItem> Should<TItem>(this Span<TItem> span)
        where TItem : IEquatable<TItem>
    {
        return new SpanAssertions<TItem>(span);
    }

    public static SpanAssertions<TItem> Should<TItem>(this ReadOnlySpan<TItem> span)
        where TItem : IEquatable<TItem>
    {
        return new SpanAssertions<TItem>(span);
    }
}