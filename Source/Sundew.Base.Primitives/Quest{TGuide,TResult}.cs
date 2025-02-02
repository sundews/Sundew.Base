// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Quest{TGuide,TResult}.cs" company="Sundews">
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
/// <typeparam name="TResult">The Result type.</typeparam>
public sealed class Quest<TGuide, TResult> : IMayBe<IDisposable>
{
    private readonly Func<CancellationToken, Task<TResult>> startFunc;
    private readonly IDisposable? disposable;
    private readonly CancellationToken cancellationToken;
    private readonly TaskCompletionSource<TResult> taskCompletionSource = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Quest{TGuide, TResult}"/> class.
    /// </summary>
    /// <param name="guide">The guide.</param>
    /// <param name="startFunc">The start func.</param>
    /// <param name="disposable">The disposable.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    internal Quest(TGuide guide, Func<CancellationToken, Task<TResult>> startFunc, IDisposable? disposable, CancellationToken cancellationToken)
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
    public Task<TResult> Goal => this.taskCompletionSource.Task;

    /// <summary>
    /// Starts the quest.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public async Task<TResult> Start()
    {
        try
        {
            var result = await this.startFunc(this.cancellationToken).ConfigureAwait(false);
            this.taskCompletionSource.SetResult(result);
            return result;
        }
        catch (Exception e)
        {
            this.taskCompletionSource.SetException(e);
        }

        return await this.taskCompletionSource.Task;
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