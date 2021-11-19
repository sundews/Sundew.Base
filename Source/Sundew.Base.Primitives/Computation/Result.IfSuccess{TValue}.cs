// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Result.IfSuccess{TValue}.cs" company="Hukano">
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
/// Factory class for creating results.
/// </summary>
public static partial class Result
{
    /// <summary>Represents a result that has a value if it is a success.</summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public readonly struct IfSuccess<TValue> : IEquatable<IfSuccess<TValue>>
    {
        private const string ErrorText = "Error";

        /// <summary>Initializes a new instance of the <see cref="IfSuccess{TValue}"/> struct.</summary>
        /// <param name="isSuccess">if set to <c>true</c> [is success].</param>
        /// <param name="value">The value.</param>
        public IfSuccess(bool isSuccess, TValue value)
        {
            this.IsSuccess = isSuccess;
            this.Value = value;
        }

        /// <summary>Gets a value indicating whether this instance is success.</summary>
        /// <value>
        ///   <c>true</c> if this instance is success; otherwise, <c>false</c>.</value>
        public bool IsSuccess { get; }

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
        public static implicit operator bool(IfSuccess<TValue> result)
        {
            return result.IsSuccess;
        }

        /// <summary>Performs an implicit conversion from <see cref="SuccessResult{TValue}"/> to <see cref="IfSuccess{TValue}"/>.</summary>
        /// <param name="successResult">The success result.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator IfSuccess<TValue>(SuccessResult<TValue> successResult)
        {
            return new IfSuccess<TValue>(true, successResult.Value);
        }

        /// <summary>Performs an implicit conversion from <see cref="ErrorResult"/> to <see cref="IfSuccess{TValue}"/>.</summary>
        /// <param name="errorResult">The error result.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator IfSuccess<TValue>(ErrorResult errorResult)
        {
            return new IfSuccess<TValue>(errorResult.IsSuccess, default!);
        }

        /// <summary>Performs an implicit conversion from <see cref="ErrorResult"/> to <see cref="IfSuccess{TValue}"/>.</summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ValueTask<IfSuccess<TValue>>(IfSuccess<TValue> result)
        {
            return result.ToValueTask();
        }

        /// <summary>Implements the operator ==.</summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(IfSuccess<TValue> left, IfSuccess<TValue> right)
        {
            return left.Equals(right);
        }

        /// <summary>Implements the operator !=.</summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(IfSuccess<TValue> left, IfSuccess<TValue> right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Converts this instance to a value task.
        /// </summary>
        /// <returns>The value task.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueTask<IfSuccess<TValue>> ToValueTask()
        {
            return new ValueTask<IfSuccess<TValue>>(this);
        }

        /// <summary>
        /// Creates a result based on the specified values.
        /// </summary>
        /// <typeparam name="TError">The type of the new error.</typeparam>
        /// <param name="error">The error.</param>
        /// <returns>
        /// A new <see cref="Result{TNewValue, TNewError}" />.
        /// </returns>
        public Result<TValue, TError> WithError<TError>(TError error)
        {
            return new Result<TValue, TError>(this.IsSuccess, this.Value, error);
        }

        /// <summary>
        /// Creates a result based on the specified values.
        /// </summary>
        /// <typeparam name="TError">The type of the new error.</typeparam>
        /// <param name="errorFunc">The error function.</param>
        /// <returns>
        /// A new <see cref="Result{TNewValue, TNewError}" />.
        /// </returns>
        public Result<TValue, TError> WithError<TError>(Func<TError> errorFunc)
        {
            return new Result<TValue, TError>(this.IsSuccess, this.Value, this.IsSuccess ? default! : errorFunc());
        }

        /// <summary>
        /// Creates a result based on the specified values.
        /// </summary>
        /// <typeparam name="TNewValue">The type of the new value.</typeparam>
        /// <param name="valueFunc">The value func.</param>
        /// <returns>
        /// A new <see cref="IfSuccess{TNewValue}" />.
        /// </returns>
        public IfSuccess<TNewValue> ConvertValue<TNewValue>(Func<TValue, TNewValue> valueFunc)
        {
            return new IfSuccess<TNewValue>(this.IsSuccess, this.IsSuccess ? valueFunc(this.Value) : default!);
        }

        /// <summary>
        /// Creates a result based on the specified values.
        /// </summary>
        /// <typeparam name="TNewValue">The type of the new value.</typeparam>
        /// <typeparam name="TError">The type of the new error.</typeparam>
        /// <param name="valueFunc">The value function.</param>
        /// <param name="error">The error.</param>
        /// <returns>
        /// A new <see cref="Result{TNewValue, TNewError}" />.
        /// </returns>
        public Result<TNewValue, TError> Convert<TNewValue, TError>(Func<TValue, TNewValue> valueFunc, TError error)
        {
            return new Result<TNewValue, TError>(this.IsSuccess, this.IsSuccess ? valueFunc(this.Value) : default!, error);
        }

        /// <summary>
        /// Creates a result based on the specified values.
        /// </summary>
        /// <typeparam name="TNewValue">The type of the new value.</typeparam>
        /// <typeparam name="TError">The type of the new error.</typeparam>
        /// <param name="valueFunc">The value function.</param>
        /// <param name="errorFunc">The error function.</param>
        /// <returns>
        /// A new <see cref="Result{TNewValue, TNewError}" />.
        /// </returns>
        public Result<TNewValue, TError> Convert<TNewValue, TError>(Func<TValue, TNewValue> valueFunc, Func<TError> errorFunc)
        {
            if (this.IsSuccess)
            {
                return new Result<TNewValue, TError>(this.IsSuccess, valueFunc(this.Value), default!);
            }

            return new Result<TNewValue, TError>(this.IsSuccess, default!, errorFunc());
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
        public void Deconstruct(out bool isSuccess, out TValue value)
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
        public bool Equals(IfSuccess<TValue> other)
        {
            return this.IsSuccess == other.IsSuccess && Equals(this.Value, other.Value);
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
            return EqualityHelper.GetHashCode(this.IsSuccess.GetHashCode(), this.Value?.GetHashCode() ?? 0);
        }
    }
}