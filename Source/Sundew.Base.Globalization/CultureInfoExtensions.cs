// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CultureInfoExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Globalization;

using System;
using System.Collections.Generic;
using System.Globalization;

/// <summary>
/// Provides extension methods for the CultureInfo class, enhancing its functionality.
/// </summary>
/// <remarks>This static class includes methods that facilitate working with culture-specific information,
/// particularly for ISO 8601 date and time formats.</remarks>
public static class CultureInfoExtensions
{
    private static readonly Lazy<CultureInfo> Iso8601Culture = new(() =>
    {
        var iso8601Culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        iso8601Culture.DateTimeFormat.ShortTimePattern = "HH:mm:ss";
        iso8601Culture.DateTimeFormat.LongTimePattern = "HH:mm:ss";
        iso8601Culture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
        iso8601Culture.DateTimeFormat.LongDatePattern = "yyyy-MM-dd";
        return iso8601Culture;
    });

#pragma warning disable SA1201
    /// <summary>
    /// Extends CultureInfo with Iso8601 culture.
    /// </summary>
    extension(CultureInfo)
    {
        /// <summary>
        /// Gets the Iso8601 culture.
        /// </summary>
        public static CultureInfo Iso8601Culture => Iso8601Culture.Value;
    }
#pragma warning restore SA1201
}