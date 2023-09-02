// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializationHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Initialization;

using System.Collections.Generic;
using System.Linq;

internal static class InitializationHelper
{
    public static IReadOnlyCollection<TItem> ToReadOnly<TItem>(this IEnumerable<TItem> initializers)
    {
        if (initializers is IReadOnlyCollection<TItem> collection)
        {
            return collection;
        }

        return initializers.ToArray();
    }
}