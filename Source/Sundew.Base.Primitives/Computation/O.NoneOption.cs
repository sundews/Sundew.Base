// --------------------------------------------------------------------------------------------------------------------
// <copyright file="O.NoneOption.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives.Computation;

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

/// <summary>
/// Factory class for creating results.
/// </summary>
public partial class O
{
    /// <summary>
    /// Indicates an none option.
    /// </summary>
    public readonly struct NoneOption
    {
        private const string NoneText = "None";

        internal static NoneOption None => default;

        /// <summary>
        /// Always returns false.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>A value indicating whether the result was successful.</returns>
        public static implicit operator bool(NoneOption result)
        {
            return false;
        }

        /// <summary>Performs an implicit conversion from <see cref="ValueTask{NoneOption}"/> to <see cref="NoneOption"/>.</summary>
        /// <param name="result">The result.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ValueTask<NoneOption>(NoneOption result)
        {
            return result.ToValueTask();
        }

        /// <summary>
        /// Converts this instance to a value task.
        /// </summary>
        /// <returns>The value task.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueTask<NoneOption> ToValueTask()
        {
            return new ValueTask<NoneOption>(this);
        }

        /// <summary>
        /// Converts the none option to an <see cref="O{TValue}"/>.
        /// </summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <returns>The none option.</returns>
        public O<TValue> For<TValue>()
        {
            return this;
        }

        /// <summary>
        /// Converts the none option to an error result.
        /// </summary>
        /// <typeparam name="TError">The error type.</typeparam>
        /// <param name="error">The error.</param>
        /// <returns>The result.</returns>
        public R<TError> ToResult<TError>(TError error)
        {
            return new R<TError>(false, error);
        }

        /// <summary>
        /// Converts the none option to an error result.
        /// </summary>
        /// <typeparam name="TError">The error type.</typeparam>
        /// <param name="errorFunc">The error func.</param>
        /// <returns>The result.</returns>
        public R<TError> ToResult<TError>(Func<TError> errorFunc)
        {
            return new R<TError>(false, errorFunc());
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return NoneText;
        }
    }
}