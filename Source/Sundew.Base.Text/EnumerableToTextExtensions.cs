// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableToTextExtensions.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

/// <summary>
/// Delegate for joining data to a <see cref="StringBuilder"/>.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
/// <param name="stringBuilder">The string builder.</param>
/// <param name="isSuccessive">Indicates that the current item is not the first.</param>
/// <param name="item">The current item to aggregate.</param>
public delegate void JoinToStringBuilderAction<in TItem>(StringBuilder stringBuilder, bool isSuccessive, TItem item);

/// <summary>
/// Extends IEnumerable with <see cref="StringBuilder"/> aggregate methods.
/// </summary>
public static class EnumerableToTextExtensions
{
    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToString(this IEnumerable<string> enumerable, char separator)
    {
        return enumerable.Aggregate(
            (stringBuilder: new StringBuilder(), isSuccessive: false),
            (builderPair, item) =>
            {
                builderPair.stringBuilder.Append(item);
                builderPair.stringBuilder.Append(separator);
                return builderPair with { isSuccessive = true };
            },
            builderPair => RemoveSeparatorAtEnd(builderPair).ToString());
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToString(this IEnumerable<string> enumerable, string separator)
    {
        return enumerable.Aggregate(
            (stringBuilder: new StringBuilder(), isSuccessive: false),
            (builderPair, item) =>
            {
                builderPair.stringBuilder.Append(item);
                builderPair.stringBuilder.Append(separator);
                return builderPair with { isSuccessive = true };
            },
            builderPair => RemoveSeparatorAtEnd(builderPair, separator).ToString());
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToString<TItem>(this IEnumerable<TItem> enumerable, char separator, IFormatProvider formatProvider)
        where TItem : notnull
    {
        return enumerable.Aggregate(
            (stringBuilder: new StringBuilder(), isSuccessive: false),
            (builderPair, item) =>
            {
                builderPair.stringBuilder.Append(item, formatProvider);
                builderPair.stringBuilder.Append(separator);
                return builderPair with { isSuccessive = true };
            },
            builderPair => RemoveSeparatorAtEnd(builderPair).ToString());
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="skipNullValues">if set to <c>true</c> [skip null values].</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToString<TItem>(this IEnumerable<TItem> enumerable, char separator, IFormatProvider formatProvider, bool skipNullValues)
        where TItem : class?
    {
        return enumerable.Aggregate(
            (stringBuilder: new StringBuilder(), isSuccessive: false),
            (builderPair, item) =>
            {
                if (item != null)
                {
                    builderPair.stringBuilder.Append(item!, formatProvider);
                }
                else if (skipNullValues)
                {
                    return builderPair with { isSuccessive = true };
                }

                builderPair.stringBuilder.Append(separator);
                return builderPair with { isSuccessive = true };
            },
            builderPair => RemoveSeparatorAtEnd(builderPair).ToString());
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToString<TItem>(this IEnumerable<TItem> enumerable, string separator, IFormatProvider formatProvider)
        where TItem : notnull
    {
        return enumerable.Aggregate(
            (stringBuilder: new StringBuilder(), isSuccessive: false),
            (builderPair, item) =>
            {
                builderPair.stringBuilder.Append(item, formatProvider);
                builderPair.stringBuilder.Append(separator);
                return builderPair with { isSuccessive = true };
            },
            builderPair => RemoveSeparatorAtEnd(builderPair, separator).ToString());
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="skipNullValues">if set to <c>true</c> [skip null values].</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToString<TItem>(this IEnumerable<TItem> enumerable, string separator, IFormatProvider formatProvider, bool skipNullValues)
        where TItem : class?
    {
        return enumerable.Aggregate(
            (stringBuilder: new StringBuilder(), isSuccessive: false),
            (builderPair, item) =>
            {
                if (item != null)
                {
                    builderPair.stringBuilder.Append(item!, formatProvider);
                }
                else if (skipNullValues)
                {
                    return builderPair with { isSuccessive = true };
                }

                builderPair.stringBuilder.Append(separator);
                return builderPair with { isSuccessive = true };
            },
            builderPair => RemoveSeparatorAtEnd(builderPair, separator).ToString());
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="aggregateAction">The aggregate function.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToString<TItem>(this IEnumerable<TItem> enumerable, Action<StringBuilder, TItem> aggregateAction, char separator)
    {
        return enumerable.Aggregate(
            (stringBuilder: new StringBuilder(), isSuccessive: false),
            (builderPair, item) =>
            {
                aggregateAction.Invoke(builderPair.stringBuilder, item);
                builderPair.stringBuilder.Append(separator);
                return builderPair with { isSuccessive = true };
            },
            builderPair => RemoveSeparatorAtEnd(builderPair).ToString());
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="aggregateAction">The aggregate action that provides the string builder and a value indicating whether data was already aggregated.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToString<TItem>(this IEnumerable<TItem> enumerable, JoinToStringBuilderAction<TItem> aggregateAction, char separator)
    {
        return enumerable.Aggregate(
            (stringBuilder: new StringBuilder(), isSuccessive: false),
            (builderPair, item) =>
            {
                aggregateAction.Invoke(builderPair.stringBuilder, builderPair.isSuccessive, item);
                builderPair.stringBuilder.Append(separator);
                return builderPair with { isSuccessive = true };
            },
            builderPair => RemoveSeparatorAtEnd(builderPair).ToString());
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="aggregateAction">The aggregate action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToString<TItem>(this IEnumerable<TItem> enumerable, Action<StringBuilder, TItem> aggregateAction, string separator)
    {
        return enumerable.Aggregate(
            (stringBuilder: new StringBuilder(), isSuccessive: false),
            (builderPair, item) =>
            {
                aggregateAction.Invoke(builderPair.stringBuilder, item);
                builderPair.stringBuilder.Append(separator);
                return builderPair with { isSuccessive = true };
            },
            builderPair => RemoveSeparatorAtEnd(builderPair, separator).ToString());
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="aggregateAction">The aggregate action that provides the string builder and a value indicating whether data was already aggregated.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToString<TItem>(this IEnumerable<TItem> enumerable, JoinToStringBuilderAction<TItem> aggregateAction, string separator)
    {
        return enumerable.Aggregate(
            (stringBuilder: new StringBuilder(), isSuccessive: false),
            (builderPair, item) =>
            {
                aggregateAction.Invoke(builderPair.stringBuilder, builderPair.isSuccessive, item);
                builderPair.stringBuilder.Append(separator);
                return builderPair with { isSuccessive = true };
            },
            builderPair => RemoveSeparatorAtEnd(builderPair, separator).ToString());
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder JoinToStringBuilder(this IEnumerable<string> enumerable, StringBuilder stringBuilder, char separator)
    {
        return enumerable.Aggregate(
            (stringBuilder, isSuccessive: false),
            (builderPair, item) =>
            {
                builderPair.stringBuilder.Append(item);
                builderPair.stringBuilder.Append(separator);
                return builderPair with { isSuccessive = true };
            },
            RemoveSeparatorAtEnd);
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder JoinToStringBuilder(this IEnumerable<string> enumerable, StringBuilder stringBuilder, string separator)
    {
        return enumerable.Aggregate(
            (stringBuilder, isSuccessive: false),
            (builderPair, item) =>
            {
                builderPair.stringBuilder.Append(item);
                builderPair.stringBuilder.Append(separator);
                return builderPair with { isSuccessive = true };
            },
            builderPair => RemoveSeparatorAtEnd(builderPair, separator));
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder JoinToStringBuilder<TItem>(this IEnumerable<TItem> enumerable, StringBuilder stringBuilder, char separator, IFormatProvider formatProvider)
        where TItem : notnull
    {
        return enumerable.Aggregate(
            (stringBuilder, isSuccessive: false),
            (builderPair, item) =>
            {
                builderPair.stringBuilder.Append(item, formatProvider);
                builderPair.stringBuilder.Append(separator);
                return builderPair with { isSuccessive = true };
            },
            RemoveSeparatorAtEnd);
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="skipNullValues">if set to <c>true</c> [skip null values].</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder JoinToStringBuilder<TItem>(this IEnumerable<TItem> enumerable, StringBuilder stringBuilder, char separator, IFormatProvider formatProvider, bool skipNullValues)
        where TItem : class?
    {
        return enumerable.Aggregate(
            (stringBuilder, isSuccessive: false),
            (builderPair, item) =>
            {
                if (item != null)
                {
                    builderPair.stringBuilder.Append(item!, formatProvider);
                }
                else if (skipNullValues)
                {
                    return builderPair with { isSuccessive = true };
                }

                builderPair.stringBuilder.Append(separator);
                return builderPair with { isSuccessive = true };
            },
            RemoveSeparatorAtEnd);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder JoinToStringBuilder<TItem>(this IEnumerable<TItem> enumerable, StringBuilder stringBuilder, string separator, IFormatProvider formatProvider)
        where TItem : notnull
    {
        return enumerable.Aggregate(
            (stringBuilder, isSuccessive: false),
            (builderPair, item) =>
            {
                builderPair.stringBuilder.Append(item, formatProvider);
                builderPair.stringBuilder.Append(separator);
                return builderPair with { isSuccessive = true };
            },
            builderPair => RemoveSeparatorAtEnd(builderPair, separator));
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="skipNullValues">if set to <c>true</c> [skip null values].</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder JoinToStringBuilder<TItem>(this IEnumerable<TItem> enumerable, StringBuilder stringBuilder, string separator, IFormatProvider formatProvider, bool skipNullValues)
        where TItem : class?
    {
        return enumerable.Aggregate(
            (stringBuilder, isSuccessive: false),
            (builderPair, item) =>
            {
                if (item != null)
                {
                    builderPair.stringBuilder.Append(item!, formatProvider);
                }
                else if (skipNullValues)
                {
                    return builderPair with { isSuccessive = true };
                }

                builderPair.stringBuilder.Append(separator);
                return builderPair with { isSuccessive = true };
            },
            builderPair => RemoveSeparatorAtEnd(builderPair, separator));
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="aggregateAction">The aggregate action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder JoinToStringBuilder<TItem>(
        this IEnumerable<TItem> enumerable,
        StringBuilder stringBuilder,
        Action<StringBuilder, TItem> aggregateAction,
        char separator)
    {
        return enumerable.Aggregate(
            (stringBuilder, isSuccessive: false),
            (builderPair, item) =>
            {
                aggregateAction.Invoke(builderPair.stringBuilder, item);
                builderPair.stringBuilder.Append(separator);
                return builderPair with { isSuccessive = true };
            },
            RemoveSeparatorAtEnd);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="aggregateAction">The aggregate action that provides the string builder and a value indicating whether data was already aggregated.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder JoinToStringBuilder<TItem>(
        this IEnumerable<TItem> enumerable,
        StringBuilder stringBuilder,
        JoinToStringBuilderAction<TItem> aggregateAction,
        char separator)
    {
        return enumerable.Aggregate(
            (stringBuilder, isSuccessive: false),
            (builderPair, item) =>
            {
                aggregateAction.Invoke(builderPair.stringBuilder, builderPair.isSuccessive, item);
                builderPair.stringBuilder.Append(separator);
                return builderPair with { isSuccessive = true };
            },
            RemoveSeparatorAtEnd);
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="aggregateAction">The aggregate action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder JoinToStringBuilder<TItem>(
        this IEnumerable<TItem> enumerable,
        StringBuilder stringBuilder,
        Action<StringBuilder, TItem> aggregateAction,
        string separator)
    {
        return enumerable.Aggregate(
            (stringBuilder, isSuccessive: false),
            (builderPair, item) =>
            {
                aggregateAction.Invoke(builderPair.stringBuilder, item);
                builderPair.stringBuilder.Append(separator);
                return builderPair with { isSuccessive = true };
            },
            builderPair => RemoveSeparatorAtEnd(builderPair, separator));
    }

    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="aggregateAction">The aggregate action that provides the string builder and a value indicating whether data was already aggregated.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder JoinToStringBuilder<TItem>(
        this IEnumerable<TItem> enumerable,
        StringBuilder stringBuilder,
        JoinToStringBuilderAction<TItem> aggregateAction,
        string separator)
    {
        return enumerable.Aggregate(
            (stringBuilder, isSuccessive: false),
            (builderPair, item) =>
            {
                aggregateAction.Invoke(builderPair.stringBuilder, builderPair.isSuccessive, item);
                builderPair.stringBuilder.Append(separator);
                return builderPair with { isSuccessive = true };
            },
            builderPair => RemoveSeparatorAtEnd(builderPair, separator));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static StringBuilder RemoveSeparatorAtEnd((StringBuilder StringBuilder, bool IsSuccessive) builderPair, string separator)
    {
        return builderPair.IsSuccessive ? builderPair.StringBuilder.Remove(builderPair.StringBuilder.Length - separator.Length, separator.Length) : builderPair.StringBuilder;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static StringBuilder RemoveSeparatorAtEnd((StringBuilder StringBuilder, bool IsSuccessive) builderPair)
    {
        return builderPair.IsSuccessive ? builderPair.StringBuilder.Remove(builderPair.StringBuilder.Length - 1, 1) : builderPair.StringBuilder;
    }
}