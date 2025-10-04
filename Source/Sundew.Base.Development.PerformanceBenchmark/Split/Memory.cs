// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Memory.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.PerformanceBenchmark.Split;

using System;
using System.Collections.Generic;
using Sundew.Base.Memory;

public static class Memory
{
    public static IEnumerable<ReadOnlyMemory<char>> SplitBasedCommandLineLexer(string input)
    {
        const char doubleQuote = '\"';
        const char slash = '\\';
        const char space = ' ';
        var isInQuote = false;
        var isInEscape = false;
        var previousWasSpace = false;
        return input.AsMemory().Split(
            (character, index, splitContext) =>
            {
                var actualIsInEscape = isInEscape;
                var actualPreviousWasSpace = previousWasSpace;
                isInEscape = false;
                previousWasSpace = false;
                switch (character)
                {
                    case slash:
                        if (splitContext.GetNextOrDefault(index) == doubleQuote)
                        {
                            isInEscape = true;
                            return SplitAction.Ignore;
                        }

                        return SplitAction.Include;
                    case doubleQuote:
                        if (!actualIsInEscape)
                        {
                            isInQuote = !isInQuote;
                        }

                        return actualIsInEscape ? SplitAction.Include : SplitAction.Ignore;
                    case space:
                        previousWasSpace = true;
                        if (actualIsInEscape)
                        {
                            isInQuote = false;
                            return SplitAction.Split;
                        }

                        if (actualPreviousWasSpace)
                        {
                            return SplitAction.Ignore;
                        }

                        return isInQuote ? SplitAction.Include : SplitAction.Split;
                    default:
                        return SplitAction.Include;
                }
            },
            SplitOptions.None);
    }
}
