// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamedFormatString.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
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
    private readonly IFormatProvider formatProvider;

    private NamedFormatString(string format, IReadOnlyCollection<(string Name, int Index)> formatNames, IFormatProvider formatProvider)
    {
        this.format = format;
        this.formatProvider = formatProvider;
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
    /// Initializes a new instance of the <see cref="NamedFormatString" /> struct.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="names">The names.</param>
    /// <returns>The new <see cref="NamedFormatString"/>.</returns>
    public static NamedFormatString Create(string format, IReadOnlyList<string> names)
    {
        if (!TryCreate(format, names, CultureInfo.CurrentCulture, out var namedFormattedString, out var unknownNames))
        {
            if (unknownNames.Count > 0)
            {
                throw new FormatException($"The string was in an invalid format: {format}");
            }
        }

        return namedFormattedString;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NamedFormatString" /> struct.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="names">The names.</param>
    /// <returns>The new <see cref="NamedFormatString"/>.</returns>
    public static NamedFormatString CreateInvariant(string format, IReadOnlyList<string> names)
    {
        if (!TryCreate(format, names, CultureInfo.InvariantCulture, out var namedFormattedString, out var unknownNames))
        {
            if (unknownNames.Count > 0)
            {
                throw new FormatException($"The string was in an invalid format: {format}");
            }
        }

        return namedFormattedString;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NamedFormatString" /> struct.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="names">The names.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns>The new <see cref="NamedFormatString"/>.</returns>
    public static NamedFormatString Create(string format, IReadOnlyList<string> names, IFormatProvider formatProvider)
    {
        if (!TryCreate(format, names, formatProvider, out var namedFormattedString, out var unknownNames))
        {
            if (unknownNames.Count > 0)
            {
                throw new FormatException($"The string was in an invalid format: {format}");
            }
        }

        return namedFormattedString;
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
    public static bool TryCreateInvariant(string format, IReadOnlyList<string> names, out NamedFormatString namedFormatString, out IReadOnlyList<string> unknownNames)
    {
        return TryCreate(format, names, CultureInfo.InvariantCulture, out namedFormatString, out unknownNames);
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
        return TryCreate(format, names, CultureInfo.CurrentCulture, out namedFormatString, out unknownNames);
    }

    /// <summary>
    /// Tries the create.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="names">The names.</param>
    /// <param name="formatProvider">The culture info.</param>
    /// <param name="namedFormatString">The named format string.</param>
    /// <param name="unknownNames">The unknown names.</param>
    /// <returns>
    /// <c>true</c> if the NamedFormatString could be created, otherwise <c>false</c>.
    /// </returns>
    public static bool TryCreate(string format, IReadOnlyList<string> names, IFormatProvider formatProvider, out NamedFormatString namedFormatString, out IReadOnlyList<string> unknownNames)
    {
        var (indexedFormat, formatNames, actualUnknownNames) = ConvertToIndexedFormat(names, format, CreateStringComparer(formatProvider, false));
        unknownNames = actualUnknownNames;
        if (unknownNames.Count > 0)
        {
            namedFormatString = default;
            return false;
        }

        namedFormatString = new NamedFormatString(indexedFormat, formatNames, formatProvider);
        return true;
    }

    /// <summary>
    /// Formats the specified string based on the named string and additional values.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="namedValues">The named strings.</param>
    /// <param name="additionalValues">The additional values.</param>
    /// <returns>A <see cref="R{StringFormatted, FormatContainedUnknownNames}"/>.</returns>
    public static R<StringFormatted, FormatContainedUnknownNames> FormatInvariant(string format, NamedValues namedValues, params string?[] additionalValues)
    {
        return Format(CultureInfo.InvariantCulture, format, namedValues, additionalValues);
    }

    /// <summary>
    /// Formats the specified string based on the named string and additional values.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="namedValues">The named strings.</param>
    /// <param name="additionalValues">The additional values.</param>
    /// <returns>A <see cref="R{StringFormatted, FormatContainedUnknownNames}"/>.</returns>
    public static R<StringFormatted, FormatContainedUnknownNames> Format(string format, NamedValues namedValues, params string?[] additionalValues)
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
    /// <returns>A <see cref="R{StringFormatted, FormatContainedUnknownNames}"/>.</returns>
    public static R<StringFormatted, FormatContainedUnknownNames> Format(IFormatProvider formatProvider, string format, NamedValues namedValues, params string?[] additionalValues)
    {
        return Format(formatProvider, format, namedValues, new ReadOnlySpan<object?>(additionalValues));
    }

    /// <summary>
    /// Formats the specified string based on the named string and additional values.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="namedValues">The named strings.</param>
    /// <param name="additionalValues">The additional values.</param>
    /// <returns>A <see cref="R{StringFormatted, FormatContainedUnknownNames}"/>.</returns>
    public static R<StringFormatted, FormatContainedUnknownNames> FormatInvariant(string format, NamedValues namedValues, params object?[] additionalValues)
    {
        return Format(CultureInfo.InvariantCulture, format, namedValues, additionalValues);
    }

    /// <summary>
    /// Formats the specified string based on the named string and additional values.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="namedValues">The named strings.</param>
    /// <param name="additionalValues">The additional values.</param>
    /// <returns>A <see cref="R{StringFormatted, FormatContainedUnknownNames}"/>.</returns>
    public static R<StringFormatted, FormatContainedUnknownNames> Format(string format, NamedValues namedValues, params object?[] additionalValues)
    {
        return Format(CultureInfo.CurrentCulture, format, namedValues, additionalValues);
    }

    /// <summary>
    /// Formats the named format string.
    /// </summary>
    /// <param name="namedFormat">The named format.</param>
    /// <param name="getNamedValueFunc">The named value func.</param>
    /// <param name="indexedArguments">The index arguments.</param>
    /// <returns>A <see cref="R{StringFormatted, FormatContainedUnknownNames}"/>.</returns>
    public static R<StringFormatted, FormatContainedUnknownNames> Format(string namedFormat, Func<string, IFormatProvider, R<string?>> getNamedValueFunc, params object?[] indexedArguments)
    {
        return Format(CultureInfo.CurrentCulture, namedFormat, getNamedValueFunc, indexedArguments);
    }

    /// <summary>
    /// Formats the named format string.
    /// </summary>
    /// <param name="namedFormat">The named format.</param>
    /// <param name="nameComparer">The name comparer.</param>
    /// <param name="getNamedValueFunc">The named value func.</param>
    /// <param name="indexedArguments">The index arguments.</param>
    /// <returns>A <see cref="R{StringFormatted, FormatContainedUnknownNames}"/>.</returns>
    public static R<StringFormatted, FormatContainedUnknownNames> Format(string namedFormat, StringComparer nameComparer, Func<string, IFormatProvider, R<string?>> getNamedValueFunc, params object?[] indexedArguments)
    {
        return Format(CultureInfo.CurrentCulture, namedFormat, nameComparer, getNamedValueFunc, indexedArguments);
    }

    /// <summary>
    /// Formats the named format string.
    /// </summary>
    /// <param name="namedFormat">The named format.</param>
    /// <param name="getNamedValueFunc">The named value func.</param>
    /// <param name="indexedArguments">The index arguments.</param>
    /// <returns>A <see cref="R{StringFormatted, FormatContainedUnknownNames}"/>.</returns>
    public static R<StringFormatted, FormatContainedUnknownNames> FormatInvariant(string namedFormat, Func<string, IFormatProvider, R<string?>> getNamedValueFunc, params object?[] indexedArguments)
    {
        return Format(CultureInfo.InvariantCulture, namedFormat, getNamedValueFunc, indexedArguments);
    }

    /// <summary>
    /// Formats the named format string.
    /// </summary>
    /// <param name="namedFormat">The named format.</param>
    /// <param name="nameComparer">The name comparer.</param>
    /// <param name="getNamedValueFunc">The named value func.</param>
    /// <param name="indexedArguments">The index arguments.</param>
    /// <returns>A <see cref="R{StringFormatted, FormatContainedUnknownNames}"/>.</returns>
    public static R<StringFormatted, FormatContainedUnknownNames> FormatInvariant(string namedFormat, StringComparer nameComparer, Func<string, IFormatProvider, R<string?>> getNamedValueFunc, params object?[] indexedArguments)
    {
        return Format(CultureInfo.InvariantCulture, namedFormat, nameComparer, getNamedValueFunc, indexedArguments);
    }

    /// <summary>
    /// Formats the named format string.
    /// </summary>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="namedFormat">The named format.</param>
    /// <param name="getNamedValueFunc">The named value func.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>A <see cref="R{StringFormatted, FormatContainedUnknownNames}"/>.</returns>
    public static R<StringFormatted, FormatContainedUnknownNames> Format(IFormatProvider formatProvider, string namedFormat, Func<string, IFormatProvider, R<string?>> getNamedValueFunc, params object?[] arguments)
    {
        return Format(formatProvider, namedFormat, CreateStringComparer(formatProvider, false), getNamedValueFunc, arguments);
    }

    /// <summary>
    /// Formats the named format string.
    /// </summary>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="namedFormat">The named format.</param>
    /// <param name="namedFormatComparer">The string comparer.</param>
    /// <param name="getNamedValueFunc">The named value func.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>A <see cref="R{StringFormatted, FormatContainedUnknownNames}"/>.</returns>
    public static R<StringFormatted, FormatContainedUnknownNames> Format(
        IFormatProvider formatProvider,
        string namedFormat,
        StringComparer namedFormatComparer,
        Func<string, IFormatProvider, R<string?>> getNamedValueFunc,
        params object?[] arguments)
    {
        var actualArguments = new Buffer<object?>(arguments.Length);
        var names = new List<string>(actualArguments.Capacity);
        actualArguments.Write(arguments);
        names.AddRange(Enumerable.Repeat(string.Empty, arguments.Length));

        var namesAndIndices = new HashSet<(string Name, int Index)>();
        var unknownNames = new HashSet<string>();
        var indexedFormat = FormatRegex.Replace(
            namedFormat,
            match =>
            {
                if (string.IsNullOrEmpty(match.Value))
                {
                    return match.Value;
                }

                if (TryGetIndex(names, match.Value, out var index, namedFormatComparer))
                {
                    namesAndIndices.Add((match.Value, index));
                    return index.ToString();
                }

                if (int.TryParse(match.Value, out index))
                {
                    if (TryGetName(names, index, out var name))
                    {
                        namesAndIndices.Add((name, index));
                    }

                    return match.Value;
                }

                var result = getNamedValueFunc(match.Value, formatProvider);
                if (result.TryGet(out var value))
                {
                    index = actualArguments.Length;
                    actualArguments.Write(value);
                    namesAndIndices.Add((match.Value, index));
                    names.Add(match.Value);
                    return index.ToString();
                }

                unknownNames.Add(match.Value);
                index = actualArguments.Length;
                actualArguments.Write(string.Empty);
                return index.ToString();
            });

        var argumentArray = actualArguments.ToFinalArray();
        if (unknownNames.Count > 0)
        {
            return R.Error(new FormatContainedUnknownNames(unknownNames));
        }

        var nullArguments = GetNullArguments(argumentArray, namesAndIndices);
        return R.Success(new StringFormatted(string.Format(formatProvider, indexedFormat, argumentArray), nullArguments));
    }

    /// <summary>
    /// Formats the specified string based on the named string and additional values.
    /// </summary>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="format">The format.</param>
    /// <param name="namedValues">The named strings.</param>
    /// <param name="additionalValues">The additional values.</param>
    /// <returns>A <see cref="R{StringFormatted, FormatContainedUnknownNames}"/>.</returns>
    public static R<StringFormatted, FormatContainedUnknownNames> Format(IFormatProvider formatProvider, string format, NamedValues namedValues, params object?[] additionalValues)
    {
        return Format(formatProvider, format, namedValues, new ReadOnlySpan<object?>(additionalValues));
    }

    /// <summary>
    /// Formats the string.
    /// </summary>
    /// <param name="arg0">The arg0.</param>
    /// <param name="arg1">The arg1.</param>
    /// <returns>
    /// the formatted string.
    /// </returns>
    public string Format(object? arg0, object? arg1)
    {
        return string.Format(this.formatProvider, this.format, arg0, arg1);
    }

    /// <summary>
    /// Formats the string.
    /// </summary>
    /// <param name="arg0">The arg0.</param>
    /// <param name="arg1">The arg1.</param>
    /// <param name="arg2">The arg2.</param>
    /// <returns>The formatted string.</returns>
    public string Format(object? arg0, object? arg1, object? arg2)
    {
        return string.Format(this.formatProvider, this.format, arg0, arg1, arg2);
    }

    /// <summary>
    /// Formats the string.
    /// </summary>
    /// <param name="arguments">The arguments.</param>
    /// <returns>
    /// the formatted string.
    /// </returns>
    public string Format(params object?[] arguments)
    {
        return string.Format(this.formatProvider, this.format, arguments);
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
        return GetNullArguments(arguments, this.FormatNames);
    }

    /// <summary>
    /// Formats the specified string based on the named string and additional values.
    /// </summary>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="format">The format.</param>
    /// <param name="namedValues">The named strings.</param>
    /// <param name="additionalValues">The additional values.</param>
    /// <returns>A <see cref="R{StringFormatted, FormatContainedUnknownNames}"/>.</returns>
    private static R<StringFormatted, FormatContainedUnknownNames> Format(IFormatProvider formatProvider, string format, NamedValues namedValues, ReadOnlySpan<object?> additionalValues)
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

        var result = TryCreate(format, names.ToFinalArray(), formatProvider, out var namedFormatString, out var unknownNames);
        if (result)
        {
            var argumentArray = arguments.ToFinalArray();
            var nullArguments = namedFormatString.GetNullArguments(argumentArray);
            return R.Success(new StringFormatted(namedFormatString.Format(argumentArray), nullArguments));
        }

        return R.Error(new FormatContainedUnknownNames(unknownNames));
    }

    /// <summary>
    /// Converts to indices format.
    /// </summary>
    /// <param name="names">The names.</param>
    /// <param name="namedFormat">The named format.</param>
    /// <param name="stringComparer">The string comparer.</param>
    /// <returns>A tuple of the indexed format and name-index pairs.</returns>
    private static (string Format, IReadOnlyCollection<(string Name, int Index)> Names, IReadOnlyList<string> UnknownNames) ConvertToIndexedFormat(IReadOnlyList<string> names, string namedFormat, StringComparer stringComparer)
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

                if (TryGetIndex(names, match.Value, out var index, stringComparer))
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
    /// <param name="stringComparer">The string comparer.</param>
    /// <returns>
    /// A value indicating whether the index was found.
    /// </returns>
    private static bool TryGetIndex(IReadOnlyList<string> names, string name, out int index, StringComparer stringComparer)
    {
        index = names.IndexOf(x => stringComparer.Equals(x, name));
        return index != -1;
    }

    private static StringComparer CreateStringComparer(IFormatProvider formatProvider, bool ignoreCase)
    {
        if (formatProvider is CultureInfo cultureInfo)
        {
#if NETSTANDARD1_3
            return new NetStringComparer(cultureInfo.CompareInfo, ignoreCase);
#else
            return StringComparer.Create(cultureInfo, ignoreCase);
#endif
        }

        return ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
    }

    private static NullArguments GetNullArguments(
        object?[] arguments,
        IReadOnlyCollection<(string Name, int Index)> namesAndIndices)
    {
        var nullArguments = new List<(string Name, int Index)>(namesAndIndices.Count);
        foreach (var (name, index) in namesAndIndices.OrderBy(x => x.Index))
        {
            if (arguments[index] == null)
            {
                nullArguments.Add((Name: name, Index: index));
            }
        }

        return new NullArguments(nullArguments, arguments);
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

#if NETSTANDARD1_3
    private class NetStringComparer : StringComparer
    {
        private readonly CompareInfo compareInfo;
        private readonly bool ignoreCase;

        public NetStringComparer(CompareInfo compareInfo, bool ignoreCase)
        {
            this.compareInfo = compareInfo;
            this.ignoreCase = ignoreCase;
        }

        public override int Compare(string x, string y)
        {
            return this.compareInfo.Compare(x, y, this.ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
        }

        public override bool Equals(string x, string y)
        {
            return this.Compare(x, y) == 0;
        }

        public override int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }
#endif
}