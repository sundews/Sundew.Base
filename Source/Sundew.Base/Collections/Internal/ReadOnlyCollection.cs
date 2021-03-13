// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyCollection.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Collections.Internal
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Provides read only access to <see cref="List{TItem}"/>.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    internal sealed class ReadOnlyCollection<TItem> : IReadOnlyCollection<TItem>
    {
        private readonly ICollection<TItem> readOnlyCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyCollection{TItem}"/> class.
        /// </summary>
        /// <param name="readOnlyCollection">The list.</param>
        public ReadOnlyCollection(ICollection<TItem> readOnlyCollection)
        {
            this.readOnlyCollection = readOnlyCollection;
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count => this.readOnlyCollection.Count;

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<TItem> GetEnumerator()
        {
            return this.readOnlyCollection.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}