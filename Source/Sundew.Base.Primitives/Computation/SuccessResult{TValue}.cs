// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SuccessResult{TValue}.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives.Computation
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    /// <summary>
    /// A successful result.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "By design.")]
    public readonly struct SuccessResult<TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessResult{TValue}"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        internal SuccessResult(TValue value)
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

        /// <summary>Gets a value indicating whether this instance is success.</summary>
        /// <value>
        ///   <c>true</c> if this instance is success; otherwise, <c>false</c>.</value>
        public bool IsSuccess => true;

        /// <summary>
        /// Always returns false.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>A value indicating whether the result was successful.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Use IsSuccess property instead.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "By design.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "By design.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Redundancy", "RCS1163:Unused parameter.", Justification = "By design.")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator bool(SuccessResult<TValue> result)
        {
            return true;
        }

        /// <summary>
        /// Gets the result's value property.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>A value indicating whether the result was successful.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Use Value property instead.")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator TValue(SuccessResult<TValue> result)
        {
            return result.Value;
        }

        /// <summary>Performs an implicit conversion from <see cref="ErrorResult"/> to <see cref="Result.IfSuccess{TValue}"/>.</summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ValueTask<SuccessResult<TValue>>(SuccessResult<TValue> result)
        {
            return result.ToValueTask();
        }

        /// <summary>
        /// Converts this instance to a value task.
        /// </summary>
        /// <returns>The value task.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueTask<SuccessResult<TValue>> ToValueTask()
        {
            return new ValueTask<SuccessResult<TValue>>(this);
        }

        /// <summary>
        /// Creates a result based on the specified values.
        /// </summary>
        /// <typeparam name="TNewValue">The type of the new value.</typeparam>
        /// <param name="valueFunc">The value function.</param>
        /// <returns>
        /// A new <see cref="Result{TNewValue, TNewError}" />.
        /// </returns>
        public SuccessResult<TNewValue> Convert<TNewValue>(Func<TValue, TNewValue> valueFunc)
        {
            return new SuccessResult<TNewValue>(valueFunc(this.Value));
        }

        /// <summary>
        /// Creates a result based on the specified values.
        /// </summary>
        /// <typeparam name="TNewValue">The type of the new value.</typeparam>
        /// <param name="valueFunc">The value function.</param>
        /// <returns>
        /// A new <see cref="Result{TNewValue, TNewError}" />.
        /// </returns>
        public ValueTask<SuccessResult<TNewValue>> ConvertAsync<TNewValue>(Func<TValue, TNewValue> valueFunc)
        {
            return new SuccessResult<TNewValue>(valueFunc(this.Value)).ToValueTask();
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
        /// Deconstructs the result and value.
        /// </summary>
        /// <param name="isSuccess">if set to <c>true</c> [is success].</param>
        /// <param name="value">The value.</param>
        public void Deconstruct(out bool isSuccess, out TValue value)
        {
            isSuccess = true;
            value = this.Value;
        }
    }
}