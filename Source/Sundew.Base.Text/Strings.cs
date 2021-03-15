// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Strings.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text
{
    using System;
    using System.Globalization;

    /// <summary>Contains reusable strings.</summary>
    public static class Strings
    {
        /// <summary>A empty string.</summary>
        public const string Empty = "";

        /// <summary>
        /// Formats the align.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <returns>the formatted string.</returns>
        public static string FormatAlign(IFormatProvider formatProvider, string format, object arg0)
        {
            return string.Format(new PadAndLimitFormatProvider(formatProvider), format, arg0);
        }

        /// <summary>
        /// Formats the align.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        /// <returns>the formatted string.</returns>
        public static string FormatAlign(IFormatProvider formatProvider, string format, object arg0, object arg1)
        {
            return string.Format(new PadAndLimitFormatProvider(formatProvider), format, arg0, arg1);
        }

        /// <summary>
        /// Formats the align.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        /// <returns>the formatted string.</returns>
        public static string FormatAlign(IFormatProvider formatProvider, string format, object arg0, object arg1, object arg2)
        {
            return string.Format(new PadAndLimitFormatProvider(formatProvider), format, arg0, arg1, arg2);
        }

        /// <summary>
        /// Formats the align.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>the formatted string.</returns>
        public static string FormatAlign(IFormatProvider formatProvider, string format, params object[] arguments)
        {
            return string.Format(new PadAndLimitFormatProvider(formatProvider), format, arguments);
        }

        /// <summary>
        /// Formats the align.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>the formatted string.</returns>
        public static string FormatAlign(string format, params object[] arguments)
        {
            return string.Format(new PadAndLimitFormatProvider(CultureInfo.CurrentCulture), format, arguments);
        }
    }
}