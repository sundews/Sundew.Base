// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IArguments.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

using System;
using System.Text;
using Sundew.DiscriminatedUnions;

/// <summary>
/// Represents a value.
/// </summary>
[DiscriminatedUnion]
public partial interface IArguments
{
    /// <summary>
    /// Appends this <see cref="ValueId"/> to the specified <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="appendOptions">The append options.</param>
    void AppendInto(StringBuilder stringBuilder, IFormatProvider formatProvider, AppendOptions appendOptions);

    /// <summary>
    /// Converts the current instance to a collection of value identifiers.
    /// </summary>
    /// <returns>A <see cref="ComplexValue"/> object representing the value identifiers, or null if the instance does not correspond
    /// to any value identifiers.</returns>
    ComplexValue ToValueIds()
    {
        return this switch
        {
            ArrayValue arrayValue => new ComplexValue([new ValueId(null, null, arrayValue)]),
            ScalarValue singleValue => new ComplexValue([new ValueId(null, null, singleValue)]),
            ComplexValue valueIds => valueIds,
        };
    }
}