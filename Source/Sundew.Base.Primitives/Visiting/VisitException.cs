// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisitException.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base.Primitives.Visiting;

using System;

/// <summary>
/// Base exception for errors in visitors.
/// </summary>
/// <seealso cref="System.Exception" />
public abstract class VisitException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VisitException" /> class.
    /// </summary>
    /// <param name="visitable">The visitable.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="variable">The variable.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
    protected VisitException(object visitable, object parameter, object variable, string? message, Exception? innerException)
        : base(GetMessage(visitable, parameter, variable, message), innerException)
    {
    }

    /// <summary>
    /// Creates the specified visitable.
    /// </summary>
    /// <typeparam name="TVisitable">The type of the visitable.</typeparam>
    /// <typeparam name="TParameter">The type of the parameter.</typeparam>
    /// <typeparam name="TVariable">The type of the variable.</typeparam>
    /// <param name="visitable">The visitable.</param>
    /// <param name="parameter">The immutable parameter.</param>
    /// <param name="variable">The mutable parameter.</param>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The inner exception.</param>
    /// <returns>
    /// A new <see cref="VisitException{TVisitable,TImmutableParameter,TVariable}" />.
    /// </returns>
    public static VisitException<TVisitable, TParameter, TVariable> Create<TVisitable, TParameter, TVariable>(
        TVisitable visitable,
        TParameter parameter,
        TVariable variable,
        string? message = null,
        Exception? innerException = null)
        where TVisitable : notnull
        where TParameter : notnull
        where TVariable : notnull
    {
        return new VisitException<TVisitable, TParameter, TVariable>(visitable, parameter, variable, message, innerException);
    }

    private static string? GetMessage(object visitable, object parameter, object variable, string? message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return $"Could not visit: {visitable} with parameters: {parameter} and {variable}";
        }

        return message;
    }
}