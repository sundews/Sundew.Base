// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CharExtensions.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Text;

    /// <summary>
    /// Easy to use methods for <see cref="char"/>s.
    /// </summary>
    public static class CharExtensions
    {
        /// <summary>
        /// Repeats the specified count.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="count">The count.</param>
        /// <returns>The string with repeated characters.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Repeat(this char character, int count)
        {
            if (count <= 0)
            {
                return string.Empty;
            }

            return new string(character, count);
        }
    }
}