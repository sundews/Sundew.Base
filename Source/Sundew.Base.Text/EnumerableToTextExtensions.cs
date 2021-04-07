// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableToTextExtensions.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

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
                new StringBuilder(),
                (builder, item) =>
                {
                    builder.Append(item);
                    builder.Append(separator);
                    return builder;
                },
                builder => builder.ToString(0, new FromEnd(1)));
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
                new StringBuilder(),
                (builder, item) =>
                {
                    builder.Append(item);
                    builder.Append(separator);
                    return builder;
                },
                builder => builder.ToString(0, separator));
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
                new StringBuilder(),
                (builder, item) =>
                {
                    builder.Append(item, formatProvider);
                    builder.Append(separator);
                    return builder;
                },
                builder => builder.ToString(0, new FromEnd(1)));
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
                new StringBuilder(),
                (builder, item) =>
                {
                    if (item != null)
                    {
                        builder.Append(item!, formatProvider);
                    }
                    else if (skipNullValues)
                    {
                        return builder;
                    }

                    builder.Append(separator);
                    return builder;
                },
                builder => builder.ToString(0, new FromEnd(1)));
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
                new StringBuilder(),
                (builder, item) =>
                {
                    builder.Append(item, formatProvider);
                    builder.Append(separator);
                    return builder;
                },
                builder => builder.ToString(0, separator));
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
                new StringBuilder(),
                (builder, item) =>
                {
                    if (item != null)
                    {
                        builder.Append(item!, formatProvider);
                    }
                    else if (skipNullValues)
                    {
                        return builder;
                    }

                    builder.Append(separator);
                    return builder;
                },
                builder => builder.ToString(0, separator));
        }

        /// <summary>
        /// Joins the specified enumerable to string builder.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="aggregateFunction">The aggregate function.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>
        /// The result of the result function.
        /// </returns>
        public static string JoinToString<TItem>(this IEnumerable<TItem> enumerable, Action<StringBuilder, TItem> aggregateFunction, char separator)
        {
            return enumerable.Aggregate(
                new StringBuilder(),
                (builder, item) =>
                {
                    aggregateFunction.Invoke(builder, item);
                    builder.Append(separator);
                    return builder;
                },
                builder => builder.ToString(0, new FromEnd(1)));
        }

        /// <summary>
        /// Joins the specified enumerable to string builder.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="aggregateFunction">The aggregate function.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>
        /// The result of the result function.
        /// </returns>
        public static string JoinToString<TItem>(this IEnumerable<TItem> enumerable, Action<StringBuilder, TItem> aggregateFunction, string separator)
        {
            return enumerable.Aggregate(
                new StringBuilder(),
                (builder, item) =>
                {
                    aggregateFunction.Invoke(builder, item);
                    builder.Append(separator);
                    return builder;
                },
                builder => builder.ToString(0, separator));
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
                stringBuilder,
                (builder, item) =>
                {
                    builder.Append(item);
                    builder.Append(separator);
                    return builder;
                },
                builder => builder.Remove(builder.Length - 1, 1));
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
                stringBuilder,
                (builder, item) =>
                {
                    builder.Append(item);
                    builder.Append(separator);
                    return builder;
                },
                builder => builder.Remove(builder.Length - separator.Length, separator.Length));
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
                stringBuilder,
                (builder, item) =>
                {
                    builder.Append(item, formatProvider);
                    builder.Append(separator);
                    return builder;
                },
                builder => builder.Remove(builder.Length - 1, 1));
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
                stringBuilder,
                (builder, item) =>
                {
                    if (item != null)
                    {
                        builder.Append(item!, formatProvider);
                    }
                    else if (skipNullValues)
                    {
                        return builder;
                    }

                    builder.Append(separator);
                    return builder;
                },
                builder => builder.Remove(builder.Length - 1, 1));
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
                stringBuilder,
                (builder, item) =>
                {
                    builder.Append(item, formatProvider);
                    builder.Append(separator);
                    return builder;
                },
                builder => builder.Remove(builder.Length - separator.Length, separator.Length));
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
                stringBuilder,
                (builder, item) =>
                {
                    if (item != null)
                    {
                        builder.Append(item!, formatProvider);
                    }
                    else if (skipNullValues)
                    {
                        return builder;
                    }

                    builder.Append(separator);
                    return builder;
                },
                builder => builder.Remove(builder.Length - separator.Length, separator.Length));
        }

        /// <summary>
        /// Aggregates to string builder.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="stringBuilder">The string builder.</param>
        /// <param name="aggregateFunction">The aggregate function.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>
        /// The result of the result function.
        /// </returns>
        public static StringBuilder JoinToStringBuilder<TItem>(
            this IEnumerable<TItem> enumerable,
            StringBuilder stringBuilder,
            Action<StringBuilder, TItem> aggregateFunction,
            char separator)
        {
            return enumerable.Aggregate(
                stringBuilder,
                (builder, item) =>
                {
                    aggregateFunction.Invoke(builder, item);
                    builder.Append(separator);
                    return builder;
                },
                builder => builder.Remove(builder.Length - 1, 1));
        }

        /// <summary>
        /// Aggregates to string builder.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="stringBuilder">The string builder.</param>
        /// <param name="aggregateFunction">The aggregate function.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>
        /// The result of the result function.
        /// </returns>
        public static StringBuilder JoinToStringBuilder<TItem>(
            this IEnumerable<TItem> enumerable,
            StringBuilder stringBuilder,
            Action<StringBuilder, TItem> aggregateFunction,
            string separator)
        {
            return enumerable.Aggregate(
                stringBuilder,
                (builder, item) =>
                {
                    aggregateFunction.Invoke(builder, item);
                    builder.Append(separator);
                    return builder;
                },
                builder => builder.Remove(builder.Length - separator.Length, separator.Length));
        }
    }
}