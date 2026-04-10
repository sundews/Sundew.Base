// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValue.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

using System;
using System.Runtime.CompilerServices;
using Sundew.DiscriminatedUnions;

/// <summary>
/// Represents a value.
/// </summary>
[DiscriminatedUnion]
public partial interface IValue : IArguments
{
    /// <summary>
    /// Gets the value from the arguments.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="referenceName">The argument name.</param>
    /// <returns>The retrieved value or the default value.</returns>
    public TValue Get<TValue>(
        TValue defaultValue,
        IFormatProvider formatProvider,
        [CallerArgumentExpression(nameof(defaultValue))] string? referenceName = null)
        where TValue : IParsable<TValue>;

    /// <summary>
    /// Gets the value from the arguments.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="referenceName">The argument name.</param>
    /// <returns>The retrieved value or the default value.</returns>
    public TValue Get2<TValue>(
        TValue defaultValue,
        IFormatProvider formatProvider,
        [CallerArgumentExpression(nameof(defaultValue))] string? referenceName = null)
        where TValue : IValueIdentifiable<TValue>;
}