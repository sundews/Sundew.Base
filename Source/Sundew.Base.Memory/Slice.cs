// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Slice.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Memory
{
    /// <summary>
    /// Determines where to slice, when slicing relative to an <see cref="IBuffer{TItem}"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1717:Only FlagsAttribute enums should have plural names", Justification = "The name is also singular.")]
    public enum Slice
    {
        /// <summary>
        /// The before
        /// </summary>
        Before,

        /// <summary>
        /// The after
        /// </summary>
        After,
    }
}