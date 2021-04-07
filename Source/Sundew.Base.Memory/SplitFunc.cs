// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplitFunc.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Memory
{
    /// <summary>
    /// Delegate for splitting memory in a LINQ manner.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="item">The item.</param>
    /// <param name="index">The index.</param>
    /// <param name="splitContext">The split context.</param>
    /// <returns>
    ///   <c>true</c>, if a split should occur otherwise false.
    /// </returns>
    public delegate SplitAction SplitFunc<TItem>(TItem item, int index, SplitContext<TItem> splitContext);
}