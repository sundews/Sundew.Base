// --------------------------------------------------------------------------------------------------------------------
// <copyright file="O.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives.Computation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Factory class for creating results.
/// </summary>
public static partial class O
{
    /// <summary>
    /// Gets a none option.
    /// </summary>
    /// <returns>A new <see cref="NoneOption"/>.</returns>
    public static NoneOption None => NoneOption.None;

    /// <summary>
    /// Creates a error result.
    /// </summary>
    /// <returns>A new <see cref="NoneOption"/>.</returns>
    public static ValueTask<NoneOption> NoneAsync()
    {
        return NoneOption.None.ToValueTask();
    }

    /// <summary>
    /// Creates a option with a value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="O{TValue}"/>.</returns>
    public static O<TValue> Some<TValue>(TValue value)
    {
        return new O<TValue>(true, value);
    }

    /// <summary>
    /// Creates a option with a value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="O{TValue}"/>.</returns>
    public static ValueTask<O<TValue>> SomeAsync<TValue>(TValue value)
    {
        return new O<TValue>(true, value).ToValueTask();
    }

    /// <summary>
    /// Creates a option with a value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="O{TValue}"/>.</returns>
    public static O<TValue> ToSome<TValue>(this TValue value)
    {
        return new O<TValue>(true, value);
    }

    /// <summary>
    /// Creates a option with a value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="O{TValue}"/>.</returns>
    public static ValueTask<O<TValue>> ToSomeAsync<TValue>(this TValue value)
    {
        return new O<TValue>(true, value).ToValueTask();
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="hasValue">The is success.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// A <see cref="O{TValue}" />.
    /// </returns>
    public static O<TValue> From<TValue>(bool hasValue, TValue value)
    {
        return new O<TValue>(hasValue, hasValue ? value : default!);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="hasValue">The is success.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// A <see cref="O{TValue}" />.
    /// </returns>
    public static O<TValue> From<TValue>(bool hasValue, Func<TValue> value)
    {
        return new O<TValue>(hasValue, hasValue ? value() : default!);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>
    /// A <see cref="O{TValue}" />.
    /// </returns>
    public static O<TValue> From<TValue>(TValue? value)
        where TValue : class
    {
        return new O<TValue>(value != null, value);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>
    /// A <see cref="O{TValue}" />.
    /// </returns>
    public static O<TValue> FromValue<TValue>(TValue value)
        where TValue : struct, IEquatable<TValue>
    {
        return !value.Equals(default) ? new O<TValue>(true, value) : None;
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>
    /// A <see cref="O{TValue}" />.
    /// </returns>
    public static O<TValue> From<TValue>(TValue? value)
        where TValue : struct, IEquatable<TValue>
    {
        return value.HasValue && !value.Value.Equals(default) ? new O<TValue>(true, value.Value) : None;
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>
    /// A <see cref="O{TValue}" />.
    /// </returns>
    public static O<TValue> ToOption<TValue>(this TValue? value)
        where TValue : class
    {
        return new O<TValue>(value != null, value);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>
    /// A <see cref="O{TValue}" />.
    /// </returns>
    public static O<TValue> ToOption<TValue>(this TValue? value)
        where TValue : struct
    {
        return value.HasValue ? new O<TValue>(true, value.Value) : None;
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="hasValue">The is success.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// A <see cref="O{TValue}" />.
    /// </returns>
    public static ValueTask<O<TValue>> FromAsync<TValue>(bool hasValue, TValue value)
    {
        return new O<TValue>(hasValue, hasValue ? value : default!).ToValueTask();
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="hasValue">The is success.</param>
    /// <param name="valueFunc">The value func.</param>
    /// <returns>
    /// A <see cref="O{TValue}" />.
    /// </returns>
    public static async ValueTask<O<TValue>> FromAsync<TValue>(bool hasValue, Func<Task<TValue>> valueFunc)
    {
        var value = hasValue ? await valueFunc().ConfigureAwait(false) : default;
        return new O<TValue>(hasValue, value);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>
    /// A <see cref="O{TValue}" />.
    /// </returns>
    public static ValueTask<O<TValue>> FromAsync<TValue>(TValue? value)
        where TValue : class
    {
        return new O<TValue>(value != null, value).ToValueTask();
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>
    /// A <see cref="O{TValue}" />.
    /// </returns>
    public static ValueTask<O<TValue>> FromAsync<TValue>(TValue? value)
        where TValue : struct
    {
        return value.ToOption();
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>
    /// A <see cref="O{TValue}" />.
    /// </returns>
    public static ValueTask<O<TValue>> ToOptionAsync<TValue>(this TValue? value)
        where TValue : class
    {
        return new O<TValue>(value != null, value).ToValueTask();
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>
    /// A <see cref="O{TValue}" />.
    /// </returns>
    public static ValueTask<O<TValue>> ToOptionAsync<TValue>(this TValue? value)
        where TValue : struct
    {
        return value.ToOption();
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
    public static bool IsSuccess<TSuccess, TError>(this in O<R<TSuccess, TError>> optionalResult, out O<TSuccess> valueOption, out R<TError> failedResult)
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

    /// <summary>
    /// Converts the option to an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="enumerableOption">The enumerable option.</param>
    /// <returns>The enumerable if the option contains one, otherwise and empty enumerable.</returns>
    public static IEnumerable<TValue> AsEnumerable<TValue>(this O<IEnumerable<TValue>> enumerableOption)
    {
        if (enumerableOption.HasValue)
        {
            return enumerableOption.Value;
        }

        return Enumerable.Empty<TValue>();
    }

    /// <summary>
    /// Converts the option to an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="option">The option.</param>
    /// <returns>The enumerable if the option contains one, otherwise and empty enumerable.</returns>
    public static TValue? AsNullable<TValue>(this O<TValue> option)
        where TValue : struct
    {
        if (option.HasValue)
        {
            return option.Value;
        }

        return null;
    }
}