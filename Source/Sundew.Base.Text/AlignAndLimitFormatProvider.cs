// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AlignAndLimitFormatProvider.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text
{
    using System;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Format provider for padding and limiting text.
    /// </summary>
    /// <seealso cref="System.IFormatProvider" />
    /// <seealso cref="System.ICustomFormatter" />
    public class AlignAndLimitFormatProvider : IFormatProvider, ICustomFormatter
    {
        private const string AlignCenter = "=";
        private const string AlignCenterLeft = "=<";
        private const string AlignCenterRight = "=>";
        private const string Left = "<";
        private const string Right = ">";
        private const string AlignAndLimit = nameof(AlignAndLimit);
        private const string Length = nameof(Length);
        private const string PaddingCharacter = nameof(PaddingCharacter);
        private const string Alignment = nameof(Alignment);
        private const string Limit = nameof(Limit);
        private const string LimitString = nameof(LimitString);
        private const string LimitMode = nameof(LimitMode);
        private const string ValueFormat = nameof(ValueFormat);
        private const string Dots = "D";
        private const string Epsilon = "E";
        private const string FormatPlaceholder = "{0}";

        private static readonly Regex AlignmentRegex =
            new(@"(?<AlignAndLimit>(?<Length>\d+),(?<PaddingCharacter>.)(?<Alignment>[=\<\>](?:\<|\>)?)(?<Limit>\<|\>)(?:\|(?<LimitString>\w+)|(?<LimitMode>\w))?)?(?:\:(?<ValueFormat>\w+))?", RegexOptions.Compiled);

        private readonly IFormatProvider formatProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlignAndLimitFormatProvider"/> class.
        /// </summary>
        public AlignAndLimitFormatProvider()
            : this(CultureInfo.CurrentCulture)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlignAndLimitFormatProvider" /> class.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        public AlignAndLimitFormatProvider(IFormatProvider formatProvider)
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
            var match = AlignmentRegex.Match(format);
            if (!match.Success)
            {
                throw new FormatException(
                    $"The format: {format} was invalid{Environment.NewLine}Expected format: <Length>,<PaddingCharacter><Alignment (=|<|>[<|>])><Limit (<|>)>[<LimitString (|<string|D|E)>][:<ValueFormat>]");
            }

            if (!match.Groups[AlignAndLimit].Success)
            {
                return string.Format(this.formatProvider, match.Groups[ValueFormat].Value, arg);
            }

            var length = int.Parse(match.Groups[Length].Value, CultureInfo.InvariantCulture);
            var paddingCharacter = match.Groups[PaddingCharacter].Value[0];
            var alignment = GetAlignment(match.Groups[Alignment].Value);
            var limit = GetLimit(match.Groups[Limit].Value, match.Groups[LimitMode], match.Groups[LimitString]);
            var stringBuilder = new StringBuilder();
            string value = GetValue(arg, match.Groups[ValueFormat], this.formatProvider);

            var isRightToLeft = this.formatProvider is CultureInfo cultureInfo && cultureInfo.TextInfo.IsRightToLeft;
            return stringBuilder.Append(value, length, paddingCharacter, alignment, limit, isRightToLeft).ToString();
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

        private static string GetValue(object? value, Group valueFormatGroup, IFormatProvider formatProvider)
        {
            value ??= string.Empty;

            if (valueFormatGroup.Success)
            {
                return value is IFormattable formattable ? formattable.ToString(valueFormatGroup.Value, formatProvider) : string.Format(formatProvider, $"{{0:{valueFormatGroup.Value}}}", value);
            }

            return string.Format(formatProvider, FormatPlaceholder, value);
        }

        private static Limit GetLimit(string limit, Group limitModeGroup, Group limitStringGroup)
        {
            var isLeft = limit switch
            {
                Right => false,
                Left => true,
                _ => throw new FormatException($"Invalid limit: {limit}.{Environment.NewLine}Supported values: >, <"),
            };

            if (limitStringGroup.Success)
            {
                return Text.Limit.With(isLeft, limitStringGroup.Value);
            }

            if (limitModeGroup.Success)
            {
                return limitModeGroup.Value switch
                {
                    Dots => Text.Limit.WithDots(isLeft),
                    Epsilon => Text.Limit.WithEpsilon(isLeft),
                    _ => throw new FormatException(
                        $"Invalid limit mode: {limitModeGroup.Value}.{Environment.NewLine}Supported values: E=>Epsilon, D=>Dots"),
                };
            }

            return Text.Limit.With(isLeft);
        }

        private static Alignment GetAlignment(string value)
        {
            return value switch
            {
                AlignCenter => Text.Alignment.CenterLeft,
                AlignCenterLeft => Text.Alignment.CenterLeft,
                AlignCenterRight => Text.Alignment.CenterRight,
                Right => Text.Alignment.Right,
                Left => Text.Alignment.Left,
                _ => throw new FormatException($"Invalid alignment: {value}.{Environment.NewLine}Supported values: =>, =<, >, <"),
            };
        }
    }
}