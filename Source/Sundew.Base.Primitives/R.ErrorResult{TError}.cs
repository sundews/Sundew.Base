// --------------------------------------------------------------------------------------------------------------------
// <copyright file="R.ErrorResult{TError}.cs" company="Sundews">
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
    /// R for indicating an error.
    /// </summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    public readonly struct ErrorResult<TError>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorResult{TError}"/> struct.
        /// </summary>
        /// <param name="error">The error.</param>
        internal ErrorResult(TError error)
        {
            this.Error = error;
        }

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public TError Error { get; }

        /// <summary>
        /// Always returns false.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>A value indicating whether the result was successful.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "By design.")]
        public static implicit operator bool(ErrorResult<TError> result)
        {
            return false;
        }

        /// <summary>
        /// Gets the result's value property.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>A value indicating whether the result was successful.</returns>
        public static implicit operator TError(ErrorResult<TError> result)
        {
            return result.Error;
        }

        /// <summary>Performs an implicit conversion from <see cref="ValueTask{ErrorResult}"/> to <see cref="ErrorResult{TValue}"/>.</summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public static implicit operator ValueTask<ErrorResult<TError>>(ErrorResult<TError> result)
        {
            return result.ToValueTask();
        }

        /// <summary>
        /// Converts this instance to a value task.
        /// </summary>
        /// <returns>The value task.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public ValueTask<ErrorResult<TError>> ToValueTask()
        {
            return new ValueTask<ErrorResult<TError>>(this);
        }

        /// <summary>
        /// Converts this instance to a task.
        /// </summary>
        /// <returns>The value task.</returns>
        [MethodImpl((MethodImplOptions)0x300)]
        public Task<ErrorResult<TError>> ToTask()
        {
            return Task.FromResult(this);
        }

        /// <summary>
        /// Creates a result based on the specified result .
        /// </summary>
        /// <typeparam name="TNewError">The type of the new error.</typeparam>
        /// <param name="errorFunc">The error function.</param>
        /// <returns>
        /// A new <see cref="R" />.
        /// </returns>
        public ErrorResult<TNewError> Map<TNewError>(Func<TError, TNewError> errorFunc)
        {
            return new ErrorResult<TNewError>(errorFunc(this.Error));
        }

        /// <summary>
        /// Creates a result based on the specified result .
        /// </summary>
        /// <typeparam name="TNewError">The type of the new error.</typeparam>
        /// <param name="errorFunc">The error function.</param>
        /// <returns>
        /// A new <see cref="R" />.
        /// </returns>
        public ValueTask<ErrorResult<TNewError>> MapAsync<TNewError>(Func<TError, TNewError> errorFunc)
        {
            return new ErrorResult<TNewError>(errorFunc(this.Error)).ToValueTask();
        }

        /// <summary>
        /// Converts this <see cref="ErrorResult{TError}"/> to a erroneous <see cref="R{TValue, TError}"/>.
        /// </summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <returns>An erroneous result.</returns>
        public R<TValue, TError> Omits<TValue>()
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
            return $"Error: {this.Error}";
        }
    }
}