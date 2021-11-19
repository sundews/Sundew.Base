// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManualResetEventAsync.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading;

/// <summary>
/// An asynchronous manual reset event.
/// </summary>
/// <seealso cref="Sundew.Base.Threading.ResetEventAsync" />
public sealed class ManualResetEventAsync : ResetEventAsync
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ManualResetEventAsync"/> class.
    /// </summary>
    public ManualResetEventAsync()
        : this(false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ManualResetEventAsync"/> class.
    /// </summary>
    /// <param name="isSet">if set to <c>true</c> [initial state].</param>
    public ManualResetEventAsync(bool isSet)
        : base(isSet)
    {
    }

    /// <summary>
    /// Called during a Wait or WaitAsync call when the event is set and the lock is acquired.
    /// </summary>
    /// <param name="isSet">if set to <c>true</c> [is set].</param>
    protected override void OnIsSetDuringWaitWhileLocked(ref bool isSet)
    {
    }
}