// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposeAsyncAction.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !NETSTANDARD1_3
namespace Sundew.Base.Disposal;

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

/// <summary>
/// An action and async action which implements <see cref="IAsyncDisposable"/>.
/// </summary>
/// <seealso cref="IAsyncDisposable" />
public sealed class DisposeAsyncAction : IAsyncDisposable
{
    private readonly Func<ValueTask> action;
    private readonly string? methodInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisposeAsyncAction"/> class.
    /// </summary>
    /// <param name="action">The action.</param>
    public DisposeAsyncAction(Func<ValueTask> action)
      : this(action, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisposeAsyncAction"/> class.
    /// </summary>
    /// <param name="action">The action.</param>
    [OverloadResolutionPriority(-1)]
    public DisposeAsyncAction(Action action)
        : this(
            () =>
            {
                action();
                return default;
            },
            action.GetMethodInfo().Name)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisposeAsyncAction"/> class.
    /// </summary>
    /// <param name="disposable">The disposable.</param>
    [OverloadResolutionPriority(-1)]
    public DisposeAsyncAction(IDisposable disposable)
        : this(
            () =>
            {
                disposable.Dispose();
                return default;
            },
            disposable.GetType().Name + nameof(IDisposable.Dispose))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisposeAsyncAction"/> class.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="methodInfo">The method info.</param>
    private DisposeAsyncAction(Func<ValueTask> action, string? methodInfo)
    {
        this.action = action;
        this.methodInfo = methodInfo;
    }

    /// <summary>
    /// Performs the dispose action asynchronously.
    /// </summary>
    /// <returns>An async task.</returns>
    public ValueTask DisposeAsync()
    {
        return this.action();
    }

    /// <summary>Converts to string.</summary>
    /// <returns>A <see cref="string"/> that represents this instance.</returns>
    public override string ToString()
    {
        return $"DisposeAsyncAction {this.action.Target}.{(string.IsNullOrEmpty(this.methodInfo) ? this.action.GetMethodInfo().Name : this.methodInfo)}";
    }
}
#endif