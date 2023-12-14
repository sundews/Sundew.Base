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
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="isSuccess">The is success.</param>
    /// <param name="error">The error.</param>
    /// <returns>A <see cref="R"/>.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static R<TError> From<TError>(bool isSuccess, TError error)
    {
        return new R<TError>(isSuccess, error);
    }

    /// <summary>Creates a result based on the specified values.</summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="isSuccess">The is success.</param>
    /// <param name="errorFunc">The error func.</param>
    /// <returns>A <see cref="R"/>.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static R<TError> From<TError>(bool isSuccess, Func<TError> errorFunc)
    {
        return new R<TError>(isSuccess, isSuccess ? default! : errorFunc());
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
    public static R<TError> From<TError>(TError? error)
    {
        return new R<TError>(error == null, error);
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
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="isSuccess">The is success.</param>
    /// <param name="errorFunc">The error func.</param>
    /// <returns>A <see cref="R"/>.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static async ValueTask<R<TError>> FromAsync<TError>(bool isSuccess, Func<ValueTask<TError>> errorFunc)
    {
        return new R<TError>(isSuccess, isSuccess ? default! : await errorFunc().ConfigureAwait(false));
    }

    /// <summary>Creates a result based on the specified values.</summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="isSuccess">The is success.</param>
    /// <param name="error">The error.</param>
    /// <returns>A <see cref="R"/>.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static ValueTask<R<TError>> FromAsync<TError>(bool isSuccess, TError error)
    {
        return new R<TError>(isSuccess, error);
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
    public static ValueTask<R<TError>> FromAsync<TError>(TError? error)
        where TError : class
    {
        return new R<TError>(error == null, error);
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
    public static R<TValue, TError>? ToOptionalResult<TValue, TError>(this R<TValue?, TError> result)
        where TValue : struct
    {
        if (!result.IsSuccess)
        {
            return new R<TValue, TError>(false, default, result.Error);
        }

        if (result.Value.HasValue)
        {
            return new R<TValue, TError>(true, result.Value.Value, result.Error);
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
    public static R<TValue, TError>? ToOptionalResult<TValue, TError>(this R<TValue?, TError> result)
        where TValue : class
    {
        if (!result.IsSuccess)
        {
            return new R<TValue, TError>(false, default, result.Error);
        }

        if (result.Value.TryGetValue(out var value))
        {
            return new R<TValue, TError>(true, value, result.Error);
        }

        return null;
    }

    /*
    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="SuccessResult{TValue}"/>.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static SuccessResult<TValue> ToSuccess<TValue>(this TValue value)
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
    /// <returns>A <see cref="R{TValue, TError}"/>.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static R<TValue, TError> ToSuccess<TValue, TError>(this TValue value)
        where TValue : notnull
    {
        return value.ToSuccess();
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="SuccessResult{TValue}"/>.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static SuccessResultOption<TValue?> ToSuccessOption<TValue>(this TValue? value)
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
    public static SuccessResultOption<TValue?> ToSuccessOption<TValue>(this TValue? value)
        where TValue : class
    {
        return new SuccessResultOption<TValue?>(value);
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="R{TValue, TError}"/>.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static R<TValue?, TError> ToSuccessOption<TValue, TError>(this TValue? value)
        where TValue : struct
    {
        return new SuccessResultOption<TValue?>(value);
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="R{TValue, TError}"/>.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static R<TValue?, TError> ToSuccessOption<TValue, TError>(this TValue? value)
        where TValue : class
    {
        return value.ToSuccessOption();
    }

    /// <summary>
    /// Creates an erroneous result.
    /// </summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="error">The error.</param>
    /// <returns>A <see cref="R" />.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static ErrorResult<TError> ToError<TError>(this TError error)
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
    public static R<TValue, TError> ToError<TValue, TError>(this TError error)
    {
        return error.ToError();
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="SuccessResult{TValue}"/>.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static ValueTask<SuccessResult<TValue>> ToSuccessAsync<TValue>(this TValue value)
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
    public static ValueTask<R<TValue, TError>> ToSuccessAsync<TValue, TError>(this TValue value)
        where TValue : notnull
    {
        return new ValueTask<R<TValue, TError>>(value.ToSuccess());
    }

    /// <summary>
    /// Creates an erroneous result.
    /// </summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="error">The error.</param>
    /// <returns>A <see cref="R" />.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static ValueTask<ErrorResult<TError>> ToErrorAsync<TError>(this TError error)
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
    public static ValueTask<R<TValue, TError>> ToErrorAsync<TValue, TError>(this TError error)
        where TError : notnull
    {
        return new ValueTask<R<TValue, TError>>(error.ToError());
    }*/
}
