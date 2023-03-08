// --------------------------------------------------------------------------------------------------------------------
// <copyright file="O.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives.Computation;

using System.Threading.Tasks;

/// <summary>
/// Factory class for creating results.
/// </summary>
public static partial class O
{
    /// <summary>
    /// Creates an none option.
    /// </summary>
    /// <returns>A new <see cref="R"/>.</returns>
    public static NoneOption None()
    {
        return NoneOption.None;
    }

    /// <summary>
    /// Creates a error result.
    /// </summary>
    /// <returns>A new <see cref="R"/>.</returns>
    public static ValueTask<NoneOption> NoneAsync()
    {
        return NoneOption.None.ToValueTask();
    }

    /// <summary>
    /// Creates a option with a value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="O"/>.</returns>
    public static O<TValue> Some<TValue>(TValue value)
    {
        return new O<TValue>(true, value);
    }

    /// <summary>
    /// Creates a option with a value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="R.SuccessResult{TValue}"/>.</returns>
    public static ValueTask<O<TValue>> SomeAsync<TValue>(TValue value)
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
    /// A <see cref="O" />.
    /// </returns>
    public static O<TValue> From<TValue>(bool hasValue, TValue value)
    {
        return new O<TValue>(hasValue, hasValue ? value : default!);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>
    /// A <see cref="O" />.
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
    /// <param name="hasValue">The is success.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// A <see cref="O" />.
    /// </returns>
    public static ValueTask<O<TValue>> FromAsync<TValue>(bool hasValue, TValue value)
    {
        return new O<TValue>(hasValue, hasValue ? value : default!).ToValueTask();
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>
    /// A <see cref="O" />.
    /// </returns>
    public static ValueTask<O<TValue>> FromAsync<TValue>(TValue? value)
        where TValue : class
    {
        return new O<TValue>(value != null, value).ToValueTask();
    }
}