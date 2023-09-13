// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading.Internal;

using System.Threading.Tasks;

internal static class TaskHelper
{
    public static readonly Task<bool> CompletedTrueTask = Task.FromResult(true);

    public static readonly Task<bool> CompletedFalseTask = Task.FromResult(false);
}