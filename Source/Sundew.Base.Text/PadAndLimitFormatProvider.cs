// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PadAndLimitFormatProvider.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Format provider for padding and limiting text.
    /// </summary>
    /// <seealso cref="System.IFormatProvider" />
    /// <seealso cref="System.ICustomFormatter" />
    public class PadAndLimitFormatProvider : IFormatProvider, ICustomFormatter
    {
        private readonly IFormatProvider formatProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="PadAndLimitFormatProvider"/> class.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        public PadAndLimitFormatProvider(IFormatProvider formatProvider)
        {
            this.formatProvider = formatProvider;
        }

        /// <summary>
        /// Converts the value of a specified object to an equivalent string representation using specified format and culture-specific formatting information.
        /// </summary>
        /// <param name="format">A format string containing formatting specifications.</param>
        /// <param name="arg">An object to format.</param>
        /// <param name="formatProvider">An object that supplies format information about the current instance.</param>
        /// <returns>
        /// The string representation of the value of <paramref name="arg" />, formatted as specified by <paramref name="format" /> and <paramref name="formatProvider" />.
        /// </returns>
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            var formats = format.Split('|');
            return formats.Length switch
            {
                0 => string.Format(this.formatProvider, format, arg),
                1 => string.Format(this.formatProvider, format, arg),
                4 => this.FormatWithAlignment(arg, formats[0], formats[1][0], formats[2], formats[3], formatProvider),
                _ => throw new ArgumentOutOfRangeException($"The format: {format} was invalid", format),
            };
        }

        /// <summary>
        /// Returns an object that provides formatting services for the specified type.
        /// </summary>
        /// <param name="formatType">An object that specifies the type of format object to return.</param>
        /// <returns>
        /// An instance of the object specified by <paramref name="formatType" />, if the <see cref="T:System.IFormatProvider" /> implementation can supply that type of object; otherwise, <see langword="null" />.
        /// </returns>
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
            {
                return this;
            }

            return default!;
        }

        private static LimitSide GetLimitSide(string limitSide)
        {
            return limitSide switch
            {
                "L" => LimitSide.Left,
                "R" => LimitSide.Right,
                _ => throw new ArgumentOutOfRangeException(nameof(limitSide), limitSide, null),
            };
        }

        private static PadSide GetPadSide(string padSide)
        {
            return padSide switch
            {
                "BL" => PadSide.BothLeft,
                "BR" => PadSide.BothRight,
                "L" => PadSide.Left,
                "R" => PadSide.Right,
                _ => throw new ArgumentOutOfRangeException(nameof(padSide), padSide, null),
            };
        }

        private string FormatWithAlignment(object argument, string lengthText, char paddingCharacter, string padSideText, string limitSideText, IFormatProvider provider)
        {
            var length =
#if NETSTANDARD2_1
                int.Parse(lengthText.AsSpan().Slice(1), provider: CultureInfo.InvariantCulture);
#else
                int.Parse(lengthText.Substring(1), provider: CultureInfo.InvariantCulture);
#endif
            return string.Format(this.formatProvider, @"{0}", argument).LimitAndPad(length, paddingCharacter, GetPadSide(padSideText), GetLimitSide(limitSideText));
        }
    }
}