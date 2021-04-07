// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Attempter.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Sundew.Base.Primitives.Computation
{
    /// <summary>
    /// Implements retry logic.
    /// </summary>
    public class Attempter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Attempter"/> class.
        /// </summary>
        /// <param name="maxAttempts">The maximum attempts.</param>
        public Attempter(int maxAttempts)
        {
            this.MaxAttempts = maxAttempts;
        }

        /// <summary>
        /// Gets the maximum attempts.
        /// </summary>
        /// <value>
        /// The maximum attempts.
        /// </value>
        public int MaxAttempts { get; private set; }

        /// <summary>
        /// Gets the current attempt.
        /// </summary>
        /// <value>
        /// The current attempt.
        /// </value>
        public int CurrentAttempt { get; private set; }

        /// <summary>
        /// Indicate another attempt.
        /// </summary>
        /// <returns><c>true</c>, if an operation should be attempted, otherwise <c>false</c>.</returns>
        public bool Attempt()
        {
            this.CurrentAttempt++;
            return this.CurrentAttempt <= this.MaxAttempts;
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            this.CurrentAttempt = 0;
        }

        /// <summary>
        /// Resets the specified maximum attempts.
        /// </summary>
        /// <param name="maxAttempts">The maximum attempts.</param>
        public void Reset(int maxAttempts)
        {
            this.MaxAttempts = maxAttempts;
            this.Reset();
        }
    }
}