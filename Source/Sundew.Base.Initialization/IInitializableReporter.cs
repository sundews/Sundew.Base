// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInitializableReporter.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Initialization
{
    using System.Collections.Generic;
    using Sundew.Base.Reporting;

    /// <summary>Interface for implementing a <see cref="IInitializable"/> reporter.</summary>
    public interface IInitializableReporter : IReporter
    {
        /// <summary>Called when initializables have been initialized.</summary>
        /// <param name="initializable">The initializable.</param>
        void Initialized(IEnumerable<IInitializable> initializable);
    }
}