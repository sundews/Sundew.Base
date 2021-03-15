// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SuccessResult.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Computation
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    /// <summary>
    /// A successful result.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "By design.")]
    public readonly struct SuccessResult
    {
        private const string SuccessText = "Success";

        /// <summary>Gets a value indicating whether this instance is success.</summary>
        /// <value>
        ///   <c>true</c> if this instance is success; otherwise, <c>false</c>.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Alternative to implicit boolean operator.")]
        public bool IsSuccess => true;

        internal static SuccessResult Result { get; } = default;

        /// <summary>
        /// Always returns false.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>Always true.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Use IsSuccess property instead.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Use IsSuccess property instead.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "By design.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Redundancy", "RCS1163:Unused parameter.", Justification = "By design.")]
        public static implicit operator bool(SuccessResult result)
        {
            return true;
        }

        /// <summary>Performs an implicit conversion from <see cref="SuccessResult"/> to <see cref="ValueTask{SuccessResult}"/>.</summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ValueTask<SuccessResult>(SuccessResult result)
        {
            return result.ToValueTask();
        }

        /// <summary>
        /// Converts this instance to a value task.
        /// </summary>
        /// <returns>The value task.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueTask<SuccessResult> ToValueTask()
        {
            return new ValueTask<SuccessResult>(this);
        }

        /// <summary>
        /// Creates a result based on the specified values.
        /// </summary>
        /// <typeparam name="TNewValue">The type of the new value.</typeparam>
        /// <param name="valueFunc">The value function.</param>
        /// <returns>
        /// A new <see cref="Result{TNewValue, TNewError}" />.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "By design.")]
        public Result.IfSuccess<TNewValue> Convert<TNewValue>(Func<TNewValue> valueFunc)
        {
            return new Result.IfSuccess<TNewValue>(true, valueFunc());
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return SuccessText;
        }
    }
}