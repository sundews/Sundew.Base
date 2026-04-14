// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValueIdentifiable.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

using System;

/// <summary>
/// Interface for implementing a value identifiable.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
public interface IValueIdentifiable<TValue> : IIdentifiable<ValueId>
{
    /// <summary>
    /// Creates a value from the value id.
    /// </summary>
    /// <param name="value">The initial value.</param>
    /// <param name="valueId">The value id.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns>The created value.</returns>
    static abstract TValue From(TValue value, ValueId valueId, IFormatProvider? formatProvider);
}