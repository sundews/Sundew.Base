// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposableHelper.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Disposal.Internal;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

internal static class DisposableHelper
{
    public static void Dispose(object disposable, IDisposableReporter? disposableReporter)
    {
        if (disposableReporter != null)
        {
            DisposeAndReport(disposable, disposableReporter);
        }
        else
        {
#if NETSTANDARD2_1
            DisposeItemPreferDispose(disposable).AsTask().Wait();
#else
            DisposeItemPreferDispose(disposable);
#endif
        }
    }

    public static void Dispose(IEnumerable<object>? disposables, IDisposableReporter? disposableReporter)
    {
        if (disposables != null)
        {
            if (disposableReporter != null)
            {
                foreach (var disposable in disposables)
                {
                    DisposeAndReport(disposable, disposableReporter);
                }
            }
            else
            {
                foreach (var disposable in disposables)
                {
#if NETSTANDARD2_1
                    DisposeItemPreferDispose(disposable).AsTask().Wait();
#else
                    DisposeItemPreferDispose(disposable);
#endif
                }
            }
        }
    }

#if NETSTANDARD2_1
    public static async ValueTask DisposeAsync(object disposable, IDisposableReporter? disposableReporter)
    {
        if (disposableReporter != null)
        {
            await DisposeAsyncAndReport(disposable, disposableReporter).ConfigureAwait(false);
        }
        else
        {
            await DisposeItemPreferAsyncDispose(disposable).ConfigureAwait(false);
        }
    }

    public static async ValueTask DisposeAsync(IEnumerable<object>? disposables, IDisposableReporter? disposableReporter)
    {
        if (disposables != null)
        {
            if (disposableReporter != null)
            {
                foreach (var disposable in disposables)
                {
                    await DisposeAsyncAndReport(disposable, disposableReporter).ConfigureAwait(false);
                }
            }
            else
            {
                foreach (var item in disposables)
                {
                    await DisposeItemPreferAsyncDispose(item).ConfigureAwait(false);
                }
            }
        }
    }

    private static async Task DisposeAsyncAndReport(object disposable, IDisposableReporter disposableReporter)
    {
        if (disposable is IReportingDisposable reportingDisposable)
        {
            reportingDisposable.SetReporter(disposableReporter);
        }

        await DisposeItemPreferAsyncDispose(disposable).ConfigureAwait(false);
        disposableReporter.Disposed(disposable);
    }

    private static async Task DisposeItemPreferAsyncDispose(object item)
    {
        switch (item)
        {
            case IAsyncDisposable asyncDisposable:
                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                break;
            case IDisposable disposable:
                disposable.Dispose();
                break;
        }
    }
#endif

    private static void DisposeAndReport(object disposable, IDisposableReporter disposableReporter)
    {
        if (disposable is IReportingDisposable reportingDisposable)
        {
            reportingDisposable.SetReporter(disposableReporter);
        }

#if NETSTANDARD2_1
        DisposeItemPreferDispose(disposable).AsTask().Wait();
#else
        DisposeItemPreferDispose(disposable);
#endif
        disposableReporter.Disposed(disposable);
    }

#if NETSTANDARD2_1
    private static async ValueTask DisposeItemPreferDispose(object item)
    {
        switch (item)
        {
            case IDisposable disposable:
                disposable.Dispose();
                break;
            case IAsyncDisposable asyncDisposable:
                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                break;
        }
    }
#else

    private static void DisposeItemPreferDispose(object item)
    {
        switch (item)
        {
            case IDisposable disposable:
                disposable.Dispose();
                break;
        }
    }
#endif
}