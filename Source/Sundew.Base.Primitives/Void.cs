// --------------------------------------------------------------------------------------------------------------------
// File header file name documentation should match file name
// <copyright file="Void.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives;

/// <summary>
/// Empty struct for use in generic types where a type parameter is required but not needed.
/// </summary>
public readonly struct ˍ
{
    /// <summary>
    /// The void.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309:Field names should not begin with underscore", Justification = "Underscore is a common pattern for indicating something should be ignored.")]
    public static readonly ˍ _ = default;
}