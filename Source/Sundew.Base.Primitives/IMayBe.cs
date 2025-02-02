// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMayBe.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Interface for implementing a type that may or may not host the specified interface.
/// </summary>
/// <typeparam name="TTarget">The TObject type.</typeparam>
public interface IMayBe<TTarget>
{
    /// <summary>
    /// Gets the target if it exists in the instance.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns><c>true</c> if this instance has the target.</returns>
    bool TryGetTarget([NotNullWhen(true)] out TTarget? target);
}