// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Result.IfError{TError}.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Computation
{
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
        /// <summary>Represents a result that if it is erroneous has error information.</summary>
        /// <typeparam name="TError">The type of the error.</typeparam>
        [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "By design.")]
        public readonly struct IfError<TError> : IEquatable<IfError<TError>>
        {
            private const string SuccessText = "Success";

            /// <summary>Initializes a new instance of the <see cref="IfError{TError}"/> struct.</summary>
            /// <param name="isSuccess">if set to <c>true</c> [is error].</param>
            /// <param name="error">The error.</param>
            public IfError(bool isSuccess, TError error)
            {
                this.IsSuccess = isSuccess;
                this.Error = error;
            }

            /// <summary>Gets a value indicating whether this instance is success.</summary>
            /// <value>
            ///   <c>true</c> if this instance is success; otherwise, <c>false</c>.</value>
            public bool IsSuccess { get; }

            /// <summary>Gets the error.</summary>
            /// <value>The error.</value>
            [AllowNull]
            public TError Error { get; }

            /// <summary>
            /// Gets the result's success property.
            /// </summary>
            /// <param name="result">The result.</param>
            /// <returns>A value indicating whether the result was successful.</returns>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Use IsSuccess property instead.")]
            public static implicit operator bool(IfError<TError> result)
            {
                return result.IsSuccess;
            }

            /// <summary>Performs an implicit conversion from <see cref="SuccessResult"/> to <see cref="IfError{TValue}"/>.</summary>
            /// <param name="successResult">The error result.</param>
            /// <returns>The result of the conversion.</returns>
            [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "By design")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator IfError<TError>(SuccessResult successResult)
            {
                return new IfError<TError>(successResult.IsSuccess, default!);
            }

            /// <summary>Performs an implicit conversion from <see cref="ErrorResult"/> to <see cref="IfSuccess{TValue}"/>.</summary>
            /// <param name="errorResult">The error result.</param>
            /// <returns>The result of the conversion.</returns>
            [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "By design")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator IfError<TError>(ErrorResult<TError> errorResult)
            {
                return new IfError<TError>(false, errorResult.Error);
            }

            /// <summary>Performs an implicit conversion from <see cref="ErrorResult"/> to <see cref="IfSuccess{TValue}"/>.</summary>
            /// <param name="errorResult">The error result.</param>
            /// <returns>The result of the conversion.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator ValueTask<IfError<TError>>(IfError<TError> errorResult)
            {
                return errorResult.ToValueTask();
            }

            /// <summary>Performs an implicit conversion from <see cref="ErrorResult"/> to <see cref="Result.IfSuccess{TValue}"/>.</summary>
            /// <param name="result">The result.</param>
            /// <returns>The result of the conversion.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator Task<IfError<TError>>(IfError<TError> result)
            {
                return result.ToTask();
            }

            /// <summary>Implements the operator ==.</summary>
            /// <param name="left">The left.</param>
            /// <param name="right">The right.</param>
            /// <returns>The result of the operator.</returns>
            public static bool operator ==(IfError<TError> left, IfError<TError> right)
            {
                return left.Equals(right);
            }

            /// <summary>Implements the operator !=.</summary>
            /// <param name="left">The left.</param>
            /// <param name="right">The right.</param>
            /// <returns>The result of the operator.</returns>
            public static bool operator !=(IfError<TError> left, IfError<TError> right)
            {
                return !(left == right);
            }

            /// <summary>
            /// Converts this instance to a value task.
            /// </summary>
            /// <returns>The value task.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<IfError<TError>> ToValueTask()
            {
                return new ValueTask<IfError<TError>>(this);
            }

            /// <summary>
            /// Converts this instance to a task.
            /// </summary>
            /// <returns>The value task.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Task<IfError<TError>> ToTask()
            {
                return Task.FromResult(this);
            }

            /// <summary>
            /// Creates a result based on the specified values.
            /// </summary>
            /// <typeparam name="TValue">The type of the value.</typeparam>
            /// <param name="value">The value.</param>
            /// <returns>
            /// A new <see cref="Result{TNewValue, TNewError}" />.
            /// </returns>
            public Result<TValue, TError> WithValue<TValue>(TValue value)
            {
                return new Result<TValue, TError>(this.IsSuccess, value, this.Error);
            }

            /// <summary>
            /// Creates a result based on the specified values.
            /// </summary>
            /// <typeparam name="TValue">The type of the value.</typeparam>
            /// <param name="valueFunc">The value function.</param>
            /// <returns>
            /// A new <see cref="Result{TNewValue, TNewError}" />.
            /// </returns>
            public Result<TValue, TError> WithValue<TValue>(Func<TValue> valueFunc)
            {
                return new Result<TValue, TError>(this.IsSuccess, this.IsSuccess ? valueFunc() : default!, this.Error);
            }

            /// <summary>
            /// Creates a result based on the specified values.
            /// </summary>
            /// <typeparam name="TNewError">The type of the new error.</typeparam>
            /// <param name="errorFunc">The error func.</param>
            /// <returns>
            /// A new <see cref="IfSuccess{TNewValue}" />.
            /// </returns>
            public Result.IfError<TNewError> ConvertError<TNewError>(Func<TError, TNewError> errorFunc)
            {
                return new IfError<TNewError>(this.IsSuccess, this.IsSuccess ? default! : errorFunc(this.Error));
            }

            /// <summary>
            /// Creates a result based on the specified values.
            /// </summary>
            /// <typeparam name="TValue">The type of the new value.</typeparam>
            /// <typeparam name="TNewError">The type of the new error.</typeparam>
            /// <param name="value">The value.</param>
            /// <param name="errorFunc">The error function.</param>
            /// <returns>
            /// A new <see cref="Result{TNewValue, TNewError}" />.
            /// </returns>
            public Result<TValue, TNewError> Convert<TValue, TNewError>(TValue value, Func<TError, TNewError> errorFunc)
            {
                return new Result<TValue, TNewError>(this.IsSuccess, value, this.IsSuccess ? default! : errorFunc(this.Error));
            }

            /// <summary>
            /// Creates a result based on the specified values.
            /// </summary>
            /// <typeparam name="TValue">The type of the new value.</typeparam>
            /// <typeparam name="TNewError">The type of the new error.</typeparam>
            /// <param name="valueFunc">The value function.</param>
            /// <param name="errorFunc">The error function.</param>
            /// <returns>
            /// A new <see cref="Result{TNewValue, TNewError}" />.
            /// </returns>
            public Result<TValue, TNewError> Convert<TValue, TNewError>(Func<TValue> valueFunc, Func<TError, TNewError> errorFunc)
            {
                if (this.IsSuccess)
                {
                    return new Result<TValue, TNewError>(this.IsSuccess, valueFunc(), default!);
                }

                return new Result<TValue, TNewError>(this.IsSuccess, default!, errorFunc(this.Error));
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
            public bool Equals(IfError<TError> other)
            {
                return this.IsSuccess == other.IsSuccess && Equals(this.Error, other.Error);
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
                return EqualityHelper.GetHashCode(this.IsSuccess.GetHashCode(), this.Error?.GetHashCode() ?? 0);
            }
        }
    }
}