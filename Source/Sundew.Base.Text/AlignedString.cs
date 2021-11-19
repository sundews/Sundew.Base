// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AlignedString.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text;

using System;
using System.Globalization;

/// <summary>
/// Contains format methods for aligning strings.
/// </summary>
public static class AlignedString
{
    /// <summary>
    /// Formats the align.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="arg0">The arg0.</param>
    /// <returns>the formatted string.</returns>
    public static string FormatInvariant(string format, object? arg0)
    {
        return string.Format(new AlignAndLimitFormatProvider(CultureInfo.InvariantCulture), format, arg0);
    }

    /// <summary>
    /// Formats the align.
    /// </summary>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="format">The format.</param>
    /// <param name="arg0">The arg0.</param>
    /// <returns>the formatted string.</returns>
    public static string Format(IFormatProvider formatProvider, string format, object? arg0)
    {
        return string.Format(new AlignAndLimitFormatProvider(formatProvider), format, arg0);
    }

    /// <summary>
    /// Formats the align.
    /// </summary>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="format">The format.</param>
    /// <param name="arg0">The arg0.</param>
    /// <param name="arg1">The arg1.</param>
    /// <returns>the formatted string.</returns>
    public static string Format(IFormatProvider formatProvider, string format, object? arg0, object? arg1)
    {
        return string.Format(new AlignAndLimitFormatProvider(formatProvider), format, arg0, arg1);
    }

    /// <summary>
    /// Formats the align.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="arg0">The arg0.</param>
    /// <param name="arg1">The arg1.</param>
    /// <returns>the formatted string.</returns>
    public static string FormatInvariant(string format, object? arg0, object? arg1)
    {
        return string.Format(new AlignAndLimitFormatProvider(CultureInfo.InvariantCulture), format, arg0, arg1);
    }

    /// <summary>
    /// Formats the align.
    /// </summary>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="format">The format.</param>
    /// <param name="arg0">The arg0.</param>
    /// <param name="arg1">The arg1.</param>
    /// <param name="arg2">The arg2.</param>
    /// <returns>The formatted string.</returns>
    public static string Format(IFormatProvider formatProvider, string format, object? arg0, object? arg1, object? arg2)
    {
        return string.Format(new AlignAndLimitFormatProvider(formatProvider), format, arg0, arg1, arg2);
    }

    /// <summary>
    /// Formats the align.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="arg0">The arg0.</param>
    /// <param name="arg1">The arg1.</param>
    /// <param name="arg2">The arg2.</param>
    /// <returns>
    /// the formatted string.
    /// </returns>
    public static string FormatInvariant(string format, object? arg0, object? arg1, object? arg2)
    {
        return string.Format(new AlignAndLimitFormatProvider(CultureInfo.InvariantCulture), format, arg0, arg1, arg2);
    }

    /// <summary>
    /// Formats the align.
    /// </summary>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="format">The format.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>the formatted string.</returns>
    public static string Format(IFormatProvider formatProvider, string format, params object?[] arguments)
    {
        return string.Format(new AlignAndLimitFormatProvider(formatProvider), format, arguments);
    }

    /// <summary>
    /// Formats the align.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>the formatted string.</returns>
    public static string Format(string format, params object?[] arguments)
    {
        return string.Format(new AlignAndLimitFormatProvider(CultureInfo.CurrentCulture), format, arguments);
    }

    /// <summary>
    /// Formats the align.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>
    /// the formatted string.
    /// </returns>
    public static string FormatInvariant(string format, params object?[] arguments)
    {
        return string.Format(new AlignAndLimitFormatProvider(CultureInfo.InvariantCulture), format, arguments);
    }
}