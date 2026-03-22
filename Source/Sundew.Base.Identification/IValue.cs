// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValue.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

/// <summary>
/// Represents a value.
/// </summary>
public interface IValue
{
    /// <summary>
    /// Appends this <see cref="ValueId"/> to the specified <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="formatProvider">The format provider.</param>
    void AppendInto(StringBuilder stringBuilder, IFormatProvider formatProvider);

    /// <summary>
    /// Converts the current instance to a collection of value identifiers.
    /// </summary>
    /// <returns>A <see cref="ValueIds"/> object representing the value identifiers, or null if the instance does not correspond
    /// to any value identifiers.</returns>
    ValueIds ToValueIds()
    {
        return this switch
        {
            SingleValue singleValue => new ValueIds([new ValueId(null, null, singleValue)]),
            ValueIds valueIds => valueIds,
            _ => throw new NotSupportedException($"The type {this.GetType()} is not supported."),
        };
    }

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