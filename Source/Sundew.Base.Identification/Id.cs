// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Id.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using Sundew.Base.Identification.Parsing;

/// <summary>
/// Represents any Id.
/// </summary>
public sealed record Id(Source Source, Path? Path, Arguments? Arguments = null) : IParsable<Id>
{
    /// <summary>
    /// Creates an Uri from this <see cref="Id"/>.
    /// </summary>
    /// <returns>A new <see cref="Uri"/>.</returns>
    public Uri ToUri()
    {
        return this.ToUriWithScheme(null);
    }

    /// <summary>
    /// Creates an Uri from this <see cref="Id"/>.
    /// </summary>
    /// <param name="scheme">The scheme.</param>
    /// <returns>A new <see cref="Uri"/>.</returns>
    public Uri ToUriWithScheme(string? scheme)
    {
        return this.ToUri(scheme, null, null, 0);
    }

    /// <summary>
    /// Creates an Uri from this <see cref="Id"/>.
    /// </summary>
    /// <param name="host">The host.</param>
    /// <returns>A new <see cref="Uri"/>.</returns>
    public Uri ToUriWithHost(string? host)
    {
        return this.ToUri(null, null, host, 0);
    }

    /// <summary>
    /// Create an Uri from this <see cref="Id"/>.
    /// </summary>
    /// <param name="host">The host.</param>
    /// <param name="port">The port.</param>
    /// <returns>A new <see cref="Uri"/>.</returns>
    public Uri ToUri(string? host, int port)
    {
        return this.ToUri(null, null, host, port);
    }

    /// <summary>
    /// Create an Uri from this <see cref="Id"/>.
    /// </summary>
    /// <param name="scheme">The scheme.</param>
    /// <param name="host">The host.</param>
    /// <returns>A new <see cref="Uri"/>.</returns>
    public Uri ToUri(string? scheme, string? host)
    {
        return this.ToUri(scheme, null, host, 0);
    }

    /// <summary>
    /// Create an Uri from this <see cref="Id"/>.
    /// </summary>
    /// <param name="scheme">The scheme.</param>
    /// <param name="host">The host.</param>
    /// <param name="port">The port.</param>
    /// <returns>A new <see cref="Uri"/>.</returns>
    public Uri ToUri(string? scheme, string? host, int port)
    {
        return this.ToUri(scheme, null, host, port);
    }

    /// <summary>
    /// Create an Uri from this <see cref="Id"/>.
    /// </summary>
    /// <param name="scheme">The scheme.</param>
    /// <param name="userInfo">The user info.</param>
    /// <param name="host">The host.</param>
    /// <param name="port">The port.</param>
    /// <returns>A new <see cref="Uri"/>.</returns>
    public Uri ToUri(string? scheme, string? userInfo, string? host, int port)
    {
        var pathPrefix = string.Empty;
        if (string.IsNullOrEmpty(host))
        {
            pathPrefix = @"///";
        }

        var uriBuilder = new UriBuilder(scheme, host, port, pathPrefix + this.ToString())
        {
            UserName = userInfo,
        };

        return uriBuilder.Uri;
    }

    /// <summary>
    /// Parses the specified input string into an instance of the <see cref="Id"/> type.
    /// </summary>
    /// <param name="inputId">The string representation of the argument to be parsed. This value must be a valid format for the <see cref="Id"/>> type.</param>
    /// <param name="provider">The format provider.</param>
    /// <returns>An instance of ValueId that represents the parsed value from the input string.</returns>
    /// <exception cref="FormatException">Thrown if the input string is not in a valid format for the <see cref="Id"/>> type.</exception>
    public static Id Parse(string inputId, IFormatProvider? provider)
    {
        if (TryParse(inputId, provider, out var result))
        {
            return result;
        }

        throw new FormatException($"The string: {inputId} is not a valid {nameof(Id)}");
    }

    /// <summary>
    /// Tries to parse the specified input string into an instance of the <see cref="Id"/> type.
    /// </summary>
    /// <param name="inputId">The string representation of the argument to be parsed. This value must be a valid format for the <see cref="Id"/>> type.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if parsing was successful, otherwise <c>false</c>.</returns>
    public static bool TryParse([NotNullWhen(true)] string? inputId, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out Id result)
    {
        return IdRouteParser.ParseId(inputId, formatProvider).TryGet(out result, out _);
    }

    /// <summary>
    /// Appends this <see cref="Id"/> to the specified <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="formatProvider">The format provider.</param>
    public void AppendInto(StringBuilder stringBuilder, IFormatProvider formatProvider)
    {
        this.Source.AppendInto(stringBuilder, formatProvider);
        if (this.Path.HasValue)
        {
            stringBuilder.Append(Path.Separator);
            this.Path.AppendInto(stringBuilder, formatProvider);
        }

        if (this.Arguments.HasValue)
        {
            stringBuilder.Append(Grammar.ArgumentsSeparator);
            this.Arguments.AppendInto(stringBuilder, formatProvider, new AppendOptions(true));
        }
    }

