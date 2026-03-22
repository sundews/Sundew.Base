// --------------------------------------------------------------------------------------------------------------------
// <copyright file="With.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base;

/// <summary>
/// Factory methods for <see cref="With{TValue}"/>.
/// </summary>
/// <param name="HasValue">Gets a value indicating whether the instance has a value.</param>
public readonly record struct With(bool HasValue)
{
    /// <summary>
    /// Gets a <see cref="With"/> without a value.
    /// </summary>
    public static With None { get; } = new With(false);

    /// <summary>
    /// Gets a valid <see cref="With"/> with without a value.
    /// </summary>
    public static With Default { get; } = new With(true);
}