// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBufferResizer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Memory
{
    /// <summary>
    /// Interface for implementing an array provider.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public interface IBufferResizer<TItem>
    {
                              /// <summary>
                              /// Resizes the specified source.
                              /// </summary>
                              /// <param name="source">The source.</param>
                              /// <param name="minimumCapacity">The new minimum capacity.</param>
                              /// <returns>A new resized array.</returns>
        TItem[] Resize(TItem[]? source, int minimumCapacity);
    }
}