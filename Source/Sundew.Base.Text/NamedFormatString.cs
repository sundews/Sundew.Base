// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamedFormatString.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Sundew.Base.Collections;
using Sundew.Base.Memory;

/// <summary>
/// A formatter that supports names and indices.
/// </summary>
public readonly struct NamedFormatString
{
    private static readonly Regex FormatRegex = new(
        @"(?:(?<CurlyOpen>(?<=(?:^|[^\{])\{))(?<Replace>[^\,\{\:\}]+)(?>(?<CurlyClosed-CurlyOpen>(?=(?:(?:\,|\:)[^\}]*)?\}(?:[^\}]|$))))(?(CurlyOpen)(?!))(?(CurlyClosed)(?<-Replace>)(?<-CurlyClosed>)))+",
        RegexOptions.Compiled);

    private readonly string format;

    /// <summary>
    /// Initializes a new instance of the <see cref="NamedFormatString" /> struct.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="names">The names.</param>
    public NamedFormatString(string format, params string[] names)
        : this(format, (IReadOnlyList<string>)names)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NamedFormatString" /> struct.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="names">The names.</param>
    public NamedFormatString(string format, IReadOnlyList<string> names)
    {
        var (indexedFormat, formatNames, unknownNames) = ConvertToIndexedFormat(names, format);
        if (unknownNames.Count > 0)
        {
            throw new FormatException($"The string was in an invalid format: {format}");
        }

        this.format = indexedFormat;
        this.FormatNames = formatNames;
    }

    private NamedFormatString(string format, IReadOnlyCollection<(string Name, int Index)> formatNames)
    {
        this.format = format;
        this.FormatNames = formatNames;
    }

    /// <summary>
    /// Gets a value indicating whether the format string is valid.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
    /// </value>
    public bool IsValid => this.format != null;

    /// <summary>
    /// Gets the indices.
    /// </summary>
    /// <value>
    /// The indices.
    /// </value>
    public IReadOnlyCollection<(string Name, int Index)> FormatNames { get; }

    /// <summary>
    /// Performs an implicit conversion from <see cref="Sundew.Base.Text.NamedFormatString"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="namedFormatString">The named format string.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    public static implicit operator string(in NamedFormatString namedFormatString)
    {
        return namedFormatString.format;
    }

    /// <summary>
    /// Tries the create.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="names">The names.</param>
    /// <param name="namedFormatString">The named format string.</param>
    /// <param name="unknownNames">The unknown names.</param>
    /// <returns>
    /// <c>true</c> if the NamedFormatString could be created, otherwise <c>false</c>.
    /// </returns>
    public static bool TryCreate(string format, IReadOnlyList<string> names, out NamedFormatString namedFormatString, out IReadOnlyList<string> unknownNames)
    {
        var (indexedFormat, formatNames, actualUnknownNames) = ConvertToIndexedFormat(names, format);
        unknownNames = actualUnknownNames;
        if (unknownNames.Count > 0)
        {
            namedFormatString = default;
            return false;
        }

        namedFormatString = new NamedFormatString(indexedFormat, formatNames);
        return true;
    }

    /// <summary>
    /// Formats the specified string based on the named string and additional values.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="namedValues">The named strings.</param>
    /// <param name="additionalValues">The additional values.</param>
    /// <returns>A <see cref="FormattedStringResult"/>.</returns>
    public static FormattedStringResult FormatInvariant(string format, NamedValues namedValues, params string?[] additionalValues)
    {
        return Format(CultureInfo.InvariantCulture, format, namedValues, additionalValues);
    }

    /// <summary>
    /// Formats the specified string based on the named string and additional values.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="namedValues">The named strings.</param>
    /// <param name="additionalValues">The additional values.</param>
    /// <returns>A <see cref="FormattedStringResult"/>.</returns>
    public static FormattedStringResult Format(string format, NamedValues namedValues, params string?[] additionalValues)
    {
        return Format(CultureInfo.CurrentCulture, format, namedValues, additionalValues);
    }

    /// <summary>
    /// Formats the specified string based on the named string and additional values.
    /// </summary>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="format">The format.</param>
    /// <param name="namedValues">The named strings.</param>
    /// <param name="additionalValues">The additional values.</param>
    /// <returns>A <see cref="FormattedStringResult"/>.</returns>
    public static FormattedStringResult Format(IFormatProvider formatProvider, string format, NamedValues namedValues, params string?[] additionalValues)
    {
        return Format(formatProvider, format, namedValues, new ReadOnlySpan<object?>(additionalValues));
    }

    /// <summary>
    /// Formats the specified string based on the named string and additional values.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="namedValues">The named strings.</param>
    /// <param name="additionalValues">The additional values.</param>
    /// <returns>A <see cref="FormattedStringResult"/>.</returns>
    public static FormattedStringResult FormatInvariant(string format, NamedValues namedValues, params object?[] additionalValues)
    {
        return Format(CultureInfo.InvariantCulture, format, namedValues, additionalValues);
    }

    /// <summary>
    /// Formats the specified string based on the named string and additional values.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="namedValues">The named strings.</param>
    /// <param name="additionalValues">The additional values.</param>
    /// <returns>A <see cref="FormattedStringResult"/>.</returns>
    public static FormattedStringResult Format(string format, NamedValues namedValues, params object?[] additionalValues)
    {
        return Format(CultureInfo.CurrentCulture, format, namedValues, additionalValues);
    }

    /// <summary>
    /// Formats the specified string based on the named string and additional values.
    /// </summary>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="format">The format.</param>
    /// <param name="namedValues">The named strings.</param>
    /// <param name="additionalValues">The additional values.</param>
    /// <returns>A <see cref="FormattedStringResult"/>.</returns>
    public static FormattedStringResult Format(IFormatProvider formatProvider, string format, NamedValues namedValues, params object?[] additionalValues)
    {
        return Format(formatProvider, format, namedValues, new ReadOnlySpan<object?>(additionalValues));
    }

    /// <summary>
    /// Formats the specified format.
    /// </summary>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>
    /// The formatted string.
    /// </returns>
    /// <exception cref="FormatException">$"The string was in an invalid format: {format}.</exception>
    public string Format(IFormatProvider? formatProvider, params object?[] arguments)
    {
        return string.Format(formatProvider, this.format, arguments);
    }

    /// <summary>
    /// Formats the invariant.
    /// </summary>
    /// <param name="arg0">The arg0.</param>
    /// <returns>
    /// The formatted string.
    /// </returns>
    public string FormatInvariant(object? arg0)
    {
        return string.Format(CultureInfo.InvariantCulture, this.format, arg0);
    }

    /// <summary>
    /// Formats the align.
    /// </summary>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="arg0">The arg0.</param>
    /// <returns>
    /// the formatted string.
    /// </returns>
    public string Format(IFormatProvider? formatProvider, object? arg0)
    {
        return string.Format(formatProvider, this.format, arg0);
    }

    /// <summary>
    /// Formats the align.
    /// </summary>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="arg0">The arg0.</param>
    /// <param name="arg1">The arg1.</param>
    /// <returns>
    /// the formatted string.
    /// </returns>
    public string Format(IFormatProvider? formatProvider, object? arg0, object? arg1)
    {
        return string.Format(formatProvider, this.format, arg0, arg1);
    }

    /// <summary>
    /// Formats the align.
    /// </summary>
    /// <param name="arg0">The arg0.</param>
    /// <param name="arg1">The arg1.</param>
    /// <returns>
    /// the formatted string.
    /// </returns>
    public string FormatInvariant(object? arg0, object? arg1)
    {
        return string.Format(CultureInfo.InvariantCulture, this.format, arg0, arg1);
    }

    /// <summary>
    /// Formats the align.
    /// </summary>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="arg0">The arg0.</param>
    /// <param name="arg1">The arg1.</param>
    /// <param name="arg2">The arg2.</param>
    /// <returns>The formatted string.</returns>
    public string Format(IFormatProvider? formatProvider, object? arg0, object? arg1, object? arg2)
    {
        return string.Format(formatProvider, this.format, arg0, arg1, arg2);
    }

    /// <summary>
    /// Formats the align.
    /// </summary>
    /// <param name="arg0">The arg0.</param>
    /// <param name="arg1">The arg1.</param>
    /// <param name="arg2">The arg2.</param>
    /// <returns>
    /// the formatted string.
    /// </returns>
    public string FormatInvariant(object? arg0, object? arg1, object? arg2)
    {
        return string.Format(CultureInfo.InvariantCulture, this.format, arg0, arg1, arg2);
    }

    /// <summary>
    /// Formats the align.
    /// </summary>
    /// <param name="arguments">The arguments.</param>
    /// <returns>
    /// the formatted string.
    /// </returns>
    public string Format(params object?[] arguments)
    {
        return string.Format(this.format, arguments);
    }

    /// <summary>
    /// Formats the align.
    /// </summary>
    /// <param name="arguments">The arguments.</param>
    /// <returns>
    /// the formatted string.
    /// </returns>
    public string FormatInvariant(params object?[] arguments)
    {
        return string.Format(CultureInfo.InvariantCulture, this.format, arguments);
    }

    /// <summary>
    /// Converts to indices format.
    /// </summary>
    /// <param name="arguments">The arguments.</param>
    /// <returns>
    /// A tuple with the indices based format and a list of null arguments.
    /// </returns>
    public NullArguments GetNullArguments(params object?[] arguments)
    {
        var nullArguments = new List<(string Name, int Index)>(this.FormatNames.Count);
        foreach (var pair in this.FormatNames.OrderBy(x => x.Index))
        {
            if (arguments[pair.Index] == null)
            {
                nullArguments.Add((pair.Name, pair.Index));
            }
        }

        return new NullArguments(nullArguments, arguments);
    }

    /// <summary>
    /// Formats the specified string based on the named string and additional values.
    /// </summary>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="format">The format.</param>
    /// <param name="namedValues">The named strings.</param>
    /// <param name="additionalValues">The additional values.</param>
    /// <returns>A <see cref="FormattedStringResult"/>.</returns>
    private static FormattedStringResult Format(IFormatProvider formatProvider, string format, NamedValues namedValues, ReadOnlySpan<object?> additionalValues)
    {
        var arguments = new Buffer<object?>(additionalValues.Length + namedValues.Pairs.Length);
        var names = new Buffer<string>(arguments.Capacity);
        arguments.Write(additionalValues);
        names.WriteRange(Enumerable.Repeat(string.Empty, additionalValues.Length));
        foreach (var namedString in namedValues.Pairs)
        {
            arguments.Write(namedString.Value);
            names.Write(namedString.Name);
        }

        var result = TryCreate(format, names.ToFinalArray(), out var namedFormatString, out var unknownNames);
        if (result)
        {
            var argumentArray = arguments.ToFinalArray();
            var nullArguments = namedFormatString.GetNullArguments(argumentArray);
            if (nullArguments.Count > 0)
            {
                return FormattedStringResult.ArgumentsContainedNullValues(nullArguments);
            }

            return FormattedStringResult.StringFormatted(namedFormatString.Format(formatProvider, argumentArray));
        }

        return FormattedStringResult.FormatContainedUnknownNames(unknownNames);
    }

    /// <summary>
    /// Converts to indices format.
    /// </summary>
    /// <param name="names">The names.</param>
    /// <param name="namedFormat">The named format.</param>
    /// <returns>A tuple of the indexed format and name-index pairs.</returns>
    private static (string Format, IReadOnlyCollection<(string Name, int Index)> Names, IReadOnlyList<string> UnknownNames) ConvertToIndexedFormat(IReadOnlyList<string> names, string namedFormat)
    {
        var unknownNames = new List<string>();
        var namesAndIndices = new HashSet<(string Name, int Index)>();
        var indexedFormat = FormatRegex.Replace(
            namedFormat,
            match =>
            {
                if (string.IsNullOrEmpty(match.Value))
                {
                    return match.Value;
                }

                if (TryGetIndex(names, match.Value, out var index))
                {
                    namesAndIndices.Add((match.Value, index));
                    return index.ToString();
                }

                if (int.TryParse(match.Value, out index) && TryGetName(names, index, out var name))
                {
                    namesAndIndices.Add((name, index));
                    return match.Value;
                }

                unknownNames.Add(match.Value);
                return match.Value;
            });

        return (indexedFormat, namesAndIndices, unknownNames);
    }

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <param name="names">The names.</param>
    /// <param name="index">The index.</param>
    /// <param name="name">The name.</param>
    /// <returns>
    /// A value indicating whether the name was found.
    /// </returns>
    private static bool TryGetName(IReadOnlyList<string> names, int index, [NotNullWhen(true)] out string? name)
    {
        if (index < names.Count)
        {
            name = names[index];
            return true;
        }

        name = null;
        return false;
    }

    /// <summary>
    /// Gets the index.
    /// </summary>
    /// <param name="names">The names.</param>
    /// <param name="name">The name.</param>
    /// <param name="index">The index.</param>
    /// <returns>
    /// A value indicating whether the index was found.
    /// </returns>
    private static bool TryGetIndex(IReadOnlyList<string> names, string name, out int index)
    {
        index = names.IndexOf(x => StringComparer.Ordinal.Equals(x, name));
        return index != -1;
    }

    /// <summary>
    /// A result that indicates whether a string format passed null arguments.
    /// </summary>
    public readonly struct NullArguments : IReadOnlyList<(string Name, int Index)>
    {
        private readonly IReadOnlyList<(string Name, int Index)> nullArguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="NullArguments" /> struct.
        /// </summary>
        /// <param name="nullArguments">The null arguments.</param>
        /// <param name="arguments">The arguments.</param>
        internal NullArguments(IReadOnlyList<(string Name, int Index)> nullArguments, object?[] arguments)
        {
            this.nullArguments = nullArguments;
            this.Arguments = arguments;
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count => this.nullArguments.Count;

        /// <summary>
        /// Gets a value indicating whether there are no null arguments.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid => this.Count == 0;

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        public object?[] Arguments { get; }

        /// <summary>
        /// Gets the name-index pair at the specified index.
        /// </summary>
        /// <value>
        /// The name-index pair.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns>A pair of name and index.</returns>
        public (string Name, int Index) this[int index] => this.nullArguments[index];

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>An enumerator for the null arguments there were null.</returns>
        public IEnumerator<(string Name, int Index)> GetEnumerator()
        {
            return this.nullArguments.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>An enumerator for the null arguments there were null.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}