// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppendOptions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Identification;

/// <summary>
/// Represents options that configure how data is appended in a specific context.
/// </summary>
/// <param name="IsRoot">Indicates whether grouping should be avoided when appending data. If set to <c>true</c>, data will be appended without grouping, otherwise, it may be grouped based on the context.</param>
public sealed record AppendOptions(bool IsRoot);