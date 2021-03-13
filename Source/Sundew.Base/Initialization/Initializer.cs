// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Initializer.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Initialization
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Sundew.Base.Collections;
    using Sundew.Base.Reporting;

    /// <summary>
    /// An implementation of <see cref="IInitializable"/> that initialized a list of <see cref="IInitializable"/>.
    /// </summary>
    public class Initializer : IInitializable
    {
        private readonly IEnumerable<IInitializable> initializables;
        private readonly bool parallelize;
        private readonly IInitializableReporter? initializableReporter;

        /// <summary>
        /// Initializes a new instance of the <see cref="Initializer" /> class.
        /// </summary>
        /// <param name="parallelize">if set to <c>true</c> [parallelize].</param>
        /// <param name="initializables">The initializables.</param>
        public Initializer(bool parallelize = false, params IInitializable[] initializables)
            : this(initializables, parallelize, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Initializer" /> class.
        /// </summary>
        /// <param name="parallelize">if set to <c>true</c> [parallelize].</param>
        /// <param name="initializableReporter">The initializable reporter.</param>
        /// <param name="initializables">The initializables.</param>
        public Initializer(bool parallelize, IInitializableReporter? initializableReporter, params IInitializable[] initializables)
            : this(initializables, parallelize, initializableReporter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Initializer" /> class.
        /// </summary>
        /// <param name="initializables">The initializables.</param>
        /// <param name="parallelize">if set to <c>true</c> [parallelize].</param>
        /// <param name="initializableReporter">The initializable reporter.</param>
        public Initializer(IEnumerable<IInitializable> initializables, bool parallelize = false, IInitializableReporter? initializableReporter = null)
        {
            this.initializables = initializables;
            this.parallelize = parallelize;
            this.initializableReporter = initializableReporter;
            this.initializableReporter?.SetSource(this);
        }

        /// <summary>
        /// Initializes the asynchronous.
        /// </summary>
        /// <returns>
        /// An async task.
        /// </returns>
        public async ValueTask InitializeAsync()
        {
            if (this.parallelize)
            {
                await Task.WhenAll(this.initializables.Select(x => x.InitializeAsync().AsTask())).ConfigureAwait(false);
            }
            else
            {
                await this.initializables.ForEachAsync(async x => await x.InitializeAsync().ConfigureAwait(false)).ConfigureAwait(false);
            }

            this.initializableReporter?.Initialized(this.initializables);
        }
    }
}