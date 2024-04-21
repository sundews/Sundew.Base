// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RwE.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

/// <summary>Represents a result that if it is erroneous has error information.</summary>
/// <typeparam name="TError">The type of the error.</typeparam>
public readonly struct RwE<TError> : IEquatable<RwE<TError>>
{
    private const string SuccessText = "Success";

    /// <summary>Initializes a new instance of the <see cref="RwE{TError}"/> struct.</summary>
    /// <param name="isSuccess">if set to <c>true</c> [is error].</param>
    /// <param name="error">The error.</param>
    internal RwE(bool isSuccess, TError? error)
    {
        this.IsSuccess = isSuccess;
        this.Error = error;
    }

    /// <summary>Gets a value indicating whether this instance is success.</summary>
    /// <value>
    ///   <c>true</c> if this instance is success; otherwise, <c>false</c>.</value>
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

    /// <summary>Gets the error.</summary>
    /// <value>The error.</value>
    public TError? Error { get; }

    /// <summary>
    /// Gets the result's success property.
    /// </summary>
    /// <param name="r">The result.</param>
    /// <returns>A value indicating whether the result was successful.</returns>
    public static implicit operator bool(RwE<TError> r)
    {
        return r.IsSuccess;
    }

    /// <summary>Performs an implicit conversion from <see cref="R.SuccessResult"/> to <see cref="R"/>.</summary>
    /// <param name="successResult">The error result.</param>
    /// <returns>The result of the conversion.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static implicit operator RwE<TError>(R.SuccessResult successResult)
    {
        return new RwE<TError>(successResult.IsSuccess, default!);
    }

    /// <summary>Performs an implicit conversion from <see cref="R.ErrorResult{TError}"/> to <see cref="RwE{TError}"/>.</summary>
    /// <param name="errorResult">The error result.</param>
    /// <returns>The result of the conversion.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static implicit operator RwE<TError>(R.ErrorResult<TError> errorResult)
    {
        return new RwE<TError>(false, errorResult.Error);
    }

    /// <summary>Performs an implicit conversion from <see cref="R"/> to <see cref="ValueTask{R}"/>.</summary>
    /// <param name="errorR">The error result.</param>
    /// <returns>The result of the conversion.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static implicit operator ValueTask<RwE<TError>>(RwE<TError> errorR)
    {
        return errorR.ToValueTask();
    }

    /// <summary>Performs an implicit conversion from <see cref="R"/> to <see cref="Task{R}"/>.</summary>
    /// <param name="r">The result.</param>
    /// <returns>The result of the conversion.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static implicit operator Task<RwE<TError>>(RwE<TError> r)
    {
        return r.ToTask();
    }

    /// <summary>Implements the operator ==.</summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(RwE<TError> left, RwE<TError> right)
    {
        return left.Equals(right);
    }

    /// <summary>Implements the operator !=.</summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(RwE<TError> left, RwE<TError> right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Checks if the result is an error and passes the error through the out parameter.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <returns><c>true</c> if the result is an error otherwise <c>false</c>.</returns>
    [MemberNotNullWhen(true, nameof(Error))]
    public bool TryGetError([NotNullWhen(true)] out TError? error)
    {
        error = this.Error;
        return !this.IsSuccess;
    }

    /// <summary>
    /// Converts this instance to a value task.
    /// </summary>
    /// <returns>The value task.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public ValueTask<RwE<TError>> ToValueTask()
    {
        return new ValueTask<RwE<TError>>(this);
    }

    /// <summary>
    /// Converts this instance to a task.
    /// </summary>
    /// <returns>The value task.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public Task<RwE<TError>> ToTask()
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
    public R<TValue, TError> With<TValue>(TValue value)
    {
        return new R<TValue, TError>(this.IsSuccess, this.IsSuccess ? value : default!, this.Error);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="valueFunc">The value function.</param>
    /// <returns>
    /// A new <see cref="R" />.
    /// </returns>
    public R<TValue, TError> With<TValue>(Func<TValue> valueFunc)
    {
        return new R<TValue, TError>(this.IsSuccess, this.IsSuccess ? valueFunc() : default!, this.Error);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TNewError">The type of the new error.</typeparam>
    /// <param name="errorFunc">The error func.</param>
    /// <returns>
    /// A new <see cref="RwE{TError}" />.
    /// </returns>
    public RwE<TNewError> With<TNewError>(Func<TError, TNewError> errorFunc)
    {
        return new RwE<TNewError>(this.IsSuccess, this.IsSuccess ? default! : errorFunc(this.Error));
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TParameter">The type of the parameter.</typeparam>
    /// <typeparam name="TNewError">The type of the new error.</typeparam>
    /// <param name="parameter">The parameter.</param>
    /// <param name="errorFunc">The error func.</param>
    /// <returns>
    /// A new <see cref="RwE{TError}" />.
    /// </returns>
    public RwE<TNewError> With<TParameter, TNewError>(
        TParameter parameter,
        Func<TError, TParameter, TNewError> errorFunc)
    {
        return new RwE<TNewError>(
            this.IsSuccess,
            this.IsSuccess ? default! : errorFunc(this.Error, parameter));
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
    public R<TValue, TNewError> With<TValue, TNewError>(TValue value, Func<TError, TNewError> errorFunc)
    {
        return this.IsSuccess
            ? new R<TValue, TNewError>(true, value, default!)
            : new R<TValue, TNewError>(false, default!, errorFunc(this.Error));
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
    public R<TValue, TNewError> With<TValue, TNewError>(Func<TValue> valueFunc, Func<TError, TNewError> errorFunc)
    {
        return this.IsSuccess
            ? new R<TValue, TNewError>(true, valueFunc(), default!)
            : new R<TValue, TNewError>(false, default!, errorFunc(this.Error));
    }

    /// <summary>
    /// Evaluates the error if it is an error result and otherwise returns the seed.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="seed">The seed.</param>
    /// <param name="errorFunc">The error function.</param>
    /// <returns>The result.</returns>
    public TResult IfError<TResult>(TResult seed, Func<TResult, TError, TResult> errorFunc)
    {
        return !this.IsSuccess ? errorFunc(seed, this.Error) : seed;
    }

    /// <summary>
    /// Evaluates the error if it has any and otherwise returns the seed.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="seed">The seed.</param>
    /// <param name="successFunc">The success function.</param>
    /// <returns>The result.</returns>
    public TResult IfAnyError<TResult>(TResult seed, Func<TResult, TError, TResult> successFunc)
    {
        return this.HasError ? successFunc(seed, this.Error) : seed;
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
    public void Deconstruct(out bool isSuccess, out TError? error)
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
    public bool Equals(RwE<TError> other)
    {
        return this.IsSuccess == other.IsSuccess && Equals(this.Error, other.Error);
    }

    /// <summary>Determines whether the specified <see cref="object"/>, is equal to this instance.</summary>
    /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        return Equality.Equality.Equals(this, obj);
    }

    /// <summary>Returns a hash code for this instance.</summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        return Equality.Equality.GetHashCode(this.IsSuccess.GetHashCode(), this.Error?.GetHashCode() ?? 0);
    }
}