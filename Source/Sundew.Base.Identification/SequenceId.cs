// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SequenceId.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

internal static class SequenceId<TId>
    where TId : ISequenceId<TId>
{
#pragma warning disable SA1401
    internal static uint CurrentId = 0;
#pragma warning restore SA1401
}