// --------------------------------------------------------------------------------------------------------------------
// <copyright file="O{TValue}.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives.Computation;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Sundew.Base.Equality;

/// <summary>Represents a result that has a value if it is a success.</summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
public readonly struct O<TValue> : IEquatable<O<TValue>>
{
    private const string NoneText = "None";

    /// <summary>Initializes a new instance of the <see cref="O{TValue}"/> struct.</summary>
    /// <param name="hasValue">if set to <c>true</c> [is success].</param>
    /// <param name="value">The value.</param>
    public O(bool hasValue, TValue? value)
    {
        this.HasValue = hasValue;
        this.Value = value;
    }

    /// <summary>Gets a value indicating whether this instance is success.</summary>
    /// <value>
    ///   <c>true</c> if this instance is success; otherwise, <c>false</c>.</value>
    [MemberNotNullWhen(true, nameof(Value))]
    public bool HasValue { get; }

    /// <summary>Gets the value.</summary>
    /// <value>The value.</value>
    [AllowNull]
    public TValue Value { get; }

    /// <summary>
    /// Gets the result's success property.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>A value indicating whether the result was successful.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [MemberNotNullWhen(true, nameof(Value))]
    public static implicit operator bool(O<TValue> result)
    {
        return result.HasValue;
    }

    /// <summary>Performs an implicit conversion from <see cref="O.NoneOption"/> to <see cref="O"/>.</summary>
    /// <param name="noneOption">The error result.</param>
    /// <returns>The result of the conversion.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator O<TValue>(O.NoneOption noneOption)
    {
        return new O<TValue>(false, default!);
    }

    /// <summary>Performs an implicit conversion from <see cref="O"/> to <see cref="ValueTask{O}"/>.</summary>
    /// <param name="result">The result.</param>
    /// <returns>The result of the conversion.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ValueTask<O<TValue>>(O<TValue> result)
    {
        return result.ToValueTask();
    }

    /// <summary>Implements the operator ==.</summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(O<TValue> left, O<TValue> right)
    {
        return left.Equals(right);
    }

    /// <summary>Implements the operator !=.</summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(O<TValue> left, O<TValue> right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Converts this instance to a value task.
    /// </summary>
    /// <returns>The value task.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<O<TValue>> ToValueTask()
    {
        return new ValueTask<O<TValue>>(this);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <param name="newValue">The new value.</param>
    /// <returns>
    /// A new <see cref="O" />.
    /// </returns>
    public O<TNewValue> With<TNewValue>(TNewValue newValue)
    {
        return new O<TNewValue>(this.HasValue, this.HasValue ? newValue : default!);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <param name="valueFunc">The value func.</param>
    /// <returns>
    /// A new <see cref="O" />.
    /// </returns>
    public O<TNewValue> With<TNewValue>(Func<TValue, TNewValue> valueFunc)
    {
        return new O<TNewValue>(this.HasValue, this.HasValue ? valueFunc(this.Value) : default!);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TError">The type of the new error.</typeparam>
    /// <param name="error">The error.</param>
    /// <returns>
    /// A new <see cref="R" />.
    /// </returns>
    public R<TValue, TError> ToResult<TError>(TError error)
    {
        return new R<TValue, TError>(this.HasValue, this.Value, this.HasValue ? default! : error);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TError">The type of the new error.</typeparam>
    /// <param name="errorFunc">The error function.</param>
    /// <returns>
    /// A new <see cref="R" />.
    /// </returns>
    public R<TValue, TError> ToResult<TError>(Func<TError> errorFunc)
    {
        return new R<TValue, TError>(this.HasValue, this.Value, this.HasValue ? default! : errorFunc());
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <typeparam name="TError">The type of the new error.</typeparam>
    /// <param name="valueFunc">The value function.</param>
    /// <param name="error">The error.</param>
    /// <returns>
    /// A new <see cref="R" />.
    /// </returns>
    public R<TNewValue, TError> ToResult<TNewValue, TError>(Func<TValue, TNewValue> valueFunc, TError error)
    {
        return new R<TNewValue, TError>(this.HasValue, this.HasValue ? valueFunc(this.Value) : default!, error);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <typeparam name="TError">The type of the new error.</typeparam>
    /// <param name="valueFunc">The value function.</param>
    /// <param name="errorFunc">The error function.</param>
    /// <returns>
    /// A new <see cref="R" />.
    /// </returns>
    public R<TNewValue, TError> ToResult<TNewValue, TError>(Func<TValue, TNewValue> valueFunc, Func<TError> errorFunc)
    {
        return this.HasValue ? new R<TNewValue, TError>(this.HasValue, valueFunc(this.Value), default!) : new R<TNewValue, TError>(this.HasValue, default!, errorFunc());
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        if (this.HasValue)
        {
            return $"Success: {this.Value}";
        }

        return NoneText;
    }

    /// <summary>
    /// Deconstructs the result and value.
    /// </summary>
    /// <param name="isSuccess">if set to <c>true</c> [is success].</param>
    /// <param name="value">The value.</param>
    public void Deconstruct(out bool isSuccess, out TValue value)
    {
        isSuccess = this.HasValue;
        value = this.Value;
    }

    /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    ///   <span class="keyword">
    ///     <span class="languageSpecificText">
    ///       <span class="cs">true</span>
    ///       <span class="vb">True</span>
    ///       <span class="cpp">true</span>
    ///     </span>
    ///   </span>
    ///   <span class="nu">
    ///     <span class="keyword">true</span> (<span class="keyword">True</span> in Visual Basic)</span> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <span class="keyword"><span class="languageSpecificText"><span class="cs">false</span><span class="vb">False</span><span class="cpp">false</span></span></span><span class="nu"><span class="keyword">false</span> (<span class="keyword">False</span> in Visual Basic)</span>.
    /// </returns>
    public bool Equals(O<TValue> other)
    {
        return this.HasValue == other.HasValue && Equals(this.Value, other.Value);
    }

    /// <summary>Determines whether the specified <see cref="object"/>, is equal to this instance.</summary>
    /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
        return EqualityHelper.Equals(this, obj);
    }

    /// <summary>Returns a hash code for this instance.</summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        return EqualityHelper.GetHashCode(this.HasValue.GetHashCode(), this.Value?.GetHashCode() ?? 0);
    }
}
