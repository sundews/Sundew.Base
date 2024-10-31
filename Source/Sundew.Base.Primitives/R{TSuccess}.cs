// --------------------------------------------------------------------------------------------------------------------
// <copyright file="R{TSuccess}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

/// <summary>Represents a result that if it is successful has a value.</summary>
/// <typeparam name="TSuccess">The type of the success value.</typeparam>
public readonly struct R<TSuccess> : IEquatable<R<TSuccess>>
{
    private const string ErrorText = "Error";

    /// <summary>Initializes a new instance of the <see cref="R{TSuccess}"/> struct.</summary>
    /// <param name="isSuccess"><c>true</c> if success otherwise <c>false</c>.</param>
    /// <param name="value">The value.</param>
    internal R(bool isSuccess, TSuccess? value)
    {
        this.IsSuccess = isSuccess;
        this.Value = value;
    }

    /// <summary>Gets a value indicating whether this instance is success.</summary>
    /// <value><c>true</c> if this instance is success; otherwise, <c>false</c>.</value>
    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether this <see cref="R"/> is an error.
    /// </summary>
    /// <value>
    ///   <c>true</c> if success; otherwise, <c>false</c>.
    /// </value>
    [MemberNotNullWhen(false, nameof(Value))]
    public bool IsError => !this.IsSuccess;

    /// <summary>Gets the value.</summary>
    /// <value>The value.</value>
    public TSuccess? Value { get; }

    /// <summary>
    /// Gets the result's success property.
    /// </summary>
    /// <param name="r">The result.</param>
    /// <returns>A value indicating whether the result was successful.</returns>
    public static implicit operator bool(R<TSuccess> r)
    {
        return r.IsSuccess;
    }

    /// <summary>Performs an implicit conversion from <see cref="R.SuccessResult"/> to <see cref="R"/>.</summary>
    /// <param name="errorResult">The error result.</param>
    /// <returns>The result of the conversion.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static implicit operator R<TSuccess>(R.ErrorResult errorResult)
    {
        return new R<TSuccess>(errorResult.IsSuccess, default!);
    }

    /// <summary>Performs an implicit conversion from <see cref="R.ErrorResult{TError}"/> to <see cref="R{TSuccess}"/>.</summary>
    /// <param name="successResult">The success result.</param>
    /// <returns>The result of the conversion.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static implicit operator R<TSuccess>(R.SuccessResult<TSuccess> successResult)
    {
        return new R<TSuccess>(true, successResult.Value);
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="R.SuccessResult{TValue}"/> to <see cref="R"/>.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static implicit operator R<TSuccess?>(R.SuccessResultOption<TSuccess?> result)
    {
        return new R<TSuccess?>(true, result.Value);
    }

    /// <summary>Performs an implicit conversion from <see cref="R{TSuccess}"/> to <see cref="R{TObject}"/>.</summary>
    /// <param name="result">The success result.</param>
    /// <returns>The result of the conversion.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static implicit operator R<object>(R<TSuccess> result)
    {
        return new R<object>(result.IsSuccess, result.Value);
    }

    /// <summary>Performs an implicit conversion from <see cref="R"/> to <see cref="ValueTask{R}"/>.</summary>
    /// <param name="resultWithValue">The error result.</param>
    /// <returns>The result of the conversion.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static implicit operator ValueTask<R<TSuccess>>(R<TSuccess> resultWithValue)
    {
        return resultWithValue.ToValueTask();
    }

    /// <summary>Performs an implicit conversion from <see cref="R"/> to <see cref="Task{R}"/>.</summary>
    /// <param name="r">The result.</param>
    /// <returns>The result of the conversion.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static implicit operator Task<R<TSuccess>>(R<TSuccess> r)
    {
        return r.ToTask();
    }

    /// <summary>Implements the operator ==.</summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(R<TSuccess> left, R<TSuccess> right)
    {
        return left.Equals(right);
    }

    /// <summary>Implements the operator !=.</summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(R<TSuccess> left, R<TSuccess> right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Evaluates the result into a single value.
    /// </summary>
    /// <param name="resultIfIsErroneous">The value if the result is erroneous.</param>
    /// <returns>The new value.</returns>
    [return: NotNullIfNotNull("resultIfIsErroneous")]
    public TSuccess? Evaluate(TSuccess? resultIfIsErroneous)
    {
        return this.IsSuccess ? this.Value : resultIfIsErroneous;
    }

    /// <summary>
    /// Evaluates the result into a single value.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="resultFunc">The result function.</param>
    /// <param name="resultIfIsErroneous">The result if is erroneous.</param>
    /// <returns>The new value.</returns>
    [return: NotNullIfNotNull("resultIfIsErroneous")]
    public TResult? Evaluate<TResult>(Func<TSuccess, TResult> resultFunc, TResult? resultIfIsErroneous)
    {
        return this.IsSuccess ? resultFunc(this.Value) : resultIfIsErroneous;
    }

    /// <summary>
    /// Evaluates the result into a single value.
    /// </summary>
    /// <param name="resultIfIsErroneousFunc">The error function.</param>
    /// <returns>The new value.</returns>
    public TSuccess Evaluate(Func<TSuccess> resultIfIsErroneousFunc)
    {
        return this.IsSuccess ? this.Value : resultIfIsErroneousFunc();
    }

    /// <summary>
    /// Evaluates the result into a single value.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="resultFunc">The result function.</param>
    /// <param name="resultIfIsErroneousFunc">The result if is erroneous function.</param>
    /// <returns>The new value.</returns>
    public TResult Evaluate<TResult>(Func<TSuccess, TResult> resultFunc, Func<TResult> resultIfIsErroneousFunc)
    {
        return this.IsSuccess ? resultFunc(this.Value) : resultIfIsErroneousFunc();
    }

    /// <summary>
    /// Evaluates the result into a single value.
    /// </summary>
    /// <param name="resultIfIsErroneousFunc">The error function.</param>
    /// <returns>The new value.</returns>
    public TSuccess? EvaluateToOption(Func<TSuccess?> resultIfIsErroneousFunc)
    {
        return this.IsSuccess ? this.Value : resultIfIsErroneousFunc();
    }

    /// <summary>
    /// Evaluates the result into a single value.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="resultFunc">The result function.</param>
    /// <param name="resultIfIsErroneousFunc">The result if is erroneous function.</param>
    /// <returns>The new value.</returns>
    public TResult? EvaluateToOption<TResult>(Func<TSuccess, TResult?> resultFunc, Func<TResult?> resultIfIsErroneousFunc)
    {
        return this.IsSuccess ? resultFunc(this.Value) : resultIfIsErroneousFunc();
    }

    /// <summary>
    /// Checks if the result is successful and passes the value through the out parameter.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if the result is successful otherwise <c>false</c>.</returns>
    [MemberNotNullWhen(true, nameof(Value))]
    public bool TryGet([NotNullWhen(true)] out TSuccess? value)
    {
        value = this.Value;
        return this.IsSuccess;
    }

    /// <summary>
    /// Converts this instance to a value task.
    /// </summary>
    /// <returns>The value task.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public ValueTask<R<TSuccess>> ToValueTask()
    {
        return new ValueTask<R<TSuccess>>(this);
    }

    /// <summary>
    /// Converts this instance to a task.
    /// </summary>
    /// <returns>The value task.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public Task<R<TSuccess>> ToTask()
    {
        return Task.FromResult(this);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="error">The value.</param>
    /// <returns>
    /// A new <see cref="R" />.
    /// </returns>
    public R<TSuccess, TError> Map<TError>(TError error)
    {
        return new R<TSuccess, TError>(this.IsSuccess, this.IsSuccess ? this.Value : default!, error);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="errorFunc">The value function.</param>
    /// <returns>
    /// A new <see cref="R" />.
    /// </returns>
    public R<TSuccess, TError> Map<TError>(Func<TError> errorFunc)
    {
        return new R<TSuccess, TError>(this.IsSuccess, this.IsSuccess ? this.Value : default!, errorFunc());
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TNewSuccess">The type of the new value.</typeparam>
    /// <param name="valueFunc">The value func.</param>
    /// <returns>
    /// A new <see cref="R{TSuccess}" />.
    /// </returns>
    public R<TNewSuccess> Map<TNewSuccess>(Func<TSuccess, TNewSuccess> valueFunc)
    {
        return new R<TNewSuccess>(this.IsSuccess, this.IsSuccess ? valueFunc(this.Value) : default!);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TParameter">The type of the parameter.</typeparam>
    /// <typeparam name="TNewSuccess">The type of the new error.</typeparam>
    /// <param name="parameter">The parameter.</param>
    /// <param name="valueFunc">The value function.</param>
    /// <returns>
    /// A new <see cref="R{TSuccess}" />.
    /// </returns>
    public R<TNewSuccess> Map<TParameter, TNewSuccess>(TParameter parameter, Func<TSuccess, TParameter, TNewSuccess> valueFunc)
    {
        return new R<TNewSuccess>(this.IsSuccess, this.IsSuccess ? valueFunc(this.Value, parameter) : default!);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TNewSuccess">The type of the new value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="valueFunc">The value func.</param>
    /// <param name="error">The error.</param>
    /// <returns>
    /// A new <see cref="R" />.
    /// </returns>
    public R<TNewSuccess, TError> Map<TNewSuccess, TError>(Func<TSuccess, TNewSuccess> valueFunc, TError error)
    {
        return this.IsSuccess ? new R<TNewSuccess, TError>(true, valueFunc(this.Value), default!) : new R<TNewSuccess, TError>(false, default!, error);
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TNewSuccess">The type of the new value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="valueFunc">The value function.</param>
    /// <param name="errorFunc">The error function.</param>
    /// <returns>
    /// A new <see cref="R" />.
    /// </returns>
    public R<TNewSuccess, TError> Map<TNewSuccess, TError>(Func<TSuccess, TNewSuccess> valueFunc, Func<TError> errorFunc)
    {
        return this.IsSuccess ? new R<TNewSuccess, TError>(true, valueFunc(this.Value), default!) : new R<TNewSuccess, TError>(false, default!, errorFunc());
    }

    /// <summary>
    /// Evaluates the value if it is a successful result and otherwise returns the seed.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="seed">The seed.</param>
    /// <param name="valueFunc">The error function.</param>
    /// <returns>The result.</returns>
    public TResult IfSuccess<TResult>(TResult seed, Func<TResult, TSuccess, TResult> valueFunc)
    {
        return this.IsSuccess ? valueFunc(seed, this.Value) : seed;
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

        return ErrorText;
    }

    /// <summary>
    /// Deconstructs the result and value.
    /// </summary>
    /// <param name="isSuccess">if set to <c>true</c> [is success].</param>
    /// <param name="value">The value.</param>
    public void Deconstruct(out bool isSuccess, out TSuccess? value)
    {
        isSuccess = this.IsSuccess;
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
    public bool Equals(R<TSuccess> other)
    {
        return this.IsSuccess == other.IsSuccess && Equals(this.Value, other.Value);
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
        return Equality.Equality.GetHashCode(this.IsSuccess.GetHashCode(), this.Value?.GetHashCode() ?? 0);
    }
}