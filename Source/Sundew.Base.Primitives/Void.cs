// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Void.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base;

/// <summary>
/// Empty struct for use in generic types where a type parameter is required but not needed.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "The name is represented by a character.")]
#pragma warning disable SA1300
public readonly struct __
#pragma warning restore SA1300
{
    /// <summary>
    /// The void.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309:Field names should not begin with underscore", Justification = "Underscore is a common pattern for indicating something should be ignored.")]
    public static readonly __ _ = default;
}