// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInitializable.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Initialization
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for implementing initializable objects.
    /// </summary>
    public interface IInitializable
    {
        /// <summary>
        /// Initializes the asynchronous.
        /// </summary>
        /// <returns>An async task.</returns>
        ValueTask InitializeAsync();
    }
}