// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Attempter.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Sundew.Base.Primitives.Computation;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

/// <summary>
/// Implements retry logic.
/// </summary>
public sealed class Attempter : IAttempt
{
    private readonly TimeSpan interval;
    private readonly Random? backoffRandomizer;

    /// <summary>
    /// Initializes a new instance of the <see cref="Attempter"/> class.
    /// </summary>
    /// <param name="maxAttempts">The maximum attempts.</param>
    /// <param name="interval">The interval.</param>
    /// <param name="useBackOff">If <c> true</c>, some time will be added between each attempt.</param>
    public Attempter(int maxAttempts, TimeSpan interval = default, bool useBackOff = false)
    {
        this.interval = interval;
        this.backoffRandomizer = useBackOff ? new Random(84161) : null;
        this.MaxAttempts = maxAttempts;
    }

    /// <summary>
    /// Gets the maximum attempts.
    /// </summary>
    /// <value>
    /// The maximum attempts.
    /// </value>
    public int MaxAttempts { get; private set; }

    /// <summary>
    /// Gets the current attempt.
    /// </summary>
    /// <value>
    /// The current attempt.
    /// </value>
    public int CurrentAttempt { get; private set; }

    /// <summary>
    /// Attempts to execute the specified action are retries the specified amount of time.
    /// </summary>
    /// <param name="action">The attempt action.</param>
    /// <param name="exceptionFilter">The exceptions to handle.</param>
    /// <param name="onFailed">The on failed action.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result.</returns>
    public async Task AttemptAsync(
        Func<IAttempt, CancellationToken, Task> action,
        ExceptionFilter exceptionFilter,
        Action<FailedAttempt>? onFailed = null,
        CancellationToken cancellationToken = default)
    {
        List<Exception> errorList = new();
        while (this.Attempt())
        {
            try
            {
                if (this.CurrentAttempt > 1)
                {
                    if (this.interval != TimeSpan.Zero)
                    {
                        await Task.Delay(this.GetInterval(), cancellationToken).ConfigureAwait(false);
                    }
                }

                await action(this, cancellationToken).ConfigureAwait(false);
                return;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                errorList.Add(e);
                if (exceptionFilter.IsInFilter(e))
                {
                    onFailed?.Invoke(new FailedAttempt(this.CurrentAttempt, this.MaxAttempts, e));
                    continue;
                }

                throw this.CreateAggregateException(errorList);
            }
        }

        throw this.CreateAggregateException(errorList);
    }

