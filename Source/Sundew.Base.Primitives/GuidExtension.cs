// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GuidExtension.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base;

using System;

/// <summary>
/// Extends <see cref="Guid"/> with easy to use methods.
/// </summary>
public static class GuidExtension
{
    /// <summary>
    /// Determines whether the specified GUID is empty.
    /// </summary>
    /// <param name="id">The GUID value.</param>
    /// <returns>
    ///   <c>true</c> if the specified GUID is empty otherwise, <c>false</c>.
    /// </returns>
    public static bool IsEmpty(this Guid id)
    {
        return id.Equals(Guid.Empty);
    }
}