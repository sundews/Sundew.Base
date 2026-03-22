// --------------------------------------------------------------------------------------------------------------------
// <copyright file="With{TValue}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base;

/// <summary>
/// Represents a tri-state value (Either no value, a value or null value).
/// </summary>
/// <typeparam name="TValue">The type of the value encapsulated by the struct.</typeparam>
/// <param name="Value">The value to be encapsulated. This parameter is used when a value is present.</param>
/// <param name="HasValue">A value indicating whether the struct contains a valid value.</param>
public readonly record struct With<TValue>(TValue Value, bool HasValue)
{
    /// <summary>
    /// Performs an implicit conversion from a value of type <typeparamref name="TValue"/> to a <see cref="With{TValue}"/> struct, indicating that the struct contains a valid value.
    /// </summary>
    /// <param name="value">The value.</param>
    public static implicit operator With<TValue>(TValue value) => new(value, true);

    /// <summary>
    /// Performs an implicit conversion from a value of type <typeparamref name="TValue"/> to a <see cref="With{TValue}"/> struct, indicating that the struct contains a valid value.
    /// </summary>
    /// <param name="with">The with.</param>
    public static implicit operator With<TValue>(With with) => new(default!, true);

    /// <summary>
    /// Gets the value or the provided default.
    /// </summary>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The value or default value.</returns>
    public TValue GetValueOrDefault(TValue defaultValue) => this.HasValue ? this.Value : defaultValue;
}