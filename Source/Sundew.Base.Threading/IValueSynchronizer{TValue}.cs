// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValueSynchronizer{TValue}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading;

using System;
using System.Threading;
using System.Threading.Tasks;
using Sundew.Base.Notifications;

/// <summary>
/// Represents an object that provides asynchronous access to a synchronized value and notifies subscribers when the
/// value changes.
/// </summary>
/// <remarks>Implementations of this interface allow consumers to observe value changes and retrieve the current
/// value asynchronously. This is useful in scenarios where value updates may occur over time or from external sources,
/// and consumers need to react to or await the latest value.</remarks>
/// <typeparam name="TValue">The type of the value object to synchronize. Must be a reference type.</typeparam>
public interface IValueSynchronizer<TValue> : INotify<TValue>, IDisposable
    where TValue : class
{
    /// <summary>
    /// Gets the asynchronous operation that retrieves the current value.
    /// </summary>
    Task<TValue> Value { get; }
}

/// <summary>
/// Represents an object that provides asynchronous access to a synchronized value based on a parameter and notifies.
/// </summary>
/// <typeparam name="TParameter">The type of parameter.</typeparam>
/// <typeparam name="TValue">The type of the value.</typeparam>
public interface IValueSynchronizer<TParameter, TValue> : IValueSynchronizer<TValue>
    where TValue : class
{
    /// <summary>
    /// Invalidates the value for the specified parameter.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    /// <param name="cancellation">The cancellation.</param>
    /// <returns>A task representing the invalidation operation.</returns>
    Task RefreshAsync(TParameter parameter, Cancellation cancellation);

    /// <summary>
    /// Attempts to apply the specified asynchronous apply function.
    /// </summary>
    /// <param name="submissionId">The requester.</param>
    /// <param name="applyFunc">A delegate that returns a task representing the apply action.</param>
    /// <param name="cancellation">The cancellation.</param>
    /// <returns>A task that represents the asynchronous operation. The task completes when the attempt to run the apply action has finished.</returns>
    Task TrySubmitAsync(object submissionId, Func<CancellationToken, Task<PostSubmitAction<TParameter, TValue>>> applyFunc, Cancellation cancellation);
}