// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringCollectionAssertionsExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Development.Tests.Assertions;

using System.Collections.Generic;
using AwesomeAssertions.Collections;

internal static class StringCollectionAssertionsExtensions
{
    public static void AtLeastContain(
        this StringCollectionAssertions stringCollectionAssertionsExtensions,
        params IEnumerable<string> list)
    {
        foreach (var item in list)
        {
            stringCollectionAssertionsExtensions.Contain(item);
        }
    }
}
