// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueIdBuilder.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Sundew.Base.Collections.Immutable;

/// <summary>
/// Builder for constructing <see cref="Arguments"/> for dynamic construction of identifiers.
/// </summary>
public sealed class ValueIdBuilder
{
    private readonly List<(string Name, object Value)> values = new();

    /// <summary>
    /// Adds a value to the builder for dynamic construction of identifiers.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value to be added to the builder. This can be any object representing an identifier component.</param>
    /// <param name="name">The name of the value being added.</param>
    /// <returns>The current instance of the builder, enabling method chaining.</returns>
    public ValueIdBuilder Add<TValue>(TValue? value, [CallerArgumentExpression(nameof(value))] string? name = null)
    {
        if (!name.HasValue)
        {
            throw new NotSupportedException($"{nameof(name)} should be filled by compiler!");
        }

        name = name.Replace("this.", string.Empty);
        if (value is IValueIdentifiable<TValue> valueIdentifiable)
        {
            this.values.Add((name, $"{Arguments.GroupStartSeparator}{valueIdentifiable.AsArguments().ToString()}{Arguments.GroupEndSeparator}"));
        }
        else if (value != null)
        {
            var stringValue = value.ToString();
            if (stringValue.HasValue)
            {
                this.values.Add((name, stringValue));
            }
        }

        return this;
    }

    /// <summary>
    /// Builds the <see cref="Arguments"/> instance based on the values added to the builder. Each value is converted to an <see cref="Argument"/> with its name and string representation of the value. The resulting <see cref="Arguments"/>.
    /// </summary>
    /// <returns>A new <see cref="Arguments"/>.</returns>
    public Arguments Build()
    {
        return new Arguments()
        {
            Items = this.values.Select(x => new Argument(x.Name, x.Value.ToString()!)).ToValueArray(),
        };
    }
}