// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Limit.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Text
{
    /// <summary>
    /// Specifies how text should be limited in length.
    /// </summary>
    public readonly struct Limit
    {
        private const string Epsilon = "…";
        private const string ThreeDots = "...";

        private Limit(bool isLeft, string? limitIndicator)
        {
            this.IsLeft = isLeft;
            this.LimitIndicator = limitIndicator;
        }

        /// <summary>
        /// Gets the left.
        /// </summary>
        /// <value>
        /// The left.
        /// </value>
        public static Limit Left { get; } = new(true, null);

        /// <summary>
        /// Gets the left.
        /// </summary>
        /// <value>
        /// The left.
        /// </value>
        public static Limit LeftWithEpsilon { get; } = new(true, Epsilon);

        /// <summary>
        /// Gets the left with dots.
        /// </summary>
        /// <value>
        /// The left with dots.
        /// </value>
        public static Limit LeftWithDots { get; } = new(true, ThreeDots);

        /// <summary>
        /// Gets the right with dots.
        /// </summary>
        /// <value>
        /// The right with dots.
        /// </value>
        public static Limit RightWithDots { get; } = new(false, ThreeDots);

        /// <summary>
        /// Gets the right with epsilon.
        /// </summary>
        /// <value>
        /// The right with epsilon.
        /// </value>
        public static Limit RightWithEpsilon { get; } = new(false, Epsilon);

        /// <summary>
        /// Gets the right.
        /// </summary>
        /// <value>
        /// The right.
        /// </value>
        public static Limit Right { get; } = default;

        /// <summary>
        /// Gets a value indicating whether this instance is left.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is left; otherwise, <c>false</c>.
        /// </value>
        public bool IsLeft { get; }

        /// <summary>
        /// Gets the limit indicator.
        /// </summary>
        /// <value>
        /// The limit indicator.
        /// </value>
        public string? LimitIndicator { get; }

        /// <summary>
        /// Lefts the specified limit indicator.
        /// </summary>
        /// <param name="limitIndicator">The limit indicator.</param>
        /// <returns>The limit.</returns>
        public static Limit LeftWith(string? limitIndicator)
        {
            return new(true, limitIndicator);
        }

        /// <summary>
        /// Rights the specified limit indicator.
        /// </summary>
        /// <param name="limitIndicator">The limit indicator.</param>
        /// <returns>The limit.</returns>
        public static Limit RightWith(string? limitIndicator)
        {
            return new(false, limitIndicator);
        }

        /// <summary>
        /// Creates a left or right limit with the specified limit indicator.
        /// </summary>
        /// <param name="isLeft">Limits the text length to the left.</param>
        /// <param name="limitIndicator">The limit indicator.</param>
        /// <returns>The limit.</returns>
        public static Limit With(bool isLeft, string? limitIndicator = null)
        {
            return new(isLeft, limitIndicator);
        }

        /// <summary>
        /// Creates a left or right limit with dots.
        /// </summary>
        /// <param name="isLeft">Limits the text length to the left.</param>
        /// <returns>The limit.</returns>
        public static Limit WithDots(bool isLeft)
        {
            return new(isLeft, ThreeDots);
        }

        /// <summary>
        /// Creates a left or right limit epsilon.
        /// </summary>
        /// <param name="isLeft">Limits the text length to the left.</param>
        /// <returns>The limit.</returns>
        public static Limit WithEpsilon(bool isLeft)
        {
            return new(isLeft, Epsilon);
        }
    }
}