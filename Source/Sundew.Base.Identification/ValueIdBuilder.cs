// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueIdBuilder.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sundew.Base.Collections.Immutable;
using Sundew.Base.Collections.Linq;

/// <summary>
/// Builder for constructing <see cref="ComplexValue"/> for dynamic construction of identifiers.
/// </summary>
/// <param name="type">The .</param>
public sealed class ValueIdBuilder(Type type)
{
    private readonly List<Argument> values = new();

    /// <summary>
    /// Adds a value to the builder for dynamic construction of identifiers.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value to be added to the builder. This can be any object representing an identifier component.</param>
    /// <param name="name">The name of the value being added.</param>
    /// <returns>The current instance of the builder, enabling method chaining.</returns>
    public ValueIdBuilder Add<TValue>(TValue? value, [CallerArgumentExpression(nameof(value))] string? name = null)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new NotSupportedException($"{nameof(name)} should be filled by compiler!");
        }

        if (char.IsLower(name[0]))
        {
            var dotIndex = name.IndexOf('.');
            if (dotIndex > -1)
            {
                name = name.Substring(dotIndex + 1);
            }
        }

        if (value != null && value is IValueIdentifiable<TValue> valueIdentifiable)
        {
            var valueId = valueIdentifiable.Id;
            this.values.Add(new Argument(name, new ValueId(GetMetadata(value.GetType(), typeof(TValue), false), valueId.Value)));
        }
        else if (value != null)
        {
            var stringValue = value.ToString();
            if (stringValue.HasValue)
            {
                this.values.Add(new Argument(name, new ValueId(GetMetadata(value.GetType(), typeof(TValue), false), new ScalarValue(stringValue))));
            }
        }

        return this;
    }

    /// <summary>
    /// Builds the <see cref="ComplexValue"/> instance based on the values added to the builder. Each value is converted to an <see cref="ValueId"/> with its name and string representation of the value. The resulting <see cref="ComplexValue"/>.
    /// </summary>
    /// <returns>A new <see cref="ComplexValue"/>.</returns>
    public ValueId Build()
    {
        var cardinality = this.values.ByCardinality();
        var metadata = Source.FromType(type).ToString();
        return cardinality switch
        {
            Empty<Argument> empty => new ValueId(metadata, new ScalarValue("null")),
            Multiple<Argument> valueIds => new ValueId(metadata, new ComplexValue(valueIds.Items.ToValueArray())),
            Single<Argument> single => single.Item.ValueId,
        };
    }

    private static string? GetMetadata(Type actualType, Type knownType, bool isRoot)
    {
        if (!isRoot && IsKnownType(actualType, knownType))
        {
            return null;
        }

        return Source.FromType(actualType).ToString();
    }

    private static bool IsKnownType(Type type, Type knownType)
    {
        return type == knownType || type.IsValueType;
    }
}