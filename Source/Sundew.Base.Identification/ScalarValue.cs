// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScalarValue.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

using System;
using System.Globalization;
using System.Text;

/// <summary>
/// Represents an argument in a <see cref="Id"/>.
/// </summary>
/// <param name="Value">The value.</param>
public sealed partial record ScalarValue(string Value) : IValue
{
    /// <summary>
    /// Appends this <see cref="ValueId"/> to the specified <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="appendOptions">The append options.</param>
    public void AppendInto(StringBuilder stringBuilder, IFormatProvider formatProvider, AppendOptions appendOptions)
    {
        stringBuilder.Append(this.Value);
    }

    /// <summary>
    /// Gets the value from the arguments.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="referenceName">The argument name.</param>
    /// <returns>The retrieved value or the default value.</returns>
    public TValue Get<TValue>(TValue defaultValue, IFormatProvider formatProvider, string? referenceName = null)
        where TValue : IParsable<TValue>
    {
        return defaultValue;
    }

    /// <summary>
    /// Gets the value from the arguments.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="referenceName">The argument name.</param>
    /// <returns>The retrieved value or the default value.</returns>
    public TValue Get2<TValue>(TValue defaultValue, IFormatProvider formatProvider, string? referenceName = null)
        where TValue : IValueIdentifiable<TValue>
    {
        return defaultValue;
    }

    /// <summary>
    /// Creates a string representation of the <see cref="ValueId"/>.
    /// </summary>
    /// <returns>A string.</returns>
    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        this.AppendInto(stringBuilder, CultureInfo.CurrentCulture, new AppendOptions(true));
        return stringBuilder.ToString();
    }
}