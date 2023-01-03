// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllOrFailed{TItem,TResult,TError}.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections;

using System.Collections.Generic;

/// <summary>
/// Represents the result of an ensured select.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
/// <typeparam name="TResult">The result type.</typeparam>
/// <typeparam name="TError">The error type.</typeparam>
[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract class AllOrFailed<TItem, TResult, TError>
{
    /// <summary>
    /// Creates a result representing all items.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <returns>An ensured item.</returns>
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(All<,,>))]
    public static AllOrFailed<TItem, TResult, TError> All(TResult[] items) => new All<TItem, TResult, TError>(items);

    /// <summary>
    /// Creates a result representing the item causing items not to be ensured.
    /// </summary>
    /// <param name="failedIndices">The failed indices.</param>
    /// <returns>An error item.</returns>
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(Failed<,,>))]
    public static AllOrFailed<TItem, TResult, TError> Failed(IReadOnlyList<FailedItem<TItem, TError>> failedIndices) => new Failed<TItem, TResult, TError>(failedIndices);
}