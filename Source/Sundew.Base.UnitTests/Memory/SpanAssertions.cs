// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpanAssertions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.UnitTests.Memory;

using System;
using System.Text;
using FluentAssertions.Execution;

public ref struct SpanAssertions<TItem>
    where TItem : IEquatable<TItem>
{
    public SpanAssertions(ReadOnlySpan<TItem> subject)
    {
        this.Subject = subject;
    }

    public ReadOnlySpan<TItem> Subject { get; }

    public void Equal(ReadOnlySpan<TItem> expected)
    {
        var stringBuilder = new StringBuilder();
        var expectedText = GetSpan(expected, stringBuilder);
        stringBuilder.Clear();
        var subjectText = GetSpan(this.Subject, stringBuilder);
        AssertionChain.GetOrCreate()
            .ForCondition(this.Subject.SequenceEqual(expected))
            .FailWith("Expected (ReadOnly)Span{0} to be {1}, but found {2}.", typeof(TItem).Name, expectedText, subjectText);
    }

    private static string GetSpan(ReadOnlySpan<TItem> expected, StringBuilder stringBuilder)
    {
        stringBuilder.Append("{ ");
        for (int i = 0; i < expected.Length; i++)
        {
            stringBuilder.Append(expected[i]);
            stringBuilder.Append(", ");
        }

        stringBuilder.Length -= 2;
        stringBuilder.Append(" }");
        return stringBuilder.ToString();
    }
}