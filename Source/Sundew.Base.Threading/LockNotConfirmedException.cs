// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LockNotConfirmedException.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading;

using System;

/// <summary>
/// Throw when the result of a <see cref="AsyncLock"/> has not been confirmed.
/// </summary>
/// <seealso cref="System.Exception" />
public sealed class LockNotConfirmedException : Exception
{
    private const string LockHasNotBeenConfirmedDidYouForgetToCallCheckText = "The lock has not been confirmed, did you forget to call Check()?";

    /// <summary>
    /// Initializes a new instance of the <see cref="LockNotConfirmedException"/> class.
    /// </summary>
    internal LockNotConfirmedException()
        : base(LockHasNotBeenConfirmedDidYouForgetToCallCheckText)
    {
    }
}