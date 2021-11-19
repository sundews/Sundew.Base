// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoResetEventAsync.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading;

/// <summary>
/// An asynchronous auto reset event.
/// </summary>
public sealed class AutoResetEventAsync : ResetEventAsync
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutoResetEventAsync"/> class.
    /// </summary>
    public AutoResetEventAsync()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoResetEventAsync"/> class.
    /// </summary>
    /// <param name="isSet">if set to <c>true</c> [initial state].</param>
    public AutoResetEventAsync(bool isSet)
        : base(isSet)
    {
    }

    /// <summary>
    /// Called during a Wait or WaitAsync call when the event is set and the lock is acquired.
    /// </summary>
    /// <param name="isSet">if set to <c>true</c> [is set].</param>
    protected override void OnIsSetDuringWaitWhileLocked(ref bool isSet)
    {
        isSet = false;
    }
}