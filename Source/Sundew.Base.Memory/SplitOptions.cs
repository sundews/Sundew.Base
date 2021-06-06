// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplitOptions.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Memory
{
    using System;

    /// <summary>
    /// Options for a split.
    /// </summary>
    [Flags]
    public enum SplitOptions
    {
        /// <summary>
        /// The none.
        /// </summary>
        None,

        /// <summary>
        /// The remove empty entries.
        /// </summary>
        RemoveEmptyEntries,
    }
}
