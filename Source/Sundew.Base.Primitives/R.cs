// --------------------------------------------------------------------------------------------------------------------
// <copyright file="R.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base;

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

/// <summary>
/// Factory class for creating results.
/// </summary>
public static partial class R
{
    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>
    /// A <see cref="SuccessResult" />.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static SuccessResult Success()
    {
        return SuccessResult.Result;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="SuccessResult{TValue}"/>.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static SuccessResult<TValue> Success<TValue>(TValue value)
        where TValue : notnull
    {
        return new SuccessResult<TValue>(value);
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <returns>A <see cref="SuccessResult{TValue}"/>.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static SuccessResultOption<TValue?> SuccessOption<TValue>()
    {
        return new SuccessResultOption<TValue?>(default);
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="SuccessResult{TValue}"/>.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static SuccessResultOption<TValue?> SuccessOption<TValue>(TValue? value)
        where TValue : struct
    {
        return new SuccessResultOption<TValue?>(value);
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="SuccessResult{TValue}"/>.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static SuccessResultOption<TValue?> SuccessOption<TValue>(TValue? value)
        where TValue : class
    {
        return new SuccessResultOption<TValue?>(value);
    }

    /// <summary>
    /// Creates an erroneous result.
    /// </summary>
    /// <returns>A <see cref="R" />.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static ErrorResult Error()
    {
        return default;
    }

    /// <summary>
    /// Creates an erroneous result.
    /// </summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="error">The error.</param>
    /// <returns>A <see cref="R" />.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static ErrorResult<TError> Error<TError>(TError error)
    {
        return new ErrorResult<TError>(error);
    }

    /// <summary>
    /// Creates an erroneous result.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="error">The error.</param>
    /// <returns>A <see cref="R" />.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static R<TValue, TError> Error<TValue, TError>(TError error)
    {
        return new ErrorResult<TError>(error);
    }

    /// <summary>Creates a result based on the specified values.</summary>
    /// <typeparam name="TSuccess">The type of the value.</typeparam>
    /// <param name="isSuccess">The is success.</param>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="R"/>.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static R<TSuccess> FromValue<TSuccess>(bool isSuccess, TSuccess value)
    {
        return new R<TSuccess>(isSuccess, value);
    }

    /// <summary>Creates a result based on the specified values.</summary>
    /// <typeparam name="TSuccess">The type of the error.</typeparam>
    /// <param name="isSuccess">The is success.</param>
    /// <param name="valueFunc">The value function.</param>
    /// <returns>A <see cref="R"/>.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static R<TSuccess> FromValue<TSuccess>(bool isSuccess, Func<TSuccess> valueFunc)
    {
        return new R<TSuccess>(isSuccess, isSuccess ? valueFunc() : default!);
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
    public static R<TSuccess> FromValue<TSuccess>(TSuccess? value)
    {
        return new R<TSuccess>(value != null, value);
    }

    /// <summary>Creates a result based on the specified values.</summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="isSuccess">The is success.</param>
    /// <param name="error">The error.</param>
    /// <returns>A <see cref="R"/>.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static RoE<TError> FromError<TError>(bool isSuccess, TError error)
    {
        return new RoE<TError>(isSuccess, error);
    }

    /// <summary>Creates a result based on the specified values.</summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="isSuccess">The is success.</param>
    /// <param name="errorFunc">The error func.</param>
    /// <returns>A <see cref="R"/>.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static RoE<TError> FromError<TError>(bool isSuccess, Func<TError> errorFunc)
    {
        return new RoE<TError>(isSuccess, isSuccess ? default! : errorFunc());
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
    public static RoE<TError> FromError<TError>(TError? error)
    {
        return new RoE<TError>(error == null, error);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="errorFunc">The error func.</param>
    /// <returns>
    /// A <see cref="R" />.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static R<TValue, TError> From<TValue, TError>(TValue? value, Func<TError> errorFunc)
        where TValue : class
    {
        if (value == null)
        {
            return R.Error(errorFunc());
        }

        return R.Success(value);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="errorFunc">The error func.</param>
    /// <returns>
    /// A <see cref="R" />.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static R<TValue, TError> FromValue<TValue, TError>(TValue value, Func<TError> errorFunc)
        where TValue : struct, IEquatable<TValue>
    {
        if (value.Equals(default))
        {
            return R.Error(errorFunc());
        }

        return R.Success(value);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="isSuccess">If set to <c>true</c> [success].</param>
    /// <param name="value">The result value.</param>
    /// <param name="error">The error.</param>
    /// <returns>
    /// A <see cref="R" />.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static R<TValue, TError> From<TValue, TError>(bool isSuccess, TValue value, TError error)
    {
        return new R<TValue, TError>(isSuccess, value, error);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="isSuccess">If set to <c>true</c> [success].</param>
    /// <param name="value">The result value.</param>
    /// <param name="error">The error.</param>
    /// <returns>
    /// A <see cref="R" />.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static ValueTask<R<TValue, TError>> FromAsync<TValue, TError>(bool isSuccess, TValue value, TError error)
    {
        return new R<TValue, TError>(isSuccess, value, error).ToValueTask();
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="isSuccess">If set to <c>true</c> [success].</param>
    /// <param name="valueFunc">The value func.</param>
    /// <param name="errorFunc">The error func.</param>
    /// <returns>
    /// A <see cref="R" />.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static R<TValue, TError> From<TValue, TError>(bool isSuccess, Func<TValue> valueFunc, Func<TError> errorFunc)
    {
        return new R<TValue, TError>(isSuccess, isSuccess ? valueFunc() : default, isSuccess ? default : errorFunc());
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="isSuccess">If set to <c>true</c> [success].</param>
    /// <param name="valueFunc">The value func.</param>
    /// <param name="errorFunc">The error func.</param>
    /// <returns>
    /// A <see cref="R" />.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static ValueTask<R<TValue, TError>> FromAsync<TValue, TError>(bool isSuccess, Func<TValue> valueFunc, Func<TError> errorFunc)
    {
        return new R<TValue, TError>(isSuccess, isSuccess ? valueFunc() : default, isSuccess ? default : errorFunc()).ToValueTask();
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>
    /// A <see cref="SuccessResult" />.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static ValueTask<SuccessResult> SuccessAsync()
    {
        return SuccessResult.Result;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="SuccessResult{TValue}"/>.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static ValueTask<SuccessResult<TValue>> SuccessAsync<TValue>(TValue value)
        where TValue : notnull
    {
        return new SuccessResult<TValue>(value);
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="SuccessResult{TValue}"/>.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static ValueTask<R<TValue, TError>> SuccessAsync<TValue, TError>(TValue value)
        where TValue : notnull
    {
        return new ValueTask<R<TValue, TError>>(Success(value));
    }

    /// <summary>
    /// Creates an erroneous result.
    /// </summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="error">The error.</param>
    /// <returns>A <see cref="R" />.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static ValueTask<ErrorResult<TError>> ErrorAsync<TError>(TError error)
        where TError : notnull
    {
        return new ErrorResult<TError>(error).ToValueTask();
    }

    /// <summary>
    /// Creates an erroneous result.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="error">The error.</param>
    /// <returns>A <see cref="R" />.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static ValueTask<R<TValue, TError>> ErrorAsync<TValue, TError>(TError error)
        where TError : notnull
    {
        return new ValueTask<R<TValue, TError>>(R.Error(error));
    }

    /// <summary>Creates a result based on the specified values.</summary>
    /// <typeparam name="TSuccess">The type of the value.</typeparam>
    /// <param name="isSuccess">The is success.</param>
    /// <param name="valueFunc">The value func.</param>
    /// <returns>A <see cref="R"/>.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static async ValueTask<R<TSuccess>> FromValueAsync<TSuccess>(bool isSuccess, Func<ValueTask<TSuccess>> valueFunc)
    {
        return new R<TSuccess>(isSuccess, isSuccess ? await valueFunc().ConfigureAwait(false) : default!);
    }

    /// <summary>Creates a result based on the specified values.</summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="isSuccess">The is success.</param>
    /// <param name="error">The error.</param>
    /// <returns>A <see cref="R"/>.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static ValueTask<R<TError>> FromValueAsync<TError>(bool isSuccess, TError error)
    {
        return new R<TError>(isSuccess, error);
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
    public static ValueTask<R<TSuccess>> FromValueAsync<TSuccess>(TSuccess? value)
        where TSuccess : class
    {
        return new R<TSuccess>(value != null, value);
    }

    /// <summary>Creates a result based on the specified values.</summary>
    /// <typeparam name="TSuccess">The type of the value.</typeparam>
    /// <param name="isSuccess">The is success.</param>
    /// <param name="valueFunc">The value function.</param>
    /// <returns>A <see cref="R"/>.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static async ValueTask<RoE<TSuccess>> FromErrorAsync<TSuccess>(bool isSuccess, Func<ValueTask<TSuccess>> valueFunc)
    {
        return new RoE<TSuccess>(isSuccess, isSuccess ? await valueFunc().ConfigureAwait(false) : default!);
    }

    /// <summary>Creates a result based on the specified values.</summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="isSuccess">The is success.</param>
    /// <param name="error">The error.</param>
    /// <returns>A <see cref="R"/>.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static ValueTask<RoE<TError>> FromErrorAsync<TError>(bool isSuccess, TError error)
    {
        return new RoE<TError>(isSuccess, error);
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
    public static ValueTask<RoE<TError>> FromErrorAsync<TError>(TError? error)
        where TError : class
    {
        return new RoE<TError>(error == null, error);
    }

    /// <summary>
    /// Converts from <see cref="R{TValue, TError}"/> to <see cref="R{TValue, TError}"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="result">The result.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static R<TValue, TError>? ToOption<TValue, TError>(this R<TValue, TError> result)
    {
        return result;
    }

    /// <summary>
    /// Converts from <see cref="R{TValue, TError}"/> to <see cref="R{TValue, TError}"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="result">The result.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static R<TValue, TError>? ToResultOption<TValue, TError>(this R<TValue?, TError> result)
    {
        if (!result.IsSuccess)
        {
            return new R<TValue, TError>(false, default, result.Error);
        }

        var value = result.Value;
        if (value != null)
        {
            return new R<TValue, TError>(true, value, result.Error);
        }

        return null;
    }

    /// <summary>
    /// Converts from <see cref="R{TValue, TError}"/> to <see cref="R{TValue, TError}"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="result">The result.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static R<TValue?, TError> ToOptionResult<TValue, TError>(this R<TValue, TError> result)
    {
        return new R<TValue?, TError>(result.IsSuccess, result.Value, result.Error);
    }

    /// <summary>
    /// Converts from <see cref="R{TValue, TError}"/> to <see cref="R{TValue, TError}"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="result">The result.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static R<TValue?, TError> ToValueOptionResult<TValue, TError>(this R<TValue, TError> result)
        where TValue : struct
    {
        return new R<TValue?, TError>(result.IsSuccess, result.Value, result.Error);
    }
}
