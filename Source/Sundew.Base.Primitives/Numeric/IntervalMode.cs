// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntervalMode.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Numeric
{
    /// <summary>
    /// Enumeration for specifying comparison of intervals.
    /// </summary>
    public enum IntervalMode
    {
        /// <summary>
        /// The inclusive, includes the interval values.
        /// </summary>
        Inclusive,

        /// <summary>
        /// The exclusive, excludes the interval values.
        /// </summary>
        Exclusive,

        /// <summary>
        /// The min exclusive, excludes the min value, but includes the max.
        /// </summary>
        MinExclusive,

        /// <summary>
        /// The max exclusive, includes the min value, but excludes the max.
        /// </summary>
        MaxExclusive,
    }
}