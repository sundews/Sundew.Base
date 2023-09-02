// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyMemoryExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Memory;

using System;
using System.Collections.Generic;
using Sundew.Base.Memory.Internal;

/// <summary>
/// Extends <see cref="ReadOnlyMemory{Char}"/> with easy to use methods.
/// </summary>
public static class ReadOnlyMemoryExtensions
{
    /// <summary>
    /// Splits the specified input with the <see cref="SplitFunc{TItem}" />.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="input">The input.</param>
    /// <param name="splitFunc">The split function.</param>
    /// <param name="splitOptions">The split options.</param>
    /// <returns>
    /// The splitted items as an <see cref="IEnumerable{T}" />.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an unknown <see cref="SplitAction" /> result is returned.</exception>
    public static IEnumerable<ReadOnlyMemory<TItem>> Split<TItem>(this ReadOnlyMemory<TItem> input, SplitFunc<TItem> splitFunc, SplitOptions splitOptions = SplitOptions.None)
    {
        if (input.IsEmpty)
        {
            yield break;
        }

        var splitContext = new SplitContext<TItem>(input);
        for (var index = 0; index < input.Length; index++)
        {
            var item = input.Span[index];
            var splitMemory = splitFunc(item, index, splitContext);
            switch (splitMemory)
            {
                case SplitAction.Include:
                    if (splitContext.StartIndex == SplitContext<TItem>.SectionNotStartedIndex)
                    {
                        splitContext.StartIncluding(index);
                        break;
                    }

                    splitContext.Include(item);
                    break;
                case SplitAction.Split:
                    {
                        var section = splitContext.GetSectionAndReset();
                        if (!section.IsEmpty || !splitOptions.HasFlag(SplitOptions.RemoveEmptyEntries))
                        {
                            yield return section;
                        }
                    }

                    break;
                case SplitAction.IncludeAndSplit:
                    {
                        if (splitContext.StartIndex == SplitContext<TItem>.SectionNotStartedIndex)
                        {
                            splitContext.StartIncluding(index);
                        }
                        else
                        {
                            splitContext.Include(item);
                        }

                        var section = splitContext.GetSectionAndReset();
                        if (!section.IsEmpty || !splitOptions.HasFlag(SplitOptions.RemoveEmptyEntries))
                        {
                            yield return section;
                        }
                    }

                    break;
                case SplitAction.Ignore:
                    splitContext.IsIgnoring = true;
                    break;
                case SplitAction.SplitAndSplitCurrent:
                    {
                        var section = splitContext.GetSectionAndReset();
                        if (!section.IsEmpty || !splitOptions.HasFlag(SplitOptions.RemoveEmptyEntries))
                        {
                            yield return section;
                        }

                        yield return input.Slice(index, 1);
                    }

                    break;
                case SplitAction.SplitAndInclude:
                    {
                        var section = splitContext.GetSectionAndReset();
                        if (!section.IsEmpty || !splitOptions.HasFlag(SplitOptions.RemoveEmptyEntries))
                        {
                            yield return section;
                        }

                        if (splitContext.StartIndex == SplitContext<TItem>.SectionNotStartedIndex)
                        {
                            splitContext.StartIncluding(index);
                            break;
                        }

                        splitContext.Include(item);
                    }

                    break;
                case SplitAction.SplitAndIncludeRest:
                {
                    var section = splitContext.GetSectionAndReset();
                    if (!section.IsEmpty || !splitOptions.HasFlag(SplitOptions.RemoveEmptyEntries))
                    {
                        yield return section;
                    }

                    yield return input.Slice(index, input.Length - index);
                    yield break;
                }

                default:
                    throw new ArgumentOutOfRangeException($"Invalid SplitMemory value: {splitMemory}");
            }
        }

        var lastSection = splitContext.GetSectionAndReset();
        if (!lastSection.IsEmpty || !splitOptions.HasFlag(SplitOptions.RemoveEmptyEntries))
        {
            yield return lastSection;
        }
    }
}