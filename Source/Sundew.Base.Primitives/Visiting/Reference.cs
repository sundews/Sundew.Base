// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reference.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Visiting
{
#nullable disable
    /// <summary>
    /// A mutable value used to pass modifications through visitors.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class Reference<TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Reference{TValue}"/> class.
        /// </summary>
        public Reference()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Reference{TValue}"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Reference(TValue value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public TValue Value { get; set; }

        /// <summary>
        /// Gets the result's success property.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>A value indicating whether the result was successful.</returns>
        public static implicit operator TValue(Reference<TValue> result)
        {
            return result.Value;
        }
    }
}