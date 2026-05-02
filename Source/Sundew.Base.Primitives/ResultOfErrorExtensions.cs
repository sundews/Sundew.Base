// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultOfErrorExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base;

using System;

/// <summary>
/// Extensions for <see cref="R{TSuccess}"/> and <see cref="R{TSuccess,TError}"/>.
/// </summary>
public static class ResultOfErrorExtensions
{
    /// <summary>
    /// Extensions for <see cref="R{TSuccess,TError}"/>.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    extension<TSuccess, TError>(RoE<TError> result)
    {
        /// <summary>
        /// Gets a new result that is successful, if both results were a success.
        /// </summary>
        /// <param name="other">The other result.</param>
        /// <returns>A new result.</returns>
        public R<TSuccess, TError> And(Func<R<TSuccess, TError>> other)
        {
#pragma warning disable SA1101
            if (result.IsSuccess)
            {
                return other();
            }

            return result.Map(default(TSuccess)!);
        }
    }
}