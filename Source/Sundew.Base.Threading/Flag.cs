// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Flag.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Threading
{
    using System;
    using System.Threading;

    /// <summary>
    /// Represents a flag.
    /// </summary>
    public sealed class Flag
    {
        private int flag;

        /// <summary>
        /// Initializes a new instance of the <see cref="Flag"/> class.
        /// </summary>
        public Flag()
            : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Flag"/> class.
        /// </summary>
        /// <param name="isSet">if set to <c>true</c> [is set].</param>
        public Flag(bool isSet)
        {
            this.flag = isSet ? 1 : 0;
        }

        /// <summary>
        /// Occurs when the flag is set.
        /// </summary>
        public event EventHandler? Raised;

        /// <summary>
        /// Occurs when the flag is cleared.
        /// </summary>
        public event EventHandler? Cleared;

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsSet => this.flag == 1;

        /// <summary>
        /// Performs an implicit conversion from <see cref="Flag" /> to <see cref="bool" />.
        /// </summary>
        /// <param name="flag">The interlocked boolean.</param>
        /// <value>
        ///   <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Use IsSet property instead.")]
        public static implicit operator bool(Flag flag)
        {
            return flag.IsSet;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns>A value indicating whether the flag was just initialized.</returns>
        public bool Set()
        {
            var result = Interlocked.Exchange(ref this.flag, 1) == 0;
            if (result)
            {
                this.Raised?.Invoke(this, EventArgs.Empty);
            }

            return result;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        /// <returns>A value indicating whether the flag was just cleared.</returns>
        public bool Clear()
        {
            var result = Interlocked.Exchange(ref this.flag, 0) == 1;
            if (result)
            {
                this.Cleared?.Invoke(this, EventArgs.Empty);
            }

            return result;
        }
    }
}