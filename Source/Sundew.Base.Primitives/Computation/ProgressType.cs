// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgressType.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives.Computation
{
    /// <summary>
    /// Specifies the type of progress.
    /// </summary>
    public enum ProgressType
    {
        /// <summary>
        /// Specifies that the reported progress is that items were added.
        /// </summary>
        ItemsAdded,

        /// <summary>
        /// Specifies that no more items will be added.
        /// </summary>
        CompletedAdding,

        /// <summary>
        /// Specifies that the reported progress is a message.
        /// </summary>
        Message,

        /// <summary>
        /// Specifies that the progress has changed.
        /// </summary>
        ItemCompleted,

        /// <summary>
        /// Specifies that progress has been cleared.
        /// </summary>
        Cleared,
    }
}