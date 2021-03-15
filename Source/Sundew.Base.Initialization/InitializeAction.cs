// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializeAction.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Initialization
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>
    /// An action and async action which implements <see cref="IInitializable"/>.
    /// </summary>
    /// <seealso cref="Sundew.Base.Initialization.IInitializable" />
    public class InitializeAction : IInitializable
    {
        private readonly Func<ValueTask> action;

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializeAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="useYield">if set to <c>true</c> [use yield].</param>
        public InitializeAction(Action action, bool useYield = false)
            : this(async () => await ActionToAsyncFunc(action, useYield).ConfigureAwait(false))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializeAction"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        public InitializeAction(Func<ValueTask> action)
        {
            this.action = action;
        }

        /// <summary>
        /// Initializes the asynchronous.
        /// </summary>
        /// <returns>
        /// An async task.
        /// </returns>
        public async ValueTask InitializeAsync()
        {
            await this.action().ConfigureAwait(false);
        }

        /// <summary>Converts to string.</summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return $"InitializeAction {this.action.Target}.{this.action.GetMethodInfo().Name}";
        }

        private static async ValueTask ActionToAsyncFunc(Action action, bool useYield)
        {
            if (useYield)
            {
                await Task.Yield();
            }

            action();
        }
    }
}