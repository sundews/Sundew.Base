// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDateTime.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Time
{
    using System;

    /// <summary>
    /// Interface for getting the current date and time.
    /// </summary>
    public interface IDateTime
    {
        /// <summary>
        /// Gets the UTC now.
        /// </summary>
        /// <value>
        /// The UTC now.
        /// </value>
        DateTime UtcTime { get; }

        /// <summary>
        /// Gets the now.
        /// </summary>
        /// <value>
        /// The now.
        /// </value>
        DateTime LocalTime { get; }

        /// <summary>
        /// Gets the offset UTC now.
        /// </summary>
        /// <value>
        /// The UTC now.
        /// </value>
        DateTimeOffset UtcTimeOffset { get; }

        /// <summary>
        /// Gets the offset now.
        /// </summary>
        /// <value>
        /// The now.
        /// </value>
        DateTimeOffset LocalTimeOffset { get; }

        /// <summary>
        /// Gets time in either local time or UTC.
        /// </summary>
        /// <param name="useUtc">if set to <c>true</c> [use UTC].</param>
        /// <returns>The <see cref="DateTimeProvider"/> now.</returns>
        DateTime GetTime(bool useUtc);

        /// <summary>
        /// Gets time offset in either local time or UTC.
        /// </summary>
        /// <param name="useUtc">if set to <c>true</c> [use UTC].</param>
        /// <returns>The <see cref="DateTimeProvider"/> now.</returns>
        DateTimeOffset GetTimeOffset(bool useUtc);
    }
}