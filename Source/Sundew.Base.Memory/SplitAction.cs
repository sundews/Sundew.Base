// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplitAction.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Memory;

/// <summary>
/// Enum which define how a <see cref="System.Memory{T}"/> is spliced.
/// </summary>
public enum SplitAction
{
    /// <summary>
    /// The ignore the item.
    /// </summary>
    Ignore = 0x00,

    /// <summary>
    /// The starts a new section or includes an item in the existing section.
    /// </summary>
    Include = 0x01,

    /// <summary>
    /// Yields a section if one is started.
    /// </summary>
    Split = 0x02,

    /// <summary>
    /// Includes the current item and yields a section.
    /// </summary>
    IncludeAndSplit = Include | Split,

    /// <summary>Yields a section if one in started and yields the current character in a new section.</summary>
    SplitAndSplitCurrent = Split | 0x0000_0110, // 0x0000_0110

    /// <summary>
    /// Yields a section if one is started and starts a new section including the current item.
    /// </summary>
    SplitAndInclude = Split | 0x0000_1000, // 0x0000_1010

    /// <summary>
    /// Yields a section if one is started and starts a new section including the rest.
    /// </summary>
    SplitAndIncludeRest = Split | 0x0001_0000, // 0x0001_0010
}