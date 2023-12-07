// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionalValue.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

/// <summary>
/// Extension methods for nullable reference type providing feature parity with option types.
/// </summary>
public static class OptionalValue
{
    /// <summary>
    /// Gets a value indicating whether value has a value (Not null).
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c>, if value is not null, otherwise <c>false</c>.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static bool TryGetValue<TValue>([NotNullWhen(true)] this TValue? value, out TValue result)
        where TValue : struct
    {
        if (value.HasValue)
        {
            result = value.Value;
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Gets a value indicating whether value has a value (Not null).
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c>, if value is not null, otherwise <c>false</c>.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static bool HasValue<TValue>([NotNullWhen(true)] this TValue? value)
        where TValue : struct
        => value.HasValue;

    /// <summary>
    /// Gets the value or the default value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="newValueFunc">The new value func.</param>
    /// <param name="alternativeValueFunc">The alternative value func.</param>
    /// <returns>The value if present, otherwise the specified default value.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static TNewValue GetValueOrDefault<TValue, TNewValue>(this TValue? value, Func<TValue, TNewValue> newValueFunc, Func<TNewValue> alternativeValueFunc)
        where TValue : struct
    {
        return value != null ? newValueFunc(value.Value) : alternativeValueFunc();
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
        where TValue : struct
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
        where TValue : struct
    {
        return value != null ? newValueFunc(value.Value) : defaultValue;
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
        where TValue : struct
    {
        return value != null ? newValueFunc(value.Value, parameter) : alternativeValueFunc(parameter);
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
        where TValue : struct
    {
        return value != null ? newValueFunc(value.Value, parameter1, parameter2) : alternativeValueFunc(parameter1, parameter2);
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
        where TNewValue : struct
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
        where TNewValue : struct
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
        where TNewValue : struct
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
        where TNewValue : struct
    {
        var hasValue = value.HasValue() && otherValue.HasValue;
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
        where TValue : struct
    {
        var isSuccess = value.TryGetValue(out var actualValue);
        return new R<TValue, TError>(isSuccess, actualValue, isSuccess ? default! : error);
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
        where TValue : struct
    {
        var isSuccess = value.TryGetValue(out var actualValue);
        return new R<TValue, TError>(isSuccess, actualValue, isSuccess ? default! : errorFunc());
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
        where TValue : struct
    {
        var isSuccess = value.TryGetValue(out var actualValue);
        return new R<TNewValue, TError>(isSuccess, isSuccess ? valueFunc(actualValue) : default!, error);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>
    /// A <see cref="ValueTask{TValue}" />.
    /// </returns>
    public static ValueTask<TValue?> ToAsync<TValue>(this TValue? value)
        where TValue : struct
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
    public static bool IsSuccess<TSuccess, TError>(this in R<TSuccess, TError>? optionalResult, out TSuccess? valueOption, out R<TError> failedResult)
        where TSuccess : struct
    {
        if (optionalResult.HasValue)
        {
            if (optionalResult.Value.IsSuccess)
            {
                valueOption = optionalResult.Value.Value;
                failedResult = R.Success();
                return true;
            }

            valueOption = default;
            failedResult = R.Error(optionalResult.Value.Error);
            return false;
        }

        valueOption = default;
        failedResult = R.Success();
        return true;
    }

    /// <summary>
    /// Converts the option to an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="option">The  option.</param>
    /// <returns>The enumerable if the option contains one, otherwise and empty enumerable.</returns>
    public static IEnumerable<TValue> ToEnumerable<TValue>(this TValue? option)
        where TValue : struct
    {
        if (option.HasValue)
        {
            yield return option.Value;
        }
    }

    /// <summary>
    /// Creates an optional value from the specified boolean.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="option">The value indicating whether the option has a value.</param>
    /// <param name="optionalValue">The optional value.</param>
    /// <returns>An optional value.</returns>
    public static TValue? ToOption<TValue>(this bool option, TValue optionalValue)
        where TValue : struct
    {
        return option ? optionalValue : null;
    }

    /// <summary>
    /// Creates an optional value from the specified boolean.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="option">The value indicating whether the option has a value.</param>
    /// <param name="optionalValueFunc">The optional value func.</param>
    /// <returns>An optional value.</returns>
    public static TValue? ToOption<TValue>(this bool option, Func<TValue> optionalValueFunc)
        where TValue : struct
    {
        return option ? optionalValueFunc() : null;
    }
}