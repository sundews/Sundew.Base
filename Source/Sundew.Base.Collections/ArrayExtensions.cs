// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrayExtensions.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections
{
    using System;

    /// <summary>
    /// Extends arrays with easy to use methods.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Gets the segment.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="array">The array.</param>
        /// <returns>An <see cref="ArraySegment{TItem}"/>.</returns>
        public static ArraySegment<TItem> GetSegment<TItem>(this TItem[] array)
        {
            return new(array, 0, array.Length);
        }

        /// <summary>
        /// Gets the segment.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>An <see cref="ArraySegment{TItem}"/>.</returns>
        public static ArraySegment<TItem> GetSegment<TItem>(this TItem[] array, int offset)
        {
            return new(array, offset, array.Length - offset);
        }

        /// <summary>
        /// Gets the segment.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns>An <see cref="ArraySegment{TItem}"/>.</returns>
        public static ArraySegment<TItem> GetSegment<TItem>(this TItem[] array, int offset, int count)
        {
            return new(array, offset, count);
        }

        /// <summary>
        /// Copies the source array segment to the specified target array.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="sourceArray">The source array.</param>
        /// <param name="sourceIndex">Index of the source.</param>
        /// <param name="targetArray">The target array.</param>
        /// <param name="targetIndex">Index of the target.</param>
        /// <param name="count">The count.</param>
        public static void CopyTo<TItem>(this TItem[] sourceArray, int sourceIndex, TItem[] targetArray, int targetIndex, int count)
        {
            Array.Copy(sourceArray, sourceIndex, targetArray, targetIndex, count);
        }

        /// <summary>
        /// Copies the source array segment to the specified target array.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="sourceArray">The source array.</param>
        /// <param name="targetArray">The target array.</param>
        public static void CopyTo<TItem>(this TItem[] sourceArray, TItem[] targetArray)
        {
            Array.Copy(sourceArray, targetArray, sourceArray.Length);
        }
    }
}