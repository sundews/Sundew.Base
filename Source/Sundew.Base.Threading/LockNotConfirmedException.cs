// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LockNotConfirmedException.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading
{
    using System;

    /// <summary>
    /// Throw when the result of a <see cref="AsyncLock"/> has not been confirmed.
    /// </summary>
    /// <seealso cref="System.Exception" />
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "RCS1194:Implement exception constructors.", Justification = "By design")]
    public sealed class LockNotConfirmedException : Exception
    {
        private const string LockHasNotBeenConfirmedDidYouForgetToCallCheckText = "The lock has not been confirmed, did you forget to call Check()?";

        /// <summary>
        /// Initializes a new instance of the <see cref="LockNotConfirmedException"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "No plans for translations.")]
        internal LockNotConfirmedException()
            : base(LockHasNotBeenConfirmedDidYouForgetToCallCheckText)
        {
        }
    }
}