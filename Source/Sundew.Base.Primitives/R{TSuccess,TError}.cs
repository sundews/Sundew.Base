// --------------------------------------------------------------------------------------------------------------------
// <copyright file="R{TSuccess,TError}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

/// <summary>
/// Result type for storing a value and/or an error. The <see cref="IsSuccess"/> property indicates whether the result is considered a success or error.
/// </summary>
/// <typeparam name="TSuccess">The type of the value.</typeparam>
/// <typeparam name="TError">The type of the error.</typeparam>
public readonly struct R<TSuccess, TError> : IEquatable<R<TSuccess, TError>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="R{TValue,TError}"/> struct.
    /// </summary>
    /// <param name="isSuccess">If set to <c>true</c> [success].</param>
    /// <param name="value">The result value.</param>
    /// <param name="error">The error.</param>
    internal R(bool isSuccess, TSuccess? value, TError? error)
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
    /// Gets a value indicating whether this <see cref="R"/> is an error.
    /// </summary>
    /// <value>
    ///   <c>true</c> if success; otherwise, <c>false</c>.
    /// </value>
    [MemberNotNullWhen(false, nameof(Value))]
    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsError => !this.IsSuccess;

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
    public TSuccess? Value { get; }

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
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public static implicit operator bool(R<TSuccess, TError> r)
    {
        return r.IsSuccess;
    }

    /// <summary>
    /// Gets the result's success property.
    /// </summary>
    /// <param name="r">The result.</param>
    /// <returns>A value indicating whether the result was successful.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static implicit operator R<TSuccess>(R<TSuccess, TError> r)
    {
        return new R<TSuccess>(r.IsSuccess, r.Value);
    }

    /// <summary>
    /// Gets the result's success property.
    /// </summary>
    /// <param name="r">The result.</param>
    /// <returns>A value indicating whether the result was successful.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static implicit operator RoE<TError>(R<TSuccess, TError> r)
    {
        return new RoE<TError>(r.IsSuccess, r.Error);
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="R.SuccessResult{TValue}"/> to <see cref="R"/>.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static implicit operator R<TSuccess?, TError>(R.SuccessResult result)
    {
        return new R<TSuccess?, TError>(true, default(TSuccess?), default!);
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="R.SuccessResult{TValue}"/> to <see cref="R"/>.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static implicit operator R<TSuccess, TError>(R.SuccessResult<TSuccess> result)
    {
        return new R<TSuccess, TError>(true, result.Value, default!);
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="R.SuccessResult{TValue}"/> to <see cref="R"/>.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static implicit operator R<TSuccess?, TError>(R.SuccessOptionResult<TSuccess?> result)
    {
        return new R<TSuccess?, TError>(true, result.Value, default!);
    }

    /// <summary>Performs an implicit conversion from <see cref="R{TSuccess, TError}"/> to <see cref="R{TObject, TObject}"/>.</summary>
    /// <param name="result">The success result.</param>
    /// <returns>The result of the conversion.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static implicit operator R<object, object>(R<TSuccess, TError> result)
    {
        return new R<object, object>(result.IsSuccess, result.Value, result.Error);
    }

    /// <summary>Performs an implicit conversion from <see cref="R{TSuccess, TError}"/> to <see cref="R{TObject}"/>.</summary>
    /// <param name="result">The success result.</param>
    /// <returns>The result of the conversion.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static implicit operator R<object>(R<TSuccess, TError> result)
    {
        return new R<object>(result.IsSuccess, result.Value);
    }

    /// <summary>Performs an implicit conversion from <see cref="R{TSuccess, TError}"/> to <see cref="RoE{TObject}"/>.</summary>
    /// <param name="result">The success result.</param>
    /// <returns>The result of the conversion.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static implicit operator RoE<object>(R<TSuccess, TError> result)
    {
        return new RoE<object>(result.IsSuccess, result.Error);
    }

#if NOT_SUPPORT_BY_LANGUAGE
    /// <summary>
    /// Performs an implicit conversion from <see cref="R{TValue, TError}"/> to <see cref="R{TValue, TError}"/>.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static implicit operator R<TValue, TError>?(R<TValue?, TError> result)
    {
        return result.ToResultOption();
    }
#endif

    /// <summary>
    /// Performs an implicit conversion from <see cref="R.ErrorResult{TValue}"/> to <see cref="R"/>.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static implicit operator R<TSuccess, TError>(R.ErrorResult<TError> result)
    {
        return new R<TSuccess, TError>(false, default!, result.Error);
    }

    /// <summary>Performs an implicit conversion from <see cref="ValueTask{R}"/> to <see cref="R"/>.</summary>
    /// <param name="r">The result.</param>
    /// <returns>The result of the conversion.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static implicit operator ValueTask<R<TSuccess, TError>>(R<TSuccess, TError> r)
    {
        return r.ToValueTask();
    }

    /// <summary>Performs an implicit conversion from <see cref="R"/> to <see cref="Task{R}"/>.</summary>
    /// <param name="r">The result.</param>
    /// <returns>The result of the conversion.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public static implicit operator Task<R<TSuccess, TError>>(R<TSuccess, TError> r)
    {
        return r.ToTask();
    }

    /// <summary>Implements the operator ==.</summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(R<TSuccess, TError> left, R<TSuccess, TError> right)
    {
        return left.Equals(right);
    }

    /// <summary>Implements the operator !=.</summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(R<TSuccess, TError> left, R<TSuccess, TError> right)
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
    public bool TryGetValue([NotNullWhen(true)] out TSuccess? value)
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
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if the result is an error otherwise <c>false</c>.</returns>
    [MemberNotNullWhen(true, nameof(Error))]
    [MemberNotNullWhen(false, nameof(Value))]
    public bool TryGetError([NotNullWhen(true)] out TError? error, [NotNullWhen(false)] out TSuccess? value)
    {
        error = this.Error;
        value = this.Value;
        return !this.IsSuccess;
    }

    /// <summary>
    /// Checks if the result is an error and passes the error through the out parameter.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <returns><c>true</c> if the result has an error otherwise <c>false</c>.</returns>
    [MemberNotNullWhen(true, nameof(Error))]
    public bool TryGetAnyError([NotNullWhen(true)] out TError? error)
    {
        error = this.Error;
        return this.HasError;
    }

    /// <summary>
    /// Checks if the result is an error and passes the error through the out parameter.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if the result has an error otherwise <c>false</c>.</returns>
    [MemberNotNullWhen(true, nameof(Error))]
    public bool TryGetAnyError([NotNullWhen(true)] out TError? error, [NotNullWhen(false)] out TSuccess? value)
    {
        error = this.Error;
        value = this.Value;
        return this.HasError;
    }

    /// <summary>
    /// Gets the value or the default value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="error">The error.</param>
    /// <returns><c>true</c> if the result is an error otherwise <c>false</c>.</returns>
    public bool TryGet([NotNullWhen(true)] out TSuccess? value, [NotNullWhen(false)] out TError? error)
    {
        value = this.Value;
        error = this.Error;
        return this.IsSuccess;
    }

    /// <summary>
    /// Converts this result to a result option.
    /// </summary>
    /// <returns>A <see cref="R{TSuccess, TError}"/>.</returns>
    public R<TSuccess?, TError> MapToOption()
    {
        return new R<TSuccess?, TError>(this.IsSuccess, this.Value, this.Error);
    }

    /// <summary>
    /// Maps this result to a <see cref="R{TSuccess}"/>.
    /// </summary>
    /// <returns>The new result.</returns>
    public R<object?, object> MapToObjectOption()
    {
        return new R<object?, object>(this.IsSuccess, this.Value, this.Error);
    }

    /// <summary>
    /// Converts this instance to a value task.
    /// </summary>
    /// <returns>The value task.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public ValueTask<R<TSuccess, TError>> ToValueTask()
    {
        return new ValueTask<R<TSuccess, TError>>(this);
    }

    /// <summary>
    /// Converts this instance to a task.
    /// </summary>
    /// <returns>The value task.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public Task<R<TSuccess, TError>> ToTask()
    {
        return Task.FromResult(this);
    }

    /// <summary>
    /// Converts this instance to a <see cref="R{TSuccess}"/>.
    /// </summary>
    /// <returns>The value task.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public R<TSuccess> ToResultWithValue()
    {
        return this;
    }

    /// <summary>
    /// Converts this instance to a <see cref="RoE{TError}"/>.
    /// </summary>
    /// <returns>The value task.</returns>
    [MethodImpl((MethodImplOptions)0x300)]
    public RoE<TError> ToResultWithError()
    {
        return this;
    }

    /// <summary>
    /// Creates a result based on the specified values.
    /// </summary>
    /// <typeparam name="TNewValue">The type of the new value.</typeparam>
    /// <param name="valueFunc">The value function.</param>
    /// <returns>
    /// A new <see cref="R" />.
    /// </returns>
    public R<TNewValue, TError> Map<TNewValue>(Func<TSuccess, TNewValue> valueFunc)
    {
        return this.Map(valueFunc, error => error);
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
    public R<TNewValue, TError> Map<TParameter, TNewValue>(TParameter parameter, Func<TSuccess, TParameter, TNewValue> valueFunc)
    {
        return this.Map(parameter, valueFunc, (error, _) => error);
    }

    /// <summary>
    /// Creates a result based on the specified result .
    /// </summary>
    /// <typeparam name="TNewError">The type of the new error.</typeparam>
    /// <param name="errorFunc">The error function.</param>
    /// <returns>
    /// A new <see cref="R" />.
    /// </returns>
    public R<TSuccess, TNewError> MapError<TNewError>(Func<TError, TNewError> errorFunc)
    {
        return this.Map(value => value, errorFunc);
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
    public R<TSuccess, TNewError> MapError<TParameter, TNewError>(TParameter parameter, Func<TError, TParameter, TNewError> errorFunc)
    {
        return this.Map(parameter, (value, _) => value, errorFunc);
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
    public R<TNewValue, TNewError> Map<TNewValue, TNewError>(Func<TSuccess, TNewValue> valueFunc, Func<TError, TNewError> errorFunc)
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
    public R<TNewValue, TNewError> Map<TParameter, TNewValue, TNewError>(TParameter parameter, Func<TSuccess, TParameter, TNewValue> valueFunc, Func<TError, TParameter, TNewError> errorFunc)
    {
        return this.IsSuccess ? new R<TNewValue, TNewError>(this.IsSuccess, valueFunc(this.Value, parameter), default!) : new R<TNewValue, TNewError>(this.IsSuccess, default!, errorFunc(this.Error, parameter));
    }

    /// <summary>
    /// Evaluates the value if it is a success result and otherwise returns the seed.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="seed">The seed.</param>
    /// <param name="successFunc">The success function.</param>
    /// <returns>The result.</returns>
    public TResult IfSuccess<TResult>(TResult seed, Func<TResult, TSuccess, TResult> successFunc)
    {
        return this.IsSuccess ? successFunc(seed, this.Value) : seed;
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
    /// Evaluates the result into a single value.
    /// </summary>
    /// <typeparam name="TResult">The new value type.</typeparam>
    /// <param name="seed">The seed.</param>
    /// <param name="successFunc">The success function.</param>
    /// <param name="resultIfIsErroneousFunc">The result if erroneous function.</param>
    /// <returns>The new value.</returns>
    public TResult Evaluate<TResult>(TResult seed, Func<TResult, TSuccess, TResult> successFunc, Func<TResult, TError, TResult> resultIfIsErroneousFunc)
    {
        return this.IsSuccess ? successFunc(seed, this.Value) : resultIfIsErroneousFunc(seed, this.Error);
    }

    /// <summary>
    /// Evaluates the result into a single value.
    /// </summary>
    /// <typeparam name="TResult">The new success type.</typeparam>
    /// <param name="successFunc">The success function.</param>
    /// <param name="resultIfIsErroneousFunc">The result if erroneous function.</param>
    /// <returns>The new value.</returns>
    public TResult Evaluate<TResult>(Func<TSuccess, TResult> successFunc, Func<TError, TResult> resultIfIsErroneousFunc)
    {
        return this.IsSuccess ? successFunc(this.Value) : resultIfIsErroneousFunc(this.Error);
    }

    /// <summary>
    /// Evaluates the result into a single value.
    /// </summary>
    /// <param name="resultIfIsErroneousFunc">The result if erroneous function.</param>
    /// <returns>The new value.</returns>
    public TSuccess Evaluate(Func<TError, TSuccess> resultIfIsErroneousFunc)
    {
        return this.IsSuccess ? this.Value : resultIfIsErroneousFunc(this.Error);
    }

    /// <summary>
    /// Evaluates the result into a single value.
    /// </summary>
    /// <typeparam name="TResult">The new success type.</typeparam>
    /// <param name="successFunc">The success function.</param>
    /// <param name="resultIfIsErroneous">The result if erroneous.</param>
    /// <returns>The new value.</returns>
    [return: NotNullIfNotNull("resultIfIsErroneous")]
    public TResult? Evaluate<TResult>(Func<TSuccess, TResult> successFunc, TResult? resultIfIsErroneous)
    {
        return this.IsSuccess ? successFunc(this.Value) : resultIfIsErroneous;
    }

    /// <summary>
    /// Evaluates the result into a single value.
    /// </summary>
    /// <param name="resultIfIsErroneous">The result if erroneous.</param>
    /// <returns>The new value.</returns>
    [return: NotNullIfNotNull("resultIfIsErroneous")]
    public TSuccess? Evaluate(TSuccess? resultIfIsErroneous)
    {
        return this.IsSuccess ? this.Value : resultIfIsErroneous;
    }

    /// <summary>
    /// Evaluates the result into a single value.
    /// </summary>
    /// <typeparam name="TResult">The new value type.</typeparam>
    /// <param name="seed">The seed.</param>
    /// <param name="successFunc">The success function.</param>
    /// <param name="resultIfIsErroneousFunc">The result if erroneous function.</param>
    /// <returns>The new value.</returns>
    public TResult? EvaluateToOption<TResult>(TResult? seed, Func<TResult?, TSuccess, TResult?> successFunc, Func<TResult?, TError, TResult?> resultIfIsErroneousFunc)
    {
        return this.IsSuccess ? successFunc(seed, this.Value) : resultIfIsErroneousFunc(seed, this.Error);
    }

    /// <summary>
    /// Evaluates the result into a single value.
    /// </summary>
    /// <typeparam name="TResult">The new success type.</typeparam>
    /// <param name="successFunc">The success function.</param>
    /// <param name="resultIfIsErroneousFunc">The result if erroneous function.</param>
    /// <returns>The new value.</returns>
    public TResult? EvaluateToOption<TResult>(Func<TSuccess, TResult?> successFunc, Func<TError, TResult?> resultIfIsErroneousFunc)
    {
        return this.IsSuccess ? successFunc(this.Value) : resultIfIsErroneousFunc(this.Error);
    }

    /// <summary>
    /// Evaluates the result into a single value.
    /// </summary>
    /// <param name="resultIfIsErroneousFunc">The result if erroneous function.</param>
    /// <returns>The new value.</returns>
    public TSuccess? EvaluateToOption(Func<TError, TSuccess?> resultIfIsErroneousFunc)
    {
        return this.IsSuccess ? this.Value : resultIfIsErroneousFunc(this.Error);
    }

    /// <summary>
    /// Gets a new result that is successful, if both results were a success.
    /// </summary>
    /// <typeparam name="TOtherValue">The other value.</typeparam>
    /// <typeparam name="TOtherError">The other error.</typeparam>
    /// <param name="other">The other result.</param>
    /// <returns>A new result.</returns>
    public R<(TSuccess Left, TOtherValue Right), (RoE<TError> Left, RoE<TOtherError> Right)> Combine<TOtherValue, TOtherError>(in R<TOtherValue, TOtherError> other)
    {
        return this.IsSuccess && other.IsSuccess
            ? R.Success((this.Value, other.Value))
            : R.Error((this.ToResultWithError(), other.ToResultWithError()));
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
    public void Deconstruct(out bool isSuccess, out TSuccess? value, out TError? error)
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
    public bool Equals(R<TSuccess, TError> other)
    {
        return this.IsSuccess == other.IsSuccess && Equals(this.Value, other.Value) && Equals(this.Error, other.Error);
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
        return Equality.Equality.GetHashCode(this.IsSuccess.GetHashCode(), this.Value?.GetHashCode() ?? 0, this.Error?.GetHashCode() ?? 0);
    }
}