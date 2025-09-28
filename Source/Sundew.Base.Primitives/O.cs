// --------------------------------------------------------------------------------------------------------------------
// <copyright file="O.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

/// <summary>
/// Extension methods for nullable reference type providing feature parity with option types.
/// </summary>
public static class O
{
    /// <summary>
    /// Extends TValue with option-like functionality.
    /// </summary>
    extension<TValue>([NotNullWhen(true)] TValue? value)
        where TValue : class
    {
        /// <summary>
        /// Gets a value indicating whether value has a value (Not null).
        /// </summary>
        /// <returns><c>true</c>, if value is not null, otherwise <c>false</c>.</returns>
#pragma warning disable SA1101
        public bool HasValue => value != null;
#pragma warning restore SA1101
    }

    /// <summary>
    /// Gets a value indicating whether value has a value (Not null).
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c>, if value is not null, otherwise <c>false</c>.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static bool TryGetValue<TValue>([NotNullWhen(true)] this TValue? value, [NotNullWhen(true)] out TValue? result)
        where TValue : class
    {
        result = value;
        return value.HasValue;
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="valueFunc">The value func.</param>
    /// <returns>
    /// An optional TNewValue.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static TNewValue? MapValue<TValue, TNewValue>(this TValue? value, Func<TValue, TNewValue> valueFunc)
        where TValue : struct
        where TNewValue : struct
    {
        return value.HasValue ? valueFunc(value.Value) : null;
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="valueFunc">The value func.</param>
    /// <returns>
    /// An optional TNewValue.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static TNewValue? MapValue<TValue, TNewValue>(this TValue? value, Func<TValue, TNewValue?> valueFunc)
        where TValue : struct
        where TNewValue : struct
    {
        return value.HasValue ? valueFunc(value.Value) : null;
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="valueFunc">The value func.</param>
    /// <returns>
    /// An optional TNewValue.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static TNewValue? Map<TValue, TNewValue>(this TValue? value, Func<TValue, TNewValue?> valueFunc)
        where TValue : struct
        where TNewValue : class
    {
        return value.HasValue ? valueFunc(value.Value) : default;
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TParameter">The type of the parameter.</typeparam>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="valueFunc">The value func.</param>
    /// <returns>
    /// An optional TNewValue.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static TNewValue? MapValue<TValue, TParameter, TNewValue>(this TValue? value, TParameter parameter, Func<TValue, TParameter, TNewValue?> valueFunc)
        where TValue : struct
        where TNewValue : struct
    {
        return value.HasValue ? valueFunc(value.Value, parameter) : null;
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TParameter">The type of the parameter.</typeparam>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="valueFunc">The value func.</param>
    /// <returns>
    /// An optional TNewValue.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static TNewValue? Map<TValue, TParameter, TNewValue>(this TValue? value, TParameter parameter, Func<TValue, TParameter, TNewValue?> valueFunc)
        where TValue : struct
        where TNewValue : class
    {
        return value.HasValue ? valueFunc(value.Value, parameter) : default;
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TParameter1">The type of the parameter 1.</typeparam>
    /// <typeparam name="TParameter2">The type of the parameter 2.</typeparam>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="parameter1">The parameter 1.</param>
    /// <param name="parameter2">The parameter 2.</param>
    /// <param name="valueFunc">The value func.</param>
    /// <returns>
    /// An optional TNewValue.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static TNewValue? MapValue<TValue, TParameter1, TParameter2, TNewValue>(this TValue? value, TParameter1 parameter1, TParameter2 parameter2, Func<TValue, TParameter1, TParameter2, TNewValue?> valueFunc)
        where TValue : struct
        where TNewValue : struct
    {
        return value.HasValue ? valueFunc(value.Value, parameter1, parameter2) : null;
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TParameter1">The type of the parameter 1.</typeparam>
    /// <typeparam name="TParameter2">The type of the parameter 2.</typeparam>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="parameter1">The parameter 1.</param>
    /// <param name="parameter2">The parameter 2.</param>
    /// <param name="valueFunc">The value func.</param>
    /// <returns>
    /// An optional TNewValue.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static TNewValue? Map<TValue, TParameter1, TParameter2, TNewValue>(this TValue? value, TParameter1 parameter1, TParameter2 parameter2, Func<TValue, TParameter1, TParameter2, TNewValue?> valueFunc)
        where TValue : struct
        where TNewValue : class
    {
        return value.HasValue ? valueFunc(value.Value, parameter1, parameter2) : default;
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="valueFunc">The value func.</param>
    /// <returns>
    /// An optional TNewValue.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static TNewValue? MapValue<TValue, TNewValue>(this TValue? value, Func<TValue, TNewValue> valueFunc)
        where TValue : class
        where TNewValue : struct
    {
        return value != null ? valueFunc(value) : null;
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="valueFunc">The value func.</param>
    /// <returns>
    /// An optional TNewValue.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static TNewValue? MapValue<TValue, TNewValue>(this TValue? value, Func<TValue, TNewValue?> valueFunc)
        where TValue : class
        where TNewValue : struct
    {
        return value != null ? valueFunc(value) : null;
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="valueFunc">The value func.</param>
    /// <returns>
    /// An optional TNewValue.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static TNewValue? Map<TValue, TNewValue>(this TValue? value, Func<TValue, TNewValue?> valueFunc)
        where TValue : class
        where TNewValue : class
    {
        return value != null ? valueFunc(value) : default;
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TParameter">The type of the parameter.</typeparam>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="valueFunc">The value func.</param>
    /// <returns>
    /// An optional TNewValue.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static TNewValue? MapValue<TValue, TParameter, TNewValue>(this TValue? value, TParameter parameter, Func<TValue, TParameter, TNewValue?> valueFunc)
        where TValue : class
        where TNewValue : struct
    {
        return value != null ? valueFunc(value, parameter) : null;
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TParameter">The type of the parameter.</typeparam>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="valueFunc">The value func.</param>
    /// <returns>
    /// An optional TNewValue.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static TNewValue? Map<TValue, TParameter, TNewValue>(this TValue? value, TParameter parameter, Func<TValue, TParameter, TNewValue?> valueFunc)
        where TValue : class
        where TNewValue : class
    {
        return value != null ? valueFunc(value, parameter) : default;
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TParameter1">The type of the parameter 1.</typeparam>
    /// <typeparam name="TParameter2">The type of the parameter 2.</typeparam>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="parameter1">The parameter 1.</param>
    /// <param name="parameter2">The parameter 2.</param>
    /// <param name="valueFunc">The value func.</param>
    /// <returns>
    /// An optional TNewValue.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static TNewValue? MapValue<TValue, TParameter1, TParameter2, TNewValue>(this TValue? value, TParameter1 parameter1, TParameter2 parameter2, Func<TValue, TParameter1, TParameter2, TNewValue?> valueFunc)
        where TValue : class
        where TNewValue : struct
    {
        return value != null ? valueFunc(value, parameter1, parameter2) : null;
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TParameter1">The type of the parameter 1.</typeparam>
    /// <typeparam name="TParameter2">The type of the parameter 2.</typeparam>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="parameter1">The parameter 1.</param>
    /// <param name="parameter2">The parameter 2.</param>
    /// <param name="valueFunc">The value func.</param>
    /// <returns>
    /// An optional TNewValue.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static TNewValue? Map<TValue, TParameter1, TParameter2, TNewValue>(this TValue? value, TParameter1 parameter1, TParameter2 parameter2, Func<TValue, TParameter1, TParameter2, TNewValue?> valueFunc)
        where TValue : class
        where TNewValue : class
    {
        return value != null ? valueFunc(value, parameter1, parameter2) : default;
    }

    /// <summary>
    /// Gets the value or the default value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The value if present, otherwise the specified default value.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static TValue GetValueOrDefault<TValue>(this TValue? value, TValue defaultValue)
        where TValue : class
    {
        return value ?? defaultValue;
    }

    /// <summary>
    /// Gets the value or the default value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="newValueFunc">The new value func.</param>
    /// <param name="defaultValueFunc">The default value func.</param>
    /// <returns>The value if present, otherwise the specified default value.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static TNewValue GetValueOrDefault<TValue, TNewValue>(this TValue? value, Func<TValue, TNewValue> newValueFunc, Func<TNewValue> defaultValueFunc)
        where TValue : class
    {
        return value != null ? newValueFunc(value) : defaultValueFunc();
    }

    /// <summary>
    /// Gets the value or the default value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="defaultValueFunc">The default value func.</param>
    /// <returns>The value if present, otherwise the specified default value.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static TValue GetValueOrDefault<TValue>(this TValue? value, Func<TValue> defaultValueFunc)
        where TValue : class
    {
        return value ?? defaultValueFunc();
    }

    /// <summary>
    /// Gets the value or the default value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="newValueFunc">The new value func.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The value if present, otherwise the specified default value.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static TNewValue GetValueOrDefault<TValue, TNewValue>(this TValue? value, Func<TValue, TNewValue> newValueFunc, TNewValue defaultValue)
        where TValue : class
    {
        return value != null ? newValueFunc(value) : defaultValue;
    }

    /// <summary>
    /// Gets the value or the default value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="newValueFunc">The new value func.</param>
    /// <returns>The value if present, otherwise the specified default value.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static TNewValue? GetValueOrDefault<TValue, TNewValue>(this TValue? value, Func<TValue, TNewValue> newValueFunc)
        where TValue : class
    {
        return value != null ? newValueFunc(value) : default;
    }

    /// <summary>
    /// Gets the value or the default value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TParameter">The type of the parameter.</typeparam>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="newValueFunc">The new value func.</param>
    /// <param name="alternativeValueFunc">The alternative value func.</param>
    /// <returns>The value if present, otherwise the specified default value.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static TNewValue GetValueOrDefault<TValue, TParameter, TNewValue>(this TValue? value, TParameter parameter, Func<TValue, TParameter, TNewValue> newValueFunc, Func<TParameter, TNewValue> alternativeValueFunc)
        where TValue : class
    {
        return value != null ? newValueFunc(value, parameter) : alternativeValueFunc(parameter);
    }

    /// <summary>
    /// Gets the value or the default value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TParameter1">The type of the parameter 1.</typeparam>
    /// <typeparam name="TParameter2">The type of the parameter 2.</typeparam>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="parameter1">The parameter 1.</param>
    /// <param name="parameter2">The parameter 2.</param>
    /// <param name="newValueFunc">The new value func.</param>
    /// <param name="alternativeValueFunc">The alternative value func.</param>
    /// <returns>The value if present, otherwise the specified default value.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static TNewValue GetValueOrDefault<TValue, TParameter1, TParameter2, TNewValue>(this TValue? value, TParameter1 parameter1, TParameter2 parameter2, Func<TValue, TParameter1, TParameter2, TNewValue> newValueFunc, Func<TParameter1, TParameter2, TNewValue> alternativeValueFunc)
        where TValue : class
    {
        return value != null ? newValueFunc(value, parameter1, parameter2) : alternativeValueFunc(parameter1, parameter2);
    }

    /// <summary>
    /// Combines  a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TOtherValue">The type of the other value.</typeparam>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="otherValue">The other value.</param>
    /// <param name="getValueFunc">The value func.</param>
    /// <returns>
    /// An optional TNewValue.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static TNewValue? Combine<TValue, TOtherValue, TNewValue>(this TValue? value, TOtherValue? otherValue, Func<TValue, TOtherValue, TNewValue> getValueFunc)
        where TValue : class
        where TOtherValue : class
        where TNewValue : class
    {
        var hasValue = value != null && otherValue != null;
        return hasValue ? getValueFunc(value!, otherValue!) : null;
    }

    /// <summary>
    /// Combines  a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TOtherValue">The type of the other value.</typeparam>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="otherValue">The other value.</param>
    /// <param name="getValueFunc">The value func.</param>
    /// <returns>
    /// An optional TNewValue.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static TNewValue? Combine<TValue, TOtherValue, TNewValue>(this TValue? value, TOtherValue? otherValue, Func<TValue, TOtherValue, TNewValue> getValueFunc)
        where TValue : struct
        where TOtherValue : struct
        where TNewValue : class
    {
        var hasValue = value.HasValue && otherValue.HasValue;
        return hasValue ? getValueFunc(value.GetValueOrDefault(), otherValue.GetValueOrDefault()) : null;
    }

    /// <summary>
    /// Combines  a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TOtherValue">The type of the other value.</typeparam>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="otherValue">The other value.</param>
    /// <param name="getValueFunc">The value func.</param>
    /// <returns>
    /// An optional TNewValue.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static TNewValue? Combine<TValue, TOtherValue, TNewValue>(this TValue? value, TOtherValue? otherValue, Func<TValue, TOtherValue, TNewValue> getValueFunc)
        where TValue : struct
        where TOtherValue : class
        where TNewValue : class
    {
        var hasValue = value.HasValue && otherValue != null;
        return hasValue ? getValueFunc(value.GetValueOrDefault(), otherValue!) : null;
    }

    /// <summary>
    /// Combines  a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TOtherValue">The type of the other value.</typeparam>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="otherValue">The other value.</param>
    /// <param name="getValueFunc">The value func.</param>
    /// <returns>
    /// An optional TNewValue.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static TNewValue? Combine<TValue, TOtherValue, TNewValue>(this TValue? value, TOtherValue? otherValue, Func<TValue, TOtherValue, TNewValue> getValueFunc)
        where TValue : class
        where TOtherValue : struct
        where TNewValue : class
    {
        var hasValue = value.HasValue && otherValue.HasValue;
        return hasValue ? getValueFunc(value!, otherValue.GetValueOrDefault()) : null;
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TError">The type of the new error.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="error">The error.</param>
    /// <returns>
    /// A new <see cref="R" />.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static R<TValue, TError> ToResult<TValue, TError>(this TValue? value, TError error)
        where TValue : class
    {
        var isSuccess = value.HasValue;
        return new R<TValue, TError>(isSuccess, value, isSuccess ? default! : error);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TError">The type of the new error.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="errorFunc">The error function.</param>
    /// <returns>
    /// A new <see cref="R" />.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static R<TValue, TError> ToResult<TValue, TError>(this TValue? value, Func<TError> errorFunc)
        where TValue : class
    {
        var isSuccess = value.HasValue;
        return new R<TValue, TError>(isSuccess, value, isSuccess ? default! : errorFunc());
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <typeparam name="TError">The type of the new error.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="valueFunc">The value function.</param>
    /// <param name="error">The error.</param>
    /// <returns>
    /// A new <see cref="R" />.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static R<TNewValue, TError> ToResult<TValue, TNewValue, TError>(this TValue? value, Func<TValue, TNewValue> valueFunc, TError error)
        where TValue : class
    {
        var isSuccess = value.HasValue;
        return new R<TNewValue, TError>(isSuccess, isSuccess ? valueFunc(value!) : default!, error);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <typeparam name="TError">The type of the new error.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="valueFunc">The value function.</param>
    /// <param name="errorFunc">The error function.</param>
    /// <returns>
    /// A new <see cref="R" />.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static R<TNewValue, TError> ToResult<TValue, TNewValue, TError>(this TValue? value, Func<TValue, TNewValue> valueFunc, Func<TError> errorFunc)
        where TValue : class
    {
        return value != null ? new R<TNewValue, TError>(true, valueFunc(value), default!) : new R<TNewValue, TError>(false, default!, errorFunc());
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>
    /// A <see cref="ValueTask" />.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static ValueTask<TValue?> ToAsync<TValue>(this TValue? value)
        where TValue : class
    {
        return new ValueTask<TValue?>(value);
    }

    /// <summary>
    /// Converts a <see cref="R{TSuccess, TError}"/>? to a TSuccess? and a R{TError} and returns a value indicating which of the two to process.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="optionalResult">The optional result.</param>
    /// <param name="valueOption">The value option.</param>
    /// <param name="failedResult">The failed result.</param>
    /// <returns><c>true</c>, if the <paramref name="valueOption"/> should be processed, or <c>false</c> if the <paramref name="failedResult"/> should be processed.</returns>
    public static bool IsSuccess<TSuccess, TError>(this in R<TSuccess, TError>? optionalResult, out TSuccess? valueOption, [NotNullWhen(false)] out TError? failedResult)
        where TSuccess : class
    {
        if (optionalResult.HasValue)
        {
            if (optionalResult.Value.IsSuccess)
            {
                valueOption = optionalResult.Value.Value;
                failedResult = default;
                return true;
            }

            valueOption = default;
            failedResult = optionalResult.Value.Error;
            return false;
        }

        valueOption = default;
        failedResult = default;
        return true;
    }

    /// <summary>
    /// Converts the option to an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="enumerableOption">The enumerable option.</param>
    /// <returns>The enumerable if the option contains one, otherwise and empty enumerable.</returns>
    public static IEnumerable<TValue> AsEnumerable<TValue>(this IEnumerable<TValue>? enumerableOption)
    {
        if (enumerableOption != default)
        {
            return enumerableOption;
        }

        return Enumerable.Empty<TValue>();
    }

    /// <summary>
    /// Creates an optional value from the specified boolean.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="option">The value indicating whether the option has a value.</param>
    /// <param name="optionalValue">The optional value.</param>
    /// <returns>An optional value.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static TValue? ToOption<TValue>(this bool option, TValue optionalValue)
        where TValue : class
    {
        return option ? optionalValue : default;
    }

    /// <summary>
    /// Creates an optional value from the specified boolean.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="option">The value indicating whether the option has a value.</param>
    /// <param name="optionalValueFunc">The optional value func.</param>
    /// <returns>An optional value.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static TValue? ToOption<TValue>(this bool option, Func<TValue> optionalValueFunc)
        where TValue : class
    {
        return option ? optionalValueFunc() : default;
    }

    /// <summary>
    /// Creates an optional value from the specified boolean.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="option">The value indicating whether the option has a value.</param>
    /// <param name="optionalValueFunc">The optional value func.</param>
    /// <returns>An optional value.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static TValue? From<TValue>(bool option, Func<TValue> optionalValueFunc)
        where TValue : class
    {
        return option ? optionalValueFunc() : default;
    }

    /// <summary>
    /// Creates an optional value from the specified boolean.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="option">The value indicating whether the option has a value.</param>
    /// <param name="optionalValueFunc">The optional value func.</param>
    /// <returns>An optional value.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static TValue? FromValue<TValue>(bool option, Func<TValue> optionalValueFunc)
        where TValue : struct
    {
        return option ? optionalValueFunc() : null;
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>
    /// A <see cref="R" />.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static R<TSuccess> ToValueResult<TSuccess>(this TSuccess? value)
    {
        return new R<TSuccess>(value != null, value);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the value.</typeparam>
    /// <param name="value">The error.</param>
    /// <returns>
    /// A <see cref="R" />.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static ValueTask<R<TSuccess>> ToValueResultAsync<TSuccess>(this TSuccess? value)
        where TSuccess : class
    {
        return new R<TSuccess>(value != null, value);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="error">The error.</param>
    /// <returns>
    /// A <see cref="R" />.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static RoE<TError> ToErrorResult<TError>(this TError? error)
    {
        return new RoE<TError>(error == null, error);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="error">The error.</param>
    /// <returns>
    /// A <see cref="R" />.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static ValueTask<RoE<TError>> ToErrorResultAsync<TError>(this TError? error)
        where TError : class
    {
        return new RoE<TError>(error == null, error);
    }
}