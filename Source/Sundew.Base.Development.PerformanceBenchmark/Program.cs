// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.PerformanceBenchmark;

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

public class Program
{
    public static void Main()
    {
        BenchmarkRunner.Run(typeof(Program).Assembly, ManualConfig.Create(DefaultConfig.Instance));
    }
}
