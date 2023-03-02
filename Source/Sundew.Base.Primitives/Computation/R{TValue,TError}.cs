// --------------------------------------------------------------------------------------------------------------------
// <copyright file="R{TValue,TError}.cs" company="Hukano">
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

/// <summary>
/// R class for storing one value and a <see cref="bool" /> indicating whether the value is valid.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
/// <typeparam name="TError">The type of the error.</typeparam>
public readonly struct R<TValue, TError> : IEquatable<R<TValue, TError>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="R{TValue,TError}"/> struct.
    /// </summary>
    /// <param name="isSuccess">If set to <c>true</c> [success].</param>
    /// <param name="value">The result value.</param>
    /// <param name="error">The error.</param>
    internal R(bool isSuccess, TValue value, TError error)
    {
        this.IsSuccess = isSuccess;
        this.Value = value;
        this.Error = error;
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="R"/> is success.
    /// </summary>
    /// <value>
    ///   <c>true</c> if success; otherwise, <c>false</c>.
    /// </value>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    [AllowNull]
    public TValue Value { get; }

    /// <summary>
    /// Gets the error.
    /// </summary>
    /// <value>
    /// The error.
    /// </value>
    [AllowNull]
    public TError Error { get; }

    /// <summary>
    /// Gets the result's success property.
    /// </summary>
    /// <param name="r">The result.</param>
    /// <returns>A value indicating whether the result was successful.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator bool(R<TValue, TError> r)
    {
        return r.IsSuccess;
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="R.SuccessResult{TValue}"/> to <see cref="R"/>.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator R<TValue, TError>(R.SuccessResult<TValue> result)
    {
        return new R<TValue, TError>(true, result.Value, default!);
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="R.ErrorResult{TValue}"/> to <see cref="R"/>.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator R<TValue, TError>(R.ErrorResult<TError> result)
    {
        return new R<TValue, TError>(false, default!, result.Error);
    }

    /// <summary>Performs an implicit conversion from <see cref="ValueTask{R}"/> to <see cref="R"/>.</summary>
    /// <param name="r">The result.</param>
    /// <returns>The result of the conversion.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ValueTask<R<TValue, TError>>(R<TValue, TError> r)
    {
        return r.ToValueTask();
    }

    /// <summary>Implements the operator ==.</summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(R<TValue, TError> left, R<TValue, TError> right)
    {
        return left.Equals(right);
    }

    /// <summary>Implements the operator !=.</summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(R<TValue, TError> left, R<TValue, TError> right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Converts this instance to a value task.
    /// </summary>
    /// <returns>The value task.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<R<TValue, TError>> ToValueTask()
    {
        return new ValueTask<R<TValue, TError>>(this);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <param name="valueFunc">The value function.</param>
    /// <returns>
    /// A new <see cref="R" />.
    /// </returns>
    public R<TNewValue, TError> WithValue<TNewValue>(Func<TValue, TNewValue> valueFunc)
    {
        return this.With(valueFunc, error => error);
    }

    /// <summary>
    /// Creates a result based on the specified result .
    /// </summary>
    /// <typeparam name="TNewError">The type of the new error.</typeparam>
    /// <param name="errorFunc">The error function.</param>
    /// <returns>
    /// A new <see cref="R" />.
    /// </returns>
    public R<TValue, TNewError> WithError<TNewError>(Func<TError, TNewError> errorFunc)
    {
        return this.With(value => value, errorFunc);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <typeparam name="TNewError">The type of the new error.</typeparam>
    /// <param name="valueFunc">The value function.</param>
    /// <param name="errorFunc">The error function.</param>
    /// <returns>
    /// A new <see cref="R" />.
    /// </returns>
    public R<TNewValue, TNewError> With<TNewValue, TNewError>(Func<TValue, TNewValue> valueFunc, Func<TError, TNewError> errorFunc)
    {
        return this.IsSuccess ? new R<TNewValue, TNewError>(this.IsSuccess, valueFunc(this.Value), default!) : new R<TNewValue, TNewError>(this.IsSuccess, default!, errorFunc(this.Error));
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        if (this.IsSuccess)
        {
            return $"Success: {this.Value}";
        }

        return $"Error: {this.Error}";
    }

    /// <summary>
    /// Deconstructs the result, value and error.
    /// </summary>
    /// <param name="isSuccess">if set to <c>true</c> [is success].</param>
    /// <param name="value">The value.</param>
    /// <param name="error">The error.</param>
    public void Deconstruct(out bool isSuccess, out TValue value, out TError error)
    {
        isSuccess = this.IsSuccess;
        value = this.Value;
        error = this.Error;
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
    public bool Equals(R<TValue, TError> other)
    {
        return this.IsSuccess == other.IsSuccess && Equals(this.Value, other.Value) && Equals(this.Error, other.Error);
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
        return EqualityHelper.GetHashCode(this.IsSuccess.GetHashCode(), this.Value?.GetHashCode() ?? 0, this.Error?.GetHashCode() ?? 0);
    }
}