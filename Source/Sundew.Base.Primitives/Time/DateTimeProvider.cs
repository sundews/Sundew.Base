// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeProvider.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives.Time;

using System;

/// <summary>
/// Provides the current date and time.
/// </summary>
/// <seealso cref="IDateTime" />
public class DateTimeProvider : IDateTime
{
    /// <summary>
    /// Gets the UTC now.
    /// </summary>
    /// <value>
    /// The UTC now.
    /// </value>
    public DateTime UtcNow => DateTime.UtcNow;

    /// <summary>
    /// Gets the local now.
    /// </summary>
    /// <value>
    /// The now.
    /// </value>
    public DateTime LocalNow => DateTime.Now;

    /// <summary>
    /// Gets the offset UTC now.
    /// </summary>
    /// <value>
    /// The UTC now.
    /// </value>
    public DateTimeOffset UtcNowOffset => DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets the local now offset.
    /// </summary>
    /// <value>
    /// The now.
    /// </value>
    public DateTimeOffset LocalNowOffset => DateTimeOffset.Now;

    /// <summary>
    /// Gets the time.
    /// </summary>
    /// <param name="useUtc">if set to <c>true</c> [use UTC].</param>
    /// <returns>
    /// The <see cref="System.DateTime" /> now.
    /// </returns>
    public DateTime GetNow(bool useUtc)
    {
        if (useUtc)
        {
            return this.UtcNow;
        }

        return this.LocalNow;
    }

    /// <summary>
    /// Gets the time offset.
    /// </summary>
    /// <param name="useUtc">if set to <c>true</c> [use UTC].</param>
    /// <returns>
    /// The <see cref="DateTimeProvider" /> now.
    /// </returns>
    public DateTimeOffset GetNowOffset(bool useUtc)
    {
        if (useUtc)
        {
            return this.UtcNowOffset;
        }

        return this.LocalNowOffset;
    }
}