    /// <summary>
    /// Creates a string representation of the <see cref="Id"/>.
    /// </summary>
    /// <returns>A string.</returns>
    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        this.AppendInto(stringBuilder, CultureInfo.CurrentCulture);
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Tries to get the source type.
    /// </summary>
    /// <returns>A result containing the source type if successful.</returns>
    public R<Type> TryGetSourceType()
    {
        return this.Source.TryGetType();
    }

    /// <summary>
    /// Tries to get the result type.
    /// </summary>
    /// <returns>A result containing the result type if successful.</returns>
    public R<Type> TryGetResultType()
    {
        return TargetEvaluator.GetResultType(this.Source, this.Path);
    }

    /// <summary>
    /// Tries to get the input types.
    /// </summary>
    /// <returns>A result containing the input types if successful.</returns>
    public R<IReadOnlyList<Type>> TryGetInputTypes()
    {
        return TargetEvaluator.GetInputTypes(this.Source, this.Path, this.Arguments);
    }

    /// <summary>
    /// Tries to get the target containing type.
    /// </summary>
    /// <returns>A result containing the containing type if successful.</returns>
    public R<Type> TryGetTargetContainingType()
    {
        return TargetEvaluator.GetDeclaringType(this.Source, this.Path);
    }

    /// <summary>
    /// Gets an <see cref="Id"/> from the specified source and expression.
    /// </summary>
    /// <param name="uri">The uri.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns>A new <see cref="Id"/>.</returns>
    public static R<Id, IIdError> From(Uri uri, IFormatProvider? formatProvider = null)
    {
        var pathAndQuery = uri.PathAndQuery;
        if (pathAndQuery.StartsWith('/'))
        {
            pathAndQuery = pathAndQuery.Substring(1);
        }

        return IdRouteParser.ParseId(pathAndQuery, formatProvider);
    }

    /// <summary>
    /// Gets an <see cref="Id"/> from the specified source and expression.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <param name="targetExpression">The target expression.</param>
    /// <returns>A new <see cref="Id"/>.</returns>
    public static Id From<TSource>(Expression<Action<TSource>> targetExpression)
    {
        var (source, path, arguments) = ExpressionEvaluator.From(targetExpression);
        return new Id(source, path, arguments);
    }

    /// <summary>
    /// Gets an <see cref="Id"/> from the specified source and expression.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <param name="targetExpression">The target expression.</param>
    /// <returns>A new <see cref="Id"/>.</returns>
    public static Id From<TSource>(Expression<Func<TSource, object>> targetExpression)
    {
        var target = ExpressionEvaluator.From(targetExpression);
        return new Id(target.Source, target.Path);
    }

    /// <summary>
    /// Gets an <see cref="Id"/> from the specified source and expression.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <param name="targetExpression">The target expression.</param>
    /// <param name="value">The value.</param>
    /// <returns>A new <see cref="Id"/>.</returns>
    public static Id From<TSource>(Expression<Action<TSource>> targetExpression, IIdentifiable<InstanceId> value)
    {
        var (source, path, valueId) = ExpressionEvaluator.From(targetExpression, new ValueId(null, new ScalarValue(value.Id.ToString())));
        return new Id(source, path, valueId);
    }

    /// <summary>
    /// Gets an <see cref="Id"/> from the specified source and expression.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <param name="targetExpression">The target expression.</param>
    /// <param name="value">The value.</param>
    /// <returns>A new <see cref="Id"/>.</returns>
    public static Id From<TSource>(Expression<Func<TSource, object>> targetExpression, IIdentifiable<InstanceId> value)
    {
        var target = ExpressionEvaluator.From(targetExpression, new ValueId(null, new ScalarValue(value.Id.ToString())));
        return new Id(target.Source, target.Path, target.Arguments);
    }

    /// <summary>
    /// Gets an <see cref="Id"/> from the specified source and expression.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <param name="targetExpression">The target expression.</param>
    /// <param name="value">The value.</param>
    /// <returns>A new <see cref="Id"/>.</returns>
    public static Id From<TSource>(Expression<Action<TSource>> targetExpression, IIdentifiable<ValueId> value)
    {
        var (source, path, valueId) = ExpressionEvaluator.From(targetExpression, value?.Id);
        return new Id(source, path, valueId);
    }

    /// <summary>
    /// Gets an <see cref="Id"/> from the specified source and expression.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <param name="targetExpression">The target expression.</param>
    /// <param name="value">The value.</param>
    /// <returns>A new <see cref="Id"/>.</returns>
    public static Id From<TSource>(Expression<Func<TSource, object>> targetExpression, IIdentifiable<ValueId> value)
    {
        var target = ExpressionEvaluator.From(targetExpression, value?.Id);
        return new Id(target.Source, target.Path, target.Arguments);
    }

    /// <summary>
    /// Indicates an argument placeholder.
    /// </summary>
    /// <typeparam name="TArgument">The argument type.</typeparam>
    /// <returns>The default value.</returns>
    public static TArgument Argument<TArgument>()
    {
        return default!;
    }
}