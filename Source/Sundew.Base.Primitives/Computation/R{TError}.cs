// --------------------------------------------------------------------------------------------------------------------
// <copyright file="R{TError}.cs" company="Hukano">
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

/// <summary>Represents a result that if it is erroneous has error information.</summary>
/// <typeparam name="TError">The type of the error.</typeparam>
public readonly struct R<TError> : IEquatable<R<TError>>
{
    private const string SuccessText = "Success";

    /// <summary>Initializes a new instance of the <see cref="R{TError}"/> struct.</summary>
    /// <param name="isSuccess">if set to <c>true</c> [is error].</param>
    /// <param name="error">The error.</param>
    internal R(bool isSuccess, TError? error)
    {
        this.IsSuccess = isSuccess;
        this.Error = error;
    }

    /// <summary>Gets a value indicating whether this instance is success.</summary>
    /// <value>
    ///   <c>true</c> if this instance is success; otherwise, <c>false</c>.</value>
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess { get; }

    /// <summary>Gets the error.</summary>
    /// <value>The error.</value>
    [AllowNull]
    public TError Error { get; }

    /// <summary>
    /// Gets the result's success property.
    /// </summary>
    /// <param name="r">The result.</param>
    /// <returns>A value indicating whether the result was successful.</returns>
    public static implicit operator bool(R<TError> r)
    {
        return r.IsSuccess;
    }

    /// <summary>Performs an implicit conversion from <see cref="R.SuccessResult"/> to <see cref="R"/>.</summary>
    /// <param name="successResult">The error result.</param>
    /// <returns>The result of the conversion.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator R<TError>(R.SuccessResult successResult)
    {
        return new R<TError>(successResult.IsSuccess, default!);
    }

    /// <summary>Performs an implicit conversion from <see cref="R.ErrorResult{TError}"/> to <see cref="R{TValue}"/>.</summary>
    /// <param name="errorResult">The error result.</param>
    /// <returns>The result of the conversion.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator R<TError>(R.ErrorResult<TError> errorResult)
    {
        return new R<TError>(false, errorResult.Error);
    }

    /// <summary>Performs an implicit conversion from <see cref="R"/> to <see cref="R{TValue}"/>.</summary>
    /// <param name="errorR">The error result.</param>
    /// <returns>The result of the conversion.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ValueTask<R<TError>>(R<TError> errorR)
    {
        return errorR.ToValueTask();
    }

    /// <summary>Performs an implicit conversion from <see cref="R"/> to <see cref="O"/>.</summary>
    /// <param name="r">The result.</param>
    /// <returns>The result of the conversion.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<R<TError>>(R<TError> r)
    {
        return r.ToTask();
    }

    /// <summary>Implements the operator ==.</summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(R<TError> left, R<TError> right)
    {
        return left.Equals(right);
    }

    /// <summary>Implements the operator !=.</summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(R<TError> left, R<TError> right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Converts this instance to a value task.
    /// </summary>
    /// <returns>The value task.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<R<TError>> ToValueTask()
    {
        return new ValueTask<R<TError>>(this);
    }

    /// <summary>
    /// Converts this instance to a task.
    /// </summary>
    /// <returns>The value task.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<R<TError>> ToTask()
    {
        return Task.FromResult(this);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>
    /// A new <see cref="R" />.
    /// </returns>
    public R<TValue, TError> To<TValue>(TValue value)
    {
        return new R<TValue, TError>(this.IsSuccess, this.IsSuccess ? value : default!, !this.IsSuccess ? this.Error : default!);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="valueFunc">The value function.</param>
    /// <returns>
    /// A new <see cref="R" />.
    /// </returns>
    public R<TValue, TError> To<TValue>(Func<TValue> valueFunc)
    {
        return new R<TValue, TError>(this.IsSuccess, this.IsSuccess ? valueFunc() : default!, this.Error);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TNewError">The type of the new error.</typeparam>
    /// <param name="errorFunc">The error func.</param>
    /// <returns>
    /// A new <see cref="R{TNewError}" />.
    /// </returns>
    public R<TNewError> With<TNewError>(Func<TError, TNewError> errorFunc)
    {
        return new R<TNewError>(this.IsSuccess, this.IsSuccess ? default! : errorFunc(this.Error));
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TParameter">The type of the parameter.</typeparam>
    /// <typeparam name="TNewError">The type of the new error.</typeparam>
    /// <param name="parameter">The parameter.</param>
    /// <param name="errorFunc">The error func.</param>
    /// <returns>
    /// A new <see cref="R{TNewError}" />.
    /// </returns>
    public R<TNewError> With<TParameter, TNewError>(TParameter parameter, Func<TError, TParameter, TNewError> errorFunc)
    {
        return new R<TNewError>(this.IsSuccess, this.IsSuccess ? default! : errorFunc(this.Error, parameter));
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the new value.</typeparam>
    /// <typeparam name="TNewError">The type of the new error.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="errorFunc">The error function.</param>
    /// <returns>
    /// A new <see cref="R" />.
    /// </returns>
    public R<TValue, TNewError> To<TValue, TNewError>(TValue value, Func<TError, TNewError> errorFunc)
    {
        return this.IsSuccess ? new R<TValue, TNewError>(this.IsSuccess, value, default!) : new R<TValue, TNewError>(this.IsSuccess, default!, errorFunc(this.Error));
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the new value.</typeparam>
    /// <typeparam name="TNewError">The type of the new error.</typeparam>
    /// <param name="valueFunc">The value function.</param>
    /// <param name="errorFunc">The error function.</param>
    /// <returns>
    /// A new <see cref="R" />.
    /// </returns>
    public R<TValue, TNewError> To<TValue, TNewError>(Func<TValue> valueFunc, Func<TError, TNewError> errorFunc)
    {
        return this.IsSuccess ? new R<TValue, TNewError>(this.IsSuccess, valueFunc(), default!) : new R<TValue, TNewError>(this.IsSuccess, default!, errorFunc(this.Error));
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
            return SuccessText;
        }

        return $"Error: {this.Error}";
    }

    /// <summary>
    /// Deconstructs the result and value.
    /// </summary>
    /// <param name="isSuccess">if set to <c>true</c> [is success].</param>
    /// <param name="error">The error.</param>
    public void Deconstruct(out bool isSuccess, out TError error)
    {
        isSuccess = this.IsSuccess;
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
    public bool Equals(R<TError> other)
    {
        return this.IsSuccess == other.IsSuccess && Equals(this.Error, other.Error);
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
        return Equality.GetHashCode(this.IsSuccess.GetHashCode(), this.Error?.GetHashCode() ?? 0);
    }
}