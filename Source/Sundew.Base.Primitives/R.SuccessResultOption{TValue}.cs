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
    /// A successful result.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public readonly struct SuccessResultOption<TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessResultOption{TValue}"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        internal SuccessResultOption(TValue value)
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
        public static implicit operator bool(SuccessResultOption<TValue> result)
        {
            return true;
        }

        /// <summary>
        /// Gets the result's value property.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>A value indicating whether the result was successful.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public static implicit operator TValue(SuccessResultOption<TValue> result)
        {
            return result.Value;
        }

        /// <summary>Performs an implicit conversion from <see cref="SuccessResultOption{TValue}"/> to <see cref="ValueTask{T}"/> of <see cref="SuccessResultOption{TValue}"/> .</summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public static implicit operator ValueTask<SuccessResultOption<TValue?>>(SuccessResultOption<TValue?> result)
        {
            return result.ToValueTask();
        }

        /// <summary>Performs an implicit conversion from <see cref="SuccessResultOption{TValue}"/> to <see cref="Task{T}"/> of <see cref="SuccessResultOption{TValue}"/> .</summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public static implicit operator Task<SuccessResultOption<TValue?>>(SuccessResultOption<TValue?> result)
        {
            return result.ToTask();
        }

        /// <summary>Performs an implicit conversion from <see cref="SuccessResultOption{TValue}"/> to <see cref="ValueTask{T}"/> of <see cref="R{TValue}"/> .</summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public static implicit operator ValueTask<R<TValue?>>(SuccessResultOption<TValue?> result)
        {
            return result.Map().ToValueTask();
        }

        /// <summary>Performs an implicit conversion from <see cref="SuccessResultOption{TValue}"/> to <see cref="Task{T}"/> of <see cref="R{TValue}"/> .</summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public static implicit operator Task<R<TValue?>>(SuccessResultOption<TValue?> result)
        {
            return result.Map().ToTask();
        }

        /// <summary>Performs an implicit conversion from <see cref="ValueTask{SuccessResult}"/> to <see cref="SuccessResult{TValue}"/>.</summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public static implicit operator SuccessResultOption<TValue?>(SuccessResult result)
        {
            return new SuccessResultOption<TValue?>(default(TValue?));
        }

        /// <summary>
        /// Converts this instance to a value task.
        /// </summary>
        /// <returns>The value task.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public ValueTask<SuccessResultOption<TValue?>> ToValueTask()
        {
            return new ValueTask<SuccessResultOption<TValue?>>(new SuccessResultOption<TValue?>(this.Value));
        }

        /// <summary>
        /// Converts this instance to a task.
        /// </summary>
        /// <returns>The value task.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public Task<SuccessResultOption<TValue?>> ToTask()
        {
            return Task.FromResult(new SuccessResultOption<TValue?>(this.Value));
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
        public SuccessResultOption<TNewValue?> Map<TNewValue>(Func<TValue?, TNewValue?> valueFunc)
        {
            return new SuccessResultOption<TNewValue?>(valueFunc(this.Value));
        }

        /// <summary>
        /// Creates a result based on the specified values.
        /// </summary>
        /// <typeparam name="TNewValue">The type of the new value.</typeparam>
        /// <param name="valueFunc">The value function.</param>
        /// <returns>
        /// A new <see cref="R" />.
        /// </returns>
        public ValueTask<SuccessResultOption<TNewValue?>> MapAsync<TNewValue>(Func<TValue?, TNewValue?> valueFunc)
        {
            return new SuccessResultOption<TNewValue?>(valueFunc(this.Value)).ToValueTask();
        }

        /// <summary>
        /// Converts the current instance to an <see cref="R{TSuccess}"/> result.
        /// </summary>
        /// <remarks>This method allows the current instance to be treated as an <see cref="R{TSuccess}"/>
        /// result, enabling seamless usage in contexts where an <see cref="R{TSuccess}"/> is expected.</remarks>
        /// <returns>The current instance as an <see cref="R{TSuccess}"/> result.</returns>
        public R<TValue> Map()
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