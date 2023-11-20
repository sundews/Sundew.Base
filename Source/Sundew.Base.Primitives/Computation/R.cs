// --------------------------------------------------------------------------------------------------------------------
// <copyright file="R.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives.Computation;

using System;
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
    public static SuccessResultOption<TValue?> SuccessOption<TValue>(TValue? value)
    {
        return new SuccessResultOption<TValue?>(value);
    }

    /// <summary>
    /// Creates an erroneous result.
    /// </summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="error">The error.</param>
    /// <returns>A <see cref="R" />.</returns>
    public static ErrorResult<TError> Error<TError>(TError error)
    {
        return new ErrorResult<TError>(error);
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="SuccessResult{TValue}"/>.</returns>
    public static SuccessResult<TValue> ToSuccess<TValue>(this TValue value)
    {
        return new SuccessResult<TValue>(value);
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="SuccessResult{TValue}"/>.</returns>
    public static SuccessResultOption<TValue?> ToSuccessOption<TValue>(this TValue? value)
    {
        return new SuccessResultOption<TValue?>(value);
    }

    /// <summary>
    /// Creates an erroneous result.
    /// </summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="error">The error.</param>
    /// <returns>A <see cref="R" />.</returns>
    public static ErrorResult<TError> ToError<TError>(this TError error)
    {
        return new ErrorResult<TError>(error);
    }

    /// <summary>Creates a result based on the specified values.</summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="isSuccess">The is success.</param>
    /// <param name="error">The error.</param>
    /// <returns>A <see cref="R"/>.</returns>
    public static R<TError> From<TError>(bool isSuccess, TError error)
    {
        return new R<TError>(isSuccess, error);
    }

    /// <summary>Creates a result based on the specified values.</summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="isSuccess">The is success.</param>
    /// <param name="errorFunc">The error func.</param>
    /// <returns>A <see cref="R"/>.</returns>
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
    public static R<TError> From<TError>(TError? error)
    {
        return new R<TError>(error == null, error);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="error">The error.</param>
    /// <returns>
    /// A <see cref="R" />.
    /// </returns>
    public static R<TError> ToResult<TError>(this TError? error)
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
    public static ValueTask<SuccessResult<TValue>> SuccessAsync<TValue>(TValue value)
    {
        return new SuccessResult<TValue>(value);
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="SuccessResult{TValue}"/>.</returns>
    public static ValueTask<SuccessResult<TValue>> ToSuccessAsync<TValue>(this TValue value)
    {
        return new SuccessResult<TValue>(value);
    }

    /// <summary>
    /// Creates an erroneous result.
    /// </summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="error">The error.</param>
    /// <returns>A <see cref="R" />.</returns>
    public static ValueTask<ErrorResult<TError>> ErrorAsync<TError>(TError error)
    {
        return new ErrorResult<TError>(error).ToValueTask();
    }

    /// <summary>
    /// Creates an erroneous result.
    /// </summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="error">The error.</param>
    /// <returns>A <see cref="R" />.</returns>
    public static ValueTask<ErrorResult<TError>> ToErrorAsync<TError>(this TError error)
    {
        return new ErrorResult<TError>(error).ToValueTask();
    }

    /// <summary>Creates a result based on the specified values.</summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="isSuccess">The is success.</param>
    /// <param name="errorFunc">The error func.</param>
    /// <returns>A <see cref="R"/>.</returns>
    public static async ValueTask<R<TError>> FromAsync<TError>(bool isSuccess, Func<ValueTask<TError>> errorFunc)
    {
        return new R<TError>(isSuccess, isSuccess ? default! : await errorFunc().ConfigureAwait(false));
    }

    /// <summary>Creates a result based on the specified values.</summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="isSuccess">The is success.</param>
    /// <param name="error">The error.</param>
    /// <returns>A <see cref="R"/>.</returns>
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
    public static ValueTask<R<TError>> FromAsync<TError>(TError? error)
        where TError : class
    {
        return new R<TError>(error == null, error);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="error">The error.</param>
    /// <returns>
    /// A <see cref="R" />.
    /// </returns>
    public static ValueTask<R<TError>> ToResultAsync<TError>(this TError? error)
        where TError : class
    {
        return new R<TError>(error == null, error);
    }
}
