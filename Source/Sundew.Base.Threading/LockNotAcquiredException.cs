// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LockNotAcquiredException.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading;

using System;

/// <summary>
/// Thrown if the lock could not be acquired.
/// </summary>
/// <seealso cref="System.Exception" />
public class LockNotAcquiredException : Exception
{
    private const string LockCouldNotBeAcquiredText = "The lock could not be acquired.";

    /// <summary>
    /// Initializes a new instance of the <see cref="LockNotAcquiredException"/> class.
    /// </summary>
    public LockNotAcquiredException()
        : base(LockCouldNotBeAcquiredText)
    {
    }
}