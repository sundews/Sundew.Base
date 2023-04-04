// --------------------------------------------------------------------------------------------------------------------
// <copyright file="R.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives.Computation;

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
    {
        return new SuccessResult<TValue>(value);
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

    /// <summary>Creates a result based on the specified values.</summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="isSuccess">The is success.</param>
    /// <param name="error">The error.</param>
    /// <returns>A <see cref="R"/>.</returns>
    public static R<TError> FromError<TError>(bool isSuccess, TError? error)
    {
        return new R<TError>(isSuccess, isSuccess ? default! : error);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="error">The error.</param>
    /// <returns>
    /// A <see cref="R" />.
    /// </returns>
    public static R<TError> FromError<TError>(TError? error)
        where TError : class
    {
        return new R<TError>(error == null, error);
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
        return new R<TValue, TError>(isSuccess, isSuccess ? value : default!, isSuccess ? default! : error);
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
        return new R<TValue, TError>(isSuccess, isSuccess ? value : default!, isSuccess ? default! : error).ToValueTask();
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
    /// Creates an erroneous result.
    /// </summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="error">The error.</param>
    /// <returns>A <see cref="R" />.</returns>
    public static ValueTask<ErrorResult<TError>> ErrorAsync<TError>(TError error)
    {
        return new ErrorResult<TError>(error).ToValueTask();
    }

    /// <summary>Creates a result based on the specified values.</summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="isSuccess">The is success.</param>
    /// <param name="error">The error.</param>
    /// <returns>A <see cref="R"/>.</returns>
    public static ValueTask<R<TError>> FromErrorAsync<TError>(bool isSuccess, TError error)
    {
        return new R<TError>(isSuccess, isSuccess ? default! : error);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="error">The error.</param>
    /// <returns>
    /// A <see cref="R" />.
    /// </returns>
    public static ValueTask<R<TError>> FromErrorAsync<TError>(TError? error)
        where TError : class
    {
        return new R<TError>(error == null, error).ToValueTask();
    }

    /// <summary>
    /// Converts a <see cref="O{R}"/> to a O{TSuccess} and a R{TError} and returns a value indicating which of the two to process.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="optionalResult">The optional result.</param>
    /// <param name="valueOption">The value option.</param>
    /// <param name="failedResult">The failed result.</param>
    /// <returns><c>true</c>, if the <paramref name="valueOption"/> should be processed, or <c>false</c> if the <paramref name="failedResult"/> should be processed.</returns>
    public static bool IsSuccess<TSuccess, TError>(this O<R<TSuccess, TError>> optionalResult, out O<TSuccess> valueOption, out R<TError> failedResult)
    {
        if (optionalResult.HasValue)
        {
            if (optionalResult.Value.IsSuccess)
            {
                valueOption = O.Some(optionalResult.Value.Value);
                failedResult = R.Success();
                return true;
            }

            valueOption = O.None;
            failedResult = R.Error(optionalResult.Value.Error);
            return false;
        }

        valueOption = O.None;
        failedResult = R.Success();
        return true;
    }
}
