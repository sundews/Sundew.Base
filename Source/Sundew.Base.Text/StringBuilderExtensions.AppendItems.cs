// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringBuilderExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

/// <summary>
/// Extends the string with extension methods.
/// </summary>
public static partial class StringBuilderExtensions
{
    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems(this StringBuilder stringBuilder, IEnumerable<string> enumerable, char separator)
    {
        return AppendItems(stringBuilder, enumerable, separator, CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems(this StringBuilder stringBuilder, IEnumerable<string> enumerable, string separator)
    {
        return AppendItems(stringBuilder, enumerable, separator, CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItemsInvariant(this StringBuilder stringBuilder, IEnumerable<string> enumerable, char separator)
    {
        return AppendItems(stringBuilder, enumerable, separator, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItemsInvariant(this StringBuilder stringBuilder, IEnumerable<string> enumerable, string separator)
    {
        return AppendItems(stringBuilder, enumerable, separator, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="skipNullValues">if set to <c>true</c> [skip null values].</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItemsInvariant<TItem>(this StringBuilder stringBuilder, IEnumerable<TItem> enumerable, char separator, bool skipNullValues = true)
        where TItem : class?
    {
        return InternalAppendItems(stringBuilder, enumerable, separator, CultureInfo.InvariantCulture, skipNullValues);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="skipNullValues">if set to <c>true</c> [skip null values].</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItemsInvariant<TItem>(this StringBuilder stringBuilder, IEnumerable<TItem> enumerable, string separator, bool skipNullValues = true)
        where TItem : class?
    {
        return InternalAppendItems(stringBuilder, enumerable, separator, CultureInfo.InvariantCulture, skipNullValues);
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="skipNullValues">if set to <c>true</c> [skip null values].</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(this StringBuilder stringBuilder, IEnumerable<TItem> enumerable, char separator, bool skipNullValues = true)
        where TItem : class?
    {
        return InternalAppendItems(stringBuilder, enumerable, separator, CultureInfo.CurrentCulture, skipNullValues);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="skipNullValues">if set to <c>true</c> [skip null values].</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(this StringBuilder stringBuilder, IEnumerable<TItem> enumerable, string separator, bool skipNullValues = true)
        where TItem : class?
    {
        return InternalAppendItems(stringBuilder, enumerable, separator, CultureInfo.CurrentCulture, skipNullValues);
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="skipNullValues">if set to <c>true</c> [skip null values].</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(this StringBuilder stringBuilder, IEnumerable<TItem> enumerable, char separator, IFormatProvider formatProvider, bool skipNullValues = true)
    {
        return InternalAppendItems(stringBuilder, enumerable, separator, formatProvider, skipNullValues);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="skipNullValues">if set to <c>true</c> [skip null values].</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(this StringBuilder stringBuilder, IEnumerable<TItem> enumerable, string separator, IFormatProvider formatProvider, bool skipNullValues = true)
    {
        return InternalAppendItems(stringBuilder, enumerable, separator, formatProvider, skipNullValues);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="appendAction">The append action.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(
        this StringBuilder stringBuilder,
        IEnumerable<TItem> enumerable,
        Action<StringBuilder, TItem> appendAction)
    {
        return InternalAppendItems(stringBuilder, enumerable, null, appendAction, appendAction, null, string.Empty);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="preAppendAction">The pre append action.</param>
    /// <param name="appendAction">The append action.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(
        this StringBuilder stringBuilder,
        IEnumerable<TItem> enumerable,
        Action<StringBuilder> preAppendAction,
        Action<StringBuilder, TItem> appendAction)
    {
        return InternalAppendItems(stringBuilder, enumerable, preAppendAction, appendAction, appendAction, null, string.Empty);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(
        this StringBuilder stringBuilder,
        IEnumerable<TItem> enumerable,
        Action<StringBuilder, TItem> appendAction,
        char separator)
    {
        return InternalAppendItems(stringBuilder, enumerable, null, appendAction, appendAction, null, separator);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="preAppendAction">The pre append action.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(
        this StringBuilder stringBuilder,
        IEnumerable<TItem> enumerable,
        Action<StringBuilder> preAppendAction,
        Action<StringBuilder, TItem> appendAction,
        char separator)
    {
        return InternalAppendItems(stringBuilder, enumerable, preAppendAction, appendAction, appendAction, null, separator);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(
        this StringBuilder stringBuilder,
        IEnumerable<TItem> enumerable,
        Action<StringBuilder, TItem> appendAction,
        string separator)
    {
        return InternalAppendItems(stringBuilder, enumerable, null, appendAction, appendAction, null, separator);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="preAppendAction">The pre append action.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(
        this StringBuilder stringBuilder,
        IEnumerable<TItem> enumerable,
        Action<StringBuilder> preAppendAction,
        Action<StringBuilder, TItem> appendAction,
        string separator)
    {
        return InternalAppendItems(stringBuilder, enumerable, preAppendAction, appendAction, appendAction, null, separator);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="firstAppendAction">The first append action.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(
        this StringBuilder stringBuilder,
        IEnumerable<TItem> enumerable,
        Action<StringBuilder, TItem> firstAppendAction,
        Action<StringBuilder, TItem> appendAction,
        char separator)
    {
        return InternalAppendItems(stringBuilder, enumerable, null, firstAppendAction, appendAction, null, separator);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="firstAppendAction">The first append action.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(
        this StringBuilder stringBuilder,
        IEnumerable<TItem> enumerable,
        Action<StringBuilder, TItem> firstAppendAction,
        Action<StringBuilder, TItem> appendAction,
        string separator)
    {
        return InternalAppendItems(stringBuilder, enumerable, null, firstAppendAction, appendAction, null, separator);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="postAppendAction">The post append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(
        this StringBuilder stringBuilder,
        IEnumerable<TItem> enumerable,
        Action<StringBuilder, TItem> appendAction,
        Action<StringBuilder> postAppendAction,
        char separator)
    {
        return InternalAppendItems(stringBuilder, enumerable, null, appendAction, appendAction, postAppendAction, separator);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="preAppendAction">The pre append action.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="postAppendAction">The post append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(
        this StringBuilder stringBuilder,
        IEnumerable<TItem> enumerable,
        Action<StringBuilder> preAppendAction,
        Action<StringBuilder, TItem> appendAction,
        Action<StringBuilder> postAppendAction,
        char separator)
    {
        return InternalAppendItems(stringBuilder, enumerable, preAppendAction, appendAction, appendAction, postAppendAction, separator);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="postAppendAction">The post append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(
        this StringBuilder stringBuilder,
        IEnumerable<TItem> enumerable,
        Action<StringBuilder, TItem> appendAction,
        Action<StringBuilder> postAppendAction,
        string separator)
    {
        return InternalAppendItems(stringBuilder, enumerable, null, appendAction, appendAction, postAppendAction, separator);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="preAppendAction">The pre append action.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="postAppendAction">The post append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(
        this StringBuilder stringBuilder,
        IEnumerable<TItem> enumerable,
        Action<StringBuilder> preAppendAction,
        Action<StringBuilder, TItem> appendAction,
        Action<StringBuilder> postAppendAction,
        string separator)
    {
        return InternalAppendItems(stringBuilder, enumerable, preAppendAction, appendAction, appendAction, postAppendAction, separator);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="firstAppendAction">The first append action.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="postAppendAction">The post append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(
        this StringBuilder stringBuilder,
        IEnumerable<TItem> enumerable,
        Action<StringBuilder, TItem> firstAppendAction,
        Action<StringBuilder, TItem> appendAction,
        Action<StringBuilder> postAppendAction,
        char separator)
    {
        return InternalAppendItems(stringBuilder, enumerable, null, firstAppendAction, appendAction, postAppendAction, separator);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="preAppendAction">The pre append action.</param>
    /// <param name="firstAppendAction">The first append action.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="postAppendAction">The post append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(
        this StringBuilder stringBuilder,
        IEnumerable<TItem> enumerable,
        Action<StringBuilder> preAppendAction,
        Action<StringBuilder, TItem> firstAppendAction,
        Action<StringBuilder, TItem> appendAction,
        Action<StringBuilder> postAppendAction,
        char separator)
    {
        return InternalAppendItems(stringBuilder, enumerable, preAppendAction, firstAppendAction, appendAction, postAppendAction, separator);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="firstAppendAction">The first append action.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="postAppendAction">The post append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(
        this StringBuilder stringBuilder,
        IEnumerable<TItem> enumerable,
        Action<StringBuilder, TItem> firstAppendAction,
        Action<StringBuilder, TItem> appendAction,
        Action<StringBuilder> postAppendAction,
        string separator)
    {
        return InternalAppendItems(stringBuilder, enumerable, null, firstAppendAction, appendAction, postAppendAction, separator);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="preAppendAction">The pre append action.</param>
    /// <param name="firstAppendAction">The first append action.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="postAppendAction">The post append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder AppendItems<TItem>(
        this StringBuilder stringBuilder,
        IEnumerable<TItem> enumerable,
        Action<StringBuilder> preAppendAction,
        Action<StringBuilder, TItem> firstAppendAction,
        Action<StringBuilder, TItem> appendAction,
        Action<StringBuilder> postAppendAction,
        string separator)
    {
        return InternalAppendItems(stringBuilder, enumerable, preAppendAction, firstAppendAction, appendAction, postAppendAction, separator);
    }

    internal static StringBuilder InternalAppendItems<TItem>(StringBuilder stringBuilder, IEnumerable<TItem> enumerable, Action<StringBuilder>? preAppendAction, Action<StringBuilder, TItem> firstAppendAction, Action<StringBuilder, TItem> successiveAppendAction, Action<StringBuilder>? postAppendAction, string separator)
    {
        using (IEnumerator<TItem> enumerator = enumerable.GetEnumerator())
        {
            if (!enumerator.MoveNext())
            {
                return stringBuilder;
            }

            preAppendAction?.Invoke(stringBuilder);
            var value = enumerator.Current;
            if (value != null)
            {
                firstAppendAction(stringBuilder, value);
            }

            while (enumerator.MoveNext())
            {
                stringBuilder.Append(separator);
                value = enumerator.Current;
                if (value != null)
                {
                    successiveAppendAction(stringBuilder, value);
                }
            }
        }

        postAppendAction?.Invoke(stringBuilder);
        return stringBuilder;
    }

    internal static StringBuilder InternalAppendItems<TItem>(StringBuilder stringBuilder, IEnumerable<TItem> enumerable, Action<StringBuilder>? preAppendAction, Action<StringBuilder, TItem> firstAppendAction, Action<StringBuilder, TItem> successiveAppendAction, Action<StringBuilder>? postAppendAction, char separator)
    {
        using (IEnumerator<TItem> enumerator = enumerable.GetEnumerator())
        {
            if (!enumerator.MoveNext())
            {
                return stringBuilder;
            }

            preAppendAction?.Invoke(stringBuilder);
            var value = enumerator.Current;
            if (value != null)
            {
                firstAppendAction(stringBuilder, value);
            }

            while (enumerator.MoveNext())
            {
                stringBuilder.Append(separator);
                value = enumerator.Current;
                if (value != null)
                {
                    successiveAppendAction(stringBuilder, value);
                }
            }
        }

        postAppendAction?.Invoke(stringBuilder);
        return stringBuilder;
    }

    internal static StringBuilder InternalAppendItems<TItem>(StringBuilder stringBuilder, IEnumerable<TItem> enumerable, string separator, IFormatProvider formatProvider, bool skipNullValues)
    {
        using (IEnumerator<TItem> enumerator = enumerable.GetEnumerator())
        {
            if (!enumerator.MoveNext())
            {
                return stringBuilder;
            }

            var value = enumerator.Current;
            var previousWasSet = value != null;
            if (value != null)
            {
                stringBuilder.Append(value, formatProvider);
            }

            while (enumerator.MoveNext())
            {
                if (previousWasSet || !skipNullValues)
                {
                    stringBuilder.Append(separator);
                }

                value = enumerator.Current;
                previousWasSet = value != null;
                if (value != null)
                {
                    stringBuilder.Append(value, formatProvider);
                }
            }
        }

        return stringBuilder;
    }

    internal static StringBuilder InternalAppendItems<TItem>(StringBuilder stringBuilder, IEnumerable<TItem> enumerable, char separator, IFormatProvider formatProvider, bool skipNullValues)
    {
        using (IEnumerator<TItem> enumerator = enumerable.GetEnumerator())
        {
            if (!enumerator.MoveNext())
            {
                return stringBuilder;
            }

            var value = enumerator.Current;
            var previousWasSet = value != null;
            if (value != null)
            {
                stringBuilder.Append(value, formatProvider);
            }

            while (enumerator.MoveNext())
            {
                if (previousWasSet || !skipNullValues)
                {
                    stringBuilder.Append(separator);
                }

                value = enumerator.Current;
                previousWasSet = value != null;
                if (value != null)
                {
                    stringBuilder.Append(value, formatProvider);
                }
            }
        }

        return stringBuilder;
    }
}