// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EqualityHelper.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Equality
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Contains helper methods for implementing equality and get hashcodes.
    /// </summary>
    public static class EqualityHelper
    {
        private const int OrderingHashPrime = 397;

        /// <summary>
        /// Check whether the specified TObjects are equal.
        /// </summary>
        /// <typeparam name="TObject">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c>, if the two TObjects where equal.</returns>
        public static bool Equals<TObject>(TObject value, object? other)
            where TObject : IEquatable<TObject>
        {
            return other is TObject o && value.Equals(o);
        }

        /// <summary>
        /// Check whether the specified TObjects are equal.
        /// </summary>
        /// <typeparam name="TObject">The type of the value.</typeparam>
        /// <param name="lhs">The lhs.</param>
        /// <param name="rhs">The rhs.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns><c>true</c>, if the two TObjects where equal.</returns>
        public static bool Equals<TObject>(TObject? lhs, TObject? rhs, Predicate<TObject> predicate)
            where TObject : class
        {
            if (ReferenceEquals(lhs, rhs))
            {
                return true;
            }

            if (rhs is null)
            {
                return false;
            }

            return predicate(rhs);
        }

        /// <summary>
        /// Gets the items hash code.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="span">The span.</param>
        /// <returns>The span hashcode.</returns>
        public static int GetItemsHashCode<TItem>(this ReadOnlySpan<TItem> span)
            where TItem : struct
        {
            var hashcode = 0;
            for (int i = 0; i < span.Length; i++)
            {
                hashcode = CombineHashCodeUnordered(hashcode, span[i].GetHashCode());
            }

            return hashcode;
        }

        /// <summary>
        /// Gets the items hash code.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="span">The span.</param>
        /// <returns>The span hashcode.</returns>
        public static int GetItemsHashCode<TItem>(this Span<TItem> span)
            where TItem : struct
        {
            var hashcode = 0;
            for (int i = 0; i < span.Length; i++)
            {
                hashcode = CombineHashCodeUnordered(hashcode, span[i].GetHashCode());
            }

            return hashcode;
        }

        /// <summary>
        /// Gets the hash code from the items.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>
        /// The hash code for the items.
        /// </returns>
        public static int GetItemsHashCode(this IEnumerable<int> enumerable)
        {
            int hashCode = 0;
            foreach (var item in enumerable)
            {
                hashCode = CombineHashCode(hashCode, item);
            }

            return hashCode;
        }

        /// <summary>
        /// Gets the unordered items hash code.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="span">The span.</param>
        /// <returns>The span hashcode.</returns>
        public static int GetUnorderedItemsHashCode<TItem>(this Span<TItem> span)
            where TItem : struct
        {
            var hashcode = 0;
            for (int i = 0; i < span.Length; i++)
            {
                hashcode = CombineHashCodeUnordered(hashcode, span[i].GetHashCode());
            }

            return hashcode;
        }

        /// <summary>
        /// Gets the unordered items hash code.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="span">The span.</param>
        /// <returns>The span hashcode.</returns>
        public static int GetUnorderedItemsHashCode<TItem>(this ReadOnlySpan<TItem> span)
            where TItem : struct
        {
            var hashcode = 0;
            for (int i = 0; i < span.Length; i++)
            {
                hashcode = CombineHashCodeUnordered(hashcode, span[i].GetHashCode());
            }

            return hashcode;
        }

        /// <summary>
        /// Gets the hash code from the items.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>
        /// The hash code for the items.
        /// </returns>
        public static int GetUnorderedItemsHashCode(this IEnumerable<int> enumerable)
        {
            int hashCode = 0;
            foreach (var item in enumerable)
            {
                hashCode = CombineHashCodeUnordered(hashCode, item);
            }

            return hashCode;
        }

        /// <summary>
        /// Returns a hash code for specified value.
        /// </summary>
        /// <typeparam name="TObject">The type of the value.</typeparam>
        /// <param name="value">The @value.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCodeOrDefault<TObject>(this TObject? value)
            where TObject : class
        {
            return value?.GetHashCode() ?? 0;
        }

        /// <summary>
        /// Gets the hash code for specified objects.
        /// </summary>
        /// <param name="hashcodes">The hashcodes.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCode(params int[] hashcodes)
        {
            var hashCode = 0;
            for (var index = 0; index < hashcodes.Length; index++)
            {
                hashCode = CombineHashCode(hashCode, hashcodes[index]);
            }

            return hashCode;
        }

        /// <summary>
        /// Gets the unordered hash code for specified objects.
        /// </summary>
        /// <param name="hashcodes">The hashcodes.</param>
        /// <returns>
        /// The hashcode.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetUnorderedHashCode(params int[] hashcodes)
        {
            var hashCode = 0;
            for (var index = 0; index < hashcodes.Length; index++)
            {
                hashCode = CombineHashCodeUnordered(hashCode, hashcodes[index]);
            }

            return hashCode;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CombineHashCode(int hashCode1, int hashcode2)
        {
            unchecked
            {
                return (hashCode1 * OrderingHashPrime) ^ hashcode2;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CombineHashCodeUnordered(int hashCode1, int hashCode2)
        {
            return hashCode1 ^ hashCode2;
        }
    }
}