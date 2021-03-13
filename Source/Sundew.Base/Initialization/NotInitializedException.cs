// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotInitializedException.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Initialization
{
    using System;

    /// <summary>Exception used to indicate that an initializable was not initialized.</summary>
    /// <seealso cref="System.Exception" />
    public class NotInitializedException : Exception
    {
        /// <summary>Initializes a new instance of the <see cref="NotInitializedException"/> class.</summary>
        /// <param name="initializable">The initializable.</param>
        public NotInitializedException(IInitializable initializable)
            : base(GetMessage(initializable))
        {
        }

        /// <summary>Initializes a new instance of the <see cref="NotInitializedException"/> class.</summary>
        /// <param name="initializable">The initializable.</param>
        /// <param name="innerException">The inner exception.</param>
        public NotInitializedException(IInitializable initializable, Exception innerException)
            : base(GetMessage(initializable), innerException)
        {
        }

        private static string GetMessage(IInitializable initializable)
        {
            return $"The initializable: {initializable} was not initialized.";
        }
    }
}