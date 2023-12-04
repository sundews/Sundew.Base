// --------------------------------------------------------------------------------------------------------------------
// <copyright file="R{TValue,TError}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
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
/// Result type for storing a value and/or an error. The <see cref="IsSuccess"/> property indicates whether the result is considered a success or error.
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
    internal R(bool isSuccess, TValue? value, TError? error)
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
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether this <see cref="R"/> has an error.
    /// </summary>
    /// <value>
    ///   <c>true</c> if error is non default; otherwise, <c>false</c>.
    /// </value>
    [MemberNotNullWhen(true, nameof(Error))]
    public bool HasError => !Equals(this.Error, default(TError));

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public TValue? Value { get; }

    /// <summary>
    /// Gets the error.
    /// </summary>
    /// <value>
    /// The error.
    /// </value>
    public TError? Error { get; }

    /// <summary>
    /// Gets the result's success property.
    /// </summary>
    /// <param name="r">The result.</param>
    /// <returns>A value indicating whether the result was successful.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static implicit operator bool(R<TValue, TError> r)
    {
        return r.IsSuccess;
    }

    /// <summary>
    /// Gets the result's success property.
    /// </summary>
    /// <param name="r">The result.</param>
    /// <returns>A value indicating whether the result was successful.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static implicit operator R<TError>(R<TValue, TError> r)
    {
        return new R<TError>(r.IsSuccess, r.IsSuccess ? default! : r.Error);
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="R.SuccessResult{TValue}"/> to <see cref="R"/>.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static implicit operator R<TValue?, TError>(R.SuccessResult result)
    {
        return new R<TValue?, TError>(true, default(TValue?), default!);
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="R.SuccessResult{TValue}"/> to <see cref="R"/>.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static implicit operator R<TValue, TError>(R.SuccessResult<TValue> result)
    {
        return new R<TValue, TError>(true, result.Value, default!);
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="R.SuccessResult{TValue}"/> to <see cref="R"/>.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static implicit operator R<TValue?, TError>(R.SuccessResultOption<TValue?> result)
    {
        return new R<TValue?, TError>(true, result.Value, default!);
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="R.ErrorResult{TValue}"/> to <see cref="R"/>.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static implicit operator R<TValue, TError>(R.ErrorResult<TError> result)
    {
        return new R<TValue, TError>(false, default!, result.Error);
    }

    /// <summary>Performs an implicit conversion from <see cref="ValueTask{R}"/> to <see cref="R"/>.</summary>
    /// <param name="r">The result.</param>
    /// <returns>The result of the conversion.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
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
    /// Checks if the result is a success and passes the value through the out parameter.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if the result is a success otherwise <c>false</c>.</returns>
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool TryGetValue([NotNullWhen(true)] out TValue? value)
    {
        value = this.Value;
        return this.IsSuccess;
    }

    /// <summary>
    /// Checks if the result is an error and passes the error through the out parameter.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <returns><c>true</c> if the result is an error otherwise <c>false</c>.</returns>
    [MemberNotNullWhen(true, nameof(Error))]
    [MemberNotNullWhen(false, nameof(Value))]
    public bool TryGetError([NotNullWhen(true)] out TError? error)
    {
        error = this.Error;
        return !this.IsSuccess;
    }

    /// <summary>
    /// Checks if the result is an error and passes the error through the out parameter.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <returns><c>true</c> if the result is an error otherwise <c>false</c>.</returns>
    [MemberNotNullWhen(true, nameof(Error))]
    public bool TryGetAnyError([NotNullWhen(true)] out TError? error)
    {
        error = this.Error;
        return this.HasError;
    }

    /// <summary>
    /// Gets the value or the default value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="error">The error.</param>
    /// <returns><c>true</c> if the result is an error otherwise <c>false</c>.</returns>
    public bool TryGet([NotNullWhen(true)] out TValue? value, [NotNullWhen(false)] out TError? error)
    {
        value = this.Value;
        error = this.Error;
        return this.IsSuccess;
    }

    /// <summary>
    /// Converts this instance to a value task.
    /// </summary>
    /// <returns>The value task.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
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
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TParameter">The type of the parameter.</typeparam>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <param name="parameter">The parameter.</param>
    /// <param name="valueFunc">The value function.</param>
    /// <returns>
    /// A new <see cref="R" />.
    /// </returns>
    public R<TNewValue, TError> WithValue<TParameter, TNewValue>(TParameter parameter, Func<TValue, TParameter, TNewValue> valueFunc)
    {
        return this.With(parameter, valueFunc, (error, _) => error);
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
    /// Creates a result based on the specified result .
    /// </summary>
    /// <typeparam name="TParameter">The type of the parameter.</typeparam>
    /// <typeparam name="TNewError">The type of the new error.</typeparam>
    /// <param name="parameter">The parameter.</param>
    /// <param name="errorFunc">The error function.</param>
    /// <returns>
    /// A new <see cref="R" />.
    /// </returns>
    public R<TValue, TNewError> WithError<TParameter, TNewError>(TParameter parameter, Func<TError, TParameter, TNewError> errorFunc)
    {
        return this.With(parameter, (value, _) => value, errorFunc);
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
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TParameter">The type of the parameter.</typeparam>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <typeparam name="TNewError">The type of the new error.</typeparam>
    /// <param name="parameter">The parameter.</param>
    /// <param name="valueFunc">The value function.</param>
    /// <param name="errorFunc">The error function.</param>
    /// <returns>
    /// A new <see cref="R" />.
    /// </returns>
    public R<TNewValue, TNewError> With<TParameter, TNewValue, TNewError>(TParameter parameter, Func<TValue, TParameter, TNewValue> valueFunc, Func<TError, TParameter, TNewError> errorFunc)
    {
        return this.IsSuccess ? new R<TNewValue, TNewError>(this.IsSuccess, valueFunc(this.Value, parameter), default!) : new R<TNewValue, TNewError>(this.IsSuccess, default!, errorFunc(this.Error, parameter));
    }

    /// <summary>
    /// Evaluates the result into a single value.
    /// </summary>
    /// <typeparam name="TNewValue">The new value type.</typeparam>
    /// <param name="successFunc">The success function.</param>
    /// <param name="errorFunc">The error function.</param>
    /// <returns>The new value.</returns>
    public TNewValue Evaluate<TNewValue>(Func<TValue, TNewValue> successFunc, Func<TError, TNewValue> errorFunc)
    {
        return this.IsSuccess ? successFunc(this.Value) : errorFunc(this.Error);
    }

    /// <summary>
    /// Evaluates the result into a single value.
    /// </summary>
    /// <param name="errorFunc">The error function.</param>
    /// <returns>The new value.</returns>
    public TValue Evaluate(Func<TError, TValue> errorFunc)
    {
        return this.IsSuccess ? this.Value : errorFunc(this.Error);
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
    public void Deconstruct(out bool isSuccess, out TValue? value, out TError? error)
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
    public override bool Equals(object? obj)
    {
        return Equality.Equals(this, obj);
    }

    /// <summary>Returns a hash code for this instance.</summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        return Equality.GetHashCode(this.IsSuccess.GetHashCode(), this.Value?.GetHashCode() ?? 0, this.Error?.GetHashCode() ?? 0);
    }
}