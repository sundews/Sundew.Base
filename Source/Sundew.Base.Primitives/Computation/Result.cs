// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Result.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Computation
{
    using System.Threading.Tasks;

    /// <summary>
    /// Factory class for creating results.
    /// </summary>
    public static partial class Result
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
        /// Creates an error result.
        /// </summary>
        /// <returns>A new <see cref="Result"/>.</returns>
        public static ErrorResult Error()
        {
            return ErrorResult.Result;
        }

        /// <summary>
        /// Creates an erroneous result.
        /// </summary>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="error">The error.</param>
        /// <returns>A <see cref="Result{TValue,TError}" />.</returns>
        public static ErrorResult<TError> Error<TError>(TError error)
        {
            return new ErrorResult<TError>(error);
        }

        /// <summary>Creates a result based on the specified values.</summary>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="isSuccess">The is success.</param>
        /// <param name="error">The error.</param>
        /// <returns>A <see cref="IfError{TError}"/>.</returns>
        public static IfError<TError> FromError<TError>(bool isSuccess, TError error)
        {
            return new IfError<TError>(isSuccess, error);
        }

        /// <summary>
        /// Creates a result based on the specified values.
        /// </summary>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="error">The error.</param>
        /// <returns>
        /// A <see cref="IfError{TError}" />.
        /// </returns>
        public static IfError<TError> FromError<TError>(TError error)
        {
            return new IfError<TError>(Equals(error, default(TError)!), error);
        }

        /// <summary>
        /// Creates a result based on the specified values.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="isSuccess">The is success.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A <see cref="IfSuccess{TValue}" />.
        /// </returns>
        public static IfSuccess<TValue> FromValue<TValue>(bool isSuccess, TValue value)
        {
            return new IfSuccess<TValue>(isSuccess, value);
        }

        /// <summary>
        /// Creates a result based on the specified values.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A <see cref="IfSuccess{TValue}" />.
        /// </returns>
        public static IfSuccess<TValue> FromValue<TValue>(TValue value)
        {
            return new IfSuccess<TValue>(!Equals(value, default(TValue)!), value);
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
        /// A <see cref="Result{TValue, TError}" />.
        /// </returns>
        public static Result<TValue, TError> From<TValue, TError>(bool isSuccess, TValue value, TError error)
        {
            return new Result<TValue, TError>(isSuccess, value, error);
        }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <returns>
        /// A <see cref="SuccessResult" />.
        /// </returns>
        public static ValueTask<SuccessResult> SuccessAsync()
        {
            return SuccessResult.Result.ToValueTask();
        }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="SuccessResult{TValue}"/>.</returns>
        public static ValueTask<SuccessResult<TValue>> SuccessAsync<TValue>(TValue value)
        {
            return new SuccessResult<TValue>(value).ToValueTask();
        }

        /// <summary>
        /// Creates a error result.
        /// </summary>
        /// <returns>A new <see cref="Result"/>.</returns>
        public static ValueTask<ErrorResult> ErrorAsync()
        {
            return ErrorResult.Result.ToValueTask();
        }

        /// <summary>
        /// Creates an erroneous result.
        /// </summary>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="error">The error.</param>
        /// <returns>A <see cref="Result{TValue,TError}" />.</returns>
        public static ValueTask<ErrorResult<TError>> ErrorAsync<TError>(TError error)
        {
            return new ErrorResult<TError>(error).ToValueTask();
        }

        /// <summary>Creates a result based on the specified values.</summary>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="isSuccess">The is success.</param>
        /// <param name="error">The error.</param>
        /// <returns>A <see cref="IfError{TError}"/>.</returns>
        public static ValueTask<IfError<TError>> FromErrorAsync<TError>(bool isSuccess, TError error)
        {
            return new IfError<TError>(isSuccess, error);
        }

        /// <summary>
        /// Creates a result based on the specified values.
        /// </summary>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="error">The error.</param>
        /// <returns>
        /// A <see cref="IfError{TError}" />.
        /// </returns>
        public static ValueTask<IfError<TError>> FromErrorAsync<TError>(TError error)
        {
            return new IfError<TError>(Equals(error, default(TError)!), error).ToValueTask();
        }

        /// <summary>
        /// Creates a result based on the specified values.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="isSuccess">The is success.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A <see cref="IfSuccess{TValue}" />.
        /// </returns>
        public static ValueTask<IfSuccess<TValue>> FromValueAsync<TValue>(bool isSuccess, TValue value)
        {
            return new IfSuccess<TValue>(isSuccess, value).ToValueTask();
        }

        /// <summary>
        /// Creates a result based on the specified values.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A <see cref="IfSuccess{TValue}" />.
        /// </returns>
        public static ValueTask<IfSuccess<TValue>> FromValueAsync<TValue>(TValue value)
        {
            return new IfSuccess<TValue>(!Equals(value, default(TValue)!), value).ToValueTask();
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
        /// A <see cref="Result{TValue, TError}" />.
        /// </returns>
        public static ValueTask<Result<TValue, TError>> FromAsync<TValue, TError>(bool isSuccess, TValue value, TError error)
        {
            return new Result<TValue, TError>(isSuccess, value, error).ToValueTask();
        }
    }
}