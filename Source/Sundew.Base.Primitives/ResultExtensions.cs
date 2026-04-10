// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base;

using System;

/// <summary>
/// Extensions for <see cref="R{TSuccess}"/> and <see cref="R{TSuccess,TError}"/>.
/// </summary>
public static class ResultExtensions
{
#pragma warning disable SA1101

    /// <summary>
    /// Gets a new result that is successful, if both results were a success.
    /// </summary>
    /// <typeparam name="TSuccess">The success 1 type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="otherResultFunc">The otherResultFunc result.</param>
    /// <returns>A new result.</returns>
    public static R<TSuccess, TError> And<TSuccess, TError>(this R<TSuccess, TError> result, Func<RoE<TError>> otherResultFunc)
    {
        if (result.IsSuccess)
        {
            var otherResult = otherResultFunc();
            if (otherResult.IsSuccess)
            {
                return result;
            }

            return new R<TSuccess, TError>(false, default, otherResult.Error);
        }

        return result;
    }

    /// <summary>
    /// Gets a new result that is successful, if both results were a success.
    /// </summary>
    /// <typeparam name="TSuccess1">The success 1 type.</typeparam>
    /// <typeparam name="TSuccess2">The success 2 type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="otherResultFunc">The otherResultFunc result.</param>
    /// <returns>A new result.</returns>
    public static R<(TSuccess1 Value1, TSuccess2 Value2), TError> And<TSuccess1, TSuccess2, TError>(this R<TSuccess1, TError> result, Func<R<TSuccess2, TError>> otherResultFunc)
    {
        if (result.IsSuccess)
        {
            var otherResult = otherResultFunc();
            if (otherResult.IsSuccess)
            {
                return new R<(TSuccess1, TSuccess2), TError>(true, (result.Value, otherResult.Value), otherResult.Error);
            }

            return new R<(TSuccess1, TSuccess2), TError>(false, default, otherResult.Error);
        }

        return new R<(TSuccess1, TSuccess2), TError>(false, default, result.Error);
    }

    /// <summary>
    /// Gets a new result that is successful, if both results were a success.
    /// </summary>
    /// <typeparam name="TSuccess1">The success 1 type.</typeparam>
    /// <typeparam name="TSuccess2">The success 2 type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="otherResultFunc">The other result func.</param>
    /// <returns>A new result.</returns>
    public static R<(TSuccess1 Value1, TSuccess2 Value2), TError> And<TSuccess1, TSuccess2, TError>(this R<(TSuccess1 Value1, TSuccess2 Value2), TError> result, Func<RoE<TError>> otherResultFunc)
    {
        if (result.IsSuccess)
        {
            var otherResult = otherResultFunc();
            if (otherResult.IsSuccess)
            {
                return result;
            }

            return new R<(TSuccess1 Value1, TSuccess2 Value2), TError>(false, default, otherResult.Error);
        }

        return result;
    }

    /// <summary>
    /// Gets a new result that is successful, if both results were a success.
    /// </summary>
    /// <typeparam name="TSuccess1">The success 1 type.</typeparam>
    /// <typeparam name="TSuccess2">The success 2 type.</typeparam>
    /// <typeparam name="TSuccess3">The success 3 type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="otherResultFunc">The otherResultFunc result.</param>
    /// <returns>A new result.</returns>
    public static R<(TSuccess1 Value1, TSuccess2 Value2, TSuccess3 Value3), TError> And<TSuccess1, TSuccess2, TSuccess3, TError>(this R<(TSuccess1 Value1, TSuccess2 Value2), TError> result, Func<R<TSuccess3, TError>> otherResultFunc)
    {
        if (result.IsSuccess)
        {
            var otherResult = otherResultFunc();
            if (otherResult.IsSuccess)
            {
                return new R<(TSuccess1, TSuccess2, TSuccess3), TError>(true, (result.Value.Value1, result.Value.Value2, otherResult.Value), default);
            }

            return new R<(TSuccess1, TSuccess2, TSuccess3), TError>(false, default, otherResult.Error);
        }

        return new R<(TSuccess1, TSuccess2, TSuccess3), TError>(false, default, result.Error);
    }

    /// <summary>
    /// Gets a new result that is successful, if both results were a success.
    /// </summary>
    /// <typeparam name="TSuccess1">The success 1 type.</typeparam>
    /// <typeparam name="TSuccess2">The success 2 type.</typeparam>
    /// <typeparam name="TSuccess3">The success 3 type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="otherResultFunc">The other result func.</param>
    /// <returns>A new result.</returns>
    public static R<(TSuccess1 Value1, TSuccess2 Value2, TSuccess3 Value3), TError> And<TSuccess1, TSuccess2, TSuccess3, TError>(this R<(TSuccess1 Value1, TSuccess2 Value2, TSuccess3 Value3), TError> result, Func<RoE<TError>> otherResultFunc)
    {
        if (result.IsSuccess)
        {
            var otherResult = otherResultFunc();
            if (otherResult.IsSuccess)
            {
                return result;
            }

            return new R<(TSuccess1, TSuccess2, TSuccess3), TError>(false, default, otherResult.Error);
        }

        return result;
    }

    /// <summary>
    /// Gets a new result that is successful, if both results were a success.
    /// </summary>
    /// <typeparam name="TSuccess1">The success 1 type.</typeparam>
    /// <typeparam name="TSuccess2">The success 2 type.</typeparam>
    /// <typeparam name="TSuccess3">The success 3 type.</typeparam>
    /// <typeparam name="TSuccess4">The success 4 type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="otherResultFunc">The other result func.</param>
    /// <returns>A new result.</returns>
    public static R<(TSuccess1 Value1, TSuccess2 Value2, TSuccess3 Value3, TSuccess4 Value4), TError> And<TSuccess1, TSuccess2, TSuccess3, TSuccess4, TError>(this R<(TSuccess1 Value1, TSuccess2 Value2, TSuccess3 Value3), TError> result, Func<R<TSuccess4, TError>> otherResultFunc)
    {
        if (result.IsSuccess)
        {
            var otherResult = otherResultFunc();
            if (otherResult.IsSuccess)
            {
                return new R<(TSuccess1, TSuccess2, TSuccess3, TSuccess4), TError>(true, (result.Value.Value1, result.Value.Value2, result.Value.Value3, otherResult.Value), default);
            }

            return new R<(TSuccess1, TSuccess2, TSuccess3, TSuccess4), TError>(false, default, otherResult.Error);
        }

        return new R<(TSuccess1, TSuccess2, TSuccess3, TSuccess4), TError>(true, default, result.Error);
    }
#pragma warning restore SA1101
}