// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PadSide.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text
{
    /// <summary>
    /// Defines where the string is padded.
    /// </summary>
    public enum PadSide
    {
        /// <summary>
        /// Pads to the left.
        /// </summary>
        Left,

        /// <summary>
        /// Pads to both sides, but for string that cannot be equally padded, the string will padded more to the left.
        /// </summary>
        BothLeft,

        /// <summary>
        /// Pads to both sides, but for string that cannot be equally padded, the string will bpadded more to the right.
        /// </summary>
        BothRight,

        /// <summary>
        /// Pads to the right.
        /// </summary>
        Right,
    }
}
