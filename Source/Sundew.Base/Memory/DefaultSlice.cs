// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultSlice.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Memory
{
    /// <summary>
    /// Determines SliceDefault creates a slices.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1717:Only FlagsAttribute enums should have plural names", Justification = "The name is also singular.")]
    public enum DefaultSlice
    {
        /// <summary>
        /// The content.
        /// </summary>
        Content,

        /// <summary>
        /// The append.
        /// </summary>
        Append,
    }
}