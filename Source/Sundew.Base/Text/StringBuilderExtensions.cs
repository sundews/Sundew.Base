// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringBuilderExtensions.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text
{
    using System;
    using System.Text;

    /// <summary>
    /// Extends the string with extension methods.
    /// </summary>
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Converts to string at the end.
        /// </summary>
        /// <param name="stringBuilder">The string builder.</param>
        /// <param name="fromEndLength">The from end length.</param>
        /// <returns>
        /// A string.
        /// </returns>
        public static string ToStringFromEnd(this StringBuilder stringBuilder, int fromEndLength)
        {
            return stringBuilder.ToString(0, stringBuilder.Length - fromEndLength);
        }

        /// <summary>
        /// Converts to string at the end.
        /// </summary>
        /// <param name="stringBuilder">The string builder.</param>
        /// <param name="index">The index.</param>
        /// <param name="fromEndLength">The from end length.</param>
        /// <returns>A string.</returns>
        public static string ToStringFromEnd(this StringBuilder stringBuilder, int index, int fromEndLength)
        {
            return stringBuilder.ToString(index, stringBuilder.Length - index - fromEndLength);
        }

#if NETSTANDARD2_1
        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <param name="stringBuilder">The string builder.</param>
        /// <param name="range">The range.</param>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public static string ToString(this StringBuilder stringBuilder, Range range)
        {
            var (offset, length) = range.GetOffsetAndLength(stringBuilder.Length);
            return stringBuilder.ToString(offset, length);
        }
#endif
    }
}