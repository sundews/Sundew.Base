// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SynchronizationContextExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Extends the <see cref="SynchronizationContext"/> with easy to use methods.
/// </summary>
public static class SynchronizationContextExtensions
{
    /// <summary>
    /// Sends the specified func async.
    /// </summary>
    /// <param name="synchronizationContext">The synchronization context.</param>
    /// <param name="action">The action.</param>
    /// <returns>The result task.</returns>
    public static Task SendAsync(this SynchronizationContext synchronizationContext, Func<Task> action)
    {
        var sendContext = new AsyncVoidContext(action);
        synchronizationContext.Send(
            o =>
            {
                var context = (AsyncVoidContext)o!;
                try
                {
                    context.SendOrPostCallback().GetAwaiter().GetResult();
                    context.TaskCompletionSource.SetResult();
                }
                catch (Exception e)
                {
                    context.TaskCompletionSource.SetException(e);
                }
            },
            sendContext);

        return sendContext.TaskCompletionSource.Task;
    }

    /// <summary>
    /// Sends the specified func async.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="synchronizationContext">The synchronization context.</param>
    /// <param name="func">The action.</param>
    /// <returns>The result task.</returns>
    public static Task<TResult> SendAsync<TResult>(this SynchronizationContext synchronizationContext, Func<Task<TResult>> func)
    {
        var sendContext = new AsyncResultContext<TResult>(func);
        synchronizationContext.Send(
            o =>
            {
                var context = (AsyncResultContext<TResult>)o!;
                try
                {
                    context.TaskCompletionSource.SetResult(context.SendOrPostCallback().GetAwaiter().GetResult());
                }
                catch (Exception e)
                {
                    context.TaskCompletionSource.SetException(e);
                }
            },
            sendContext);

        return sendContext.TaskCompletionSource.Task;
    }

    /// <summary>
    /// Sends the specified func async.
    /// </summary>
    /// <param name="synchronizationContext">The synchronization context.</param>
    /// <param name="action">The action.</param>
    /// <returns>The result task.</returns>
    public static Task SendAsync(this SynchronizationContext synchronizationContext, Action action)
    {
        var sendContext = new VoidContext(action);
        synchronizationContext.Send(
            o =>
            {
                var context = (VoidContext)o!;
                try
                {
                    context.SendOrPostCallback();
                    context.TaskCompletionSource.SetResult();
                }
                catch (Exception e)
                {
                    context.TaskCompletionSource.SetException(e);
                }
            },
            sendContext);

        return sendContext.TaskCompletionSource.Task;
    }

    /// <summary>
    /// Sends the specified func async.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="synchronizationContext">The synchronization context.</param>
    /// <param name="func">The action.</param>
    /// <returns>The result task.</returns>
    public static Task<TResult> SendAsync<TResult>(this SynchronizationContext synchronizationContext, Func<TResult> func)
    {
        var sendContext = new ResultContext<TResult>(func);
        synchronizationContext.Send(
            o =>
            {
                var context = (ResultContext<TResult>)o!;
                try
                {
                    context.TaskCompletionSource.SetResult(context.SendOrPostCallback());
                }
                catch (Exception e)
                {
                    context.TaskCompletionSource.SetException(e);
                }
            },
            sendContext);

        return sendContext.TaskCompletionSource.Task;
    }

    /// <summary>
    /// Sends the specified func async.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <param name="synchronizationContext">The synchronization context.</param>
    /// <param name="action">The action.</param>
    /// <param name="state">The state.</param>
    /// <returns>The result task.</returns>
    public static Task SendAsync<TState>(this SynchronizationContext synchronizationContext, Func<TState, Task> action, TState state)
    {
        var sendContext = new AsyncVoidContext<TState>(action, state);
        synchronizationContext.Send(
            o =>
            {
                var context = (AsyncVoidContext<TState>)o!;
                try
                {
                    context.SendOrPostCallback(context.State).GetAwaiter().GetResult();
                    context.TaskCompletionSource.SetResult();
                }
                catch (Exception e)
                {
                    context.TaskCompletionSource.SetException(e);
                }
            },
            sendContext);

        return sendContext.TaskCompletionSource.Task;
    }

    /// <summary>
    /// Sends the specified func async.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="synchronizationContext">The synchronization context.</param>
    /// <param name="func">The action.</param>
    /// <param name="state">The state.</param>
    /// <returns>The result task.</returns>
    public static Task<TResult> SendAsync<TState, TResult>(this SynchronizationContext synchronizationContext, Func<TState, Task<TResult>> func, TState state)
    {
        var sendContext = new AsyncResultContext<TState, TResult>(func, state);
        synchronizationContext.Send(
            o =>
            {
                var context = (AsyncResultContext<TState, TResult>)o!;
                try
                {
                    context.TaskCompletionSource.SetResult(context.SendOrPostCallback(context.State).GetAwaiter().GetResult());
                }
                catch (Exception e)
                {
                    context.TaskCompletionSource.SetException(e);
                }
            },
            sendContext);

        return sendContext.TaskCompletionSource.Task;
    }

