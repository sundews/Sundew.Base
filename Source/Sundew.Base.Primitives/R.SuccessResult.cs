// --------------------------------------------------------------------------------------------------------------------
// <copyright file="R.SuccessResult.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base;

using System.Diagnostics.CodeAnalysis;
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
    public readonly struct SuccessResult
    {
        private const string SuccessText = "Success";

        /// <summary>Gets a value indicating whether this instance is success.</summary>
        /// <value>
        ///   <c>true</c> if this instance is success; otherwise, <c>false</c>.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Alternative to implicit boolean operator.")]
        public bool IsSuccess => true;

        internal static SuccessResult Result => default;

        /// <summary>
        /// Always returns false.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>Always true.</returns>
        public static implicit operator bool(SuccessResult result)
        {
            return true;
        }

        /// <summary>Performs an implicit conversion from <see cref="SuccessResult"/> to <see cref="ValueTask{SuccessResult}"/>.</summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public static implicit operator ValueTask<SuccessResult>(SuccessResult result)
        {
            return result.ToValueTask();
        }

        /// <summary>
        /// Converts this instance to a value task.
        /// </summary>
        /// <returns>The value task.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public ValueTask<SuccessResult> ToValueTask()
        {
            return new ValueTask<SuccessResult>(this);
        }

        /// <summary>
        /// Converts this <see cref="R.SuccessResult"/> to a successful <see cref="R{TError}"/>.
        /// </summary>
        /// <typeparam name="TError">The error type.</typeparam>
        /// <returns>An erroneous result.</returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Design choice")]
        public R<TError> _<TError>()
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
            return SuccessText;
        }
    }
}