    /// <summary>
    /// Attempts to execute the specified action are retries the specified amount of time.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="exceptionFilter">The exceptions to handle.</param>
    /// <param name="onFailed">The on failed action.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public void Attempt(
        Action<IAttempt> action,
        ExceptionFilter exceptionFilter,
        Action<FailedAttempt>? onFailed = null,
        CancellationToken cancellationToken = default)
    {
        var errorList = new List<Exception>();
        while (this.Attempt())
        {
            try
            {
                this.WaitForIntervalIfNeeded(cancellationToken);
                action(this);
                return;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                errorList.Add(e);
                if (exceptionFilter.IsInFilter(e))
                {
                    onFailed?.Invoke(new FailedAttempt(this.CurrentAttempt, this.MaxAttempts, e));
                    continue;
                }

                throw this.CreateAggregateException(errorList);
            }
        }

        throw this.CreateAggregateException(errorList);
    }

    /// <summary>
    /// Attempts to execute the specified action are retries the specified amount of time.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="func">The attempt func.</param>
    /// <param name="exceptionFilter">The exceptions to handle.</param>
    /// <param name="onFailed">The on failed action.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result.</returns>
    public Task<R<TResult, FailedAttempts<object>>> AttemptAsync<TResult>(
        Func<IAttempt, CancellationToken, Task<TResult>> func,
        ExceptionFilter exceptionFilter,
        Action<FailedAttempt<object>>? onFailed = null,
        CancellationToken cancellationToken = default)
        where TResult : notnull
    {
        return this.AttemptAsync<TResult, object>(
            async (attempt, token) => R.Success(await func(attempt, token)),
            exceptionFilter,
            onFailed,
            cancellationToken);
    }

    /// <summary>
    /// Attempts to execute the specified action are retries the specified amount of time.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="func">The attempt func.</param>
    /// <param name="exceptionFilter">The exceptions to handle.</param>
    /// <param name="onFailed">The on failed action.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result.</returns>
    public R<TResult, FailedAttempts<object>> Attempt<TResult>(
        Func<IAttempt, TResult> func,
        ExceptionFilter exceptionFilter,
        Action<FailedAttempt<object>>? onFailed = null,
        CancellationToken cancellationToken = default)
        where TResult : notnull
    {
        return this.Attempt<TResult, object>(
            attempt => R.From<TResult, object>(true, func(this), default!),
            exceptionFilter,
            onFailed,
            cancellationToken);
    }

    /// <summary>
    /// Attempts to execute the specified action are retries the specified amount of time.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="func">The attempt func.</param>
    /// <param name="exceptionFilter">The exceptions to handle.</param>
    /// <param name="onFailed">The on failed action.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result.</returns>
    public async Task<R<TResult, FailedAttempts<TError>>> AttemptAsync<TResult, TError>(
        Func<IAttempt, CancellationToken, Task<R<TResult, TError>>> func,
        ExceptionFilter exceptionFilter,
        Action<FailedAttempt<TError>>? onFailed = null,
        CancellationToken cancellationToken = default)
        where TResult : notnull
        where TError : notnull
    {
        List<FailedAttempt<TError>> errorList = new();
        while (this.Attempt())
        {
            try
            {
                if (this.CurrentAttempt > 1)
                {
                    if (this.interval != TimeSpan.Zero)
                    {
                        await Task.Delay(this.GetInterval(), cancellationToken).ConfigureAwait(false);
                    }
                }

                var result = await func(this, cancellationToken).ConfigureAwait(false);
                if (result.IsSuccess)
                {
                    return R.Success(result.Value);
                }

                var failedAttempt = new FailedAttempt<TError>(this.CurrentAttempt, this.MaxAttempts, result.Error, null);
                errorList.Add(failedAttempt);
                onFailed?.Invoke(failedAttempt);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                var failedAttempt = new FailedAttempt<TError>(this.CurrentAttempt, this.MaxAttempts, default, e);
                errorList.Add(failedAttempt);
                if (exceptionFilter.IsInFilter(e))
                {
                    onFailed?.Invoke(failedAttempt);
                    continue;
                }

                return R.Error(new FailedAttempts<TError>(errorList));
            }
        }

        return R.Error(new FailedAttempts<TError>(errorList));
    }

    /// <summary>
    /// Attempts to execute the specified action are retries the specified amount of time.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="func">The attempt func.</param>
    /// <param name="exceptionFilter">The exceptions to handle.</param>
    /// <param name="onFailed">The on failed action.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result.</returns>
    public R<TResult, FailedAttempts<TError>> Attempt<TResult, TError>(
        Func<IAttempt, R<TResult, TError>> func,
        ExceptionFilter exceptionFilter,
        Action<FailedAttempt<TError>>? onFailed = null,
        CancellationToken cancellationToken = default)
        where TResult : notnull
        where TError : notnull
    {
        List<FailedAttempt<TError>> errorList = new();
        while (this.Attempt())
        {
            try
            {
                this.WaitForIntervalIfNeeded(cancellationToken);

                var result = func(this);
                if (result.IsSuccess)
                {
                    return R.Success(result.Value);
                }

                var failedAttempt = new FailedAttempt<TError>(this.CurrentAttempt, this.MaxAttempts, result.Error, null);
                errorList.Add(failedAttempt);
                onFailed?.Invoke(failedAttempt);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                var failedAttempt = new FailedAttempt<TError>(this.CurrentAttempt, this.MaxAttempts, default, e);
                errorList.Add(failedAttempt);
                if (exceptionFilter.IsInFilter(e))
                {
                    onFailed?.Invoke(failedAttempt);
                    continue;
                }

                return R.Error(new FailedAttempts<TError>(errorList));
            }
        }

        return R.Error(new FailedAttempts<TError>(errorList));
    }

    /// <summary>
    /// Resets this instance.
    /// </summary>
    public void Reset()
    {
        this.CurrentAttempt = 0;
    }

    /// <summary>
    /// Resets the specified maximum attempts.
    /// </summary>
    /// <param name="maxAttempts">The maximum attempts.</param>
    public void Reset(int maxAttempts)
    {
        this.MaxAttempts = maxAttempts;
        this.Reset();
    }

    /// <summary>
    /// Indicate another attempt.
    /// </summary>
    /// <returns><c>true</c>, if an operation should be attempted, otherwise <c>false</c>.</returns>
    public bool Attempt()
    {
        this.CurrentAttempt++;
        return this.CurrentAttempt <= this.MaxAttempts;
    }

    private void WaitForIntervalIfNeeded(CancellationToken cancellationToken)
    {
        if (this.CurrentAttempt > 1 && this.interval != TimeSpan.Zero)
        {
            Task.Delay(this.GetInterval(), cancellationToken).Wait(cancellationToken);
        }
    }

    private TimeSpan GetInterval()
    {
        if (this.backoffRandomizer != null && this.CurrentAttempt != 0)
        {
            double max = this.MaxAttempts;
            var percentage = (int)(100 * (max / this.CurrentAttempt));
            return this.interval + TimeSpan.FromTicks(this.backoffRandomizer.Next(0, percentage));
        }

        return this.interval;
    }

    private AggregateException CreateAggregateException(List<Exception> exceptions)
    {
        return new AggregateException(exceptions);
    }
}