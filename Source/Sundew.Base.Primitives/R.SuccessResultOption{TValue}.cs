// --------------------------------------------------------------------------------------------------------------------
// <copyright file="R.SuccessResultOption{TValue}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base;

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

/// <summary>
/// Factory class for creating results.
/// </summary>
public partial class R
{
    /// <summary>
    /// Converts the success option result into a <see cref="R{TValue}"/> which will fail if the value is null.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="result">The result.</param>
    /// <returns>A <see cref="R{TValue}"/>.</returns>
    public static R<TValue> Map<TValue>(this SuccessOptionResult<TValue?> result)
    {
        return new R<TValue>(result.Value != null, result.Value);
    }

    /// <summary>
    /// Converts the success option result into a <see cref="R{TValue}"/> which will fail if the value is null.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="errorFunc">The error func.</param>
    /// <returns>A <see cref="R{TValue}"/>.</returns>
    public static R<TValue, TError> Map<TValue, TError>(this SuccessOptionResult<TValue?> result, Func<TError> errorFunc)
    {
        return new R<TValue, TError>(result.Value != null, result.Value, result.Value != null ? errorFunc() : default);
    }

    /// <summary>
    /// A successful result.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public readonly struct SuccessOptionResult<TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessOptionResult{TValue}"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        internal SuccessOptionResult(TValue value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public TValue Value { get; }

        /// <summary>
        /// Always returns false.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>A value indicating whether the result was successful.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public static implicit operator bool(SuccessOptionResult<TValue> result)
        {
            return true;
        }

        /// <summary>
        /// Gets the result's value property.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>A value indicating whether the result was successful.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public static implicit operator TValue?(SuccessOptionResult<TValue> result)
        {
            return result.Value;
        }

        /// <summary>Performs an implicit conversion from <see cref="SuccessOptionResult{TValue}"/> to <see cref="ValueTask{T}"/> of <see cref="SuccessOptionResult{TValue}"/> .</summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public static implicit operator ValueTask<SuccessOptionResult<TValue?>>(SuccessOptionResult<TValue?> result)
        {
            return result.ToValueTask();
        }

        /// <summary>Performs an implicit conversion from <see cref="SuccessOptionResult{TValue}"/> to <see cref="Task{T}"/> of <see cref="SuccessOptionResult{TValue}"/> .</summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public static implicit operator Task<SuccessOptionResult<TValue?>>(SuccessOptionResult<TValue?> result)
        {
            return result.ToTask();
        }

        /// <summary>Performs an implicit conversion from <see cref="SuccessOptionResult{TValue}"/> to <see cref="ValueTask{T}"/> of <see cref="R{TValue}"/> .</summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public static implicit operator ValueTask<R<TValue?>>(SuccessOptionResult<TValue?> result)
        {
            return new R<TValue?>(true, result.Value).ToValueTask();
        }

        /// <summary>Performs an implicit conversion from <see cref="SuccessOptionResult{TValue}"/> to <see cref="Task{T}"/> of <see cref="R{TValue}"/> .</summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public static implicit operator Task<R<TValue?>>(SuccessOptionResult<TValue?> result)
        {
            return new R<TValue?>(true, result.Value).ToTask();
        }

        /// <summary>Performs an implicit conversion from <see cref="ValueTask{SuccessResult}"/> to <see cref="SuccessResult{TValue}"/>.</summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public static implicit operator SuccessOptionResult<TValue?>(SuccessResult result)
        {
            return new SuccessOptionResult<TValue?>(default(TValue?));
        }

        /// <summary>
        /// Converts this instance to a value task.
        /// </summary>
        /// <returns>The value task.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public ValueTask<SuccessOptionResult<TValue?>> ToValueTask()
        {
            return new ValueTask<SuccessOptionResult<TValue?>>(new SuccessOptionResult<TValue?>(this.Value));
        }

        /// <summary>
        /// Converts this instance to a task.
        /// </summary>
        /// <returns>The value task.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public Task<SuccessOptionResult<TValue?>> ToTask()
        {
            return Task.FromResult(new SuccessOptionResult<TValue?>(this.Value));
        }

        /// <summary>
        /// Creates a result based on the specified values.
        /// </summary>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <returns>
        /// A new <see cref="R" />.
        /// </returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public R<TValue?, TError> Omits<TError>()
        {
            return new R<TValue?, TError>(true, this.Value, default);
        }

        /// <summary>
        /// Creates a result based on the specified values.
        /// </summary>
        /// <typeparam name="TNewValue">The type of the new value.</typeparam>
        /// <param name="valueFunc">The value function.</param>
        /// <returns>
        /// A new <see cref="R" />.
        /// </returns>
        public SuccessOptionResult<TNewValue?> Map<TNewValue>(Func<TValue?, TNewValue?> valueFunc)
        {
            return new SuccessOptionResult<TNewValue?>(valueFunc(this.Value));
        }

        /// <summary>
        /// Creates a result based on the specified values.
        /// </summary>
        /// <typeparam name="TNewValue">The type of the new value.</typeparam>
        /// <param name="valueFunc">The value function.</param>
        /// <returns>
        /// A new <see cref="R" />.
        /// </returns>
        public ValueTask<SuccessOptionResult<TNewValue?>> MapAsync<TNewValue>(Func<TValue?, TNewValue?> valueFunc)
        {
            return new SuccessOptionResult<TNewValue?>(valueFunc(this.Value)).ToValueTask();
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"Success: {this.Value}";
        }

        /// <summary>
        /// Converts this result to a result option.
        /// </summary>
        /// <returns>A <see cref="R{TSuccess}"/>.</returns>
        public R<TValue?> ToOption()
        {
            return new R<TValue?>(true, this.Value);
        }

        /// <summary>
        /// Converts this result to a result option.
        /// </summary>
        /// <returns>A <see cref="R{TSuccess}"/>.</returns>
        public R<object?> ToObjectOption()
        {
            return new R<object?>(true, this.Value);
        }
    }
}