    /// <summary>
    /// Sends the specified func async.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <param name="synchronizationContext">The synchronization context.</param>
    /// <param name="action">The action.</param>
    /// <param name="state">The state.</param>
    /// <returns>The result task.</returns>
    public static Task SendAsync<TState>(this SynchronizationContext synchronizationContext, Action<TState> action, TState state)
    {
        var sendContext = new VoidContext<TState>(action, state);
        synchronizationContext.Send(
            o =>
            {
                var context = (VoidContext<TState>)o!;
                try
                {
                    context.SendOrPostCallback(context.State);
                    context.TaskCompletionSource.SetResult();
                }
                catch (Exception e)
                {
                    context.TaskCompletionSource.SetException(e);
                }
            },
            sendContext);

        return sendContext.TaskCompletionSource.Task;
    }

    /// <summary>
    /// Sends the specified func async.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="synchronizationContext">The synchronization context.</param>
    /// <param name="func">The action.</param>
    /// <param name="state">The state.</param>
    /// <returns>The result task.</returns>
    public static Task<TResult> SendAsync<TState, TResult>(this SynchronizationContext synchronizationContext, Func<TState, TResult> func, TState state)
    {
        var sendContext = new ResultContext<TState, TResult>(func, state);
        synchronizationContext.Send(
            o =>
            {
                var context = (ResultContext<TState, TResult>)o!;
                try
                {
                    context.TaskCompletionSource.SetResult(context.SendOrPostCallback(context.State));
                }
                catch (Exception e)
                {
                    context.TaskCompletionSource.SetException(e);
                }
            },
            sendContext);

        return sendContext.TaskCompletionSource.Task;
    }

#if !NET6_0_OR_GREATER
    private readonly struct EmptyStruct
    {
    }

    private sealed class TaskCompletionSource : TaskCompletionSource<EmptyStruct>
    {
        public void SetResult()
        {
            this.SetResult(default);
        }
    }
#endif

    private sealed class VoidContext
    {
        public VoidContext(Action sendOrPostCallback)
        {
            this.TaskCompletionSource = new TaskCompletionSource();
            this.SendOrPostCallback = sendOrPostCallback;
        }

        public TaskCompletionSource TaskCompletionSource { get; }

        public Action SendOrPostCallback { get; }
    }

    private sealed class ResultContext<TResult>
    {
        public ResultContext(Func<TResult> sendOrPostCallback)
        {
            this.TaskCompletionSource = new TaskCompletionSource<TResult>();
            this.SendOrPostCallback = sendOrPostCallback;
        }

        public TaskCompletionSource<TResult> TaskCompletionSource { get; }

        public Func<TResult> SendOrPostCallback { get; }
    }

    private sealed class AsyncVoidContext
    {
        public AsyncVoidContext(Func<Task> sendOrPostCallback)
        {
            this.TaskCompletionSource = new TaskCompletionSource();
            this.SendOrPostCallback = sendOrPostCallback;
        }

        public TaskCompletionSource TaskCompletionSource { get; }

        public Func<Task> SendOrPostCallback { get; }
    }

    private sealed class AsyncResultContext<TResult>
    {
        public AsyncResultContext(Func<Task<TResult>> sendOrPostCallback)
        {
            this.TaskCompletionSource = new TaskCompletionSource<TResult>();
            this.SendOrPostCallback = sendOrPostCallback;
        }

        public TaskCompletionSource<TResult> TaskCompletionSource { get; }

        public Func<Task<TResult>> SendOrPostCallback { get; }
    }

    private sealed class VoidContext<TState>
    {
        public VoidContext(Action<TState> sendOrPostCallback, TState state)
        {
            this.TaskCompletionSource = new TaskCompletionSource();
            this.SendOrPostCallback = sendOrPostCallback;
            this.State = state;
        }

        public TaskCompletionSource TaskCompletionSource { get; }

        public Action<TState> SendOrPostCallback { get; }

        public TState State { get; }
    }

    private sealed class ResultContext<TState, TResult>
    {
        public ResultContext(Func<TState, TResult> sendOrPostCallback, TState state)
        {
            this.TaskCompletionSource = new TaskCompletionSource<TResult>();
            this.SendOrPostCallback = sendOrPostCallback;
            this.State = state;
        }

        public TaskCompletionSource<TResult> TaskCompletionSource { get; }

        public Func<TState, TResult> SendOrPostCallback { get; }

        public TState State { get; }
    }

    private sealed class AsyncVoidContext<TState>
    {
        public AsyncVoidContext(Func<TState, Task> sendOrPostCallback, TState state)
        {
            this.TaskCompletionSource = new TaskCompletionSource();
            this.SendOrPostCallback = sendOrPostCallback;
            this.State = state;
        }

        public TaskCompletionSource TaskCompletionSource { get; }

        public Func<TState, Task> SendOrPostCallback { get; }

        public TState State { get; }
    }

    private sealed class AsyncResultContext<TState, TResult>
    {
        public AsyncResultContext(Func<TState, Task<TResult>> sendOrPostCallback, TState state)
        {
            this.TaskCompletionSource = new TaskCompletionSource<TResult>();
            this.SendOrPostCallback = sendOrPostCallback;
            this.State = state;
        }

        public TaskCompletionSource<TResult> TaskCompletionSource { get; }

        public Func<TState, Task<TResult>> SendOrPostCallback { get; }

        public TState State { get; }
    }
}