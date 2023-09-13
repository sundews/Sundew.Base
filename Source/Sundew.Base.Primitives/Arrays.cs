// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Arrays.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives;

using System;
using System.Runtime.CompilerServices;

/// <summary>
/// Array helper methods.
/// </summary>
public static class Arrays
{
    /// <summary>
    /// Gets an empty array.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <returns>An empty array.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TItem[] Empty<TItem>()
    {
#if NETSTANDARD1_3_OR_GREATER
        return Array.Empty<TItem>();
#else
            return new TItem[0];
#endif
    }
}