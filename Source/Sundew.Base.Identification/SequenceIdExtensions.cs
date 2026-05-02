// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SequenceIdExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

using System.Runtime.CompilerServices;
using System.Threading;

/// <summary>
/// Extends <see cref="ISequenceId{TId}"/> with easy to use methods.
/// </summary>
public static class SequenceIdExtensions
{
    extension<TId>(TId id)
        where TId : ISequenceId<TId>
    {
        /// <summary>
        /// Creates the next id.
        /// </summary>
        /// <returns>The next id.</returns>
        public static TId Next()
        {
            return TId.Create(unchecked((uint)Interlocked.Increment(ref Unsafe.As<uint, int>(ref SequenceId<TId>.CurrentId))));
        }

        /// <summary>
        /// Check whether this instance is newer than the other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c>, when this instance is newer.</returns>
        public bool IsNewer(TId other)
        {
#pragma warning disable SA1101
            return (int)(id.Number - other.Number) > 0;
#pragma warning restore SA1101
        }
    }
}