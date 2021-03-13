// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllocatingBufferResizer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Memory
{
    using System;

    /// <summary>
    /// Implementation of <see cref="IBufferResizer{TItem}" /> which always allocates an array if necessary.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <seealso cref="IBufferResizer{TItem}" />
    public sealed class AllocatingBufferResizer<TItem> : IBufferResizer<TItem>
    {
        internal const int MaxDoublingLimit = 5000;

        /// <summary>
        /// Resizes the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="minimumCapacity">The minimum capacity.</param>
        /// <returns>A new resized array.</returns>
        public TItem[] Resize(TItem[]? source, int minimumCapacity)
        {
            if (source == null)
            {
                source = new TItem[minimumCapacity];
                return source;
            }

            if (minimumCapacity < MaxDoublingLimit)
            {
                minimumCapacity <<= 1;
            }

            if (source.Length < minimumCapacity)
            {
                Array.Resize(ref source, minimumCapacity);
            }

            return source;
        }
    }
}