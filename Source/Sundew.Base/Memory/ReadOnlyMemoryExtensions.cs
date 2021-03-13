// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyMemoryExtensions.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Memory
{
    using System;
    using System.Collections.Generic;
    using Sundew.Base.Text;

    /// <summary>
    /// Extends <see cref="ReadOnlyMemory{Char}"/> with easy to use methods.
    /// </summary>
    public static class ReadOnlyMemoryExtensions
    {
        /// <summary>
        /// Splits the specified input with the <see cref="SplitFunc" />.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="input">The input.</param>
        /// <param name="splitFunc">The split function.</param>
        /// <returns>
        /// The splitted items as an <see cref="IEnumerable{T}" />.
        /// </returns>
        /// <exception cref="NotSupportedException">Thrown when attempting to include an items while ignoring previous items.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an unknown <see cref="SplitAction" /> result is returned.</exception>
        public static IEnumerable<ReadOnlyMemory<TItem>> Split<TItem>(this ReadOnlyMemory<TItem> input, SplitFunc<TItem> splitFunc)
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
                            if (!section.IsEmpty)
                            {
                                yield return section;
                            }
                        }

                        break;
                    case SplitAction.SplitInclusive:
                        {
                            splitContext.Include(item);
                            var section = splitContext.GetSectionAndReset();
                            if (!section.IsEmpty)
                            {
                                yield return section;
                            }
                        }

                        break;
                    case SplitAction.Ignore:
                        splitContext.IsIgnoring = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Invalid SplitMemory value: {splitMemory}");
                }
            }

            var lastSection = splitContext.GetSectionAndReset();
            if (!lastSection.IsEmpty)
            {
                yield return lastSection;
            }
        }
    }
}