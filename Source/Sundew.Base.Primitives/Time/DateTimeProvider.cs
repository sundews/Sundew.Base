// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeProvider.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Time
{
    using System;

    /// <summary>
    /// Provides the current date and time.
    /// </summary>
    /// <seealso cref="Sundew.Base.Time.IDateTime" />
    public class DateTimeProvider : IDateTime
    {
        /// <summary>
        /// Gets the UTC time.
        /// </summary>
        /// <value>
        /// The UTC now.
        /// </value>
        public DateTime UtcTime => DateTime.UtcNow;

        /// <summary>
        /// Gets the local time.
        /// </summary>
        /// <value>
        /// The now.
        /// </value>
        public DateTime LocalTime => DateTime.Now;

        /// <summary>
        /// Gets the offset UTC now.
        /// </summary>
        /// <value>
        /// The UTC now.
        /// </value>
        public DateTimeOffset UtcTimeOffset => DateTimeOffset.UtcNow;

        /// <summary>
        /// Gets the local time offset.
        /// </summary>
        /// <value>
        /// The now.
        /// </value>
        public DateTimeOffset LocalTimeOffset => DateTimeOffset.Now;

        /// <summary>
        /// Gets the time.
        /// </summary>
        /// <param name="useUtc">if set to <c>true</c> [use UTC].</param>
        /// <returns>
        /// The <see cref="System.DateTime" /> now.
        /// </returns>
        public DateTime GetTime(bool useUtc)
        {
            if (useUtc)
            {
                return this.UtcTime;
            }

            return this.LocalTime;
        }

        /// <summary>
        /// Gets the time offset.
        /// </summary>
        /// <param name="useUtc">if set to <c>true</c> [use UTC].</param>
        /// <returns>
        /// The <see cref="Sundew.Base.Time.DateTimeProvider" /> now.
        /// </returns>
        public DateTimeOffset GetTimeOffset(bool useUtc)
        {
            if (useUtc)
            {
                return this.UtcTimeOffset;
            }

            return this.LocalTimeOffset;
        }
    }
}