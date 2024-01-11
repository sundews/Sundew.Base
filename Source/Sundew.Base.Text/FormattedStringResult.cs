// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormattedStringResult.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text;

/// <summary>
/// Represents a result of formatting a <see cref="NamedFormatString"/>.
/// </summary>
[Sundew.DiscriminatedUnions.DiscriminatedUnion]
#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
public abstract partial record FormattedStringResult
#else
public abstract partial class FormattedStringResult
#endif
{
}