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
using Sundew.Base.Collections.Linq;

/// <summary>
/// Builder for constructing <see cref="ValueIds"/> for dynamic construction of identifiers.
/// </summary>
/// <param name="type">The .</param>
/// <param name="isRoot">Indicates whether the builder is for a root identifier.</param>
public sealed class ValueIdBuilder(Type type, bool isRoot)
{
    private readonly List<ValueId> values = new();

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
            var valueId = valueIdentifiable.GetValueId(false);
            this.values.Add(new ValueId(name, GetMetadata(value.GetType(), typeof(TValue), false), valueId.Value));
        }
        else if (value != null)
        {
            var stringValue = value.ToString();
            if (stringValue.HasValue)
            {
                this.values.Add(new ValueId(name, GetMetadata(value.GetType(), typeof(TValue), false), new SingleValue(stringValue)));
            }
        }

        return this;
    }

    /// <summary>
    /// Builds the <see cref="ValueIds"/> instance based on the values added to the builder. Each value is converted to an <see cref="ValueId"/> with its name and string representation of the value. The resulting <see cref="ValueIds"/>.
    /// </summary>
    /// <returns>A new <see cref="ValueIds"/>.</returns>
    public ValueId Build()
    {
        var cardinality = this.values.ByCardinality();
        var metadata = isRoot ? Source.FromType(type).ToString() : null;
        return cardinality switch
        {
            Empty<ValueId> empty => new ValueId(null, metadata, new SingleValue("null")),
            Multiple<ValueId> valueIds => new ValueId(null, metadata, new ValueIds(this.values.ToValueArray())),
            Single<ValueId> single => single.Item,
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