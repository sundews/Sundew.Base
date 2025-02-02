// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Quest{TGuide}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Represents a quest for processing some type of operation.
/// </summary>
/// <typeparam name="TGuide">The Guide type.</typeparam>
public sealed class Quest<TGuide>
{
    private readonly Func<CancellationToken, Task> startFunc;
    private readonly IDisposable? disposable;
    private readonly CancellationToken cancellationToken;
#if NETSTANDARD2_1 || NETSTANDARD2_0
    private readonly TaskCompletionSource<__> taskCompletionSource = new();
#else
    private readonly TaskCompletionSource taskCompletionSource = new();
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="Quest{TGuide}"/> class.
    /// </summary>
    /// <param name="guide">The guide.</param>
    /// <param name="startFunc">The start func.</param>
    /// <param name="disposable">The disposable.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    internal Quest(TGuide guide, Func<CancellationToken, Task> startFunc, IDisposable? disposable, CancellationToken cancellationToken)
    {
        this.startFunc = startFunc;
        this.disposable = disposable;
        this.cancellationToken = cancellationToken;
#if NETSTANDARD2_1 || NETSTANDARD2_0
        this.cancellationToken.Register(this.taskCompletionSource.SetCanceled);
#else
        this.cancellationToken.Register(() => this.taskCompletionSource.SetCanceled(cancellationToken));
#endif
        this.Guide = guide;
    }

    /// <summary>
    /// Gets the Guide.
    /// </summary>
    public TGuide Guide { get; }

    /// <summary>
    /// Gets the Goal task.
    /// </summary>
    public Task Goal => this.taskCompletionSource.Task;

    /// <summary>
    /// Starts the quest.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public async Task Start()
    {
        try
        {
            await this.startFunc(this.cancellationToken).ConfigureAwait(false);
#if NETSTANDARD2_1 || NETSTANDARD2_0
            this.taskCompletionSource.SetResult(__._);
#else
            this.taskCompletionSource.SetResult();
#endif
        }
        catch (Exception e)
        {
            this.taskCompletionSource.SetException(e);
        }
    }

    /// <summary>
    /// Gets the <see cref="IDisposable"/> if this quest is disposable.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns><c>true</c>, if this quest is disposable, otherwise <c>false</c>.</returns>
    public bool TryGetTarget([NotNullWhen(true)] out IDisposable? target)
    {
        target = this.disposable;
        return target != default;
    }
}