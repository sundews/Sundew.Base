// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Result.TrueFalse.cs" company="Hukano">
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
        /// Creates a true result.
        /// </summary>
        /// <returns>
        /// A <see cref="TrueResult" />.
        /// </returns>
        public static TrueResult True()
        {
            return TrueResult.Result;
        }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Result{TValue}"/>.</returns>
        public static Result<TValue> True<TValue>(TValue value)
        {
            return new Result<TValue>(true, value);
        }

        /// <summary>
        /// Creates an value result.
        /// </summary>
        /// <returns>A new <see cref="Result"/>.</returns>
        public static FalseResult False()
        {
            return FalseResult.Result;
        }

        /// <summary>
        /// Creates an erroneous result.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Result{TValue}" />.</returns>
        public static Result<TValue> False<TValue>(TValue value)
        {
            return new Result<TValue>(false, value);
        }

        /// <summary>
        /// Creates a result based on the specified values.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="isSuccess">If set to <c>true</c> [success].</param>
        /// <param name="value">The result value.</param>
        /// <returns>
        /// A <see cref="Result{TValue}" />.
        /// </returns>
        public static Result<TValue> From<TValue>(bool isSuccess, TValue value)
        {
            return new Result<TValue>(isSuccess, value);
        }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <returns>
        /// A <see cref="SuccessResult" />.
        /// </returns>
        public static ValueTask<TrueResult> TrueAsync()
        {
            return TrueResult.Result.ToValueTask();
        }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="SuccessResult{TValue}"/>.</returns>
        public static ValueTask<Result<TValue>> TrueAsync<TValue>(TValue value)
        {
            return new Result<TValue>(true, value).ToValueTask();
        }

        /// <summary>
        /// Creates a value result.
        /// </summary>
        /// <returns>A new <see cref="Result"/>.</returns>
        public static ValueTask<FalseResult> FalseAsync()
        {
            return FalseResult.Result.ToValueTask();
        }

        /// <summary>
        /// Creates an erroneous result.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Result{TValue,TError}" />.</returns>
        public static ValueTask<ErrorResult<TValue>> FalseAsync<TValue>(TValue value)
        {
            return new ErrorResult<TValue>(value).ToValueTask();
        }

        /// <summary>
        /// Creates a result based on the specified values.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="isSuccess">If set to <c>true</c> [success].</param>
        /// <param name="value">The result value.</param>
        /// <returns>
        /// A <see cref="Result{TValue}" />.
        /// </returns>
        public static ValueTask<Result<TValue>> FromAsync<TValue>(bool isSuccess, TValue value)
        {
            return new Result<TValue>(isSuccess, value).ToValueTask();
        }
    }
}