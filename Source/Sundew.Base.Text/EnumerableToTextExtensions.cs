﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableToTextExtensions.cs" company="Sundews">
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
        return new StringBuilder().AppendItems(enumerable, separator, CultureInfo.CurrentCulture).ToString();
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
        return new StringBuilder().AppendItems(enumerable, separator, CultureInfo.CurrentCulture).ToString();
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToStringInvariant(this IEnumerable<string> enumerable, char separator)
    {
        return new StringBuilder().AppendItems(enumerable, separator, CultureInfo.InvariantCulture).ToString();
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToStringInvariant(this IEnumerable<string> enumerable, string separator)
    {
        return new StringBuilder().AppendItems(enumerable, separator, CultureInfo.InvariantCulture).ToString();
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="skipNullValues">if set to <c>true</c> [skip null values].</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToStringInvariant<TItem>(this IEnumerable<TItem> enumerable, char separator, bool skipNullValues = true)
    {
        return StringBuilderExtensions.InternalAppendItems(new StringBuilder(), enumerable, separator, CultureInfo.InvariantCulture, skipNullValues).ToString();
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="skipNullValues">if set to <c>true</c> [skip null values].</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToStringInvariant<TItem>(this IEnumerable<TItem> enumerable, string separator, bool skipNullValues = true)
    {
        return StringBuilderExtensions.InternalAppendItems(new StringBuilder(), enumerable, separator, CultureInfo.InvariantCulture, skipNullValues).ToString();
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="skipNullValues">if set to <c>true</c> [skip null values].</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToString<TItem>(this IEnumerable<TItem> enumerable, char separator, bool skipNullValues = true)
    {
        return StringBuilderExtensions.InternalAppendItems(new StringBuilder(), enumerable, separator, CultureInfo.CurrentCulture, skipNullValues).ToString();
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="skipNullValues">if set to <c>true</c> [skip null values].</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToString<TItem>(this IEnumerable<TItem> enumerable, string separator, bool skipNullValues = true)
    {
        return StringBuilderExtensions.InternalAppendItems(new StringBuilder(), enumerable, separator, CultureInfo.CurrentCulture, skipNullValues).ToString();
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
    public static string JoinToString<TItem>(this IEnumerable<TItem> enumerable, char separator, IFormatProvider formatProvider, bool skipNullValues = true)
    {
        return StringBuilderExtensions.InternalAppendItems(new StringBuilder(), enumerable, separator, formatProvider, skipNullValues).ToString();
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
    public static string JoinToString<TItem>(this IEnumerable<TItem> enumerable, string separator, IFormatProvider formatProvider, bool skipNullValues = true)
    {
        return StringBuilderExtensions.InternalAppendItems(new StringBuilder(), enumerable, separator, formatProvider, skipNullValues).ToString();
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="appendAction">The append function.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToString<TItem>(this IEnumerable<TItem> enumerable, Action<StringBuilder, TItem> appendAction, char separator)
    {
        return StringBuilderExtensions.InternalAppendItems(new StringBuilder(), enumerable, null, appendAction, appendAction, null, separator).ToString();
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="preAppendAction">The pre append function.</param>
    /// <param name="appendAction">The append function.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToString<TItem>(this IEnumerable<TItem> enumerable, Action<StringBuilder> preAppendAction, Action<StringBuilder, TItem> appendAction, char separator)
    {
        return StringBuilderExtensions.InternalAppendItems(new StringBuilder(), enumerable, preAppendAction, appendAction, appendAction, null, separator).ToString();
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToString<TItem>(this IEnumerable<TItem> enumerable, Action<StringBuilder, TItem> appendAction, string separator)
    {
        return StringBuilderExtensions.InternalAppendItems(new StringBuilder(), enumerable, null, appendAction, appendAction, null, separator).ToString();
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="preAppendAction">The pre append action.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToString<TItem>(this IEnumerable<TItem> enumerable, Action<StringBuilder> preAppendAction, Action<StringBuilder, TItem> appendAction, string separator)
    {
        return StringBuilderExtensions.InternalAppendItems(new StringBuilder(), enumerable, preAppendAction, appendAction, appendAction, null, separator).ToString();
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="firstAppendAction">The first append action.</param>
    /// <param name="appendAction">The append function.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToString<TItem>(this IEnumerable<TItem> enumerable, Action<StringBuilder, TItem> firstAppendAction, Action<StringBuilder, TItem> appendAction, char separator)
    {
        return StringBuilderExtensions.InternalAppendItems(new StringBuilder(), enumerable, null, firstAppendAction, appendAction, null, separator).ToString();
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="preAppendAction">The pre append action.</param>
    /// <param name="firstAppendAction">The first append action.</param>
    /// <param name="appendAction">The append function.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToString<TItem>(this IEnumerable<TItem> enumerable, Action<StringBuilder> preAppendAction, Action<StringBuilder, TItem> firstAppendAction, Action<StringBuilder, TItem> appendAction, char separator)
    {
        return StringBuilderExtensions.InternalAppendItems(new StringBuilder(), enumerable, preAppendAction, firstAppendAction, appendAction, null, separator).ToString();
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="firstAppendAction">The first append action.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToString<TItem>(this IEnumerable<TItem> enumerable, Action<StringBuilder, TItem> firstAppendAction, Action<StringBuilder, TItem> appendAction, string separator)
    {
        return StringBuilderExtensions.InternalAppendItems(new StringBuilder(), enumerable, null, firstAppendAction, appendAction, null, separator).ToString();
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="preAppendAction">The pre append action.</param>
    /// <param name="firstAppendAction">The first append action.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToString<TItem>(this IEnumerable<TItem> enumerable, Action<StringBuilder> preAppendAction, Action<StringBuilder, TItem> firstAppendAction, Action<StringBuilder, TItem> appendAction, string separator)
    {
        return StringBuilderExtensions.InternalAppendItems(new StringBuilder(), enumerable, preAppendAction, firstAppendAction, appendAction, null, separator).ToString();
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="appendAction">The append function.</param>
    /// <param name="postAppendAction">The post append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToString<TItem>(this IEnumerable<TItem> enumerable, Action<StringBuilder, TItem> appendAction, Action<StringBuilder> postAppendAction, char separator)
    {
        return StringBuilderExtensions.InternalAppendItems(new StringBuilder(), enumerable, null, appendAction, appendAction, postAppendAction, separator).ToString();
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="preAppendAction">The pre append action.</param>
    /// <param name="appendAction">The append function.</param>
    /// <param name="postAppendAction">The post append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToString<TItem>(this IEnumerable<TItem> enumerable, Action<StringBuilder> preAppendAction, Action<StringBuilder, TItem> appendAction, Action<StringBuilder> postAppendAction, char separator)
    {
        return StringBuilderExtensions.InternalAppendItems(new StringBuilder(), enumerable, preAppendAction, appendAction, appendAction, postAppendAction, separator).ToString();
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="postAppendAction">The post append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToString<TItem>(this IEnumerable<TItem> enumerable, Action<StringBuilder, TItem> appendAction, Action<StringBuilder> postAppendAction, string separator)
    {
        return StringBuilderExtensions.InternalAppendItems(new StringBuilder(), enumerable, null, appendAction, appendAction, postAppendAction, separator).ToString();
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="preAppendAction">The pre append action.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="postAppendAction">The post append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToString<TItem>(this IEnumerable<TItem> enumerable, Action<StringBuilder> preAppendAction, Action<StringBuilder, TItem> appendAction, Action<StringBuilder> postAppendAction, string separator)
    {
        return StringBuilderExtensions.InternalAppendItems(new StringBuilder(), enumerable, preAppendAction, appendAction, appendAction, postAppendAction, separator).ToString();
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="firstAppendAction">The first append action.</param>
    /// <param name="appendAction">The append function.</param>
    /// <param name="postAppendAction">The post append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToString<TItem>(this IEnumerable<TItem> enumerable, Action<StringBuilder, TItem> firstAppendAction, Action<StringBuilder, TItem> appendAction, Action<StringBuilder> postAppendAction, char separator)
    {
        return StringBuilderExtensions.InternalAppendItems(new StringBuilder(), enumerable, null, firstAppendAction, appendAction, postAppendAction, separator).ToString();
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="preAppendAction">The pre append action.</param>
    /// <param name="firstAppendAction">The first append action.</param>
    /// <param name="appendAction">The append function.</param>
    /// <param name="postAppendAction">The post append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToString<TItem>(this IEnumerable<TItem> enumerable, Action<StringBuilder> preAppendAction, Action<StringBuilder, TItem> firstAppendAction, Action<StringBuilder, TItem> appendAction, Action<StringBuilder> postAppendAction, char separator)
    {
        return StringBuilderExtensions.InternalAppendItems(new StringBuilder(), enumerable, preAppendAction, firstAppendAction, appendAction, postAppendAction, separator).ToString();
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="firstAppendAction">The first append action.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="postAppendAction">The post append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToString<TItem>(this IEnumerable<TItem> enumerable, Action<StringBuilder, TItem> firstAppendAction, Action<StringBuilder, TItem> appendAction, Action<StringBuilder> postAppendAction, string separator)
    {
        return StringBuilderExtensions.InternalAppendItems(new StringBuilder(), enumerable, null, firstAppendAction, appendAction, postAppendAction, separator).ToString();
    }

    /// <summary>
    /// Joins the specified enumerable to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="preAppendAction">The pre append action.</param>
    /// <param name="firstAppendAction">The first append action.</param>
    /// <param name="appendAction">The append action.</param>
    /// <param name="postAppendAction">The post append action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static string JoinToString<TItem>(this IEnumerable<TItem> enumerable, Action<StringBuilder> preAppendAction, Action<StringBuilder, TItem> firstAppendAction, Action<StringBuilder, TItem> appendAction, Action<StringBuilder> postAppendAction, string separator)
    {
        return StringBuilderExtensions.InternalAppendItems(new StringBuilder(), enumerable, preAppendAction, firstAppendAction, appendAction, postAppendAction, separator).ToString();
    }
}