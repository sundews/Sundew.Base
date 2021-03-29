// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisitException{TVisitable,TParameter,TVariable}.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives.Visiting
{
    using System;

    /// <summary>
    /// Generic exception for visitors.
    /// </summary>
    /// <typeparam name="TVisitable">The type of the visitable.</typeparam>
    /// <typeparam name="TParameter">The type of the parameter.</typeparam>
    /// <typeparam name="TVariable">The type of the variable.</typeparam>
    /// <seealso cref="VisitException" />
    /// <seealso cref="System.Exception" />
    public class VisitException<TVisitable, TParameter, TVariable> : VisitException
        where TVisitable : notnull
        where TParameter : notnull
        where TVariable : notnull
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VisitException{TVisitable, TParameter, TVariable}" /> class.
        /// </summary>
        /// <param name="visitable">The visitable.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="variable">The variable.</param>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public VisitException(TVisitable visitable, TParameter parameter, TVariable variable, string? message = null, Exception? innerException = null)
            : base(visitable, parameter, variable, message, innerException)
        {
            this.Visitable = visitable;
            this.Parameter = parameter;
            this.Variable = variable;
        }

        /// <summary>
        /// Gets the visitable.
        /// </summary>
        /// <value>
        /// The visitable.
        /// </value>
        public TVisitable Visitable { get; }

        /// <summary>
        /// Gets the parameter.
        /// </summary>
        /// <value>
        /// The parameter.
        /// </value>
        public TParameter Parameter { get; }

        /// <summary>
        /// Gets the variable.
        /// </summary>
        /// <value>
        /// The variable.
        /// </value>
        public TVariable Variable { get; }
    }
}