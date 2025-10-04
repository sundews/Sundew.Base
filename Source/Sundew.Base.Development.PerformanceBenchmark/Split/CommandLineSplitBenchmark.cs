// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandLineSplitBenchmark.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.PerformanceBenchmark.Split;

using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net48, baseline: true)]
[SimpleJob(RuntimeMoniker.Net50)]
public class CommandLineSplitBenchmark
{
    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public List<string> TextSplit(string value)
    {
        return Split.Text.SplitBasedCommandLineParser(value).Select(x => x.ToString()).ToList();
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public List<ReadOnlyMemory<char>> MemorySplit(string value)
    {
        return Split.Memory.SplitBasedCommandLineLexer(value).ToList();
    }

    public IEnumerable<object[]> Data()
    {
        yield return new object[] { @"-a -b ""1 """" ewr """" 23"" -c -d 32 -e 34" };
        yield return new object[] { @"-c ""git|tag -a {0}_{1} -m \""Release: {1} {0}\"""" ""git|push https://github.com {0}_{1}"" -b ""1.0.1"" Sundew.CommandLine" };
    }
}
