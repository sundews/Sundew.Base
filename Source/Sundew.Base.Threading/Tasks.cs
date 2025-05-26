// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Tasks.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading;

using System.Threading.Tasks;

internal static class Tasks
{
    public static async Task<(TValue1 Item1, TValue2 Item2)> WhenAll<TValue1, TValue2>(Task<TValue1> task1, Task<TValue2> task2)
    {
        await Task.WhenAll(task1, task2).ConfigureAwait(false);
        return (Item1: await task1.ConfigureAwait(false), Item2: await task2.ConfigureAwait(false));
    }

    public static async Task<(TValue1 Item1, TValue2 Item2, TValue3 Item3)> WhenAll<TValue1, TValue2, TValue3>(Task<TValue1> task1, Task<TValue2> task2, Task<TValue3> task3)
    {
        await Task.WhenAll(task1, task2, task3).ConfigureAwait(false);
        return (Item1: await task1.ConfigureAwait(false), Item2: await task2.ConfigureAwait(false), Item3: await task3.ConfigureAwait(false));
    }

    public static async Task<(TValue1 Item1, TValue2 Item2, TValue3 Item3, TValue4 Item4)> WhenAll<TValue1, TValue2, TValue3, TValue4>(Task<TValue1> task1, Task<TValue2> task2, Task<TValue3> task3, Task<TValue4> task4)
    {
        await Task.WhenAll(task1, task2, task3, task4).ConfigureAwait(false);
        return (Item1: await task1.ConfigureAwait(false), Item2: await task2.ConfigureAwait(false), Item3: await task3.ConfigureAwait(false), await task4.ConfigureAwait(false));
    }
}
