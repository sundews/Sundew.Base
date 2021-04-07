// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Result{TValue}.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives.Computation
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Sundew.Base.Equality;

    /// <summary>Contains extension methods for the Result class.</summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public readonly struct Result<TValue> : IEquatable<Result<TValue>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Result{TValue}" /> struct.
        /// </summary>
        /// <param name="isSuccess">If set to <c>true</c> [success].</param>
        /// <param name="value">The result value.</param>
        internal Result(bool isSuccess, TValue value)
        {
            this.IsSuccess = isSuccess;
            this.Value = value;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Result{TValue}"/> is success.
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
        /// Gets the result's success property.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>A value indicating whether the result was successful.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Use IsSuccess property instead.")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator bool(Result<TValue> result)
        {
            return result.IsSuccess;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="SuccessResult{TValue}"/> to <see cref="Result{TValue}"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "By design.")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Result<TValue>(SuccessResult<TValue> result)
        {
            return new Result<TValue>(true, result.Value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="ErrorResult{TValue}"/> to <see cref="Result{TValue, TError}"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "By design.")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Result<TValue>(ErrorResult<TValue> result)
        {
            return new Result<TValue>(false, result.Error);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="SuccessResult{TValue}"/> to <see cref="Result{TValue, TError}"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "By design.")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Result<TValue>(TrueResult result)
        {
            return new Result<TValue>(result.IsSuccess, default!);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="ErrorResult{TValue}"/> to <see cref="Result{TValue, TError}"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "By design.")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Result<TValue>(FalseResult result)
        {
            return new Result<TValue>(result.IsSuccess, default!);
        }

        /// <summary>Performs an implicit conversion from <see cref="ErrorResult"/> to <see cref="Result.IfSuccess{TValue}"/>.</summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ValueTask<Result<TValue>>(Result<TValue> result)
        {
            return result.ToValueTask();
        }

        /// <summary>Implements the operator ==.</summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Result<TValue> left, Result<TValue> right)
        {
            return left.Equals(right);
        }

        /// <summary>Implements the operator !=.</summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Result<TValue> left, Result<TValue> right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Converts this instance to a value task.
        /// </summary>
        /// <returns>The value task.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueTask<Result<TValue>> ToValueTask()
        {
            return new ValueTask<Result<TValue>>(this);
        }

        /// <summary>
        /// Creates a result based on the specified values.
        /// </summary>
        /// <typeparam name="TNewValue">The type of the new value.</typeparam>
        /// <param name="valueFunc">The value function.</param>
        /// <returns>
        /// A new <see cref="Result{TNewValue, TError}" />.
        /// </returns>
        public Result<TNewValue, TValue> ConvertValue<TNewValue>(Func<TValue, TNewValue> valueFunc)
        {
            return this.Convert(valueFunc, error => error);
        }

        /// <summary>
        /// Creates a result based on the specified result .
        /// </summary>
        /// <typeparam name="TError">The type of the new error.</typeparam>
        /// <param name="errorFunc">The error function.</param>
        /// <returns>
        /// A new <see cref="Result{TNewValue, TNewError}" />.
        /// </returns>
        public Result<TValue, TError> ConvertError<TError>(Func<TValue, TError> errorFunc)
        {
            return this.Convert(value => value, errorFunc);
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
        public Result<TNewValue, TError> Convert<TNewValue, TError>(Func<TValue, TNewValue> valueFunc, Func<TValue, TError> errorFunc)
        {
            if (this.IsSuccess)
            {
                return new Result<TNewValue, TError>(this.IsSuccess, valueFunc(this.Value), default!);
            }

            return new Result<TNewValue, TError>(this.IsSuccess, default!, errorFunc(this.Value));
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
                return $"True: {this.Value}";
            }

            return $"False: {this.Value}";
        }

        /// <summary>
        /// Deconstructs the result, value and error.
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
        public bool Equals(Result<TValue> other)
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