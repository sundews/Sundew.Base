// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NaturalText.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.PerformanceBenchmark.Text;

using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Sundew.Base.Development.Baselines.Text;
using Sundew.Base.Text;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net48, baseline: true)]
[SimpleJob(RuntimeMoniker.Net70)]
public class NaturalText
{
    private readonly Win32NaturalTextComparer win32NaturalTextComparer;
    private readonly NaturalTextComparer naturalTextComparer;

    public NaturalText()
    {
        this.win32NaturalTextComparer = new Win32NaturalTextComparer();
        this.naturalTextComparer = new NaturalTextComparer(StringComparison.CurrentCulture);
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public int Win32NaturalTextCompare(string lhs, string rhs)
    {
        return this.win32NaturalTextComparer.Compare(lhs, rhs);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int NaturalTextCompare(string lhs, string rhs)
    {
        return this.naturalTextComparer.Compare(lhs, rhs);
    }

    public IEnumerable<object[]> Data()
    {
        yield return new object[]
        {
            "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit10 amet.",
            "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit2 amet.",
        };
        yield return new object[]
        {
            "D@EL: Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit10 amet.",
            "D@EL: Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit2 amet.",
        };
        yield return new object[]
        {
            "WND@EL: Lorem 23ipsum dolor sit amet, consetetur sadipscing543 elitr, sed diam nonumy eirmod 123tempor invidunt ut labore et dolore magna aliquyam 23.23erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit10 amet.",
            "WND@EL: Lorem 23ipsum dolor sit amet, consetetur sadipscing543 elitr, sed diam nonumy eirmod 123tempor invidunt ut labore et dolore magna aliquyam 23.23erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit2 amet.",
        };
        yield return new object[]
        {
            "WNEL: Lorem 23ipsum dolor sit amet, consetetur sadipscing543 elitr, sed diam nonumy eirmod 123tempor invidunt ut labore et dolore magna aliquyam 23.23erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit10 amet.",
            "WNEL: Lorem 23ipsum dolor sit amet, consetetur sadipscing543 elitr, sed diam nonumy eirmod 123tempor invidunt ut labore et dolore magna aliquyam 23.23erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit10 amet.",
        };
    }
}
