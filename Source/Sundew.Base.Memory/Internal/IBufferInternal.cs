// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBufferInternal.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Memory.Internal;

internal interface IBufferInternal<TItem> : IBuffer<TItem>
{
    void EnsureAdditionalCapacity(int requiredAdditionalCapacity);

    void WriteInternal(TItem item);
}