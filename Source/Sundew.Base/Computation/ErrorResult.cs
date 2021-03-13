// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorResult.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Computation
{
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    /// <summary>
    /// Indicates an erroneous result.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "By design.")]
    public readonly struct ErrorResult
    {
        private const string ErrorText = "Error";

        /// <summary>Gets a value indicating whether this instance is success.</summary>
        /// <value>
        ///   <c>true</c> if this instance is success; otherwise, <c>false</c>.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Alternative to implicit boolean operator.")]
        public bool IsSuccess => false;

        internal static ErrorResult Result { get; } = default;

        /// <summary>
        /// Always returns false.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>A value indicating whether the result was successful.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Operators have to take a parameter.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "By design.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Use IsSuccess property instead.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Redundancy", "RCS1163:Unused parameter.", Justification = "By design.")]
        public static implicit operator bool(ErrorResult result)
        {
            return false;
        }

        /// <summary>Performs an implicit conversion from <see cref="ErrorResult"/> to <see cref="Computation.Result.IfSuccess{TValue}"/>.</summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ValueTask<ErrorResult>(ErrorResult result)
        {
            return result.ToValueTask();
        }

        /// <summary>
        /// Converts this instance to a value task.
        /// </summary>
        /// <returns>The value task.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueTask<ErrorResult> ToValueTask()
        {
            return new ValueTask<ErrorResult>(this);
        }

        /// <summary>
        /// Creates a result based on the specified result .
        /// </summary>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="error">The error.</param>
        /// <returns>
        /// A new <see cref="Result{TNewValue, TNewError}" />.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Part of the Api design.")]
        public ErrorResult<TError> WithError<TError>(TError error)
        {
            return new ErrorResult<TError>(error);
        }

        /// <summary>
        /// Creates a result based on the specified result .
        /// </summary>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="error">The error.</param>
        /// <returns>
        /// A new <see cref="Result{TNewValue, TNewError}" />.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Part of the Api design.")]
        public ValueTask<ErrorResult<TError>> WithErrorAsync<TError>(TError error)
        {
            return new ErrorResult<TError>(error).ToValueTask();
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ErrorText;
        }
    }
}