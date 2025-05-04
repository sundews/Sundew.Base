// --------------------------------------------------------------------------------------------------------------------
// <copyright file="R.SuccessResult{TValue}.cs" company="Sundews">
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
    /// A successful result.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the value.</typeparam>
    public readonly struct SuccessResult<TSuccess>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessResult{TValue}"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        internal SuccessResult(TSuccess value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public TSuccess Value { get; }

        /// <summary>
        /// Always returns false.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>A value indicating whether the result was successful.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public static implicit operator bool(SuccessResult<TSuccess> result)
        {
            return true;
        }

        /// <summary>
        /// Gets the result's value property.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>A value indicating whether the result was successful.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public static implicit operator TSuccess(SuccessResult<TSuccess> result)
        {
            return result.Value;
        }

        /// <summary>Performs an implicit conversion from <see cref="SuccessResult{TSuccess}"/> to <see cref="ValueTask{T}"/> of <see cref="SuccessResult{TValue}"/>.</summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public static implicit operator ValueTask<SuccessResult<TSuccess>>(SuccessResult<TSuccess> result)
        {
            return result.ToValueTask();
        }

        /// <summary>Performs an implicit conversion from <see cref="SuccessResult{TSuccess}"/> to <see cref="Task{T}"/> of <see cref="SuccessResult{TValue}"/>.</summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public static implicit operator Task<SuccessResult<TSuccess>>(SuccessResult<TSuccess> result)
        {
            return result.ToTask();
        }

        /// <summary>Performs an implicit conversion from <see cref="SuccessResult{TSuccess}"/> to <see cref="ValueTask{T}"/> of <see cref="R{TValue}"/>.</summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public static implicit operator ValueTask<R<TSuccess>>(SuccessResult<TSuccess> result)
        {
            return result.Map().ToValueTask();
        }

        /// <summary>Performs an implicit conversion from <see cref="SuccessResult{TSuccess}"/> to  <see cref="Task{T}"/> of <see cref="R{TValue}"/>.</summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public static implicit operator Task<R<TSuccess>>(SuccessResult<TSuccess> result)
        {
            return result.Map().ToTask();
        }

        /// <summary>Performs an implicit conversion from <see cref="SuccessResult"/> to <see cref="SuccessResult{TValue}"/>.</summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public static implicit operator SuccessResult<TSuccess?>(SuccessResult result)
        {
            return new SuccessResult<TSuccess?>(default(TSuccess?));
        }

        /// <summary>
        /// Converts this instance to a value task.
        /// </summary>
        /// <returns>The value task.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public ValueTask<SuccessResult<TSuccess>> ToValueTask()
        {
            return new ValueTask<SuccessResult<TSuccess>>(this);
        }

        /// <summary>
        /// Converts this instance to a task.
        /// </summary>
        /// <returns>The value task.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public Task<SuccessResult<TSuccess>> ToTask()
        {
            return Task.FromResult(this);
        }

        /// <summary>
        /// Converts this result to a <see cref="R{TValue, TError}"/>.
        /// </summary>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <returns>
        /// A new <see cref="R" />.
        /// </returns>
        public R<TSuccess, TError> Omits<TError>()
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
        public SuccessResult<TNewValue> To<TNewValue>(Func<TSuccess, TNewValue> valueFunc)
        {
            return new SuccessResult<TNewValue>(valueFunc(this.Value));
        }

        /// <summary>
        /// Creates a result based on the specified values.
        /// </summary>
        /// <typeparam name="TNewValue">The type of the new value.</typeparam>
        /// <param name="valueFunc">The value function.</param>
        /// <returns>
        /// A new <see cref="R" />.
        /// </returns>
        public ValueTask<SuccessResult<TNewValue>> ToAsync<TNewValue>(Func<TSuccess, TNewValue> valueFunc)
        {
            return new SuccessResult<TNewValue>(valueFunc(this.Value)).ToValueTask();
        }

        /// <summary>
        /// Maps this <see cref="SuccessResult"/> to a <see cref="R{TNewSuccess}"/>.
        /// </summary>
        /// <typeparam name="TNewSuccess">The new success.</typeparam>
        /// <param name="mapFunc">The map function.</param>
        /// <returns>A <see cref="R{TNewSuccess}"/>.</returns>
        public R<TNewSuccess> Map<TNewSuccess>(Func<TSuccess, TNewSuccess> mapFunc)
        {
            return new R<TNewSuccess>(true, mapFunc(this.Value));
        }

        /// <summary>
        /// Converts this result to a result option.
        /// </summary>
        /// <returns>A <see cref="R{TSuccess}"/>.</returns>
        public R<TSuccess?> ToOptional()
        {
            return this.Map(x => (TSuccess?)x);
        }

        /// <summary>
        /// Converts the current instance to an <see cref="R{TSuccess}"/> result.
        /// </summary>
        /// <remarks>This method allows the current instance to be treated as an <see cref="R{TSuccess}"/>
        /// result, enabling seamless usage in contexts where an <see cref="R{TSuccess}"/> is expected.</remarks>
        /// <returns>The current instance as an <see cref="R{TSuccess}"/> result.</returns>
        public R<TSuccess> Map()
        {
            return this;
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
    }
}