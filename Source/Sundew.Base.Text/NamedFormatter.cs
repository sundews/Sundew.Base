// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamedFormatter.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Sundew.Base.Collections;

    /// <summary>
    /// A format provider that supports names and indices.
    /// </summary>
    public sealed class NamedFormatter
    {
        private const string IndexGroupName = "Index";
        private static readonly Regex FormatRegex = new(@"(?:(?>(?<CurlyOpen>\{\{)|\{)(?<Index>[^\:\}]+)(?:\:[\+\-\.\,\:\w\d]*)?(?>(?<CurlyClosed-CurlyOpen>\}\})|\})(?(CurlyOpen)(?!))(?(CurlyClosed)(?<-Index>)(?<-CurlyClosed>)))+");

        private readonly IFormatProvider formatProvider;
        private readonly NamesIndicesMap names;

        /// <summary>Initializes a new instance of the <see cref="NamedFormatter" /> class.</summary>
        /// <param name="names">The names.</param>
        /// <param name="nameStringComparison">The name string comparison.</param>
        public NamedFormatter(IEnumerable<string> names, StringComparison nameStringComparison)
            : this(names, nameStringComparison, CultureInfo.CurrentCulture)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedFormatter" /> class.
        /// </summary>
        /// <param name="names">The names.</param>
        /// <param name="nameStringComparison">The name string comparison.</param>
        /// <param name="formatProvider">The format provider.</param>
        public NamedFormatter(IEnumerable<string> names, StringComparison nameStringComparison, IFormatProvider formatProvider)
        {
            this.names = new NamesIndicesMap(names, nameStringComparison);
            this.formatProvider = formatProvider;
        }

        /// <summary>
        /// Formats the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>The formatted string.</returns>
        public string Format(string format, object[] arguments)
        {
            format = FormatRegex.Replace(
                format,
                match =>
            {
                var indexGroup = match.Groups[IndexGroupName];
                if (string.IsNullOrEmpty(indexGroup.Value))
                {
                    return match.Value;
                }

                if (int.TryParse(indexGroup.Value, out int index))
                {
                    return match.Value;
                }

                if (this.names.TryGet(indexGroup.Value, out index))
                {
                    return $"{match.Value.Substring(0, indexGroup.Index - match.Index)}{index}{match.Value.Substring(indexGroup.Index + indexGroup.Length - match.Index)}";
                }

                throw new FormatException($"The string was in an invalid format: {format}");
            });

            return string.Format(format, arguments);
        }

        private class NamesIndicesMap
        {
            private readonly IReadOnlyList<string> names;
            private readonly StringComparer stringComparer;

            public NamesIndicesMap(IEnumerable<string> names, StringComparison stringComparison)
            {
                this.names = names as IReadOnlyList<string> ?? names.ToList();
#if NETSTANDARD1_3
                StringComparer FromComparison(StringComparison stringComparison)
                {
                    return stringComparison switch
                    {
                        StringComparison.OrdinalIgnoreCase => StringComparer.OrdinalIgnoreCase,
                        StringComparison.Ordinal => StringComparer.Ordinal,
                        StringComparison.CurrentCulture => StringComparer.CurrentCulture,
                        StringComparison.CurrentCultureIgnoreCase => StringComparer.CurrentCultureIgnoreCase,
                        _ => throw new ArgumentException($"The string comparison: {stringComparison} was not known.", nameof(stringComparison)),
                    };
                }

                this.stringComparer = FromComparison(stringComparison);
#else
                this.stringComparer = StringComparer.FromComparison(stringComparison);
#endif
            }

            public int Count => this.names.Count;

            public bool TryGet(string name, out int index)
            {
                index = this.names.IndexOf(x => this.stringComparer.Equals(x, name));
                return index != -1;
            }

            public bool TryGet(int index, [NotNullWhen(true)] out string? name)
            {
                if (this.Count > index)
                {
                    name = null;
                    return false;
                }

                name = this.names[index];
                return true;
            }
        }
    }
}
