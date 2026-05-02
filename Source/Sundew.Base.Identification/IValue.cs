// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValue.cs" company="Sundews">
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
public partial interface IValue
{
    /// <summary>
    /// Appends this <see cref="ValueId"/> to the specified <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="appendOptions">The append options.</param>
    void AppendInto(StringBuilder stringBuilder, IFormatProvider formatProvider, AppendOptions appendOptions);
}