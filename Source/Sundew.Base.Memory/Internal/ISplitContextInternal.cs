// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISplitContextInternal.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Memory.Internal;

using System;

/// <summary>
/// Interface for split context.
/// </summary>
/// <typeparam name="TItem">The type of the item.</typeparam>
internal interface ISplitContextInternal<TItem> : ISplitContext<TItem>
{
    int Length { get; }

    int StartIndex { get; }

    bool IsIgnoring { get; set; }

    ReadOnlyMemory<TItem> GetSectionAndReset();

    void Include(TItem item);

    void StartIncluding(int startIndex);
}