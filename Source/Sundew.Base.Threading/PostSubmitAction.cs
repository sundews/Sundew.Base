// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostSubmitAction.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading;

using Sundew.DiscriminatedUnions;

/// <summary>
/// Represents an action that can be performed after a value as been submitted.
/// </summary>
/// <typeparam name="TValue">The type of value.</typeparam>
/// <typeparam name="TParameter">The type of parameter.</typeparam>
[DiscriminatedUnion]
public abstract partial record PostSubmitAction<TParameter, TValue>
{
    /// <summary>
    /// Represents a post apply action that performs no operation.
    /// </summary>
    public sealed record None : PostSubmitAction<TParameter, TValue>;

    /// <summary>
    /// Represents an action that sets a value during post-processing.
    /// </summary>
    /// <param name="Value">The value to be set by this action.</param>
    public sealed record SetValue(TValue Value) : PostSubmitAction<TParameter, TValue>;

    /// <summary>
    /// Represents a post apply action that triggers a refresh of the value, carrying associated information.
    /// </summary>
    public sealed record Refresh(TParameter Parameter) : PostSubmitAction<TParameter, TValue>;

    /// <summary>
    /// Represents a post-apply action that refreshes the target when it becomes idle.
    /// </summary>
    /// <remarks>Use this action to ensure that the target is refreshed or re-evaluated after it has been
    /// idle, which can be useful for scenarios where changes should only take effect once activity has ceased. This
    /// type is typically used in data binding or update scenarios to optimize performance by deferring invalidation
    /// until the system is not busy.</remarks>
    public sealed record RefreshOnIdle(TParameter Parameter) : PostSubmitAction<TParameter, TValue>;
}