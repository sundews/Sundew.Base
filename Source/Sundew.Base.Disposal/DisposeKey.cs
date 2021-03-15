// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposeKey.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Disposal
{
    /// <summary>
    /// Defines how a key in a <see cref="DisposingDictionary{TKey}"/> should be disposed.
    /// </summary>
    public enum DisposeKey
    {
        /// <summary>
        /// The key will not tried to be disposed.
        /// </summary>
        No,

        /// <summary>
        /// The key will be tried to be disposed before the value.
        /// </summary>
        BeforeValue,

        /// <summary>
        /// The key will be tried to be disposed after the value.
        /// </summary>
        AfterValue,
    }
}