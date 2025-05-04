// --------------------------------------------------------------------------------------------------------------------
// <copyright file="R.ErrorResult.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base;

using System.Runtime.CompilerServices;
using System.Threading.Tasks;

/// <summary>
/// Factory class for creating results.
/// </summary>
public partial class R
{
    /// <summary>
    /// R for indicating an error.
    /// </summary>
    public readonly struct ErrorResult
    {
        private const string ErrorText = "Error";

        /// <summary>Gets a value indicating whether this instance is success.</summary>
        /// <value>
        ///   <c>true</c> if this instance is success; otherwise, <c>false</c>.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Alternative to implicit boolean operator.")]
        public bool IsSuccess => false;

        /// <summary>
        /// Always returns false.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>A value indicating whether the result was successful.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "By design.")]
        public static implicit operator bool(ErrorResult result)
        {
            return false;
        }

        /// <summary>Performs an implicit conversion from <see cref="ValueTask{ErrorResult}"/> to <see cref="ErrorResult{TValue}"/>.</summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public static implicit operator ValueTask<ErrorResult>(ErrorResult result)
        {
            return result.ToValueTask();
        }

        /// <summary>
        /// Converts this instance to a value task.
        /// </summary>
        /// <returns>The value task.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public ValueTask<ErrorResult> ToValueTask()
        {
            return new ValueTask<ErrorResult>(this);
        }

        /// <summary>
        /// Converts this instance to a task.
        /// </summary>
        /// <returns>The value task.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public Task<ErrorResult> ToTask()
        {
            return Task.FromResult(this);
        }

        /// <summary>
        /// Converts the current instance to an <see cref="R{TSuccess}"/> result.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success.</typeparam>
        /// <remarks>This method allows the current instance to be treated as an <see cref="R{TSuccess}"/>
        /// result, enabling seamless usage in contexts where an <see cref="R{TSuccess}"/> is expected.</remarks>
        /// <returns>The current instance as an <see cref="R{TSuccess}"/> result.</returns>
        public R<TSuccess> Omits<TSuccess>()
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
            return ErrorText;
        }
    }
}